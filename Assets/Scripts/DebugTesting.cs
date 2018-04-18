using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugTesting : MonoBehaviour {

    public UnityEvent[] CustomEvent;


    void Update () {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CustomEvent[0].Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CustomEvent[1].Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CustomEvent[2].Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CustomEvent[3].Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CustomEvent[4].Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            CustomEvent[5].Invoke();
        }
    }
}
