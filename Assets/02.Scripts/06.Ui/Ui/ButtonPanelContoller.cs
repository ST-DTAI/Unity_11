using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine; // Cinemachine ���ӽ����̽� �߰�

public class ButtonPanelController : MonoBehaviour
{
    public GameObject selectedPeriodPanel;
    public GameObject ManualPanel;

    public CinemachineVirtualCamera[] cameras; // Virtual Camera �迭�� ����
    public Button button0; // ù ��° ��ư ����ī�޶�
    public Button button1; // ù ��° ��ư
    public Button button2; // �� ��° ��ư

    private void Start()
    {
        // ��� ī�޶� ��Ȱ��ȭ
        foreach (CinemachineVirtualCamera cam in cameras)
        {
            cam.gameObject.SetActive(false);
        }
        // ù ��° ī�޶� Ȱ��ȭ
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }
        // ��ư Ŭ�� �̺�Ʈ ���
        button0.onClick.AddListener(() => OnClickSwitchCamera(0)); // 1�� ��ư Ŭ�� �� 1�� ī�޶�
        button1.onClick.AddListener(() => OnClickSwitchCamera(1)); // 1�� ��ư Ŭ�� �� 1�� ī�޶�
        button2.onClick.AddListener(() => OnClickSwitchCamera(2)); // 2�� ��ư Ŭ�� �� 2�� ī�޶�
    }

    public void OnClickSelectedPeriodPanel()
    {
        if (selectedPeriodPanel != null)
        {
            selectedPeriodPanel.SetActive(true);
        }
    }

    public void OnClickManualPanel()
    {
        if (ManualPanel != null)
        {
            ManualPanel.SetActive(true);
        }
    }

    public void OnClickSwitchCamera(int index)
    {
        // ��� ī�޶� ��Ȱ��ȭ
        foreach (CinemachineVirtualCamera cam in cameras)
        {
            cam.gameObject.SetActive(false);
        }

        // ������ ī�޶� Ȱ��ȭ
        if (index >= 0 && index < cameras.Length)
        {
            cameras[index].gameObject.SetActive(true);
        }
    }
}
