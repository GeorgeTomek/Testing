using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : Singleton<MessagePanel>
{

    public enum MessageTime
    {
        Empty = 0,
        Short = 2,
        Normal = 4,
        Long = 6
    }

    [SerializeField]
    private Text textComponent;
    private Camera _cam;
    private Coroutine currentCor;

    private void Start()
    {
        _cam = Camera.main;
        textComponent.enabled = false;
    }

    void Update()
    {
        transform.position = _cam.transform.position + _cam.transform.forward * 3f + _cam.transform.up * 0.3f;
        transform.rotation = _cam.transform.rotation;
    }

    public void ShowMessage(string message)
    {
        if (currentCor != null)
            StopCoroutine(currentCor);

        textComponent.text = message;
        currentCor = StartCoroutine(PrintMessage(MessageTime.Normal));
    }

    public void ShowMessage(string message, MessageTime time)
    {
        if (currentCor != null)
            StopCoroutine(currentCor);

        textComponent.text = message;
        currentCor = StartCoroutine(PrintMessage(time));
    }

    private IEnumerator PrintMessage(MessageTime time)
    {
        var wait = new WaitForSeconds((float)time);

        textComponent.enabled = true;
        yield return wait;
        textComponent.enabled = false;
    }
}
