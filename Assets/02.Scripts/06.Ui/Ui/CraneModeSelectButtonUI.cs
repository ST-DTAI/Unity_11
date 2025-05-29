using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraneModeSelectButtonUI : MonoBehaviour
{

    // 수동 모드에서만 보일 패널
    [SerializeField] private GameObject manualModePanel;
    [SerializeField] private RectTransform layoutRootPanel;

    public GameObject moveCommandUI;
    public GameObject upCommandUI;
    public GameObject downCommandUI;

    public TMP_Text statusText;



    //수동모드/자동모드/리모컨모드

    public void OnSelectManualMode()
    {
        moveCommandUI.SetActive(false);
        upCommandUI.SetActive(false);
        downCommandUI.SetActive(false);
        statusText.text = "수동 모드가 활성화되었습니다.";
    }
    public void OnSelectAutoMode()
    {
        moveCommandUI.SetActive(false);
        upCommandUI.SetActive(false);
        downCommandUI.SetActive(false);
        statusText.text = "자동 모드가 활성화되었습니다.";
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootPanel);
    }

    public void OnSelectRemoteMode()
    {

        moveCommandUI.SetActive(false);
        upCommandUI.SetActive(false);
        downCommandUI.SetActive(false);
        statusText.text = "리모컨 모드가 활성화되었습니다.";
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootPanel);
    }



    // 이동지시/권상지시/권하지시
    public void OnClickMoveCommand()
    {
        Debug.Log("이동지시 활성화");
        moveCommandUI.SetActive(true);
        upCommandUI.SetActive(false);
        downCommandUI.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootPanel);
    }

    public void OnClickUpCommand()
    {
        Debug.Log("권상지시 활성화");
        moveCommandUI.SetActive(false);
        upCommandUI.SetActive(true);
        downCommandUI.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootPanel);
    }

    public void OnClickDownCommand()
    {
        Debug.Log("권하지시 활성화");
        moveCommandUI.SetActive(false);
        upCommandUI.SetActive(false);
        downCommandUI.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootPanel);
    }




}
