using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coil : MonoBehaviour
{
    YardMap yardMapInfo;

    [SerializeField]
    GameObject CoilTextObj;

    public Material clickedMaterial;    // 클릭 시 적용할 새로운 Material
    Material defaultMaterial;           // 기본 Material

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
                    Debug.Log($"{yardMapInfo.PdNo} >> Coil 클릭됨");

                    // 이전 클릭된 Coil 되돌리기
                    if (_prevClickedCoil != null && _prevClickedCoil != this)
                    {
                        _prevClickedCoil.GetComponent<Renderer>().material = defaultMaterial;
                    }

                    // 현재 Coil 강조 표시
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
        { //바닥 밑으로 가면 임시로 고정함.. 1단때문에 지금 우선 그리기 2단은 지켜 봐야 함 _250611
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
