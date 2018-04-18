using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.Networking;


public static class NetworkHelper
{

    public static void Login(MonoBehaviour content, string email, string password)
    {
       content.StartCoroutine(LoginCoroutine(email, password));
    }

    private static IEnumerator LoginCoroutine(string email, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        Debug.Log(email + " " + password);
        using (UnityWebRequest www = UnityWebRequest.Post("https://api.qa.apprentice.io/v1/login", form))
        {
            yield return www.SendWebRequest();

            //if (www.isNetworkError || www.isHttpError)
            //{
            //    Debug.Log(www.error);
            //    StartupManager.Instance.LoginCredentials = new LoginObject()
            //    {
            //        message = "login failed",
            //    };
            //}
            //else
            //{
            Debug.Log(www.downloadHandler.text);
            if (www.downloadHandler.text.Contains("Your username / password"))
            {
                StartupManager.Instance.LoginCredentials = new LoginObject()
                {
                    message = "login failed",
                };

            }
            else
            {
                StartupManager.Instance.LoginCredentials = JsonConvert.DeserializeObject<LoginObject>(www.downloadHandler.text);
            }
            //}
        }
    }

    public static void UserSessions(MonoBehaviour content)
    {
        content.StartCoroutine(SendSessionsRequestCoroutine());
    }

    private static IEnumerator SendSessionsRequestCoroutine()
    {
        string token = "Bearer " + StartupManager.Instance.LoginCredentials.token;
        System.Diagnostics.Debug.WriteLine(token);
        var headers = new Dictionary<string, string>();
        headers.Add("Authorization", token);
        //Tandem Sessions https://api.qa.apprentice.io/v1/tandem?filter=all
        //var www = new WWW("https://api.qa.apprentice.io/v1/users?filter=org_list", null, headers);
        using (UnityWebRequest www = UnityWebRequest.Get("https://api.qa.apprentice.io/v1/users?filter=org_list"))
        {
            www.SetRequestHeader("Authorization", token);
            MessagePanel.Instance.ShowMessage("Downloading Users Data");
            yield return www.SendWebRequest();

            System.Diagnostics.Debug.WriteLine(www.downloadHandler.text);
            Debug.Log(www.downloadHandler.text);
            //SessionObject[] sesArray = JsonConvert.DeserializeObject<SessionObject[]>(www.text);
            MessagePanel.Instance.ShowMessage("Users Data Received");
            User[] sesArray = JsonConvert.DeserializeObject<User[]>(www.downloadHandler.text);
            StartupManager.Instance.SessionArray = sesArray;

        }

      //  }
    }

    public static void GetSession(MonoBehaviour content, string id)
    {
        Coroutine coroutine = content.StartCoroutine(SendGetSessionRequestCoroutine(id));
    }

    private static IEnumerator SendGetSessionRequestCoroutine(string id)
    {
        string token = "Bearer " + StartupManager.Instance.LoginCredentials.token;
        var headers = new Dictionary<string, string>();
        headers.Add("Authorization", token);
        string url = "https://api.qa.apprentice.io/v1/tandem/" + id;
        var www = new WWW(url, null, headers);
        yield return www;
        if (www.error != null)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.text);
            UserSessionsObject usrSesObject = JsonConvert.DeserializeObject<UserSessionsObject>(www.text);
            StartupManager.Instance.UserSessionsObject = usrSesObject;
        }
    }



    public static List<User> RequestPostEquipment()
    {
        throw new System.NotImplementedException();
    }


}
