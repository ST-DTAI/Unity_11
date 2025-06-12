using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coil : MonoBehaviour
{
    YardMap yardMapInfo;

    [SerializeField]
    GameObject CoilTextObj;

    public Material clickedMaterial;    // Ŭ�� �� ������ ���ο� Material
    Material defaultMaterial;           // �⺻ Material

    static Coil _prevClickedCoil;
    Renderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        defaultMaterial = _renderer.material;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log($"{yardMapInfo.PdNo} >> Coil Ŭ����");

                    // ���� Ŭ���� Coil �ǵ�����
                    if (_prevClickedCoil != null && _prevClickedCoil != this)
                    {
                        _prevClickedCoil.GetComponent<Renderer>().material = defaultMaterial;
                    }

                    // ���� Coil ���� ǥ��
                    _renderer.material = clickedMaterial;
                    _prevClickedCoil = this;
                }
            }
        }
    }
    public void FetchInfo(YardMap info)
    {
        yardMapInfo = info;
    }
    public void FetchDraw()
    {
        if (yardMapInfo.PdYN == "0")
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 position = new Vector3(
            yardMapInfo.Dx * Global.UnityCorrectValue,
            (yardMapInfo.Dz - yardMapInfo.Outdia * 0.5f) * Global.UnityCorrectValue,
            yardMapInfo.Dy * Global.UnityCorrectValue
        );

        if (position.y < 0.3f)  // *-*-
        { //�ٴ� ������ ���� �ӽ÷� ������.. 1�ܶ����� ���� �켱 �׸��� 2���� ���� ���� �� _250611
            position.y = 0.3f;
        }

        if (yardMapInfo.Dong == 2)
        {
            position.z += 40f;
        }


        gameObject.transform.position = position;
        gameObject.transform.rotation = Quaternion.Euler(0, yardMapInfo.Dir, 0);
        gameObject.transform.localScale = new Vector3(
            Global.UnityCorrectValue * yardMapInfo.Width,
            Global.UnityCorrectValue * yardMapInfo.Outdia,
            Global.UnityCorrectValue * yardMapInfo.Outdia
        );
        CoilTextObj.GetComponent<CurvedCoilText>().SetCurvedText(yardMapInfo.PdNo, yardMapInfo.Outdia * Global.UnityCorrectValue);

        gameObject.SetActive(true);
    }
}
