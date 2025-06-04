using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEditor.PlayerSettings;
using Color = UnityEngine.Color;

public class FenceManager : MonoBehaviour
{
    public GameObject fencePrefab;
    public GameObject doorPrefab;
    int fenceGap = 2;
    int lightFlag = 0;  //0,1,2,3 -> X,빨,노,초

    void Start()
    {
        List<string> doorNames = new List<string> { "Door_111", "Door_112", "Door_113", "Door_114", "Door_115" };
        List<Point> fencePoints = new List<Point> { new Point(39, 24), new Point(39, 2), new Point(93, 2), new Point(93, 24) };

        //List<Point> doorPoints = new List<Point> { new Point(39, 22), new Point(39, 15), new Point(50, 2), new Point(93, 10), new Point(50, 24) };
        List<Point> doorPoints = new List<Point>();

        //SkidPointsToFencePoints(fencePoints);
        DrawFence(fencePoints, doorPoints);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LightTest();
        }
    }
    void LightTest()
    {
        int colorIndex = lightFlag % 4;
        lightFlag++;
        lightFlag %= 4;

        GameObject LightG = GameObject.Find("LightGreen");
        GameObject LightR = GameObject.Find("LightRed");
        GameObject LightY = GameObject.Find("LightYellow");

        LightG.GetComponent<Light>().enabled = false;
        LightR.GetComponent<Light>().enabled = false;
        LightY.GetComponent<Light>().enabled = false;

        Light light;
        if (colorIndex == 1)
        {
            light = LightR.GetComponent<Light>();
        }
        else if (colorIndex == 2)
        {
            light = LightY.GetComponent<Light>();
        }
        else if (colorIndex == 3)
        {
            light = LightG.GetComponent<Light>();
        }
        else
        {
            return;
        }

        light.enabled = true;


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
            
            if (nowDoors.Count > 0)
            {
                // point1과 거리를 기준으로 정렬
                nowDoors = nowDoors.OrderBy(p => Vector3.Distance(new Vector3(point1.X, 0, point1.Y), p)).ToList();
            }
            DrawFencesBetween(p1, p2, nowDoors);
        }
    }
    void DrawFencesBetween(Vector3 p1, Vector3 p2, List<Vector3> doors)
    {
        Vector3 direction = (p1 - p2).normalized;                   // 방향벡터
        Quaternion rotation = Quaternion.LookRotation(direction);   // 회전: 자동 회전 감지
        float dist = Vector3.Distance(p1, p2);                      // 거리
        int segmentCount = Mathf.FloorToInt(dist / fenceGap);       // 몇개

        // 마지막에 반쪽 펜스 그리는가?
        bool xHalf = false;
        bool zHalf = false;
        if (direction.x == 0)       // 세로
        {
            if (Mathf.Abs(p1.z - p2.z) % 2 == 1)
                zHalf = true;
        }
        else if (direction.z == 0)  // 가로
        {
            if (Mathf.Abs(p1.x - p2.x) % 2 == 1)
                xHalf = true;
        }

        // 한칸씩 그리기
        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 pos = p1 - direction * (i * fenceGap);              // 한칸 위치
            Vector3 tmp = p1 - direction * ((i + 0.5f) * fenceGap);     // 반칸 더 간 위치
            bool isDrawDoor = false;
            if (doors.Count > 0 && doors[0] != null)
            {
                isDrawDoor = DrawSafeDoor(doors[0], direction, pos, tmp);// 안전문 그리면 true 반환
            }


            if (isDrawDoor)
            {
                doors.RemoveAt(0);
            }
            else
            {
                GameObject fence = Instantiate(fencePrefab, pos, rotation);
                fence.transform.SetParent(transform);
            }
        }

        // 반쪽 펜스 처리
        if (xHalf || zHalf)
        {
            Vector3 halfPos = p1 - direction * (segmentCount * fenceGap);
            GameObject halfFence = Instantiate(fencePrefab, halfPos, rotation);
            halfFence.transform.localScale = new Vector3(
                halfFence.transform.localScale.x,
                halfFence.transform.localScale.y,
                halfFence.transform.localScale.z * 0.5f
            );
            halfFence.name = $"FENCE_HALFHALFHALFHALFHALF";
            halfFence.transform.SetParent(transform);
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
    bool DrawSafeDoor(Vector3 doorVec, Vector3 direction, Vector3 pos, Vector3 tmp)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        if (direction.x == 0)// 세로로 그려짐
        {
            if (doorVec.z == pos.z)
            {
                GameObject door = Instantiate(doorPrefab, pos, rotation);
                door.transform.SetParent(transform);

                GameObject halfFence = Instantiate(fencePrefab, tmp, rotation);
                halfFence.transform.localScale = new Vector3(
                    halfFence.transform.localScale.x,
                    halfFence.transform.localScale.y,
                    halfFence.transform.localScale.z * 0.5f
                );
                halfFence.name = "FENCE_Z_WITH_DOOR";
                halfFence.transform.SetParent(transform);

                return true;
            }
            else if (doorVec.z == tmp.z)
            {
                GameObject halfFence = Instantiate(fencePrefab, pos, rotation);
                halfFence.transform.localScale = new Vector3(
                    halfFence.transform.localScale.x,
                    halfFence.transform.localScale.y,
                    halfFence.transform.localScale.z * 0.5f
                );
                halfFence.name = "FENCE_Z_WITH_DOOR";
                halfFence.transform.SetParent(transform);

                GameObject door = Instantiate(doorPrefab, tmp, rotation);
                door.transform.SetParent(transform);

                return true;
            }
        }
        else if (direction.z == 0)// 가로로 그려짐
        {
            if (doorVec.x == pos.x)
            {
                GameObject door = Instantiate(doorPrefab, pos, rotation);
                door.transform.SetParent(transform);

                GameObject halfFence = Instantiate(fencePrefab, tmp, rotation);
                halfFence.transform.localScale = new Vector3(
                    halfFence.transform.localScale.x,
                    halfFence.transform.localScale.y,
                    halfFence.transform.localScale.z * 0.5f
                );
                halfFence.name = "FENCE_X_WITH_DOOR";
                halfFence.transform.SetParent(transform);

                return true;
            }
            else if (doorVec.x == tmp.x)
            {
                GameObject halfFence = Instantiate(fencePrefab, pos, rotation);
                halfFence.transform.localScale = new Vector3(
                    halfFence.transform.localScale.x,
                    halfFence.transform.localScale.y,
                    halfFence.transform.localScale.z * 0.5f
                );
                halfFence.name = "FENCE_X_WITH_DOOR";
                halfFence.transform.SetParent(transform);

                GameObject door = Instantiate(doorPrefab, tmp, rotation);
                door.transform.SetParent(transform);
                return true;
            }
        }
        return false;
    }
}
