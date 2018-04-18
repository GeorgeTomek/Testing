using System.Collections.Generic;
using UnityEngine;

internal class DebugDataProvider : IDataProvider
{

    public void Login(MonoBehaviour content, string username, string password)
    {
    }

    public void RequestTandemManualList(MonoBehaviour content)
    {
        //return new List<Manual>()
        //{
        //    new Manual(){ManualName = "Concentration of Bulk Antibody" },
        //    new Manual(){ManualName = "Preparation of BioReactor" },
        //    new Manual(){ManualName = "Reagent Assay" },

        //};
    }

    public void RequestTandemUserList(MonoBehaviour content)
    {
        //return new List<User>()
        //{
        //    new User(){UserName = "Angelo Stracquatanio" },
        //    new User(){UserName = "Alexandra Buttke" },
        //    new User(){UserName = "Gary Pignata" },
        //    new User(){UserName = "Nabil Chehade" },
        //};
    }
}