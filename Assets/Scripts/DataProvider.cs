using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataProvider : IDataProvider
{
    public void Login(MonoBehaviour content, string username, string password)
    {
        NetworkHelper.Login(content, username, password);
    }

    public void RequestTandemManualList(MonoBehaviour content)
    {
        throw new System.NotImplementedException();
    }

    public void RequestTandemUserList(MonoBehaviour content)
    {
        NetworkHelper.UserSessions(content);

    }
}
