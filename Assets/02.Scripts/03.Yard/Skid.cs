using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skid : MonoBehaviour
{
    YardMap yardMapInfo;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObj = hit.collider.gameObject;

                if (clickedObj == gameObject)
                {
                    Debug.Log($"{yardMapInfo.SkidNo} >> Skid 클릭됨");

                    gameObject.GetComponent<Renderer>().material.color = Color.blue; // 클릭 시 색상 변경
                    //OnSkidClicked();
                }
            }
        }
    }
    public void FetchInfo(YardMap info)
    {
        yardMapInfo = info;
    }
    public void FetchDraw()
    {
        if (yardMapInfo.DzNo == 2)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 position = new Vector3(yardMapInfo.Dx * Global.UnityCorrectValue, 0, yardMapInfo.Dy * Global.UnityCorrectValue);
        if (yardMapInfo.Dong == 2)  // *-*-
        {
            position.z += 40f;
        }

        gameObject.transform.position = position;
        gameObject.transform.rotation = Quaternion.Euler(0, yardMapInfo.Dir, 0);

        gameObject.SetActive(true);
    }
}
