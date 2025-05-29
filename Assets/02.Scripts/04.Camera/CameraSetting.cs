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


    // 이동 제한
    public Vector2 xBounds = new Vector2(-100f, 300f);
    public Vector2 zBounds = new Vector2(-100f, 100f);
    public Vector2 yBounds = new Vector2(5f, 100f); // 줌 높이 제한


    private enum CamType { None, Main, Cam01, Cam02 }
    private CamType currentCam = CamType.None;

    private Vector3 mainCamInitialPosition;
    private Quaternion mainCamInitialRotation;
    private bool isMainCamActive = false;

    private Vector3 lastMousePosition;
    private bool isRightMouseDown = false;

    void Start()
    {
        // 저장: mainCam의 초기 위치/회전
        mainCamInitialPosition = mainCam.transform.position;
        mainCamInitialRotation = mainCam.transform.rotation;

        // 초기 카메라 설정
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

            Vector3 moveDir = new Vector3(inputX, -inputY, 0f); //x축 키보드 ← → y축 키보드(줌)  ↑ ↓
            Transform camTransform = (currentCam == CamType.Cam01) ? dongCam01.transform : dongCam02.transform;
            camTransform.position += moveDir * moveSpeed * Time.deltaTime;
        
        }

        else if (currentCam == CamType.Main)
        

        {
            //Transform camTransform = mainCam.transform;

            //// 1. 키보드 이동
            //float h = Input.GetAxisRaw("Horizontal");
            //float v = Input.GetAxisRaw("Vertical");
            //Vector3 move = camTransform.forward * v + camTransform.right * h;
            //move.y = 0f;
            //camTransform.position += move * moveSpeed * Time.deltaTime;

            //// 2. 마우스 휠 줌 (Y축 이동)
            //float scroll = Input.GetAxis("Mouse ScrollWheel");
            //Vector3 zoomMove = Vector3.up * scroll * zoomSpeed * Time.deltaTime;
            //camTransform.position += zoomMove;

            //// 3. 마우스 우클릭 회전
            //if (Input.GetMouseButtonDown(1))
            //{
            //    isRightMouseDown = true;
            //    lastMousePosition = Input.mousePosition;
            //}

            //if (Input.GetMouseButton(1))
            //{
            //    Vector3 delta = Input.mousePosition - lastMousePosition;
            //    float rotationY = delta.x * rotateSpeed * Time.deltaTime;

            //    camTransform.Rotate(0f, rotationY, 0f, Space.World);
            //    lastMousePosition = Input.mousePosition;
            //}

            //if (Input.GetMouseButtonUp(1))
            //{
            //    isRightMouseDown = false;
            //}

            //// 4. 이동 제한
            //Vector3 pos = camTransform.position;
            //pos.x = Mathf.Clamp(pos.x, xBounds.x, xBounds.y);
            //pos.y = Mathf.Clamp(pos.y, yBounds.x, yBounds.y);
            //pos.z = Mathf.Clamp(pos.z, zBounds.x, zBounds.y);
            //camTransform.position = pos;
            Transform camTransform = mainCam.transform;

            // 마우스 우클릭으로 자유 회전
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

            // 이동 (WASD 또는 방향키)
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            float upDown = 0f;

            // Q/E로 수직 이동 추가 (선택)
            if (Input.GetKey(KeyCode.Q)) upDown = -1f;
            if (Input.GetKey(KeyCode.E)) upDown = 1f;

            // Shift로 속도 증가
            float currentSpeed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f);

            Vector3 moveDir = (camTransform.forward * v + camTransform.right * h + Vector3.up * upDown).normalized;
            camTransform.position += moveDir * currentSpeed * Time.deltaTime;

            // 마우스 휠로 줌 (앞/뒤 이동)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                camTransform.position += camTransform.forward * scroll * zoomSpeed * Time.deltaTime;
            }

            // 이동 제한
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
