using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.Events;

/// <summary>
/// Script to move GameObject to cursor position
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlaceOnTap : MonoBehaviour, IInputClickHandler
{

    private float distFromCamera = 4f;
    private bool _isRectTrans;
    [Tooltip("Rotate on Y axis only | True by default")]
    public bool RotateOnY = true;
    public Camera Cam;
    public RectTransform RectTrans;
    public UnityEvent CustomEvent;

    private void Start()
    {
        if (GetComponent<RectTransform>())
        {
            RectTrans = GetComponent<RectTransform>();
            _isRectTrans = true;
        }
        Cam = Camera.main;
    }

    private void Update()
    {
        if (_isRectTrans)
        {
            RectTrans.position = Vector3.Slerp(transform.position, Cam.transform.position + Cam.transform.forward * distFromCamera, Time.deltaTime * 3f);
        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, Cam.transform.position + Cam.transform.forward * distFromCamera, Time.deltaTime * 3f);
        }
        if (RotateOnY)
        {
            var rot = RectTrans.rotation;
            rot.y = Cam.transform.rotation.y;
            RectTrans.rotation = rot;
        }
        else
        {
            RectTrans.rotation = Cam.transform.rotation;
        }

    }


    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (CustomEvent != null)
        {
            CustomEvent.Invoke();
        }
        GetComponent<Collider>().enabled = false;
        Destroy(this, 0.05f);
    }
}
