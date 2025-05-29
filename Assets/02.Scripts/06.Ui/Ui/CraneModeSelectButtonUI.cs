using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraneModeSelectButtonUI : MonoBehaviour
{

    // ���� ��忡���� ���� �г�
    [SerializeField] private GameObject manualModePanel;
    [SerializeField] private RectTransform layoutRootPanel;

    public GameObject moveCommandUI;
    public GameObject upCommandUI;
    public GameObject downCommandUI;

    public TMP_Text statusText;



    //�������/�ڵ����/���������

    public void OnSelectManualMode()
    {
        moveCommandUI.SetActive(false);
        upCommandUI.SetActive(false);
        downCommandUI.SetActive(false);
        statusText.text = "���� ��尡 Ȱ��ȭ�Ǿ����ϴ�.";
    }
    public void OnSelectAutoMode()
    {
        moveCommandUI.SetActive(false);
        upCommandUI.SetActive(false);
        downCommandUI.SetActive(false);
        statusText.text = "�ڵ� ��尡 Ȱ��ȭ�Ǿ����ϴ�.";
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootPanel);
    }

    public void OnSelectRemoteMode()
    {

        moveCommandUI.SetActive(false);
        upCommandUI.SetActive(false);
        downCommandUI.SetActive(false);
        statusText.text = "������ ��尡 Ȱ��ȭ�Ǿ����ϴ�.";
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootPanel);
    }



    // �̵�����/�ǻ�����/��������
    public void OnClickMoveCommand()
    {
        Debug.Log("�̵����� Ȱ��ȭ");
        moveCommandUI.SetActive(true);
        upCommandUI.SetActive(false);
        downCommandUI.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootPanel);
    }

    public void OnClickUpCommand()
    {
        Debug.Log("�ǻ����� Ȱ��ȭ");
        moveCommandUI.SetActive(false);
        upCommandUI.SetActive(true);
        downCommandUI.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootPanel);
    }

    public void OnClickDownCommand()
    {
        Debug.Log("�������� Ȱ��ȭ");
        moveCommandUI.SetActive(false);
        upCommandUI.SetActive(false);
        downCommandUI.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootPanel);
    }




}
