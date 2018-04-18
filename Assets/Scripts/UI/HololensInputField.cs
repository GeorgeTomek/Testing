using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.UI.Keyboard;
using System;
using UnityEngine.UI;

public class HololensInputField : MonoBehaviour, IInputClickHandler
{
    private InputField.ContentType _contentType;


    public void OnInputClicked(InputClickedEventData eventData)
    {
        _contentType = GetComponent<InputField>().contentType;
        Keyboard.Instance.InputField.contentType = _contentType;

        Keyboard.Instance.OnTextSubmitted += TextReceived;
        Keyboard.Instance.OnTextUpdated += TextUpdated;
        Keyboard.Instance.PresentKeyboard();
    }

    private void TextUpdated(string obj)
    {
        var textTransform = transform.Find("Text");
        GetComponent<InputField>().text = obj;
    }

    private void TextReceived(object sender, EventArgs e)
    {
        Keyboard.Instance.OnTextSubmitted -= TextReceived;
        Keyboard.Instance.OnTextUpdated -= TextUpdated;

    }
}
