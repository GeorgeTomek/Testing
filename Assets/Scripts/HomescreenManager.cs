using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HomescreenManager : Singleton<HomescreenManager>
{
    //GameObjects to keep track if they are available in the scene
    private GameObject _tandemUserPanel;
    public GameObject TandemUserPanel
    {
        get { return _tandemUserPanel; }
        private set { _tandemUserPanel = value; }
    }
    private GameObject _tandemManualPanel;
    public GameObject TandemManualPanel
    {
        get { return _tandemManualPanel; }
        private set { _tandemManualPanel = value; }
    }

    [SerializeField]
    private GameObject btnOne;
    [SerializeField]
    private GameObject btnTwo;
    [SerializeField]
    private Image imagePanel;


    #region MenuInteractions
    public void OnClickTandem()
    {
        //TODO: Check if panel already exists, Animate TandemUserList Panel, call API for available users
        if (TandemUserPanel == null)
        {
            TandemUserPanel = Instantiate(ResourceHelper.GetObject<GameObject>(ResourceHelper.TandemUserPanel));
            TandemUserPanel.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
            StartCoroutine(OnPopUpPanelCoroutine(TandemUserPanel));

            var onTap = TandemUserPanel.gameObject.AddComponent<PlaceOnTap>();
            onTap.CustomEvent = new UnityEvent();

            Debug.Log("OnClickTandem is running");
            onTap.CustomEvent.AddListener(() =>
            {
                StartCoroutine(TandemUserList());
               // var users = StartupManager.Instance.DataProvider.RequestTandemUserList();
                //var parent = GameObject.Find("UsersPanel");

                ////TODO: Implement http request

                //foreach (var user in users)
                //{
                //    GameObject holder = Object.Instantiate(ResourceHelper.GetObject<GameObject>(ResourceHelper.TandemUserItem), parent.transform, false);
                //    var tUser = holder.AddComponent<TandemUser>();
                //    tUser.Username = user.first_name + " " + user.last_name;
                //    tUser.AppendValues();
                //}
            });
        }
    }

    private IEnumerator TandemUserList()
    {
        var wait = new WaitForSeconds(0.5f);
        StartupManager.Instance.DataProvider.RequestTandemUserList(this);

        while (StartupManager.Instance.SessionArray == null)
        {
            MessagePanel.Instance.ShowMessage("Downloading Users");
            yield return wait;
        }
        MessagePanel.Instance.ShowMessage("Users Received");
        var parent = GameObject.Find("UsersPanel");

        //TODO: Implement http request
        var users = StartupManager.Instance.SessionArray;
        foreach (var user in users)
        {
            GameObject holder = Object.Instantiate(ResourceHelper.GetObject<GameObject>(ResourceHelper.TandemUserItem), parent.transform, false);
            var tUser = holder.AddComponent<TandemUser>();
            tUser.Username = user.first_name + " " + user.last_name;
            tUser.AppendValues();
        }


        yield return wait;
    }

    public void OnClickManual()
    {

        //TODO: Check if panel already exists, Animate Manuals Panel, send request for single procedures/manuals
        if (TandemManualPanel == null)
        {
            TandemManualPanel = Instantiate(ResourceHelper.GetObject<GameObject>(ResourceHelper.TandemManualPanel));
            TandemManualPanel.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
            StartCoroutine(OnPopUpPanelCoroutine(TandemManualPanel));

            var onTap = TandemManualPanel.gameObject.AddComponent<PlaceOnTap>();
            onTap.CustomEvent = new UnityEvent();
            onTap.CustomEvent.AddListener(() =>
            {
                var manuals = new List<Manual>()
        {
            new Manual(){ManualName = "Concentration of Bulk Antibody" },
            new Manual(){ManualName = "Preparation of BioReactor" },
            new Manual(){ManualName = "Reagent Assay" },
                };
                var parent = GameObject.Find("ManualsPanel");

                //TODO: Implement http request

                foreach (var user in manuals)
                {
                    GameObject holder = Object.Instantiate(ResourceHelper.GetObject<GameObject>(ResourceHelper.TandemManualItem), parent.transform, false);
                    var tManual = holder.AddComponent<TandemManual>();
                    tManual.ManualName = user.ManualName;
                    tManual.AppendValues();
                }
            });
        }
    }

    public void OnMenuPinned()
    {
        StartCoroutine(OnMenuPinnedCoroutine());
    }

    public void OnMenuUnPinned()
    {
        StartCoroutine(OnMenuUnPinnedCoroutine());
    }
    #endregion

    #region MenuAnimations
    private IEnumerator OnPopUpPanelCoroutine(GameObject go)
    {
        var wait = new WaitForEndOfFrame();
        var finalScale = new Vector3(0.001f, 0.001f, 0.001f);

        go.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        go.transform.position = transform.position;

        while (go.transform.localScale.x <= 0.001f)
        {
            go.transform.localScale = Vector3.LerpUnclamped(go.transform.localScale, finalScale, Time.deltaTime * 2f);
            yield return wait;
        }
    }

    private IEnumerator OnMenuPinnedCoroutine()
    {
        var wait = new WaitForEndOfFrame();

        imagePanel.CrossFadeAlpha(0f, 2f, true);
        btnOne.SetActive(true);
        btnTwo.SetActive(true);

        for (int k = 0; k < 40; k++)
        {
            btnOne.transform.position = Vector3.Slerp(btnOne.transform.position, btnOne.transform.position + btnOne.transform.forward, Time.deltaTime * 0.1f);
            btnTwo.transform.position = Vector3.Slerp(btnTwo.transform.position, btnTwo.transform.position + -btnTwo.transform.forward, Time.deltaTime * 0.1f);
            yield return wait;
        }
        for (int i = 0; i < 60; i++)
        {
            btnOne.transform.position = Vector3.Slerp(btnOne.transform.position, btnOne.transform.position + btnOne.transform.right, Time.deltaTime * 0.1f);
            btnTwo.transform.position = Vector3.Slerp(btnTwo.transform.position, btnTwo.transform.position + -btnTwo.transform.right, Time.deltaTime * 0.1f);
            yield return wait;
        }
    }

    private IEnumerator OnMenuUnPinnedCoroutine()
    {
        var wait = new WaitForEndOfFrame();

        imagePanel.CrossFadeAlpha(1f, 2f, true);

        for (int i = 0; i < 60; i++)
        {
            btnOne.transform.position = Vector3.Slerp(btnOne.transform.position, btnOne.transform.position + -btnOne.transform.right, Time.deltaTime * 0.1f);
            btnTwo.transform.position = Vector3.Slerp(btnTwo.transform.position, btnTwo.transform.position + btnTwo.transform.right, Time.deltaTime * 0.1f);
            yield return wait;
        }

        for (int k = 0; k < 40; k++)
        {
            btnOne.transform.position = Vector3.Slerp(btnOne.transform.position, btnOne.transform.position + -btnOne.transform.forward, Time.deltaTime * 0.1f);
            btnTwo.transform.position = Vector3.Slerp(btnTwo.transform.position, btnTwo.transform.position + btnTwo.transform.forward, Time.deltaTime * 0.1f);
            yield return wait;
        }

        btnOne.SetActive(false);
        btnTwo.SetActive(false);

    }
    #endregion

    #region Actions

    private void TandemUserPanelAction()
    {
        StartupManager.Instance.DataProvider.RequestTandemUserList(this);
    }

    #endregion
}
