using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraSetting : MonoBehaviour
{

    public Transform target;  // 중심 오브젝트 (공장 중앙)
    public float distance = 50.0f; // 카메라 거리
    public float xSpeed = 120.0f;  // 좌우 회전 속도
    public float ySpeed = 120.0f;  // 상하 회전 속도

    public float panSpeed = 0.5f; // 팬 속도

    public float yMinLimit = -20f; // 아래로 회전 한계
    public float yMaxLimit = 80f;  // 위로 회전 한계

    public float zoomSpeed = 5f;   // 줌 속도
    public float minDistance = 20f;
    public float maxDistance = 100f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // 마우스 커서 숨기지 않음
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (target)
        {
            // 왼쪽 클릭 드래그 = 회전
            if (Input.GetMouseButton(0))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }

            // 오른쪽 클릭 드래그 = 팬 (target 이동)
            if (Input.GetMouseButton(1))
            {
                float moveX = -Input.GetAxis("Mouse X") * panSpeed;
                float moveY = -Input.GetAxis("Mouse Y") * panSpeed;

                Vector3 move = new Vector3(moveX, moveY, 0f);
                target.Translate(move, Space.Self);
            }

            // 마우스 휠 = 줌
            distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            // 카메라 위치 계산
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}


