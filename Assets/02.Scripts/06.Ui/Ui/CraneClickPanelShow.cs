using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneClickPanelShow : MonoBehaviour
{

    public GameObject panel; // 패널을 연결할 변수
    public CinemachineVirtualCamera[] virtualCameras;

    private float doubleClickTime = 0.3f; // 더블 클릭 간격
    private float lastClickTime = 0f; // 마지막 클릭 시간


    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭
        {
            foreach (CinemachineVirtualCamera virtualCamera in virtualCameras) // 모든 가상 카메라에 대해 확인
            {
                // 가상 카메라의 실제 카메라 가져오기
                Camera camera = virtualCamera.GetComponent<Camera>();
                if (camera == null) continue; // 카메라가 없으면 건너뜀

                // Raycast를 위해 현재 카메라에서 Ray 생성
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform) // 현재 스크립트가 붙어 있는 오브젝트인지 확인
                    {
                        if (Time.time - lastClickTime < doubleClickTime) // 더블 클릭 감지
                        {
                            ShowPanel(); // 패널 표시
                        }
                        lastClickTime = Time.time; // 마지막 클릭 시간 갱신
                    }
                }
            }
        }
    }


    private void ShowPanel()
    {
        if (panel != null)
        {
            panel.SetActive(true); // 패널 활성화
        }
    }
}
