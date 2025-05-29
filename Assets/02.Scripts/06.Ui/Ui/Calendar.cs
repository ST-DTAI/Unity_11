using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

public class Calendar : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textValue = null;

    //[SerializeField]
    //private CalendarInfo info = null;

    [SerializeField]
    private Image select = null;
    private Button button = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void InitComponent()
    {
        if (null != button)
        {
            return;
        }
        button = GetComponent<Button>();
    }


}
