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
        // ���Ϲ�ȣ ��
        if (string.IsNullOrEmpty(coilNo))
            return;

        string reversed = new string(coilNo.Reverse().ToArray());
        TMPText.text = reversed;
        TMPText.ForceMeshUpdate();

        // ������ �ܰ��� 1/3����, ���� �������� 1/2, 0.05f ���� ����� ����
        float radius = (outDia / 6f) + 0.05f;
        int charCount = reversed.Length;
        float angleStep = 180f / Mathf.Max(1, charCount - 1);   // 180���� ���� ����
        var mesh = TMPText.mesh;
        var vertices = mesh.vertices;

        for (int i = 0; i < charCount; i++)
        {
            var charInfo = TMPText.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            if (vertexIndex + 3 >= vertices.Length)
            {
                Debug.LogWarning($"Character {i}�� vertex index ���� �ʰ�: {vertexIndex}, vertices: {vertices.Length}");
                continue;
            }

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
