using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillCoolTime : MonoBehaviour
{
    [SerializeField] Text textCoolTime;
    [SerializeField] Image imageFill;
    [SerializeField] float coolTime;
    private float currentTime;
    private float startTime;
    private bool isEnd = true;

    private void Start()
    {
        imageFill.type = Image.Type.Filled;
        imageFill.fillMethod = Image.FillMethod.Radial360;
        imageFill.fillOrigin = (int)Image.Origin360.Top;
        imageFill.fillClockwise = false;
        TriggerSkill();
    }

    private void Update()
    {
        if (isEnd)
            return;
        CheckCoolTime();
    }


    private void CheckCoolTime()
    {
        currentTime = Time.time - startTime;
        if(currentTime < coolTime)
        {
            SetFillAmount(coolTime - currentTime);
        }
        else if(!isEnd)
        {
            EndCoolTime();
        }
    }

    private void EndCoolTime()
    {
        SetFillAmount(0);
        isEnd = true;
        textCoolTime.gameObject.SetActive(false);
        Debug.Log("Skills Available!");
    }

    private void TriggerSkill()
    {
        if(!isEnd)
        {
            Debug.LogError("Hold On");
            return;
        }

        ResetCoolTime();
        //Debug.LogError("Trigger_Skill!");
    }

    private void ResetCoolTime()
    {
        textCoolTime.gameObject.SetActive(true);
        currentTime = coolTime;
        startTime = Time.time;
        SetFillAmount(coolTime);
        isEnd = false;
    }

    private void SetFillAmount(float _value)
    {
        imageFill.fillAmount = _value / coolTime;
        string txt = _value.ToString("0.0");
        textCoolTime.text = txt;
        Debug.Log(txt);
    }
}
