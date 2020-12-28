using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Color hoveredColor;
    public Color selectedColor;
    public Color idleColor;

    [SerializeField]
    private TabButton _selectedTab;

    public TabButton selectedTab
    {
        get => _selectedTab;
        set
        {
            _selectedTab = value;
            ResetTabs();
        }
    }


    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
        if (tabButtons.Count == 1)
        {
            selectedTab = button;
        }
        else ResetTabs();
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        button.background.color = hoveredColor;
    }
    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }
    public void OnTabSelected(TabButton button)
    {
        selectedTab = button;
    }

    public void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            button.background.color = idleColor;
            button.panel.SetActive(false);
        }
        selectedTab.background.color = selectedColor;
        selectedTab.panel.SetActive(true);
    }

    public void AddNewLayer()
    {
        LayerManager.Instance.AddNewLayer();
        selectedTab = tabButtons[0];
    }

    public void AddNewLight()
    {
        LightManager.Instance.CreateLight();
        selectedTab = tabButtons[1];
    }

    public void AddNewImage()
    {

        selectedTab = tabButtons[2];
    }

}
