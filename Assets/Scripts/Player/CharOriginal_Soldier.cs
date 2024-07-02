using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharOriginal_Soldier : Character
{
    // 원거리 데이터
    public GunController equippedGun;
    private GunController savedGun;
    [SerializeField] private GunController SkillGun;

    // 원거리 캐릭터
    public void EquipGunWeapon(GunController gunToEquip)
    {
        equippedGun = gunToEquip;
    }

    // 무기장착은 무기선택창에서 함수실행 할 예정
    // 캐릭터 별로 스킬만 다르고 무기는 다양한 무기 선택가능하게 할 예정
    public void EquipWeapon(int weaponIndex)
    {
        EquipGunWeapon(itemData.AllGuns[weaponIndex]);

        // UI 창에 무기 표시해주기
        //UIManager.instance.selectWeapon(weaponIndex);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        equippedGun.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        equippedGun.gameObject.SetActive(false);
    }

    public override void GetAmmo(int ammo)
    {
        equippedGun.getAmmoPack(ammo);
        //savedGun.getAmmoPack(ammo);
    }

    //스킬
    protected override void UseSkill()
    {
        base.UseSkill();

        if(playerInput.skillSet1 && isSkillUse == false)
        {
            isSkillUse = true;
            savedGun = equippedGun;
            OnDisable();
            StartCoroutine(SwapMinigun());
        }
    }

    IEnumerator SwapMinigun()
    {
        equippedGun = SkillGun;
        OnEnable();
        yield return new WaitForSeconds(5f);
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
            UIManager.instance.UpdateGrenadeText(itemData.maxGrenade, hasGrenades);
            UIManager.instance.SetPlayer(ID,lv,uniqueSkillKind);
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
