using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeDoor : MonoBehaviour
{
    public string NameKey;

    bool isOpen = false;
    int Dong;
    enum State
    {
        None,   // 없음
        Red,    // 열림
        Yellow, // 요청
        Green   // 닫힘
    }
    [SerializeField] State state = State.None;

    public void SetState(int iState)
    {
        if (iState >= 0 && iState < 4)
        {
            if ((State)iState == state) return;

            state = (State)iState;
            UpdateState();
        }
    }
    private void UpdateState()
    {
        Light LightR = transform.Find("LightRed").GetComponent<Light>();
        Light LightY = transform.Find("LightYellow").GetComponent<Light>();
        Light LightG = transform.Find("LightGreen").GetComponent<Light>();

        bool[] lights = { false, false, false };    // 빨, 노, 초

        switch (state)
        {
            case State.Red:
                // 빨강킨다
                lights[0] = true;
                if (!isOpen)
                    ChangeDoor(true);
                break;
            case State.Yellow:
                // 노랑 킨다
                lights[1] = true;
                if (isOpen)
                    ChangeDoor(false);
                break;
            case State.Green:
                // 초록 킨다
                lights[2] = true;
                if (isOpen)
                    ChangeDoor(false);
                break;
        }
        LightR.enabled = lights[0];
        LightY.enabled = lights[1];
        LightG.enabled = lights[2];
    }
    public void ChangeDoor(bool isChangeToOpen)
    {
        Transform child = transform.Find("Door");
        if (child == null)
        {
            Debug.LogWarning("자식에 'Door' 오브젝트가 없습니다! [" + NameKey + "]");
            return;
        }

        if (isChangeToOpen)
        {
            // 연다
            StartCoroutine(RotateDoor(0, child));
            isOpen = true;
            
        }
        else
        {
            // 닫는다
            StartCoroutine(RotateDoor(-90, child));
            isOpen = false;
        }
    }
    IEnumerator RotateDoor(float targetY, Transform doorHinge)
    {
        float duration = 0.5f;
        float time = 0f;

        Transform child = transform.Find("Door");
        Quaternion startRot = doorHinge.localRotation;
        Quaternion endRot = Quaternion.Euler(0, targetY, 90);

        while (time < duration)
        {
            doorHinge.localRotation = Quaternion.Slerp(startRot, endRot, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        doorHinge.localRotation = endRot;
    }
    void Update()
    {
        
    }
}
