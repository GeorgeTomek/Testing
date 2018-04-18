using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupManager : Singleton<StartupManager>
{

    public IDataProvider DataProvider;
    public LoginObject LoginCredentials = null;
    public User[] SessionArray = null;
    public UserSessionsObject UserSessionsObject = null;

    void Start()
    {
        LoginCredentials = null;
        SessionArray = null;
        UserSessionsObject = null;


        // DataProvider = new DebugDataProvider();
        DataProvider = new DataProvider();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
