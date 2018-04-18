using System.Collections.Generic;
using UnityEngine;

public interface IDataProvider
{
    void Login(MonoBehaviour content, string username, string password);
    void RequestTandemUserList(MonoBehaviour content);
    void RequestTandemManualList(MonoBehaviour content);
}
