using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;  // ���̾� ����ũ
    public SpriteRenderer dot;
    public Color dotHighlightColor;  // �� �ν� �� ����
    Color originalDotColor;  // ���� ����

    private void Start()
    {
        Cursor.visible = false; // ���콺 Ŀ�� �����
        originalDotColor = dot.color;
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * (-40) * Time.deltaTime);
    }

    // ���� �ִ��� �Ǻ�
    public void DetectTargets(Ray ray)
    {
        if(Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColor;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }
}
