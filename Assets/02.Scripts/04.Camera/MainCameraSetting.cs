using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraSetting : MonoBehaviour
{

    public Transform target;  // �߽� ������Ʈ (���� �߾�)
    public float distance = 50.0f; // ī�޶� �Ÿ�
    public float xSpeed = 120.0f;  // �¿� ȸ�� �ӵ�
    public float ySpeed = 120.0f;  // ���� ȸ�� �ӵ�

    public float panSpeed = 0.5f; // �� �ӵ�

    public float yMinLimit = -20f; // �Ʒ��� ȸ�� �Ѱ�
    public float yMaxLimit = 80f;  // ���� ȸ�� �Ѱ�

    public float zoomSpeed = 5f;   // �� �ӵ�
    public float minDistance = 20f;
    public float maxDistance = 100f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // ���콺 Ŀ�� ������ ����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (target)
        {
            // ���� Ŭ�� �巡�� = ȸ��
            if (Input.GetMouseButton(0))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }

            // ������ Ŭ�� �巡�� = �� (target �̵�)
            if (Input.GetMouseButton(1))
            {
                float moveX = -Input.GetAxis("Mouse X") * panSpeed;
                float moveY = -Input.GetAxis("Mouse Y") * panSpeed;

                Vector3 move = new Vector3(moveX, moveY, 0f);
                target.Translate(move, Space.Self);
            }

            // ���콺 �� = ��
            distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            // ī�޶� ��ġ ���
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


