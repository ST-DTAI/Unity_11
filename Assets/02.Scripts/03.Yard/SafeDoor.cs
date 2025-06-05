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

        switch (state)
        {
            case State.None:
                // �� ����
                LightR.enabled = false;
                LightY.enabled = false;
                LightG.enabled = false;
                break;
            case State.Red:
                // ����Ų��
                LightR.enabled = true;
                LightY.enabled = false;
                LightG.enabled = false;
                if (!isOpen)
                    ChangeDoor(true);
                break;
            case State.Yellow:
                // ��� Ų��
                LightR.enabled = false;
                LightY.enabled = true;
                LightG.enabled = false;
                if (isOpen)
                    ChangeDoor(false);
                break;
            case State.Green:
                // �ʷ� Ų��
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
            Debug.LogWarning("�ڽĿ� 'Door' ������Ʈ�� �����ϴ�! [" + NameKey + "]");
            return;
        }

        if (isChangeToOpen)
        {
            // ����
            StartCoroutine(RotateDoor(0, child));
            isOpen = true;
            Debug.Log("�� ����! [" + NameKey + "]");
            
        }
        else
        {
            // �ݴ´�
            StartCoroutine(RotateDoor(-90, child));
            isOpen = false;
            Debug.Log("�� ����! [" + NameKey + "]");
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
