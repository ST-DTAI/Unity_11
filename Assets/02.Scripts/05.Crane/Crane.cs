using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Crane : MonoBehaviour
{
    public CrStatus crStatus;
    public int crNo;
    public int dxOffset;
    public int minDx;
    public int maxDx;
    public int dyOffset;
    public int minDy;
    public int maxDy;


    public GameObject craneRootObject;
    public GameObject hoistRootObject;
    public GameObject liftRootObject;

    public GameObject coilObject;

    public GameObject liftArm_L;
    public GameObject liftArm_R;

    public Transform coilAttachTransform;
    private bool isCoilAttached = false;

    // 목표 위치를 저장할 Vector3 변수 선언
    private Vector3 targetPositionCrane;
    private Vector3 targetPositionHoist;
    private Vector3 targetPositionLift;
    public float moveSpeed = 1.0f;

    bool isFirstDone = false;

    public void FetchCraneData()
    {
        float offsetZ = dyOffset * Global.UnityCorrectValue;
        float offsetX = dxOffset * Global.UnityCorrectValue;

        float unityX = crStatus.Dx * Global.UnityCorrectValue + offsetX;
        float unityY = crStatus.Dz * Global.UnityCorrectValue + 1.3f;
        float unityZ = crStatus.Dy * Global.UnityCorrectValue + offsetZ;

        targetPositionCrane = new Vector3(
            unityX,
            craneRootObject.transform.position.y,
            craneRootObject.transform.position.z
        );
        targetPositionHoist = new Vector3(
            unityX,
            hoistRootObject.transform.position.y,
            unityZ
        );
        targetPositionLift = new Vector3(
            unityX,
            unityY,
            unityZ
        );

        liftRootObject.transform.rotation = Quaternion.Euler(0, crStatus.SwivAng, 0);

        UpdateCoilObjectStatus(crStatus.Status, crStatus.PdNo, crStatus.SwivAng);
        UpdateLiftArmWidth(crStatus.ArmWid);
    }

    private void UpdateCoilObjectStatus(string status, string pdNo, int swivAng)
    {
        if (pdNo != "0" && !isCoilAttached)
        {
            AttachCoilObject(swivAng, pdNo);
            isCoilAttached = true;
        }
        else if (pdNo == "0" && isCoilAttached)
        {
            // 코일이 내려간 경우 초기화
            isCoilAttached = false;
        }
    }



    private void AttachCoilObject(int swivAng, string pdNo)
    {
        GameObject coilObjectInstance;

        if (PrefabUtility.GetPrefabAssetType(coilObject) != PrefabAssetType.NotAPrefab)
        {
            coilObjectInstance = Instantiate(coilObject);
        }
        else
        {
            coilObjectInstance = coilObject;
        }

        coilObjectInstance.transform.SetParent(coilAttachTransform, false);
        coilObjectInstance.transform.localPosition = Vector3.zero;
        coilObjectInstance.SetActive(true);
        coilObjectInstance.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        coilObjectInstance.transform.localRotation = Quaternion.identity;

        //코일 텍스트 설정 
        TextMeshPro tmp = coilObjectInstance.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = pdNo;
        }
        else
        {
            Debug.LogWarning("TextMeshPro component not found in Coil Object.");
        }
    }

    private void UpdateLiftArmWidth(int armWid)
    {
        float armWidth = armWid * Global.UnityCorrectValue *0.1f; //0.1를 더곱해야됨.
        float halfWidth = armWidth * 0.5f;

        liftArm_L.transform.localPosition = new Vector3(-halfWidth, liftArm_L.transform.localPosition.y, liftArm_L.transform.localPosition.z);
        liftArm_R.transform.localPosition = new Vector3(halfWidth, liftArm_R.transform.localPosition.y, liftArm_R.transform.localPosition.z);
    }


    private void Update()
    {
        // MoveTowards를 사용하여 오브젝트를 목표 위치로 부드럽게 이동
        if (isFirstDone)
        {
            float newX = Mathf.MoveTowards(liftRootObject.transform.position.x, targetPositionLift.x, Time.deltaTime * moveSpeed);
            float newY = Mathf.MoveTowards(liftRootObject.transform.position.y, targetPositionLift.y, Time.deltaTime * moveSpeed);
            float newZ = Mathf.MoveTowards(liftRootObject.transform.position.z, targetPositionLift.z, Time.deltaTime * moveSpeed);

            craneRootObject.transform.position = new Vector3(newX, targetPositionCrane.y, targetPositionCrane.z);
            hoistRootObject.transform.position = new Vector3(newX, targetPositionHoist.y, newZ);
            liftRootObject.transform.position = new Vector3(newX, newY, newZ);
        }
        else if (targetPositionCrane.x != 0)
        {
            craneRootObject.transform.position = targetPositionCrane;
            hoistRootObject.transform.position = targetPositionHoist;
            liftRootObject.transform.position = targetPositionLift;
            isFirstDone = true;
        }

    }

}
