using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableManager : MonoBehaviour
{

    public YardSkidCoil coilData2;

    // text를 표시할 TextMeshProUGUI들
    public TextMeshProUGUI pdNoText;
    public TextMeshProUGUI widthText;
    public TextMeshProUGUI outdiaText;
    public TextMeshProUGUI indiaText;
    public TextMeshProUGUI thickText;


    // HorizontalLayoutGroup
    //public HorizontalLayoutGroup layoutGroup;

    void Start()
    {

        pdNoText.text = "";
        widthText.text = "";
        outdiaText.text = "";
        indiaText.text = "";
        thickText.text = "";

    }

    public void UpdateSkidUI(YardSkidCoil coil)
    {
        pdNoText.text = coil.PdNo;
        widthText.text = coil.Width.ToString();
        outdiaText.text = coil.Outdia.ToString();
        indiaText.text = coil.India.ToString();
        thickText.text = coil.Thick.ToString();

    }

    }

