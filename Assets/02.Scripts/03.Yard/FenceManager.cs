using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class FenceManager : MonoBehaviour
{
    public GameObject fencePrefab;
    public GameObject doorPrefab;
    int fenceGap = 2;
    int lightFlag = 0;  //0,1,2->11의 빨,노,초 / 3,4,5->12의 빨,노,초

    void Start()
    {
        Point[] points = { new Point(39, 24), new Point(39, 2), new Point(93, 2), new Point(93, 24), new Point(39, 24) };
        Point[] doorPoints = { new Point(39, 24), new Point(39, 2), new Point(93, 2), new Point(93, 24), new Point(39, 24) };

        DrawFence(points, fenceGap);
        // DrawFence11();
        // DrawFence12();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LightTest();
        }
    }
    void DrawFence11()
    {
        int DxCount = 11;
        int DyCount = 27;

        int xPoint = 39;
        int zPoint = 24;

        for (int i = 0; i < DxCount; i++)
        {
            GameObject fence = Instantiate(fencePrefab, new Vector3(xPoint, 0, zPoint - i * fenceGap), Quaternion.identity);
            fence.name = "FENCE_11";
        }

        zPoint -= 2 * DxCount;
        for (int i = 0; i < DyCount; i++)
        {
            GameObject fence = Instantiate(fencePrefab, new Vector3(xPoint + i * fenceGap, 0, zPoint), Quaternion.Euler(0, 270, 0));
            fence.name = "FENCE_11";
        }

        xPoint += 2 * DyCount;
        for (int i = 0; i < DxCount; i++)
        {
            GameObject fence = Instantiate(fencePrefab, new Vector3(xPoint, 0, zPoint + i * fenceGap), Quaternion.Euler(0, 180, 0));
            fence.name = "FENCE_11";
        }

        zPoint += 2 * DxCount;
        for (int i = 0; i < (DyCount - 1); i++)
        {
            GameObject fence = Instantiate(fencePrefab, new Vector3(xPoint - i * fenceGap, 0, zPoint), Quaternion.Euler(0, 90, 0));
            fence.name = "FENCE_11";
        }

        // 안전문 추가
        xPoint -= 2 * (DyCount - 1);
        GameObject door = Instantiate(doorPrefab, new Vector3(xPoint, 0, zPoint), Quaternion.Euler(0, 90, 0));
        door.name = "DOOR11";

        GameObject lightGO = new GameObject("Light11");
        lightGO.transform.SetParent(door.transform); // ✅ Door에 자식으로 붙임

        // 위치를 Door 기준으로 적절히 배치 (예: 위쪽으로)
        lightGO.transform.localPosition = new Vector3(0f, 1.5f, 0f);

        Light light = lightGO.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 3f;
        light.intensity = 1f;
        light.enabled = false;

        // GameObject lightGO = new GameObject("Light11");
        // Light light = lightGO.AddComponent<Light>();
        // light.type = LightType.Point;
        // light.range = 3f;
        // light.intensity = 1f;
        // light.enabled = false;


        GameObject halfFence = Instantiate(fencePrefab, new Vector3(xPoint - fenceGap / 2, 0, zPoint), Quaternion.Euler(0, 90, 0));
        halfFence.name = "FENCE_11_HALF";
        halfFence.transform.localScale = new Vector3(
            halfFence.transform.localScale.x,
            halfFence.transform.localScale.y,
            halfFence.transform.localScale.z * 0.5f
        );
    }
    void DrawFence12()
    {
        int DxCount = 11;
        int DyCount = 21;

        int xPoint = 126;
        int zPoint = 24;

        for (int i = 0; i < DxCount; i++)
        {
            GameObject fence = Instantiate(fencePrefab, new Vector3(xPoint, 0, zPoint - i * fenceGap), Quaternion.identity);
            fence.name = "FENCE_12";
        }

        zPoint -= 2 * DxCount;
        for (int i = 0; i < DyCount; i++)
        {
            GameObject fence = Instantiate(fencePrefab, new Vector3(xPoint + i * fenceGap, 0, zPoint), Quaternion.Euler(0, 270, 0));
            fence.name = "FENCE_12";
        }

        xPoint += 2 * DyCount;
        for (int i = 0; i < DxCount; i++)
        {
            GameObject fence = Instantiate(fencePrefab, new Vector3(xPoint, 0, zPoint + i * fenceGap), Quaternion.Euler(0, 180, 0));
            fence.name = "FENCE_12";
        }

        zPoint += 2 * DxCount;
        for (int i = 0; i < (DyCount - 1); i++)
        {
            GameObject fence = Instantiate(fencePrefab, new Vector3(xPoint - i * fenceGap, 0, zPoint), Quaternion.Euler(0, 90, 0));
            fence.name = "FENCE_12";
        }

        // 안전문 추가
        xPoint -= 2 * (DyCount - 1);
        GameObject door = Instantiate(doorPrefab, new Vector3(xPoint, 0, zPoint), Quaternion.Euler(0, 90, 0));
        door.name = "DOOR12";


        GameObject lightGO = new GameObject("Light12");
        Light light = lightGO.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 3f;
        light.intensity = 1f;
        light.enabled = false;




        GameObject halfFence = Instantiate(fencePrefab, new Vector3(xPoint - fenceGap / 2, 0, zPoint), Quaternion.Euler(0, 90, 0));
        halfFence.name = "FENCE_12_HALF";
        halfFence.transform.localScale = new Vector3(
            halfFence.transform.localScale.x,
            halfFence.transform.localScale.y,
            halfFence.transform.localScale.z * 0.5f
        );
    }
    void LightTest()
    {
        // lightFlag에 맞게 빛 효과 주기
        int span = lightFlag / 3;
        int colorIndex = lightFlag % 3;

        GameObject lightGO = GameObject.Find("Light" + (span + 11));
        if (lightGO == null)
        {
            Debug.LogWarning("Light object not found for span: " + span);
            return;
        }

        Light light = lightGO.GetComponent<Light>();
        if (light == null)
        {
            Debug.LogWarning("Light component missing on: " + lightGO.name);
            return;
        }

        // 다른 빛 비활성화
        int otherSpan = (span == 0) ? 1 : 0;
        GameObject otherLightGO = GameObject.Find("Light" + (otherSpan + 11));
        if (otherLightGO != null)
        {
            Light otherLight = otherLightGO.GetComponent<Light>();
            if (otherLight != null) otherLight.enabled = false;
        }

        // 빛 색상 및 위치 설정
        GameObject doorGO = GameObject.Find("DOOR" + (span + 11));
        if (doorGO == null)
        {
            Debug.LogWarning("Door not found: DOOR" + (span + 11));
            return;
        }

        string[] colorNames = { "LightRed", "LightYellow", "LightGreen" };
        Color[] colors = { Color.red, Color.yellow, Color.green };

        Transform lightPos = doorGO.transform.Find(colorNames[colorIndex]);
        if (lightPos == null)
        {
            Debug.LogWarning("Color point not found: " + colorNames[colorIndex]);
            return;
        }

        light.transform.position = lightPos.position;
        light.color = colors[colorIndex];
        light.intensity = 1f;
        light.enabled = true;

        lightFlag++;
        lightFlag %= 6;
    }
    void DrawFence(Point[] points, float fenceSpacing)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 p1 = new Vector3(points[i].X, 0, points[i].Y);
            Vector3 p2 = new Vector3(points[i + 1].X, 0, points[i + 1].Y);

            DrawFencesBetween(p1, p2, fenceSpacing);
        }
    }
    void DrawFencesBetween(Vector3 p1, Vector3 p2, float fenceSpacing)
    {
        float dist = Vector3.Distance(p1, p2);
        int segmentCount = Mathf.FloorToInt(dist / fenceSpacing);

        // 방향 벡터 계산
        Vector3 direction = (p1 - p2).normalized;

        // 회전: 자동 회전 감지
        Quaternion rotation = Quaternion.LookRotation(direction);

        for (int i = 1; i <= segmentCount; i++)
        {
            Vector3 pos = p2 + direction * (i * fenceSpacing); // ← 주의!
            Instantiate(fencePrefab, pos, rotation);
        }
    }
}
