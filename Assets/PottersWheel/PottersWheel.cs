using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PottersWheel : MonoBehaviour, IMovable
{
    private Layer _attatchedLayer;

    public bool IsActive { get; set; }

    public bool IsHidden { get; set; }

    public GameObject wheelMesh;

    public GameObject wheelCollider;

    public ButtonHandler uiButton;

    public PotteryUI ui;

    private float _rotationSpeed = 0.0f;
    [SerializeField]
    private float _maxRotationSpeed;
    [SerializeField]
    private float _speedChangeStep;
    public float RotationSpeed
    {
        get => _rotationSpeed;
        set
        {
            _rotationSpeed = Mathf.Clamp(value, -_maxRotationSpeed, _maxRotationSpeed);
        }
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
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
        IsHidden = true;

        uiButton.OnButtonDown += ShowUI;
    }

    private IEnumerator _rotateCoroutine;

    IEnumerator RotateLayer(Layer layer)
    {
        while (true)
        {
            transform.Rotate(new Vector3(0, _rotationSpeed, 0));
            yield return new WaitForSeconds(0.0111f);
        }
    }

    void TurnOn()
    {
        if (!IsActive)
        {
            IsActive = true;
            RotationSpeed = _maxRotationSpeed * 0.5f;
            _attatchedLayer = LayerManager.Instance.ActiveLayer;
            LayerManager.Instance.ActiveLayer.transform.SetParent(transform);
            _rotateCoroutine = RotateLayer(_attatchedLayer);
            StartCoroutine(_rotateCoroutine);
        }
    }

    void TurnOff()
    {
        if (IsActive)
        {
            IsActive = false;
            StopCoroutine(_rotateCoroutine);
            _attatchedLayer.transform.SetParent(LayerManager.Instance.LayersHolder.transform);
        }
    }

    void ToggleVisibility(bool value)
    {
        IsHidden = value;
        if (IsHidden)
        {
            wheelMesh.SetActive(true);
            wheelCollider.SetActive(true);
        }
        else
        {
            wheelMesh.SetActive(false);
            wheelCollider.SetActive(false);
        }
    }


    public void SetPosition(Vector3 pos)
    {
    }

    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }


    private void ShowUI(XRController controller)
    {
        if (ui != null)
        {
            ui.Close();
            return;
        }

        ui = UIController.Instance.ShowPotteryUI();
        ui.wheel = this;

        ui.playButton.onClick.AddListener(() =>
        {
            if (IsActive) TurnOff();
            else TurnOn();
            ui.RefreshUI();
        });


        ui.hideButton.onClick.AddListener(() =>
        {
            ToggleVisibility(!IsHidden);
            ui.RefreshUI();
        });

        ui.forwardButton.onClick.AddListener(() =>
        {
            RotationSpeed += _speedChangeStep;
        }
        );

        ui.forwardButton.onClick.AddListener(() =>
        {
            RotationSpeed -= _speedChangeStep;
        }
        );

    }
}
