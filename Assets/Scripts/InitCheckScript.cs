using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class InitCheckScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        _ShowAndroidToastMessage("ARCore Init");
        StartCoroutine(CheckCompatibility());
    }

    private IEnumerator CheckCompatibility()
    {
        AsyncTask<ApkAvailabilityStatus> checkTask = Session.CheckApkAvailability();
        CustomYieldInstruction customYield = checkTask.WaitForCompletion();
        yield return customYield;

        ApkAvailabilityStatus result = checkTask.Result;

        switch (result)
        {
            case ApkAvailabilityStatus.SupportedApkTooOld:
                _ShowAndroidToastMessage("Supported apk too old");
                break;
            case ApkAvailabilityStatus.SupportedInstalled:
                _ShowAndroidToastMessage("Supported and installed");
                break;
            case ApkAvailabilityStatus.SupportedNotInstalled:
                _ShowAndroidToastMessage("Supported, not installed, requesting installation");
                Session.RequestApkInstallation(false);
                break;
            case ApkAvailabilityStatus.UnknownChecking:
                _ShowAndroidToastMessage("Unknown Checking");
                break;
            case ApkAvailabilityStatus.UnknownError:
                _ShowAndroidToastMessage("Unknown Error");
                break;
            case ApkAvailabilityStatus.UnknownTimedOut:
                _ShowAndroidToastMessage("Unknown Timed out");
                break;
            case ApkAvailabilityStatus.UnsupportedDeviceNotCapable:
                _ShowAndroidToastMessage("Unsupported Device Not Capable");
                break;
        }
    }

    //Copied from ComputerVisionController.cs
    private static void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }

}
