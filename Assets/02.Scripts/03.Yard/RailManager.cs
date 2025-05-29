using System;
using UnityEngine;

public class RailManager : MonoBehaviour
{
    public GameObject railPrefab; // ��ġ�� ������
    public int railNumber = 11;// ��ġ�� �������� ����
    public float railSpacing = 10f; // ����

    public GameObject yardPrefab;
    public int yardNumber = 11;
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
    /// ���� ����� �Լ� 
    /// </summary>
    void PlaceRailPrefabs()
    {

        for (int i = 0; i < railNumber; i++)
        {
            Vector3 position = new Vector3(i * railSpacing, 0, 0); // X �������� spacing �������� ��ġ
            Instantiate(railPrefab, position, Quaternion.identity);
        }
    }

    /// <summary>
    /// ���� �ٴ� ����� �Լ�
    /// </summary>
    void PlaceYardPrefabs()
    {
        for (int i = 0; i < yardNumber; i++)
        {
            for (int j = 0; j < 5; j++) // Z �������� 5�� ��ġ
            {
                Vector3 position = new Vector3(i * yardSpacing, 0, j * yardSpacing); // X�� Z �������� spacing �������� ��ġ
                Instantiate(yardPrefab, position, Quaternion.identity);
            }
        }

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
