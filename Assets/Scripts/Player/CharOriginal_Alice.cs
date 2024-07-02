using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharOriginal_Alice : Character
{
    // ���� ĳ����

    // ĳ������ ���� : ü��, �ӵ�, ���ݷ�, ���ݼӵ�, ��ų(�����Ʈ�� ��������), ��ȯ������ ����
    // ������ٵ�, �÷��̾� ����

    //[SerializeField] private Transform weaponPivot;
    //[SerializeField] public Transform weaponHold;

    //[SerializeField] private MeleeController equippedMelee;
    //[SerializeField] private ItemData itemData;

    // ���� �޺�
    protected bool comboPossible;
    protected int comboStep;
    protected float lastAttackTime;

    // ���� ������
    [SerializeField] MeleeData meleeData;
    [SerializeField] protected MeleeController equippedMelee;
    [SerializeField] GameObject meleeCollider;
    [SerializeField] GameObject skillCollider;
    // ��ų ������
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

        // UI â�� ���� ǥ�����ֱ�
        //UIManager.instance.selectWeapon(weaponIndex);
    }

    // ��ų ���
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

                        // ������ box collider, capsule collider �� raycasthit �迭 �ϳ��� ���� ������ ������ �ΰ��� ����Ǿ
                        // ����Ʈ�� ����� ���������� raycasthit�� ����Ǽ� i+=2 �� ���ؼ� �ݶ��̴� �ϳ��� �پ�Ѱ� ����
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

        //    // ������ box collider, capsule collider �� raycasthit �迭 �ϳ��� ���� ������ ������ �ΰ��� ����Ǿ
        //    // ����Ʈ�� ����� ���������� raycasthit�� ����Ǽ� i+=2 �� ���ؼ� �ݶ��̴� �ϳ��� �پ�Ѱ� ����
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

    // ������ų
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


    // ���� ���� �޺�
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
        // �������⿡ �´� ui �����ϱ�
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
        // ���ݽ� �޺�üũ �� ���� ����
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
        // ��������� ���� ������ ����
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

        // ��ų �κ�
        //Grenade();
        //Flare();
    }
}