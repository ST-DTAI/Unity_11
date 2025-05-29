using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneLineRender : MonoBehaviour
{

    //크레인 줄 표시
    [Tooltip("크레인의 왼쪽 라인")]
    public LineRenderer line1_1;
    [Tooltip("크레인의 오른쪽 라인")]
    public LineRenderer line1_2;

    [Tooltip("크레인의 왼쪽 리프트 리턴 포인트")]
    public Transform returnPoint1_1;
    [Tooltip("크레인의 오른쪽 리프트 리턴 포인트")]
    public Transform returnPoint1_2;

    [Tooltip("크레인의 왼쪽 리프트 포인트")]
    public Transform liftPoint1_1;
    [Tooltip("크레인의 오른쪽 리프트 포인트")]
    public Transform liftPoint1_2;


    ////1번째 크레인
    //[Tooltip("1번 크레인의 왼쪽 라인")]
    //public LineRenderer line1_1;
    //[Tooltip("1번 크레인의 오른쪽 라인")]
    //public LineRenderer line1_2;

    //[Tooltip("1번 크레인의 왼쪽 리프트 리턴 포인트")]
    //public Transform returnPoint1_1; 
    //[Tooltip("1번 크레인의 오른쪽 리프트 리턴 포인트")]
    //public Transform returnPoint1_2;

    //[Tooltip("1번 크레인의 왼쪽 리프트 포인트")]
    //public Transform liftPoint1_1;
    //[Tooltip("1번 크레인의 오른쪽 리프트 포인트")]
    //public Transform liftPoint1_2;



    ////2번째 크레인
    //[Tooltip("2번 크레인의 왼쪽 라인")]
    //public LineRenderer line2_1;
    //[Tooltip("2번 크레인의 오른쪽 라인")]
    //public LineRenderer line2_2;

    //[Tooltip("2번 크레인의 왼쪽 리프트 리턴 포인트")]
    //public Transform returnPoint2_1; 
    //[Tooltip("2번 크레인의 오른쪽 리프트 리턴 포인트")]
    //public Transform returnPoint2_2;

    //[Tooltip("2번 크레인의 왼쪽 리프트 포인트")]
    //public Transform liftPoint2_1;
    //[Tooltip("2번 크레인의 오른쪽 리프트 포인트")]
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
