using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrivateSkillCool : MonoBehaviour
{
    public Image fill;
    public Text text;

    [SerializeField] SkillState skillState;
    [SerializeField] private float maxCooldown;
    private float currentCooldown;
    private bool skillCheck = false;
    private string usable = "Usable";

    private void Start()
    {
        maxCooldown = SkillManager.instance.GetSkillTime(skillState);
        currentCooldown = maxCooldown;
        text.text = usable;
        SetMaxCooldown(maxCooldown);
    }

    public void SetMaxCooldown(in float value)
    {
        maxCooldown = value;
        UpdateFiilAmount();
    }

    public void SetCurrentCooldown(in float value)
    {
        text.text = currentCooldown.ToString("F0");
        currentCooldown = value;
        UpdateFiilAmount();
    }

    private void UpdateFiilAmount()
    {
        fill.fillAmount = currentCooldown / maxCooldown;
    }

    // Test
    private void Update()
    {
        if (SkillManager.instance.isCharChanged) InitSetting();


        skillState = SkillManager.instance.currentSkillState;
        if (SkillManager.instance.isSkillUsed) skillCheck = SkillManager.instance.isSkillUsed;

        if (skillCheck)
        {
            SetCurrentCooldown(currentCooldown - Time.deltaTime);

            // Loop
            if (currentCooldown < 0f) InitSetting();
        }
    }

    private void InitSetting()
    {
        currentCooldown = maxCooldown;
        fill.fillAmount = 1;
        text.text = usable;
        skillCheck = false;
        SkillManager.instance.isSkillUsed = false;
        SkillManager.instance.isCharChanged = false;
    }
}
