using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeDoor : MonoBehaviour
{
    enum State
    {
        None,   // 없음
        Red,    // 열림
        Yellow, // 요청
        Green   // 닫힘
    }

    public string NameKey;
    [SerializeField] State state = State.None;

    [SerializeField]
    GameObject LightR;
    [SerializeField]
    GameObject LightY;
    [SerializeField]
    GameObject LightG;

    Light[] Lights = new Light[3];
    Material[] Materials = new Material[3];
    bool isOpen = false;

    private void Awake()
    {
        Lights[0] = LightR.GetComponent<Light>();
        Lights[1] = LightY.GetComponent<Light>();
        Lights[2] = LightG.GetComponent<Light>();

        Materials[0] = LightR.GetComponent<Renderer>().material;
        Materials[1] = LightY.GetComponent<Renderer>().material;
        Materials[2] = LightG.GetComponent<Renderer>().material;
    }
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
        // 빨, 노, 초
        bool[] lights = { false, false, false };

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

        for (int i = 0; i < 3; i++)
        {
            if (lights[i])
            {
                Materials[i].EnableKeyword("_EMISSION");
                Materials[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                Lights[i].enabled = true;
            }
            else
            {
                Materials[i].DisableKeyword("_EMISSION");
                Materials[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                Lights[i].enabled = false;
            }
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
            StartCoroutine(RotateDoor(-90, child));
            isOpen = true;
            
        }
        else
        {
            // 닫는다
            StartCoroutine(RotateDoor(0, child));
            isOpen = false;
        }
    }
    IEnumerator RotateDoor(float targetY, Transform doorHinge)
    {
        float duration = 0.5f;
        float time = 0f;

        Transform child = transform.Find("Door");
        Quaternion startRot = doorHinge.localRotation;
        Quaternion endRot = Quaternion.Euler(0, targetY, 0);

        while (time < duration)
        {
            doorHinge.localRotation = Quaternion.Slerp(startRot, endRot, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        doorHinge.localRotation = endRot;
    }
}
