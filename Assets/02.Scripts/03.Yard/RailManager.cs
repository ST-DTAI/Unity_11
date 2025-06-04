using System;
using UnityEngine;

public class RailManager : MonoBehaviour
{
    public GameObject railPrefab;
    public int railNumber = 11;
    public float railSpacing = 10f;


    public GameObject floorPrefab;

    public GameObject cranePrefab;
    public int craneNumber = 2;
    public float craneSpacing = 20f;

    public GameObject wallPrefab;
    public int wallNumberX = 11;
    public int wallNumberZ = 5;
    public float wallSpacing = 10f;

    void Start()
    {
        PlaceRailPrefabs();
        PlaceFloorPrefabs();
        // PlaceWallPrefabs();
    }


    void PlaceRailPrefabs()
    {

        for (int i = 0; i < railNumber; i++)
        {
            Vector3 position = new Vector3(i * railSpacing, 0, 0);
            GameObject newRail = Instantiate(railPrefab, position, Quaternion.identity);

            // 부모를 RailManager로 설정
            newRail.transform.SetParent(transform, false);
        }
    }

    /// <summary>
    /// 바닥 프리팹 하나를 크게 만들어 배치하는 함수
    /// </summary>
    void PlaceFloorPrefabs()
    {
        // 바닥 배치
        GameObject floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);
        floor.transform.SetParent(transform, false);
        float offsetX = 5f;  // X축 여유
        float offsetZ = 5f;  // Z축 여유

        // railNumber에 따라 X축 전체 길이 계산 (양쪽 offset 포함)
        float floorWidth = railSpacing * (railNumber - 1) + offsetX * 2f;

        // Z축 전체 길이 계산 (양쪽 offset 포함)
        float floorLength = wallSpacing * (wallNumberZ - 1) + offsetZ * 2f;

        // Plane prefab일 경우 기본 크기가 10x10이라서 10으로 나눔
        floor.transform.localScale = new Vector3(floorWidth / 10f, 1f, floorLength / 10f);

        // 바닥 위치: -offset 만큼 이동한 후 중앙 배치
        float floorPosX = -offsetX + (floorWidth / 2f);
        float floorPosZ = -offsetZ + (floorLength / 2f);

        floor.transform.position = new Vector3(floorPosX, 0, floorPosZ);
    }
        /// <summary>
        /// ���� �� ����� �Լ�
        /// </summary>
        void PlaceWallPrefabs()
    {

        //x�� �������� ��ġ
        for (int i = 0; i < wallNumberX; i++)
        {
            Vector3 position = new Vector3(i * wallSpacing, 10f, -5f); // X �������� spacing �������� ��ġ
            Instantiate(wallPrefab, position, Quaternion.identity);
        }

        for (int i = 0; i < wallNumberX; i++)
        {
            Vector3 position = new Vector3(i * wallSpacing, 10f, 45f);
            Instantiate(wallPrefab, position, Quaternion.identity);
        }


        //z�� �������� ��ġ
        for (int j = 0; j < wallNumberZ; j++)
        {
            Vector3 position = new Vector3(-5f, 10f, j * wallSpacing);
            Quaternion rotation = Quaternion.Euler(0, 90, 0); // Y�� �������� 90�� ȸ��
            Instantiate(wallPrefab, position, rotation);
        }

        for (int j = 0; j < wallNumberZ; j++)
        {
            Vector3 position = new Vector3(105f, 10f, j * wallSpacing);
            Quaternion rotation = Quaternion.Euler(0, 90, 0);
            Instantiate(wallPrefab, position, rotation);
        }
    }




    /// <summary>
    /// ���� ũ���ε� ������ȭ �Ѵٸ� ����� �Լ�(���ؼ� ������) ����!!
    /// </summary>
    void PlaceCranePrefabs()
    {
        for (int i = 0; i < craneNumber; i++)
        {
            Vector3 position = new Vector3(i * craneSpacing, 0, 0); // X �������� spacing �������� ��ġ
            Instantiate(cranePrefab, position, Quaternion.identity);
        }
    }
}
