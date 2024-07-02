using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : BossBase
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float chargeDamage = 15f;

    private float boss1RangeAttackTime = 6f;
    private bool isMove = false;

    private BoxCollider boxCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }

    protected override void Start()
    {
        base.Start();
        bossInfo.text = "<Tanker>";
      
        var skill = Instantiate(bossRangeSkill);
        bossRangeSkill = skill;
        bossRangeSkill.gameObject.SetActive(false);
    }
    //protected override void SpecialAttack()

    //protected override void RangeAttack()
    protected override void SpecialAttack()
    {
        Debug.Log("SpecialAttack");
        lastRangeAttackTime = Time.time;
        bossAnimator.SetTrigger("SpecialAttack");
        bossRangeSkill.transform.position = playerPos;
        bossRangeSkill.gameObject.SetActive(true);

        rigidbody.MovePosition(rigidbody.position + skillDirection * moveSpeed * Time.deltaTime);

        StartCoroutine(DisableRangeAttack(2f));

        //rigidbody.velocity = Vector3.zero;
    }
    protected override IEnumerator DisableRangeAttack(float time)
    {
        yield return new WaitForSeconds(time);
        EffectManager.Instance.boss1SPAttackEffect.transform.position = bossRangeSkill.transform.position;
        EffectManager.Instance.boss1SPAttackEffect.Play();
        bossRangeSkill.OnCollider();
        yield return new WaitForSeconds(0.1f);
        bossRangeSkill.OffCollider();
        bossRangeSkill.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (bossInfo)
        {
            bossInfo.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 7f);
           
        }
        // 데미지 ui
        //UIManager.instance.DamageText(transform.position);
        if (dead)
            return;

        if(playerEntity != null)
            playerPos = playerEntity.transform.position;

        var direction = (playerPos - transform.position).normalized;
        var rayPos = rigidbody.position;
        var dist = Vector3.Distance(rigidbody.position, playerPos);
        inRange = dist <= hasPlayerRange ? true : false;

        if (!dead && inRange)
        {
            if (Physics.Raycast(rayPos, direction, hasPlayerRange, LayerMask.GetMask("Player", "NPC")))
            {
                hasPlayerRange = 25f;
                inRange = true;
                isMove = true;
                Debug.DrawLine(rigidbody.position, rigidbody.position + direction * hasPlayerRange, Color.blue);
                rigidbody.velocity = direction * moveSpeed;
            }

            if (Time .time >= lastRangeAttackTime + boss1RangeAttackTime)
            {
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

        if(!dead )
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
    protected override void RangeAttack()
    {
        Debug.Log("RangeAttack");
        boxCollider.enabled = true;

        bossAnimator.SetBool("RangeAttack", true);
        SPAttackStack++;
        StartCoroutine(chargeSkillTime());
    }

    private IEnumerator chargeSkillTime()
    {
        yield return new WaitForSeconds(2f);
        bossAnimator.SetBool("RangeAttack", false);
        rigidbody.velocity = Vector3.zero;
    }

    // Z_Attack 애니메이션 Event에 필요한 함수.
    private void EndAttack()
    {
        bossAnimator.SetBool("Attack", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "NPC")
        {
            LivingEntity attackTarget = other.gameObject.GetComponent<LivingEntity>();
            attackTarget.OnDamage(chargeDamage, other.transform.position, other.transform.forward);
            rigidbody.velocity = Vector3.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 범위 그려주는 코드 
        Gizmos.DrawWireSphere(transform.position, hasPlayerRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    //protected override void RangeAttack()
    //{
    //    boxCollider.enabled = true;

    //    Debug.Log("SpecialAttack");
    //    bossAnimator.SetTrigger("RangeAttack");

    //    var direction = (playerPos - transform.position).normalized;
    //    direction.y = 0;

    //    var rayPos = transform.position + Vector3.up * 1f;


    //    if (Physics.Raycast(rayPos, direction, maxDist, LayerMask.GetMask("Player")))
    //    {
    //        Debug.DrawLine(rigidbody.position, rigidbody.position + direction * maxDist, Color.blue);
    //        rigidbody.velocity = direction * moveSpeed;
    //    }

    //    SPAttack = true;
    //    bossStates = BossStates.Stay;
    //    //if(Physics.Raycast(rayPos, direction, 1f, LayerMask.GetMask("Player")))
    //    //{
    //    //    rigidbody.velocity = Vector3.zero;
    //    //    bossStates = BossStates.Stay;
    //    //}
    //}
    //protected override void Attack()
    //{
    //    Debug.Log("Attack");
    //    bossAnimator.SetTrigger("Attack");
    //    base.Attack();
    //}
    //protected override void Stay()
    //{
    //    if (!SPAttack && (health >= 3000 && health < 4000))
    //    {
    //        bossStates = BossStates.SpecialAttack;
    //    }
    //    base.Stay();
    //}
}
