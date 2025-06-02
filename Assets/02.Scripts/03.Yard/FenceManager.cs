using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        List<Point> fencePoints = new List<Point> { new Point(39, 24), new Point(39, 2), new Point(93, 2), new Point(93, 24) };
        List<Point> doorPoints = new List<Point> { new Point(39, 22), new Point(39, 10), new Point(50, 2), new Point(93, 10), new Point(50, 24) };
        //List<Point> doorPoints = new List<Point> { new Point(50, 2) };

        //SkidPointsToFencePoints(fencePoints);
        DrawFence(fencePoints, doorPoints);
        // DrawFence11();
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
    void SkidPointsToFencePoints(List<Point> points)
    {
        int pointCount = points.Count;

        // 스키드 points에서 안전펜스 points로 변경
        int minX = points.Min(p => p.X);
        int maxX = points.Max(p => p.X);
        int minY = points.Min(p => p.Y);
        int maxY = points.Max(p => p.Y);

        int offsetX = 1;
        int offsetY = 2;
        for (int i = 0; i < pointCount; i++)
        {
            Point currentPoint = points[i];

            if (currentPoint.X == minX) currentPoint.X = minX - offsetX;
            else if (currentPoint.X == maxX) currentPoint.X = maxX + offsetX;

            if (currentPoint.Y == minY) currentPoint.Y = minY - offsetY;
            else if (currentPoint.Y == maxY) currentPoint.Y = maxY + offsetY;

            points[i] = currentPoint;
        }
    }
    void DrawFence(List<Point> fences, List<Point> doors)
    {
        int pointCount = fences.Count;

        for (int i = 0; i < pointCount; i++)
        {
            Point point1 = fences[i];
            Point point2 = pointCount - 1 == i ? fences[0] : fences[i + 1];
            Vector3 p1 = new Vector3(point1.X, 0, point1.Y);
            Vector3 p2 = new Vector3(point2.X, 0, point2.Y);

            // 펜스 포인트 사이에 존재하는 안전문 포인트 리스트
            List<Vector3> nowDoors = new List<Vector3>();
            foreach (Point door in doors)
            {
                if (IsBetween(door, point1, point2))
                    nowDoors.Add(new Vector3(door.X, 0, door.Y));
            }
            // point1과 거리를 기준으로 정렬
            if (nowDoors.Count > 0)
            {
                nowDoors = nowDoors.OrderBy(p => Vector3.Distance(new Vector3(point1.X, 0, point1.Y), p)).ToList();
                Debug.Log($"펜스 사이에 있는 안전문: {nowDoors}");
                DrawFencesBetween(p1, p2, nowDoors);
            }
            else
            {
                DrawFencesBetween(p1, p2);
            }
        }
    }
    void DrawFencesBetween(Vector3 p1, Vector3 p2)
    {
        Debug.Log($"{p1} -> {p2}");
        bool xHalf = Mathf.Abs(p1.x - p2.x) % 2 == 1;
        bool zHalf = Mathf.Abs(p1.z - p2.z) % 2 == 1;

        float dist = Vector3.Distance(p1, p2);
        int segmentCount = Mathf.FloorToInt(dist / fenceGap);

        // 방향 벡터 계산
        Vector3 direction = (p1 - p2).normalized;   // 방향(p1 - p2)

        // 회전: 자동 회전 감지
        Quaternion rotation = Quaternion.LookRotation(direction);

        float fenceLocation = 0f;
        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 pos = p1 - direction * (fenceLocation * fenceGap);
            float next = 1.0f;
            
            Instantiate(fencePrefab, pos, rotation);

            fenceLocation += next;
        }

        // 반쪽 펜스 처리
        if (xHalf)
        {
            Debug.Log("FENCE_X_HALFHALFHALFHALFHALF");
            Vector3 halfPos = p1 + direction * (segmentCount * fenceGap * 1.5f);
            GameObject halfFence = Instantiate(fencePrefab, halfPos, rotation);
            halfFence.transform.localScale = new Vector3(
                halfFence.transform.localScale.x,
                halfFence.transform.localScale.y,
                halfFence.transform.localScale.z * 0.5f
            );
            halfFence.name = "FENCE_X_HALFHALFHALFHALFHALF";
        }
        if (zHalf)
        {
            Debug.Log("FENCE_Z_HALFHALFHALFHALFHALF");
            Vector3 halfPos = p1 + direction * (segmentCount * fenceGap * 1.5f);
            GameObject halfFence = Instantiate(fencePrefab, halfPos, rotation);
            halfFence.transform.localScale = new Vector3(
                halfFence.transform.localScale.x,
                halfFence.transform.localScale.y,
                halfFence.transform.localScale.z * 0.5f
            );
            halfFence.name = "FENCE_Z_HALFHALFHALFHALFHALF";
        }
    }
    void DrawFencesBetween(Vector3 p1, Vector3 p2, List<Vector3> doors)
    {
        Debug.Log($"{p1} -> {p2}");
        // 방향 벡터 계산
        Vector3 direction = (p1 - p2).normalized;   // 방향(p1 - p2)
        Debug.Log($"direction : {direction}");
        // 회전: 자동 회전 감지
        Quaternion rotation = Quaternion.LookRotation(direction);


        bool xHalf = false;
        bool zHalf = false;
        bool isDoor = doors.Count > 0;  // 문이 있는지 확인
        if (direction.x == 0)   // 세로
        {
            if (Mathf.Abs(p1.z - p2.z) % 2 == 1)
                zHalf = true;
        }
        else if (direction.z == 0)  // 가로
        {
            if (Mathf.Abs(p1.x - p2.x) % 2 == 1)
                xHalf = true;
        }

        float dist = Vector3.Distance(p1, p2);
        int segmentCount = Mathf.FloorToInt(dist / fenceGap);

        float fenceLocation = 0f;
        for (int i = 0; i < segmentCount; i++)
        {
            if (fenceLocation >= segmentCount)
            {
                break;
            }
            Vector3 pos = p1 - direction * (fenceLocation * fenceGap);
            Vector3 tmp = p1 - direction * ((fenceLocation + 0.5f) * fenceGap);
            float next = 0.0f;
            if (doors.Count > 0 && doors[0] != null)
            {
                if (direction.x == 0)// 세로로 그려짐
                {
                    if (doors[0].z == pos.z)
                    {
                        Instantiate(doorPrefab, pos, rotation);

                        GameObject halfFence = Instantiate(fencePrefab, tmp, rotation);
                        halfFence.transform.localScale = new Vector3(
                            halfFence.transform.localScale.x,
                            halfFence.transform.localScale.y,
                            halfFence.transform.localScale.z * 0.5f
                        );
                        halfFence.name = "FENCE_Z_WITH_DOOR";

                        next = 1.0f;
                    }
                    else if (doors[0].z == tmp.z)
                    {
                        GameObject halfFence = Instantiate(fencePrefab, pos, rotation);
                        halfFence.transform.localScale = new Vector3(
                            halfFence.transform.localScale.x,
                            halfFence.transform.localScale.y,
                            halfFence.transform.localScale.z * 0.5f
                        );
                        halfFence.name = "FENCE_Z_WITH_DOOR";

                        Instantiate(doorPrefab, tmp, rotation);

                        next = 1.0f;
                    }
                }
                else if (direction.z == 0)// 가로로 그려짐
                {
                    if (doors[0].x == pos.x)
                    {
                        Instantiate(doorPrefab, pos, rotation);

                        GameObject halfFence = Instantiate(fencePrefab, tmp, rotation);
                        halfFence.transform.localScale = new Vector3(
                            halfFence.transform.localScale.x,
                            halfFence.transform.localScale.y,
                            halfFence.transform.localScale.z * 0.5f
                        );
                        halfFence.name = "FENCE_X_WITH_DOOR";

                        next = 1.0f;
                    }
                    else if (doors[0].x == tmp.x)
                    {
                        GameObject halfFence = Instantiate(fencePrefab, pos, rotation);
                        halfFence.transform.localScale = new Vector3(
                            halfFence.transform.localScale.x,
                            halfFence.transform.localScale.y,
                            halfFence.transform.localScale.z * 0.5f
                        );
                        halfFence.name = "FENCE_X_WITH_DOOR";

                        Instantiate(doorPrefab, tmp, rotation);
                        next = 1.0f;
                    }
                }
            }

            if (next == 0.0f)
            {
                Instantiate(fencePrefab, pos, rotation);
                next = 1.0f;
            }
            else
            {
                doors.RemoveAt(0);
                Debug.Log("문 생성");
            }

            fenceLocation += next;
        }

        // 반쪽 펜스 처리
        if (xHalf || zHalf)
        {
            Vector3 halfPos = p1 - direction * (fenceLocation * fenceGap);
            GameObject halfFence = Instantiate(fencePrefab, halfPos, rotation);
            halfFence.transform.localScale = new Vector3(
                halfFence.transform.localScale.x * (xHalf ? 0.5f : 1.0f),
                halfFence.transform.localScale.y,
                halfFence.transform.localScale.z * (zHalf ? 0.5f : 1.0f)
            );
            halfFence.name = $"FENCE_HALFHALFHALFHALFHALF";
        }
    }
    bool IsBetween(Point p, Point a, Point b)
    {
        // 점 p가 선분 ab 위에 있는지 확인
        float cross = (p.X - a.X) * (b.Y - a.Y) - (p.Y - a.Y) * (b.X - a.X);
        if (Mathf.Abs(cross) > 0.01f) return false;

        float dot = (p.X - a.X) * (b.X - a.X) + (p.Y - a.Y) * (b.Y - a.Y);
        if (dot < 0) return false;

        float sqLen = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
        if (dot > sqLen) return false;

        Debug.Log($"point {p} / between {a} and {b}");
        return true;
    }
}
