using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelCRWorkCheck : MonoBehaviour
{
    public Button closeButton;

    void Start()
    {
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
    }


    // �г��� �ݴ� �޼���
    void ClosePanel()
    {
        gameObject.SetActive(false);
    }



}
