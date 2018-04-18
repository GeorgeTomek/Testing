using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TandemManual : MonoBehaviour {

    public string ManualName;
    //TODO: Add important information that will be needed later (request for all procedures inside a manual)


    public void AppendValues()
    {
        //TODO: Append information to the panel
        transform.Find("TextManualName").GetComponent<Text>().text = ManualName;
    }
}
