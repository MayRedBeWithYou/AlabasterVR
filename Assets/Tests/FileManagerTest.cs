using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;
using UnityEditor;

public class FileManagerTest
{
    DirectoryInfo testDirectory;
    DirectoryInfo testDirectoryChild;
    DirectoryInfo testDirectoryGrandChild;
    DirectoryButtonManager grandparentButton;
    DirectoryButtonManager parentButton;
    DirectoryButtonManager currentButton;
    FileExplorer fileExplorer;

    [OneTimeSetUp]
    public void SetUp()
    {      
        testDirectory= Directory.CreateDirectory(".alabaster_temporary_testing");
        testDirectoryChild= Directory.CreateDirectory(".alabaster_temporary_testing/childDirectory");
        testDirectoryGrandChild= Directory.CreateDirectory(".alabaster_temporary_testing/childDirectory/grandchildDirectory");
        File.Create(testDirectoryGrandChild.FullName+"/m1.obj");
        File.Create(testDirectoryGrandChild.FullName+"/m2.obj");
        File.Create(testDirectoryGrandChild.FullName+"/m3.abs");
        File.Create(testDirectoryGrandChild.FullName+"/p1.png");
        File.Create(testDirectoryGrandChild.FullName+"/p2.jpg");
        File.Create(testDirectoryGrandChild.FullName+"/p3.jpg");

        File.Create(testDirectoryChild.FullName+"/p1.jpg");

        GameObject prefab= AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scriptable/Prefabs/UI/FileExplorer/FileExplorer.prefab");
        GameObject go=GameObject.Instantiate(prefab);
        fileExplorer=go.GetComponent<FileExplorer>();
        fileExplorer.Awake();
        grandparentButton=fileExplorer.GrandparentButtonScript;
        parentButton=fileExplorer.ParentButtonScript;
        currentButton=fileExplorer.CurrentButtonScript;
        grandparentButton.Awake();
        parentButton.Awake();
        currentButton.Awake();
        grandparentButton.mainScript=fileExplorer;
        parentButton.mainScript=fileExplorer;
        currentButton.mainScript=fileExplorer;
        fileExplorer.mode=FileExplorerMode.Open;
        fileExplorer.SetExtensionsArray(new string[]{".jpg"});
        fileExplorer.ChangeDirectory(new DirectoryInfo(testDirectoryGrandChild.FullName));
        //fileExplorer.UpdateDirectory();
    }

    [SetUp]
    public void SetUpEach()
    {
        fileExplorer.ChangeDirectory(new DirectoryInfo(testDirectoryGrandChild.FullName));
    }

    [OneTimeTearDown]
    public void CleanUp()
    {
        fileExplorer.ChangeDirectory(testDirectory.Parent);
        //fileExplorer.ClearItems();
        currentButton.Deactivate();
        parentButton.Deactivate();
        grandparentButton.Deactivate();
        fileExplorer.Close();
        string path=testDirectory.FullName;
        testDirectory=null;
        testDirectoryChild=null;
        testDirectoryGrandChild=null;
        Directory.Delete(path, true);
    }

    [Test]
    public void FileManager_WraperLong()
    {
        string testString="RealyLongText";
        string expected="RealyL..";
        string received=currentButton.Wraper(testString);
        Assert.AreEqual(expected, received);
    }

    [Test]
    public void FileManager_WraperShort()
    {
        string testString="Short";
        string expected="Short";
        string received=currentButton.Wraper(testString);
        Assert.AreEqual(expected, received);
    }

    [Test]
    public void FileManager_GrandparentButton()
    {
        grandparentButton.OnClick();
        Assert.AreEqual(testDirectory.FullName, fileExplorer.CurrentDirecory.FullName);
    }

    [Test]
    public void FileManager_ParentButton()
    {
        Assert.AreEqual(grandparentButton.dirInfo.FullName, parentButton.dirInfo.FullName);
        parentButton.OnClick();
        //Assert.AreEqual(testDirectoryChild.FullName, fileExplorer.CurrentDirecory.FullName);
    }

    [Test]
    public void FileManager_CurrentButton()
    {
        currentButton.OnClick();
        //Assert.AreEqual(testDirectoryGrandChild.FullName, fileExplorer.CurrentDirecory.FullName);
        Assert.AreEqual(parentButton.dirInfo.FullName, fileExplorer.CurrentDirecory.FullName);
        //Assert.AreEqual(testDirectoryGrandChild.FullName, currentButton.dirInfo.FullName);
    }

    [Test]
    public void FileManager_OpenModelIncludingDirectories()
    {
        fileExplorer.SetExtensionsArray(new string[]{".jpg"});
        fileExplorer.mode=FileExplorerMode.Open;
        fileExplorer.ChangeDirectory(new DirectoryInfo(testDirectoryChild.FullName));
        Assert.AreEqual(2,fileExplorer.ItemsCounter);
    }

    [Test]
    public void FileManager_OpenReference()
    {
        fileExplorer.SetExtensionsArray(new string[]{".jpg",".png"});
        fileExplorer.mode=FileExplorerMode.Open;
        Assert.AreEqual(3,fileExplorer.ItemsCounter);
    }

    [Test]
    public void FileManager_OpenModels()
    {
        fileExplorer.SetExtensionsArray(new string[]{".abs"});
        fileExplorer.mode=FileExplorerMode.Open;
        Assert.AreEqual(1,fileExplorer.ItemsCounter);
    }

    [Test]
    public void FileManager_ImportModels()
    {
        fileExplorer.SetExtensionsArray(new string[]{".obj"});
        fileExplorer.mode=FileExplorerMode.Open;
        Assert.AreEqual(2,fileExplorer.ItemsCounter);
    }
}
