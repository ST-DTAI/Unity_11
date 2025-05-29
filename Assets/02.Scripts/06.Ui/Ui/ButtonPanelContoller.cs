using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine; // Cinemachine 네임스페이스 추가

public class ButtonPanelController : MonoBehaviour
{
    public GameObject selectedPeriodPanel;
    public GameObject ManualPanel;

    public CinemachineVirtualCamera[] cameras; // Virtual Camera 배열로 변경
    public Button button0; // 첫 번째 버튼 메인카메라
    public Button button1; // 첫 번째 버튼
    public Button button2; // 두 번째 버튼

    private void Start()
    {
        // 모든 카메라 비활성화
        foreach (CinemachineVirtualCamera cam in cameras)
        {
            cam.gameObject.SetActive(false);
        }
        // 첫 번째 카메라 활성화
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }
        // 버튼 클릭 이벤트 등록
        button0.onClick.AddListener(() => OnClickSwitchCamera(0)); // 1번 버튼 클릭 시 1번 카메라
        button1.onClick.AddListener(() => OnClickSwitchCamera(1)); // 1번 버튼 클릭 시 1번 카메라
        button2.onClick.AddListener(() => OnClickSwitchCamera(2)); // 2번 버튼 클릭 시 2번 카메라
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
        // 모든 카메라 비활성화
        foreach (CinemachineVirtualCamera cam in cameras)
        {
            cam.gameObject.SetActive(false);
        }

        // 선택한 카메라 활성화
        if (index >= 0 && index < cameras.Length)
        {
            cameras[index].gameObject.SetActive(true);
        }
    }
}
