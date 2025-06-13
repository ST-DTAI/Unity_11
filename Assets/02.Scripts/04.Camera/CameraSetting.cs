using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSetting : MonoBehaviour
{
    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera dongCam01;
    public CinemachineVirtualCamera dongCam02;

    public float moveSpeed = 20f;
    public float rotateSpeed = 500f;
    public float zoomSpeed = 500f;

    // �̵� ����
    Vector2 xBounds = new Vector2(-20f, 20f);
    Vector2 zBounds = new Vector2(-20f, 20f);
    Vector2 yBounds = new Vector2(2f, 50f);

    private enum CamType { None, Main, Cam01, Cam02 }
    private CamType currentCam = CamType.None;

    private Vector3 mainCamInitialPosition;
    private Quaternion mainCamInitialRotation;
    private bool isMainCamActive = false;

    void Start()
    {
        // ī�޶� ���ѹ��� ����
        foreach (YardSetUp data in Global.YardSetUpList)
        {
            xBounds.y = Mathf.Max(xBounds.y, data.DxMax + 20f);
            zBounds.y += data.DyMax;
            yBounds.y = Mathf.Max(yBounds.y, data.Height + 50f);
        }

        // ����: mainCam�� �ʱ� ��ġ/ȸ��
        mainCamInitialPosition = mainCam.transform.position;
        mainCamInitialRotation = mainCam.transform.rotation;

        // �ʱ� ī�޶� ����
        SwitchToCam(mainCam);
        currentCam = CamType.Main;
    }

    void Update()
    {
        HandleMovementInput();
    }

    void HandleMovementInput()
    {
        if (currentCam == CamType.Cam01 || currentCam == CamType.Cam02)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            Vector3 moveDir = new Vector3(inputX, -inputY, 0f); //x�� Ű���� �� �� y�� Ű����(��)  �� ��
            Transform camTransform = (currentCam == CamType.Cam01) ? dongCam01.transform : dongCam02.transform;
            camTransform.position += moveDir * moveSpeed * Time.deltaTime;
        
        }

        else if (currentCam == CamType.Main)
        {
            Transform camTransform = mainCam.transform;

            // ���콺 ��Ŭ������ ���� ȸ��
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                Vector3 eulerAngles = camTransform.eulerAngles;
                eulerAngles.x -= mouseY * rotateSpeed * Time.deltaTime;
                eulerAngles.y += mouseX * rotateSpeed * Time.deltaTime;
                camTransform.eulerAngles = eulerAngles;
            }
            if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // �̵� (WASD �Ǵ� ����Ű)
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            float upDown = 0f;

            // Q/E�� ���� �̵� �߰� (����)
            if (Input.GetKey(KeyCode.Q))
                upDown = -1f;
            if (Input.GetKey(KeyCode.E))
                upDown = 1f;

            // Shift�� �ӵ� ����
            float currentSpeed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f);

            Vector3 moveDir = (camTransform.forward * v + camTransform.right * h + Vector3.up * upDown).normalized;
            camTransform.position += moveDir * currentSpeed * Time.deltaTime;

            // ���콺 �ٷ� �� (��/�� �̵�)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                camTransform.position += camTransform.forward * scroll * zoomSpeed * Time.deltaTime;
            }
            
            // ���콺 �� Ŭ�� �巡�׷� Y�� �̵�
            if (Input.GetMouseButton(2)) // Mouse Button 2 = �� Ŭ��
            {
                float mouseY = Input.GetAxis("Mouse Y");
                camTransform.position += Vector3.up * -mouseY * moveSpeed * Time.deltaTime;
            }

            // �̵� ����
            Vector3 pos = camTransform.position;
            pos.x = Mathf.Clamp(pos.x, xBounds.x, xBounds.y);
            pos.y = Mathf.Clamp(pos.y, yBounds.x, yBounds.y);
            pos.z = Mathf.Clamp(pos.z, zBounds.x, zBounds.y);
            camTransform.position = pos;
        }
    }



    public void OnClickButtonMainCam()
    {
        if (currentCam == CamType.Main && isMainCamActive)
        {
            ResetMainCamPosition();
            return;
        }

        SwitchToCam(mainCam);
        currentCam = CamType.Main;
        isMainCamActive = true;
    }

    public void OnClickButtonCam1()
    {
        SwitchToCam(dongCam01);
        currentCam = CamType.Cam01;
        isMainCamActive = false;
    }

    public void OnClickButtonCam2()
    {
        SwitchToCam(dongCam02);
        currentCam = CamType.Cam02;
        isMainCamActive = false;
    }

    private void ResetMainCamPosition()
    {
        mainCam.transform.position = mainCamInitialPosition;
        mainCam.transform.rotation = mainCamInitialRotation;
    }

    private void SwitchToCam(CinemachineVirtualCamera cam)
    {
        mainCam.Priority = 0;
        dongCam01.Priority = 0;
        dongCam02.Priority = 0;
      
        cam.Priority = 10;
    }

    private void SwitchToCam(CinemachineFreeLook cam)
    {
        mainCam.Priority = 10;
        dongCam01.Priority = 0;
        dongCam02.Priority = 0;

    }
}
