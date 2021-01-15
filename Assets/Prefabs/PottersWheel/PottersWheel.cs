// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : MayRe
// Created          : 01-15-2021
//
// Last Modified By : MayRe
// Last Modified On : 01-15-2021
// ***********************************************************************
// <copyright file="PottersWheel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class PottersWheel.
/// Implements the <see cref="UnityEngine.MonoBehaviour" />
/// Implements the <see cref="IMovable" />
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
/// <seealso cref="IMovable" />
public class PottersWheel : MonoBehaviour, IMovable
{
    private Layer _attatchedLayer;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is active.
    /// </summary>
    /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is hidden.
    /// </summary>
    /// <value><c>true</c> if this instance is hidden; otherwise, <c>false</c>.</value>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Mesh for the PottersWheel object on scene.
    /// </summary>
    public GameObject wheelMesh;

    /// <summary>
    /// Collider for the PottersWheel object on scene.
    /// </summary>
    public GameObject wheelCollider;

    /// <summary>
    /// The UI button
    /// </summary>
    public ButtonHandler uiButton;

    /// <summary>
    /// The UI
    /// </summary>
    public PotteryUI ui;

    private float _rotationSpeed = 0.0f;
    [SerializeField]
    private float _maxRotationSpeed;
    [SerializeField]
    private float _speedChangeStep;

    /// <summary>
    /// Gets or sets the rotation speed.
    /// </summary>
    /// <value>The rotation speed.</value>
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
        if (!IsHidden)
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


    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="pos">The position.</param>
    public void SetPosition(Vector3 pos)
    {
    }

    /// <summary>
    /// Sets the rotation.
    /// </summary>
    /// <param name="rot">The rot.</param>
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

        ui.reverseButton.onClick.AddListener(() =>
        {
            RotationSpeed -= _speedChangeStep;
        }
        );

    }
}
