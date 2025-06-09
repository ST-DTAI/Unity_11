using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    // ��ǥ ��ġ�� ������ Vector3 ���� ����
    private Vector3 targetPositionCrane;
    private Vector3 targetPositionHoist;
    private Vector3 targetPositionLift;
    public float moveSpeed = 1.0f;

    bool isFirstDone = false;

    public void FetchCraneData()
    {
        //Debug.Log("������ ũ���� ������ ����: " + craneStatusList.Count);

        // CrNo�� 2�� �����ϴ� ��� z �������� +20�� ����

        float offsetZ = dyOffset * Global.UnityCorrectValue;
        float offsetX = dxOffset * Global.UnityCorrectValue;

        targetPositionCrane = new Vector3(crStatus.Dx * Global.UnityCorrectValue + offsetX, craneRootObject.transform.position.y, craneRootObject.transform.position.z);
        targetPositionHoist = new Vector3(craneRootObject.transform.position.x, hoistRootObject.transform.position.y, crStatus.Dy * Global.UnityCorrectValue + offsetZ);
        targetPositionLift = new Vector3(craneRootObject.transform.position.x, crStatus.Dz * Global.UnityCorrectValue + 1.3f, hoistRootObject.transform.position.z);

        liftRootObject.transform.rotation = Quaternion.Euler(0, crStatus.SwivAng, 0);
        UpdateCoilObjectStatus(crStatus.Status, crStatus.PdNo, crStatus.SwivAng);

        UpdateLiftArmWidth(crStatus.ArmWid);
        //Debug.Log($"Updated Crane{crStatus.CrNo} position to x: {crStatus.Dx}, y: {crStatus.Dy}, z: {crStatus.Dz}, angle: {crStatus.SwivAng}");

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
            // ������ ������ ��� �ʱ�ȭ
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

        //���� �ؽ�Ʈ ���� 
        TextMeshPro tmp = coilObjectInstance.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = pdNo;
        }
        else
        {
            Debug.LogWarning("TextMeshPro component not found in Coil Object.");
        }
        //Debug.Log("Coil Object Attached to " + newParentTransform.name);
    }

    private void UpdateLiftArmWidth(int armWid)
    {
        float armWidth = armWid * Global.UnityCorrectValue *0.1f; //0.1�� �����ؾߵ�.
        float halfWidth = armWidth * 0.5f;

        liftArm_L.transform.localPosition = new Vector3(-halfWidth, liftArm_L.transform.localPosition.y, liftArm_L.transform.localPosition.z);
        liftArm_R.transform.localPosition = new Vector3(halfWidth, liftArm_R.transform.localPosition.y, liftArm_R.transform.localPosition.z);

        //Debug.Log($"Lift Arms updated: L({-halfWidth}), R({halfWidth})");
    }


    private void Update()
    {
        // MoveTowards�� ����Ͽ� ������Ʈ�� ��ǥ ��ġ�� �ε巴�� �̵�
        if (isFirstDone)
        {
            hoistRootObject.transform.position = Vector3.MoveTowards(hoistRootObject.transform.position, targetPositionHoist, Time.deltaTime * 0.32f);
            craneRootObject.transform.position = Vector3.MoveTowards(craneRootObject.transform.position, targetPositionCrane, Time.deltaTime * 1.0f);
            liftRootObject.transform.position = Vector3.MoveTowards(liftRootObject.transform.position, targetPositionLift, Time.deltaTime * 0.1f);
        }
        else
        {
            hoistRootObject.transform.position = targetPositionHoist;
            craneRootObject.transform.position = targetPositionCrane;
            liftRootObject.transform.position = targetPositionLift;
        }

    }

}
