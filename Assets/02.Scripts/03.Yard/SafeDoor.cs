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
        None,   // ����
        Red,    // ����
        Yellow, // ��û
        Green   // ����
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

        bool[] lights = { false, false, false };    // ��, ��, ��

        switch (state)
        {
            case State.Red:
                // ����Ų��
                lights[0] = true;
                if (!isOpen)
                    ChangeDoor(true);
                break;
            case State.Yellow:
                // ��� Ų��
                lights[1] = true;
                if (isOpen)
                    ChangeDoor(false);
                break;
            case State.Green:
                // �ʷ� Ų��
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
            Debug.LogWarning("�ڽĿ� 'Door' ������Ʈ�� �����ϴ�! [" + NameKey + "]");
            return;
        }

        if (isChangeToOpen)
        {
            // ����
            StartCoroutine(RotateDoor(0, child));
            isOpen = true;
            
        }
        else
        {
            // �ݴ´�
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
