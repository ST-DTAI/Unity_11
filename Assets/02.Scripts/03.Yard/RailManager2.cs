using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailManager2 : MonoBehaviour
{
   
    public GameObject railPrefab; // 배치할 프리팹
    public int railNumber = 30;// 배치할 프리팹의 개수
    public float railSpacing = 10f; // 간격

    public GameObject yardPrefab;
    public int yardNumber = 20;
    public float yardSpacing = 10f;

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
        PlaceYardPrefabs();
       // PlaceWallPrefabs();
    }

    /// <summary>
    /// 레일 세우는 함수 
    /// </summary>
    void PlaceRailPrefabs()
    {

        for (int i = 0; i < railNumber; i++)
        {
            Vector3 position = new Vector3(i * railSpacing, 0, 0); // X 방향으로 spacing 간격으로 배치
            Instantiate(railPrefab, position, Quaternion.identity);
        }
        for (int i = 0; i < railNumber; i++)
        {
            Vector3 position = new Vector3(i * railSpacing, 0, 40); // X 방향으로 spacing 간격으로 배치
            Instantiate(railPrefab, position, Quaternion.identity);
        }
    }

    /// <summary>
    /// 공장 바닥 만드는 함수
    /// </summary>
    void PlaceYardPrefabs()
    {
        for (int i = 0; i < yardNumber; i++)
        {
            for (int j = 0; j < 5; j++) // Z 방향으로 5개 배치
            {
                Vector3 position = new Vector3(i * yardSpacing, 0, j * yardSpacing); // X와 Z 방향으로 spacing 간격으로 배치
                Instantiate(yardPrefab, position, Quaternion.identity);
            }
        }

        for (int i = 0; i < yardNumber; i++)
        {
            for (int j = 0; j < 5; j++) // Z 방향으로 5개 배치
            {
                Vector3 position = new Vector3(i * yardSpacing, 0, j * yardSpacing +48); // X와 Z 방향으로 spacing 간격으로 배치
                Instantiate(yardPrefab, position, Quaternion.identity);
            }
        }

    }

    /// <summary>
    /// 공장 벽 만드는 함수
    /// </summary>
    void PlaceWallPrefabs()
    {

        //x축 방향으로 배치
        for (int i = 0; i < wallNumberX; i++)
        {
            Vector3 position = new Vector3(i * wallSpacing, 10f, -5f); // X 방향으로 spacing 간격으로 배치
            Instantiate(wallPrefab, position, Quaternion.identity);
        }

        for (int i = 0; i < wallNumberX; i++)
        {
            Vector3 position = new Vector3(i * wallSpacing, 10f, 45f);
            Instantiate(wallPrefab, position, Quaternion.identity);
        }


        //z축 방향으로 배치
        for (int j = 0; j < wallNumberZ; j++)
        {
            Vector3 position = new Vector3(-5f, 10f, j * wallSpacing);
            Quaternion rotation = Quaternion.Euler(0, 90, 0); // Y축 기준으로 90도 회전
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
    /// 만약 크레인도 프리팹화 한다면 사용할 함수(못해서 사용안함)
    /// </summary>
    void PlaceCranePrefabs()
    {
        for (int i = 0; i < craneNumber; i++)
        {
            Vector3 position = new Vector3(i * craneSpacing, 0, 0); // X 방향으로 spacing 간격으로 배치
            Instantiate(cranePrefab, position, Quaternion.identity);
        }
    }
}

