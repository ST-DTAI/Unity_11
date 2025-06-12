using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class CoilnSkid : MonoBehaviour
{
    int _SkidNo = 0;

    [SerializeField]
    GameObject SkidObj;
    [SerializeField]
    GameObject CoilObj;
    [SerializeField]
    GameObject CoilTextObj;

    public void InitializeCoilSkid(YardMap info)
    {
        _SkidNo = info.SkidNo;

        SkidObj.GetComponent<Skid>().FetchInfo(info);
        CoilObj.GetComponent<Coil>().FetchInfo(info);
    }
    public void FetchInfo(YardMap info)
    {
        if (_SkidNo != info.SkidNo)
        {
            Debug.LogError("YardMap SkidNo mismatch! " + _SkidNo + " != " + info.SkidNo);
            return;
        }

        SkidObj.GetComponent<Skid>().FetchInfo(info);
        CoilObj.GetComponent<Coil>().FetchInfo(info);
    }
    public void FetchDraw()
    {
        SkidObj.GetComponent<Skid>().FetchDraw();
        CoilObj.GetComponent<Coil>().FetchDraw();
    }
}
