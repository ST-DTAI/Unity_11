using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneLineRender : MonoBehaviour
{

    //ũ���� �� ǥ��
    [Tooltip("ũ������ ���� ����")]
    public LineRenderer line1_1;
    [Tooltip("ũ������ ������ ����")]
    public LineRenderer line1_2;

    [Tooltip("ũ������ ���� ����Ʈ ���� ����Ʈ")]
    public Transform returnPoint1_1;
    [Tooltip("ũ������ ������ ����Ʈ ���� ����Ʈ")]
    public Transform returnPoint1_2;

    [Tooltip("ũ������ ���� ����Ʈ ����Ʈ")]
    public Transform liftPoint1_1;
    [Tooltip("ũ������ ������ ����Ʈ ����Ʈ")]
    public Transform liftPoint1_2;


    ////1��° ũ����
    //[Tooltip("1�� ũ������ ���� ����")]
    //public LineRenderer line1_1;
    //[Tooltip("1�� ũ������ ������ ����")]
    //public LineRenderer line1_2;

    //[Tooltip("1�� ũ������ ���� ����Ʈ ���� ����Ʈ")]
    //public Transform returnPoint1_1; 
    //[Tooltip("1�� ũ������ ������ ����Ʈ ���� ����Ʈ")]
    //public Transform returnPoint1_2;

    //[Tooltip("1�� ũ������ ���� ����Ʈ ����Ʈ")]
    //public Transform liftPoint1_1;
    //[Tooltip("1�� ũ������ ������ ����Ʈ ����Ʈ")]
    //public Transform liftPoint1_2;



    ////2��° ũ����
    //[Tooltip("2�� ũ������ ���� ����")]
    //public LineRenderer line2_1;
    //[Tooltip("2�� ũ������ ������ ����")]
    //public LineRenderer line2_2;

    //[Tooltip("2�� ũ������ ���� ����Ʈ ���� ����Ʈ")]
    //public Transform returnPoint2_1; 
    //[Tooltip("2�� ũ������ ������ ����Ʈ ���� ����Ʈ")]
    //public Transform returnPoint2_2;

    //[Tooltip("2�� ũ������ ���� ����Ʈ ����Ʈ")]
    //public Transform liftPoint2_1;
    //[Tooltip("2�� ũ������ ������ ����Ʈ ����Ʈ")]
    //public Transform liftPoint2_2;

    void Start()
    {
        line1_1.positionCount = 2;
        line1_2.positionCount = 2;

        //line2_1.positionCount = 2;
        //line2_2.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        line1_1.SetPosition(0, liftPoint1_1.position);       
        line1_1.SetPosition(1, returnPoint1_1.position);

        line1_2.SetPosition(0, liftPoint1_2.position);
        line1_2.SetPosition(1, returnPoint1_2.position);



        //line2_1.SetPosition(0, liftPoint2_1.position);
        //line2_1.SetPosition(1, returnPoint2_1.position);

        //line2_2.SetPosition(0, liftPoint2_2.position);
        //line2_2.SetPosition(1, returnPoint2_2.position);
    }
}
