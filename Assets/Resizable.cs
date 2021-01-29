using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resizable : MonoBehaviour, IResizable
{
    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
}
