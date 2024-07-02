using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : BossBase
{
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float chargeDamage = 15f;
    [SerializeField] private ParticleSystem flameEffect;
    [SerializeField] private GameObject bossGrenadeVisible;
    [SerializeField] private Transform FireTransform;

    private float boss2RangeAttackTime = 8f;
    private bool isMove = false;
    private bool isUsingSkill = false;

    protected override void Start()
    {
        base.Start();
        bossInfo.text = "<Firefighter>";
        bossRangeSkill.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (bossInfo)
        {
            bossInfo.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 7f);

        }
        if (dead)
            return;

        if(playerEntity != null)
            playerPos = playerEntity.transform.position;

        var direction = (playerPos - transform.position + Vector3.up * 1f);
        direction.y = 0;
        direction = direction.normalized;
        var rayPos = rigidbody.position + Vector3.up * 1f;
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
                //rigidbody.MovePosition(rigidbody.position + direction * moveSpeed * Time.deltaTime);
                rigidbody.velocity = direction * moveSpeed;
            }

            if (Time.time >= lastRangeAttackTime + boss2RangeAttackTime)
            {
                isUsingSkill = true;
                lastRangeAttackTime = Time.time;
                isMove = false;
                //rigidbody.MovePosition(rigidbody.position);
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
                //rigidbody.MovePosition(rigidbody.position);
            }
        }
        if (!dead)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);


        if (Physics.Raycast(rayPos, direction, out RaycastHit hit, 2f, LayerMask.GetMask("Player", "NPC")))
        {
            isMove = false;
            //rigidbody.MovePosition(rigidbody.position);
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
        rigidbody.velocity = Vector3.zero;
        Debug.Log("RangeAttack");
        bossAnimator.SetBool("RangeAttack", true);
        bossRangeSkill.gameObject.SetActive(true);
        flameEffect.Play();
        SPAttackStack++;
        StartCoroutine(flameSkillTime());
    }

    private IEnumerator flameSkillTime()
    {
        yield return new WaitForSeconds(0.5f);
        bossRangeSkill.OnCollider();
        yield return new WaitForSeconds(2f);
        bossRangeSkill.OffCollider();
        bossRangeSkill.gameObject.SetActive(false);
        bossAnimator.SetBool("RangeAttack", false);
        rigidbody.velocity = Vector3.zero;
        isUsingSkill = false;
    }

    protected override void SpecialAttack()
    {
        rigidbody.velocity = Vector3.zero;
        Debug.Log("SpecialAttack");
        lastRangeAttackTime = Time.time;
        bossAnimator.SetTrigger("SpecialAttack");

        StartCoroutine(BossGrenadeWait());
    }

    private IEnumerator BossGrenadeWait()
    {
        yield return new WaitForSeconds(2f);
        Vector3 nextVec = playerPos - transform.position;
        nextVec.y = 5;

        var bossGrenade = ItemManager.instance.GetBossGrenade(FireTransform.position);
        Rigidbody rigidGrenade = bossGrenade.GetComponent<Rigidbody>();
        rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
        rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

        yield return new WaitForSeconds(1.5f);
        isUsingSkill = false;
    }

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


