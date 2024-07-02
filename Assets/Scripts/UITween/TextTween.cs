using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class TextTween : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //RectTransform rt = GetComponent<RectTransform>();
        //rt.DOAnchorPosY(0, 1).SetDelay(1.5f).SetEase(Ease.OutElastic);

        Text txt = GetComponent<Text>();
        txt.DOText("프로젝트 얼라이브", 2, true, ScrambleMode.All).SetDelay(2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
