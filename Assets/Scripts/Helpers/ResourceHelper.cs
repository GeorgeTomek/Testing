using System.Collections.Generic;
using UnityEngine;


public static class ResourceHelper
{
    //Panels
    public const string TandemUserPanel = @"UI/TandemUserPanel";
    public const string TandemManualPanel = @"UI/TandemManualPanel";

    //Items
    public const string TandemUserItem = @"UI/UserItem";
    public const string TandemManualItem = @"UI/ManualItem";


    private static Dictionary<string, Object> _allReadyBaked = new Dictionary<string, Object>();



    /// <summary>
    /// Method create or use existing prefab for required object
    /// </summary>
    /// <typeparam name="T">Type of output unity object</typeparam>
    /// <param name="pathInResources">Path of prefab in Resources folder</param>
    /// <returns>Instance of prefab</returns>
    public static T GetObject<T>(string pathInResources)
        where T : Object
    {
        Object outputObject;
        if (!_allReadyBaked.TryGetValue(pathInResources, out outputObject))
        {

            outputObject = Resources.Load(pathInResources);
            _allReadyBaked.Add(pathInResources, outputObject);
        }

        return outputObject as T;
    }
    public static IEnumerator<T> GetObjectAsync<T>(string pathInResources)
     where T : Object
    {
        Object outputObject;
        if (!_allReadyBaked.TryGetValue(pathInResources, out outputObject))
        {

            ResourceRequest req = Resources.LoadAsync(pathInResources);
            while (!req.isDone)
            {
                yield return null;
            }
            outputObject = req.asset;
            _allReadyBaked.Add(pathInResources, outputObject);
        }

        yield return outputObject as T;
    }
    public static ResourceRequest ResourcRequest(string pathInResources)
    {
        return Resources.LoadAsync(pathInResources);
    }
}


