using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PottersWheel : MonoBehaviour
{
    private Layer _attatchedLayer;
    public bool IsActive { get; set; }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            TurnOn();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TurnOff();
        }
    }

    private void Awake()
    {
        IsActive = false;
    }
    private IEnumerator _rotateCoroutine;
    IEnumerator RotateLayer(Layer layer)
    {
        while(true)
        {
            transform.Rotate(new Vector3(0, 3.0f, 0));
            yield return new WaitForSeconds(0.016f);
        }
    }
    void TurnOn()
    {
        if(!IsActive)
        {
            IsActive = true;
            _attatchedLayer = LayerManager.Instance.ActiveLayer;
            LayerManager.Instance.ActiveLayer.transform.SetParent(transform);
            _rotateCoroutine = RotateLayer(_attatchedLayer);
            StartCoroutine(_rotateCoroutine);
        }

    }

    void TurnOff()
    {
        if(IsActive)
        {
            IsActive = false;
            StopCoroutine(_rotateCoroutine);
            _attatchedLayer.transform.SetParent(LayerManager.Instance.LayersHolder.transform);
        }
    }
    
}
