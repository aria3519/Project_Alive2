using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3 : BossBase
{
    [SerializeField] private int skillMaxSpawnCount;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] private float moveSpeed = 5f;

    private float boss1RangeAttackTime = 6f;

    private bool isUsingSkill = false;
    private bool isMove = false;
    //   private void Awake()
    //{
    //    lookat = false;
    //}

    protected override void Start()
    {
        base.Start();
        bossInfo.text = "<Witch>";
       
        var skill = Instantiate(bossRangeSkill);
        bossRangeSkill = skill;
        bossRangeSkill.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (bossInfo)
        {
            bossInfo.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 7f);

        }
        if (playerEntity != null)
            playerPos = playerEntity.transform.position;

        var direction = (playerPos - transform.position).normalized;
        var rayPos = rigidbody.position;
        var dist = Vector3.Distance(rigidbody.position, playerPos);
        inRange = dist <= hasPlayerRange ? true : false;

        if (!dead && inRange)
        {
            if (!isUsingSkill && Physics.Raycast(rayPos, direction, hasPlayerRange, LayerMask.GetMask("Player", "NPC")))
            {
                hasPlayerRange = 25f;
                inRange = true;
                isMove = true;
                Debug.DrawLine(rigidbody.position, rigidbody.position + direction * hasPlayerRange, Color.blue);
                rigidbody.velocity = direction * moveSpeed;
            }

            if (Time.time >= lastRangeAttackTime + boss1RangeAttackTime)
            {
                isUsingSkill = true;
                lastRangeAttackTime = Time.time;
                isMove = false;
                rigidbody.velocity = Vector3.zero;

                if (SPAttackStack >= 4)
                {
                    SPAttackStack = 0;
                    SpecialAttack();
                }
                else
                {
                    skillDirection = direction;
                    RangeAttack();
                }
            }
        }
        else
        {
            if (dist > hasPlayerRange)
            {
                inRange = false;
                isMove = false;
                rigidbody.velocity = Vector3.zero;
            }
        }
        if (!dead)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
        //transform.rotation = Quaternion.LookRotation(direction);
        if (!dead && Physics.Raycast(rayPos, direction, out RaycastHit hit, 2f, LayerMask.GetMask("Player", "NPC")))
        {
            isMove = false;
            rigidbody.velocity = Vector3.zero;
            if (Time.time >= lastAttackTime + attackTime)
            {
                lastAttackTime = Time.time;
                LivingEntity livingEntity = hit.collider.GetComponent<LivingEntity>();

                bossAnimator.SetTrigger("Attack");
                if (livingEntity == null)
                    return;

                livingEntity.OnDamage(20f, livingEntity.transform.position, direction);
                SPAttackStack++;
            }
        }
        bossAnimator.SetBool("Move", isMove);
    }

    protected override void Stay(Vector3 direction)
    {
        return;
        // 아무것도 안해야됨
    }

    protected override void SpecialAttack()
    { 
        Debug.Log("SpecialAttack");
        bossAnimator.SetTrigger("SpecialAttack");

        // UI 에 웨이브 시작했다고 띄워주기
        // Sound 비명소리 넣어주기

        GameManager.instance.isWaveOn = true;
        SPAttack = true;
        EnemyManager.Instance.SpawnUnitsSkill(spawnPoints, skillMaxSpawnCount);
        isUsingSkill = false;
    }

    protected override void RangeAttack()
    {
        Debug.Log("RangeAttack");

        bossAnimator.SetTrigger("RangeAttack");
        //lastRangeAttackTime = Time.time;
        bossRangeSkill.transform.position = playerPos;
        bossRangeSkill.gameObject.SetActive(true);

        StartCoroutine(DisableRangeAttack(2.5f));
        SPAttackStack++;
    }

    protected override IEnumerator DisableRangeAttack(float time)
    {
        yield return new WaitForSeconds(time);
        bossRangeSkill.OnCollider();

        var bossEffect = EffectManager.Instance.GetBossEffect();
        bossEffect.transform.position = bossRangeSkill.transform.position;
        yield return new WaitForSeconds(0.1f);
        bossRangeSkill.OffCollider();
        bossRangeSkill.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        isUsingSkill = false;

        yield return new WaitForSeconds(2.8f);
        EffectManager.Instance.InsertBossEffect(bossEffect);
    }
    //protected override void Attack()
    //{
    //    Debug.Log("Attack");
    //    bossAnimator.SetTrigger("Attack");
    //    base.Attack();

    //}
    //protected override void RangeAttack()
    //{
    //    Debug.Log("RangeAttack");
    //    bossAnimator.SetTrigger("Attack");
    //    base.RangeAttack();
    //}


    //protected override void Stay()
    //{


    //    if(!SPAttack&&(health>=3000 && health<4000))
    //    {
    //        bossStates = BossStates.SpecialAttack;
    //    }
    //    base.Stay();
    //}

    // Z_Attack 애니메이션 Event에 필요한 함수.
    private void EndAttack()
    {
        bossAnimator.SetBool("Attack", false);
    }

    private void OnDrawGizmosSelected()
    {
        // 범위 그려주는 코드 
        Gizmos.DrawWireSphere(transform.position, hasPlayerRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}


