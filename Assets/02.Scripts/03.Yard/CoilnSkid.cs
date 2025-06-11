using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoilnSkid : MonoBehaviour
{
    YardMap yardMapInfo;

    [SerializeField]
    GameObject SkidObj;
    [SerializeField]
    GameObject CoilObj;
    [SerializeField]
    GameObject CoilTextObj;

    public void InitializeYardMap(YardMap info)
    {
        yardMapInfo = info;
    }
    public void FetchInfo(YardMap info)
    {
        if (yardMapInfo.SkidNo != info.SkidNo)
        {
            Debug.LogError("YardMap SkidNo mismatch! " + yardMapInfo.SkidNo + " != " + info.SkidNo);
            return;
        }

        yardMapInfo.Dir = info.Dir;
        yardMapInfo.PdYN = info.PdYN;
        yardMapInfo.Hold = info.Hold;
        yardMapInfo.CrRev = info.CrRev;
        yardMapInfo.SupRev = info.SupRev;
        yardMapInfo.OutRev = info.OutRev;
        yardMapInfo.FwdYN = info.FwdYN;
        yardMapInfo.BwdYN = info.BwdYN;
        yardMapInfo.PdNo = info.PdNo;
        yardMapInfo.State = info.State;
        yardMapInfo.Width = info.Width;
        yardMapInfo.Outdia = info.Outdia;
        yardMapInfo.India = info.India;
        yardMapInfo.Thick = info.Thick;
        yardMapInfo.Weight = info.Weight;
        yardMapInfo.Temp = info.Temp;
        yardMapInfo.Date = info.Date;
        yardMapInfo.ToNo = info.ToNo;
    }
    public void FetchDraw()
    {
        FetchSkid();
        FetchCoil();
    }
    void FetchSkid()
    {
        if (yardMapInfo.DzNo == 2)
        {
            SkidObj.SetActive(false);
            return;
        }

        Vector3 position = new Vector3(yardMapInfo.Dx * Global.UnityCorrectValue, 0, yardMapInfo.Dy * Global.UnityCorrectValue);
        if (yardMapInfo.Dong == 2)  // *-*-
        {
            position.z += 40f;
        }

        SkidObj.transform.position = position;
        SkidObj.transform.rotation = Quaternion.Euler(0, yardMapInfo.Dir, 0);

        SkidObj.SetActive(true);
    }
    void FetchCoil()
    {
        if (yardMapInfo.PdYN == "0")
        {
            CoilObj.SetActive(false);
            return;
        }

        Vector3 position = new Vector3(
            yardMapInfo.Dx * Global.UnityCorrectValue, 
            (yardMapInfo.Dz - yardMapInfo.Outdia * 0.5f) * Global.UnityCorrectValue, 
            yardMapInfo.Dy * Global.UnityCorrectValue
        );

        if (position.y < 0.3f) // Adjust height if below a certain threshold
        { //바닥 밑으로 가면 임시로 고정함.. 1단때문에 지금 우선 그리기 2단은 지켜 봐야 함 _250611
            position.y = 0.3f;
        }

        if (yardMapInfo.Dong == 2)
        {
            position.z += 40f;
        }

        
        CoilObj.transform.position = position;
        CoilObj.transform.rotation = Quaternion.Euler(0, yardMapInfo.Dir, 0);
        CoilObj.transform.localScale = new Vector3(
            Global.UnityCorrectValue * yardMapInfo.Width,
            Global.UnityCorrectValue * yardMapInfo.Outdia,
            Global.UnityCorrectValue * yardMapInfo.Outdia
        );

        TextMeshPro coilText = CoilTextObj.GetComponent<TextMeshPro>();
        coilText.text = yardMapInfo.PdNo;

        CoilObj.SetActive(true);
    }
}
