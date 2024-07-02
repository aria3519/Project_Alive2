using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharOriginal_Alice : Character
{
    // 근접 캐릭터

    // 캐릭터의 정보 : 체력, 속도, 공격력, 공격속도, 스킬(무브먼트의 닷지포함), 소환가능한 유닛
    // 리지드바디, 플레이어 웨폰

    //[SerializeField] private Transform weaponPivot;
    //[SerializeField] public Transform weaponHold;

    //[SerializeField] private MeleeController equippedMelee;
    //[SerializeField] private ItemData itemData;

    // 근접 콤보
    protected bool comboPossible;
    protected int comboStep;
    protected float lastAttackTime;

    // 근접 데이터
    [SerializeField] MeleeData meleeData;
    [SerializeField] protected MeleeController equippedMelee;
    [SerializeField] GameObject meleeCollider;
    [SerializeField] GameObject skillCollider;
    // 스킬 데이터
    protected float alphaLastSkillTime;
    protected float alphaSkillTime = 5f;

    //public float WeaponHeight
    //{
    //    get
    //    {
    //        return weaponHold.position.y;
    //    }
    //}
    public override void Awake()
    {
        SkillManager.instance.SetSkillTime(SkillState.ALPHA, alphaSkillTime);
        uniqueSkillKind = 3;
        base.Awake();
    }

    public void EquipMeleeWeapon(MeleeController meleeToEquip)
    {
        equippedMelee = meleeToEquip;
    }

    public void EquipWeapon(int weaponIndex)
    {
        EquipMeleeWeapon(itemData.AllMelee[weaponIndex]);

        // UI 창에 무기 표시해주기
        //UIManager.instance.selectWeapon(weaponIndex);
    }

    // 스킬 사용
    protected override void UseSkill()
    {
        base.UseSkill();
        float maxDistance = 10f;
        int maxCount = 0;

        if(playerInput.skillSet1)
        {
            switch(uniqueSkillKind)
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
                    if(false == isSkillUse && Time.time >= alphaLastSkillTime + alphaSkillTime)
                    {
                        alphaLastSkillTime = Time.time;
                        SkillManager.instance.isSkillUsed = true;

                        isSkillUse = true;
                        RaycastHit[] enemies = Physics.SphereCastAll(transform.position
                            , maxDistance, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

                        // 몬스터의 box collider, capsule collider 가 raycasthit 배열 하나당 같은 몬스터의 정보가 두개씩 저장되어서
                        // 리스트를 만들고 순차적으로 raycasthit에 저장되서 i+=2 를 통해서 콜라이더 하나를 뛰어넘게 만듦
                        List<RaycastHit> temp = new List<RaycastHit>();
                        for (int i = 0; enemies.Length > i; i += 2)
                        {
                            if (maxCount >= 10)
                                break;

                            temp.Add(enemies[i]);
                            maxCount++;
                        }

                        playerCollider.enabled = false;
                        rigidbody.useGravity = false;
                        StartCoroutine(UseAlpha(temp.ToArray()));
                        rigidbody.velocity = Vector3.zero;
                        //StartCoroutine(UseAlpha(enemies));
                        Debug.Log("Alpha Skill Used");
                    }
                    break;
            }
        }

        //if (playerInput.skillSet1 && false == isSkillUse &&
        //    Time.time >= alphaLastSkillTime + alphaSkillTime)
        //{
        //    alphaLastSkillTime = Time.time;
        //    SkillManager.instance.isSkillUsed = true;

        //    isSkillUse = true;
        //    RaycastHit[] enemies = Physics.SphereCastAll(transform.position
        //        , maxDistance, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        //    // 몬스터의 box collider, capsule collider 가 raycasthit 배열 하나당 같은 몬스터의 정보가 두개씩 저장되어서
        //    // 리스트를 만들고 순차적으로 raycasthit에 저장되서 i+=2 를 통해서 콜라이더 하나를 뛰어넘게 만듦
        //    List<RaycastHit> temp = new List<RaycastHit>();
        //    for (int i = 0; enemies.Length > i; i += 2)
        //    {
        //        if (maxCount >= 10)
        //            break;

        //        temp.Add(enemies[i]);
        //        maxCount++;
        //    }

        //    playerCollider.enabled = false;
        //    rigidbody.useGravity = false;
        //    StartCoroutine(UseAlpha(temp.ToArray()));
        //    rigidbody.velocity = Vector3.zero;
        //    //StartCoroutine(UseAlpha(enemies));
        //    Debug.Log("Skill Used");
        //}

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10f);
    }

    IEnumerator UseAlpha(RaycastHit[] enemies)
    {
        if (enemies.Length == 0)
        {
            playerCollider.enabled = true;
            rigidbody.useGravity = true;
            isSkillUse = false;
            yield break;
        }

        playerCollider.enabled = false;
        Vector3 savePos = transform.position;

        foreach (RaycastHit hitObj in enemies)
        {
            var enemy = hitObj.transform.GetComponent<LivingEntity>();
            if (enemy && false == enemy.isDead())
            {
                transform.position = enemy.transform.position;
                enemy.OnDamage(100, transform.position, transform.forward);
                //playerAnimator.Play("Attack1");
                yield return new WaitForSeconds(0.25f);
            }
        }

        //for(int i = 0; i < enemies.Length; i++)
        //{
        //    transform.position = enemies[i].transform.position;
        //    enemies[i].GetComponent<Enemy>().OnDamage(500, transform.position, transform.forward);
        //    playerAnimator.SetTrigger("Attack1");
        //    yield return new WaitForSeconds(0.5f);
        //}
        //yield return new WaitForSeconds(2f);
        transform.position = savePos;

        yield return new WaitForSeconds(1f);
        isSkillUse = false;
        playerCollider.enabled = true;
        rigidbody.useGravity = true;
    }

    // 닷지스킬
    protected override IEnumerator Dodge(Vector3 moveinput)
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashTime &&
             !Physics.Raycast(transform.position, moveinput, 5f, ~(1 << LayerMask.GetMask("Enemy"))))
        {
            rigidbody.MovePosition(rigidbody.position + moveinput * dashSpeed * Time.deltaTime);
            yield return null;
        }

        skillCollider.gameObject.transform.rotation = Quaternion.LookRotation(-moveinput);
        skillCollider.gameObject.transform.rotation = Quaternion.LookRotation(-moveinput);
        skillCollider.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        playerCollider.enabled = true;
        rigidbody.useGravity = true;
        skillCollider.gameObject.SetActive(false);
    }


    // 근접 공격 콤보
    public void ComboPossible()
    {
        comboPossible = true;
    }
    public void Combo()
    {
        if (comboStep == 2)
            playerAnimator.SetTrigger("Attack2");
        //if (comboStep == 3)
        //    playerAnimator.SetTrigger("Attack3");
    }
    public void ComboReset()
    {
        comboPossible = false;
        comboStep = 0;
    }

    protected override void CharacterSwap()
    {
        base.CharacterSwap();
        if (playerInput.swap)
        {
            comboStep = 0;
            meleeCollider.SetActive(false);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        equippedMelee.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        equippedMelee.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateHealthText(health, startingHealth);
            UIManager.instance.UpdateSpText(spPoint, startingSpPoint);
            UIManager.instance.UpdateGrenadeText(itemData.maxGrenade, hasGrenades);
            UIManager.instance.SetPlayer(ID, lv, uniqueSkillKind);
        }
        // 근접무기에 맞는 ui 설정하기
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateAmmoText(0, 0);
            UIManager.instance.UpdateFlareText(0, 0);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        weaponPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, equippedMelee.leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, equippedMelee.leftHandMount.rotation);

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, equippedMelee.rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, equippedMelee.rightHandMount.rotation);
    }
    

    

    private void Update()
    {
        // 공격시 콤보체크 및 공격 실행
        if (playerInput.fire)
        {
            if (Time.time >= lastAttackTime + meleeData.TimeBetAttack)
            {
                lastAttackTime = Time.time;

                if (comboStep == 0)
                {
                    playerAnimator.SetTrigger("Attack");
                    comboStep = 1;
                    return;
                }
                if (comboStep != 0)
                {
                    if (comboPossible)
                    {
                        comboPossible = false;
                        comboStep += 1;
                    }
                }

                SkillManager.instance.UseSkill(playerInput.mousePoint, 10,100);
            }
        }
        UseSkill();
        ThrowGrenade();
        ThrowFlare();
        CharacterSwap();
        UpdateUI();
        // 근접무기는 무기 스왑이 없음
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

        // 스킬 부분
        //Grenade();
        //Flare();
    }
}