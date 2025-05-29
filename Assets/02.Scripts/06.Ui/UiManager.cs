using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using static SkidManager;

public class UiManager : MonoBehaviour
{
    public TMP_Text pdNoText;
    public TMP_Text widthText;
    public TMP_Text outdiaText;
    public TMP_Text indiaText;
    public TMP_Text thickText;
    public TMP_Text weightText;
    public TMP_Text tempText;
    public TMP_Text dateText;
    public TMP_Text dxText;
    public TMP_Text dyText;
    public TMP_Text dzText;

    public void UpdateSkidUI(YardMap coil)
    {
        pdNoText.text = coil.PdNo;
        widthText.text = coil.Width.ToString();
        outdiaText.text = coil.Outdia.ToString();
        indiaText.text = coil.India.ToString();
        thickText.text = coil.Thick.ToString();
        weightText.text = coil.Weight.ToString();
        tempText.text = coil.Temp.ToString();
        dateText.text = coil.Date;
        dxText.text = coil.Dx.ToString();
        dyText.text = coil.Dy.ToString();
        dzText.text = coil.Dz.ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
        pdNoText.text = "";
        widthText.text = "";
        outdiaText.text = "";
        indiaText.text = "";
        thickText.text = "";
        weightText.text = "";
        tempText.text = "";
        dateText.text = "";
        dxText.text = "";
        dyText.text = "";
        dzText.text = "";
    }

}
