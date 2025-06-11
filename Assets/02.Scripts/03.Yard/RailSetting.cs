using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSetting : MonoBehaviour
{
    public Transform ColumnObect;
    public Transform Rail;
    public Transform RailSupport;

    public void SetRailTransform(float height, float dxSpacing)
    {
        ColumnObect.localScale = new Vector3(1, height - 1.5f, 1);

        Rail.position = new Vector3(Rail.position.x, ColumnObect.position.y + ColumnObect.localScale.y, Rail.position.z);
        RailSupport.position = new Vector3(RailSupport.position.x, ColumnObect.position.y + ColumnObect.localScale.y, RailSupport.position.z);

        Rail.localScale = new Vector3(dxSpacing, 1, 1);
        RailSupport.localScale = Vector3.one;
    }
}
