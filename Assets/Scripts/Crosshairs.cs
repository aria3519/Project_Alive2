using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;  // 레이어 마스크
    public SpriteRenderer dot;
    public Color dotHighlightColor;  // 적 인식 후 색상
    Color originalDotColor;  // 원래 색상

    private void Start()
    {
        Cursor.visible = false; // 마우스 커서 숨기기
        originalDotColor = dot.color;
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * (-40) * Time.deltaTime);
    }

    // 적이 있는지 판별
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
