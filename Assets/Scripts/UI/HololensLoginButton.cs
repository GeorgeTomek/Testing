using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HololensLoginButton : MonoBehaviour, IInputClickHandler
{
    public InputField UserName;
    public InputField Password;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        StartCoroutine(LoginProcess());
        //if(StartupManager.Instance.DataProvider.Login(UserName.text, Password.text))
        //  {
        //      SceneManager.LoadScene(1);
        //  }
        //else
        //  {
        //      MessagePanel.Instance.ShowMessage("Login failed");
        //  }
    }

    IEnumerator LoginProcess()
    {
        StartupManager.Instance.DataProvider.Login(this, UserName.text, Password.text);
        var wait = new WaitForSeconds(0.5f);
        if (StartupManager.Instance.LoginCredentials != null)
            Debug.Log("Not null");

        while (StartupManager.Instance.LoginCredentials == null)
        {
            Debug.Log("empty");
            MessagePanel.Instance.ShowMessage("Logging in..");
            yield return wait;
        }
        if (StartupManager.Instance.LoginCredentials.message == "login failed")
        {
            MessagePanel.Instance.ShowMessage("Login failed");
        }
        else
        {
            MessagePanel.Instance.ShowMessage("Login sucessful");
            SceneManager.LoadScene(1);
        }
        yield return new WaitForEndOfFrame();

    }
}
