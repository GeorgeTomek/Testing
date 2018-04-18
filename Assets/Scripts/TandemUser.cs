using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TandemUser : MonoBehaviour {

    public string Username;
    //TODO: Add important information that will be needed to establish connection + all the info we can get from Apprentice API


    public void AppendValues()
    {
        //TODO: Append information to the panel
        transform.Find("TextUsername").GetComponent<Text>().text = Username;
    }
}


