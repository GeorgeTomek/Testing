using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkHelper {

    static public void LoadObject(MonoBehaviour behaviour, string url)
    {
       behaviour.StartCoroutine(LoadObj(url));
    }

    private static IEnumerator LoadObj(string url)
    {
        WWW www = new WWW(url);

        yield return www;
    }

}
