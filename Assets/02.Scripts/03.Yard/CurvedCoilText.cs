using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CurvedCoilText : MonoBehaviour
{
    public TMP_Text TMPText;

    public void SetCurvedText(string coilNo, float outDia)
    {
        string reversed = new string(coilNo.Reverse().ToArray());
        TMPText.text = reversed;

        int charCount = reversed.Length;
        float angleStep = 180f / Mathf.Max(1, charCount - 1);

        float margin = 0.05f; // 내경에서 떨어진 거리
        float radius = (outDia / 6f) + margin; // 외경만으로 위치 계산

        TMPText.ForceMeshUpdate();
        var mesh = TMPText.mesh;
        var vertices = mesh.vertices;

        for (int i = 0; i < charCount; i++)
        {
            var charInfo = TMPText.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            Vector3 charMid = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;

            float angleDeg = i * angleStep;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius, 0f);
            Quaternion rot = Quaternion.Euler(0, 0, angleDeg - 90f);

            for (int j = 0; j < 4; j++)
            {
                Vector3 offset = vertices[vertexIndex + j] - charMid;
                vertices[vertexIndex + j] = pos + rot * offset;
            }
        }

        mesh.vertices = vertices;
    }
}
