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

        switch (state)
        {
            case State.None:
                // 다 끈다
                LightR.enabled = false;
                LightY.enabled = false;
                LightG.enabled = false;
                break;
            case State.Red:
                // 빨강킨다
                LightR.enabled = true;
                LightY.enabled = false;
                LightG.enabled = false;
                if (!isOpen)
                    ChangeDoor(true);
                break;
            case State.Yellow:
                // 노랑 킨다
                LightR.enabled = false;
                LightY.enabled = true;
                LightG.enabled = false;
                if (isOpen)
                    ChangeDoor(false);
                break;
            case State.Green:
                // 초록 킨다
                LightR.enabled = false;
                LightY.enabled = false;
                LightG.enabled = true;
                if (isOpen)
                    ChangeDoor(false);
                break;
        }
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
            Debug.Log("문 열림! [" + NameKey + "]");
            
        }
        else
        {
            // 닫는다
            StartCoroutine(RotateDoor(-90, child));
            isOpen = false;
            Debug.Log("문 닫힘! [" + NameKey + "]");
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
