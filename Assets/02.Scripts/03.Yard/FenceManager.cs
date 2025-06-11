using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using UnityEngine;


public class FenceManager : MonoBehaviour
{
    readonly int fenceGap = 2;          // 펜스 너비(=간격)
    public GameObject fencePrefab;
    public GameObject doorPrefab;
    List<GameObject> safeDoorObject = new List<GameObject>();

    List<string> doorNames = new List<string>();
    List<int> doorIndex = new List<int>();

    Dictionary<string, int> copyDoorStateDict = new Dictionary<string, int>();  // 복사본
    public float updateInterval = 0.5f; // 코루틴 업데이트 간격

    void Start()
    {
        InitializeFenceAndDoor();

        StartCoroutine(UpdateSafeDoorStatusCoroutine());
    }
    void OnDestroy()
    {
        foreach (GameObject door in safeDoorObject)
        {
            Destroy(door);
        }
        safeDoorObject.Clear();
    }
    void InitializeFenceAndDoor()
    {
        List<Point> fencePoints = new List<Point>();
        List<Point> doorPoints = new List<Point>();

        const string query = "SELECT TypeNo, Name, Pos1, Pos2 FROM unity_draw WHERE TypeNo LIKE 'Safe%';";
        using (MySqlCommand cmd = new MySqlCommand(query, DatabaseConnection.Instance.Connection))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    doorNames.Clear();
                    fencePoints.Clear();
                    doorPoints.Clear();

                    string typeNo = reader.GetString("TypeNo");

                    // Name 파싱
                    string nameJson = reader.IsDBNull("Name") ? null : reader.GetString("Name");
                    if (!string.IsNullOrWhiteSpace(nameJson))
                    {
                        var nameArray = JArray.Parse(nameJson);
                        foreach (var name in nameArray)
                        {
                            doorNames.Add(name.Value<string>());
                        }
                    }
                    else
                    {
                        Debug.Log($"[{typeNo}]: 안전문 이름이 없는 구역입니다.");
                    }


                    // Pos1 파싱
                    string pos1Json = reader.IsDBNull("Pos1") ? null : reader.GetString("Pos1");
                    if (!string.IsNullOrWhiteSpace(pos1Json))
                    {
                        // Pos1이 비어있지 않으면 파싱
                        var pos1Array = JArray.Parse(pos1Json);
                        foreach (var point in pos1Array)
                        {
                            int x = point[0].Value<int>();
                            int y = point[1].Value<int>();
                            fencePoints.Add(new Point(x, y));
                        }
                    }
                    else
                    {
                        Debug.LogError($"[{typeNo}]: 펜스 포인트가 없습니다.");
                        continue;
                    }

                    // Pos2 파싱
                    string pos2Json = reader.IsDBNull("Pos2") ? null : reader.GetString("Pos2");
                    if (!string.IsNullOrWhiteSpace(pos2Json))
                    {
                        var pos2Array = JArray.Parse(pos2Json);
                        foreach (var point in pos2Array)
                        {
                            int x = point[0].Value<int>();
                            int y = point[1].Value<int>();
                            doorPoints.Add(new Point(x, y));
                        }
                    }
                    else
                    {
                        Debug.Log($"[{typeNo}]: 안전문 포인트가 없는 구역입니다.");
                    }

                    if (doorNames.Count == doorPoints.Count)
                    {
                        DrawFence(fencePoints, doorPoints);
                    }
                    else
                    {
                        Debug.LogError($"[{typeNo}]: DB의 DoorNames 길이랑 DoorPoints 길이가 다릅니다.");
                    }
                }
            }
        }
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


            // index랑 같이 정렬되어야 하니까 튜플로 묶어서 List 만들기
            List<(Vector3 pos, int index)> doorData = new List<(Vector3, int)>();
            for (int d = 0; d < doors.Count; d++)
            {
                Point door = doors[d];
                if (IsBetween(door, point1, point2))
                {
                    Vector3 vec = new Vector3(door.X, 0, door.Y);
                    doorData.Add((vec, d));
                }
            }

            if (doorData.Count > 0)
            {
                // point1과 거리를 기준으로 정렬
                doorData = doorData
                    .OrderBy(item => Vector3.Distance(new Vector3(point1.X, 0, point1.Y), item.pos))
                    .ToList();
            }

            // 튜플 다시 분리
            List<Vector3> nowDoors = doorData.Select(item => item.pos).ToList();
            doorIndex = doorData.Select(item => item.index).ToList();

            // 펜스, 안전문 한쪽면만 그리기
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
                doorIndex.RemoveAt(0);
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

        //Debug.Log($"point {p} / between {a} and {b}");
        return true;
    }
    bool DrawSafeDoor(Vector3 doorVec, Vector3 direction, Vector3 pos, Vector3 tmp)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        if ((direction.x == 0 && doorVec.z == pos.z) || (direction.z == 0 && doorVec.x == pos.x))
        {
            GameObject door = Instantiate(doorPrefab, pos, rotation);
            door.transform.SetParent(transform);
            door.GetComponent<SafeDoor>().NameKey = doorNames[doorIndex[0]].Substring(4);
            safeDoorObject.Add(door);

            GameObject halfFence = Instantiate(fencePrefab, tmp, rotation);
            halfFence.transform.localScale = new Vector3(
                halfFence.transform.localScale.x,
                halfFence.transform.localScale.y,
                halfFence.transform.localScale.z * 0.5f
            );
            halfFence.name = "FENCE2_WITH_DOOR1";
            halfFence.transform.SetParent(transform);

            return true;
        }
        else if ((direction.x == 0 && doorVec.z == tmp.z) || (direction.z == 0 && doorVec.x == tmp.x))
        {
            GameObject halfFence = Instantiate(fencePrefab, pos, rotation);
            halfFence.transform.localScale = new Vector3(
                halfFence.transform.localScale.x,
                halfFence.transform.localScale.y,
                halfFence.transform.localScale.z * 0.5f
            );
            halfFence.name = "FENCE1_Z_WITH_DOOR2";
            halfFence.transform.SetParent(transform);

            GameObject door = Instantiate(doorPrefab, tmp, rotation);
            door.transform.SetParent(transform);
            door.GetComponent<SafeDoor>().NameKey = doorNames[doorIndex[0]].Substring(4);
            safeDoorObject.Add(door);

            return true;
        }
        return false;
    }
    private IEnumerator UpdateSafeDoorStatusCoroutine()
    {
        while (true)
        {
            lock (Global.dbLocks[(int)Global.DbLockType.DOORSTATE])
            {
                copyDoorStateDict.Clear();
                copyDoorStateDict = Global.DoorStateDict.ToDictionary(entry => entry.Key, entry => entry.Value);
            }

            foreach (var entry in copyDoorStateDict)
            {
                string name = entry.Key;
                int state = entry.Value;

                foreach (GameObject doorObject in safeDoorObject)
                {
                    if (doorObject.GetComponent<SafeDoor>().NameKey == name)
                    {
                        doorObject.GetComponent<SafeDoor>().SetState(state);
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }
}
