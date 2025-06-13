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
        if (string.IsNullOrEmpty(coilNo))
            return;

        float radius = (outDia / 6f) + 0.05f;   // 내경은 외경의 1/3정도, 내경 반지름은 1/2, 0.05f 정도 띄워서 쓰기
        string reversed = new string(coilNo.Reverse().ToArray());

        TMPText.text = reversed;
        TMPText.fontStyle = FontStyles.Bold;
        TMPText.ForceMeshUpdate();
        TMPText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        var mesh = TMPText.mesh;
        var textInfo = TMPText.textInfo;
        int charCount = textInfo.characterCount;
        float angleStep = 180f / Mathf.Max(1, charCount - 1);   // 180도로 글자 쓴다
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            var vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            if (vertexIndex + 3 >= vertices.Length)
            {
                Debug.LogWarning($"Character {i}의 vertex index 범위 초과: {vertexIndex}, vertices: {vertices.Length}");
                continue;
            }


            float angleDeg = i * angleStep;
            float angleRad = angleDeg * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius, 0f);
            Quaternion rot = Quaternion.Euler(0, 0, angleDeg - 90f);


            Vector3 charMid = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;
            for (int j = 0; j < 4; j++)
            {
                Vector3 offset = vertices[vertexIndex + j] - charMid;
                vertices[vertexIndex + j] = pos + rot * offset;
            }
        }

        TMPText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}
