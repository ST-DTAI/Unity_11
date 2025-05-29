using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamSetting : MonoBehaviour
{
    public float moveSpeed = 10f;         // �⺻ �̵� �ӵ�
    public float boostMultiplier = 2f;    // Shift ������ �� �ӵ� ���
    public float mouseSensitivity = 2f;   // ���콺 ����
    public float verticalMoveSpeed = 5f;  // ���콺 �ٷ� ������ �̵� �ӵ�
    public CinemachineVirtualCamera virtualCam; // ������ Virtual Camera

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.x;
        rotationY = angles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
        HandleVerticalMove();
        HandleCursorUnlock();
    }

    void HandleRotation()
    {
        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // A, D
        float moveZ = Input.GetAxis("Vertical");   // W, S

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move.Normalize();

        float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * boostMultiplier : moveSpeed;

        transform.position += move * speed * Time.deltaTime;
    }

    void HandleVerticalMove()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            transform.position += transform.up * scroll * verticalMoveSpeed;
        }
    }

    void HandleCursorUnlock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
