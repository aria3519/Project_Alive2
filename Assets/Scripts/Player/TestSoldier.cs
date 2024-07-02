using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSoldier : Character
{
    // 원거리 데이터
    public TestGunController equippedGun;
    private TestGunController savedGun;
    [SerializeField] private TestGunController[] SkillGuns;

    // 스킬 데이터
    protected float minigunLastSkillTime;
    protected float minigunSkillTime = 5f;
    protected float minigunUseTime = 5f;

    protected float flameLastSkillTime;
    protected float flameSkillTime = 5f;
    protected float flameUseTime = 10f;
    // 원거리 캐릭터

    public override void Awake()
    {
        SkillManager.instance.SetSkillTime(SkillState.MINIGUN, minigunSkillTime);
        SkillManager.instance.SetSkillTime(SkillState.FLAMETHROWER, flameUseTime);
        uniqueSkillKind = 4;
        base.Awake();
    }

    public void EquipGunWeapon(TestGunController gunToEquip)
    {
        equippedGun = gunToEquip;
    }

    // 무기장착은 무기선택창에서 함수실행 할 예정
    // 캐릭터 별로 스킬만 다르고 무기는 다양한 무기 선택가능하게 할 예정

    protected override void OnEnable()
    {
        base.OnEnable();
        equippedGun.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        equippedGun.gameObject.SetActive(false);
    }

    public override void ResetCharacter()
    {
        base.ResetCharacter();

        if (savedGun)
        {
            isSkillUse = false;
            OnDisable();
            equippedGun = savedGun;
            OnEnable();
        }
        equippedGun.ResetAmmo();
    }

    //스킬
    protected override void UseSkill()
    {
        base.UseSkill();

        if (playerInput.skillSet1)
        {
            switch (uniqueSkillKind)
            {
                case 0:
                    Debug.Log("No Available Skill");
                    break;
                case 1:
                    if (false == isSkillUse && Time.time >= healLastSkillTime + healSkillTime)
                    {
                        healLastSkillTime = Time.time;
                        UseHealSKill();
                        Debug.Log("heal Skill Used");
                    }
                    break;
                case 2:
                    if (false == isSkillUse && Time.time >= speedLastSkillTime + speedSkillTime)
                    {
                        speedLastSkillTime = Time.time;
                        speedEffect.Play();
                        isFastSkill = true;
                        Debug.Log("speedup Skill Used");
                    }
                    break;
                case 3:
                    if (false == isSkillUse && Time.time >= minigunLastSkillTime + minigunSkillTime + minigunUseTime)
                    {
                        minigunLastSkillTime = Time.time;
                        UIManager.instance.PrivateSkillUse();

                        isSkillUse = true;
                        savedGun = equippedGun;
                        OnDisable();
                        StartCoroutine(SwapMinigun());
                    }
                    break;
                case 4:
                    if (false == isSkillUse && Time.time >= flameLastSkillTime + flameSkillTime + flameUseTime)
                    {
                        flameLastSkillTime = Time.time;
                        UIManager.instance.PrivateSkillUse();

                        isSkillUse = true;
                        savedGun = equippedGun;
                        OnDisable();
                        StartCoroutine(SwapFlameThrower());
                    }
                    break;
            }
        }
    }

    IEnumerator SwapMinigun()
    {
        equippedGun = SkillGuns[0];
        OnEnable();
        yield return new WaitForSeconds(minigunUseTime);
        UIManager.instance.PrivateSkillCool();
        SkillManager.instance.isSkillUsed = true;
        SkillGuns[0].ResetAmmo();
        OnDisable();
        equippedGun = savedGun;
        OnEnable();
        isSkillUse = false;
    }

    IEnumerator SwapFlameThrower()
    {
        equippedGun = SkillGuns[1];
        OnEnable();
        yield return new WaitForSeconds(flameUseTime);
        UIManager.instance.PrivateSkillCool();
        SkillManager.instance.isSkillUsed = true;
        OnDisable();
        equippedGun = savedGun;
        OnEnable();
        isSkillUse = false;
    }

    private void UpdateUI()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateHealthText(health,startingHealth);
            UIManager.instance.UpdateSpText(spPoint, startingSpPoint);
            UIManager.instance.UpdateGrenadeText(itemData.maxGrenade, hasGrenades);
            UIManager.instance.SetPlayer(ID,lv, uniqueSkillKind);
        }

        if (equippedGun != null && UIManager.instance != null)
        {
            UIManager.instance.UpdateAmmoText(equippedGun.getMagAmmo(), equippedGun.getAmmoRemain());
            //UIManager.instance.UpdateFlareText(maxFlare, hasFlare);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        weaponPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, equippedGun.leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, equippedGun.leftHandMount.rotation);

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, equippedGun.rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, equippedGun.rightHandMount.rotation);
    }

    private void Update()
    {
        if (playerInput.fire)
        {
            equippedGun.Fire();
        }
        else if (playerInput.reload)
        {
            UIManager.instance.reloadAlarm.gameObject.SetActive(false);
            if (equippedGun.Reload())
            {
                playerAnimator.SetTrigger("Reload");
            }
        }

        // 1,2,3 스킬 넣을 예정
        //if (playerInput.gunChange1)
        //{
        //    OnDisable();
        //    EquipWeapon(0);
        //    playerAnimator.SetTrigger("Reload");
        //}
        //else if (playerInput.gunChange2)
        //{
        //    OnDisable();
        //    EquipWeapon(1);
        //    playerAnimator.SetTrigger("Reload");
        //}
        //else if (playerInput.gunChange3)
        //{
        //    OnDisable();
        //    EquipWeapon(2);
        //    playerAnimator.SetTrigger("Reload");
        //}
        //OnEnable();

        ThrowGrenade();
        ThrowFlare();
        CharacterSwap();
        UpdateUI();
        UseSkill();
    }
}
