using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
    public LayerMask whatIsTarget;
    private LivingEntity targetEntity;
    private NavMeshAgent pathFinder;
    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;
    private Animator enemyAnimator;
    private AudioSource enemyAudioPlayer;
    private Renderer enemyRenderer;
    //private Rigidbody enemyRigidbody;

    public float damage = 20f;
    public float timeBetAttack = 2f;
    private float lastAttackTime;
    private bool isAttack = false;
    public EnemyKind enemyKind;

    public enum EnemyStates
    {
        IDLE,
        SAERCH
    }

    private bool hasTarget
    {
        get
        {
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }

            return false;
        }
    }


    private void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();

        enemyRenderer = GetComponentInChildren<Renderer>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor, EnemyKind enemyKind)
    {
        switch (enemyKind)
        {
            case EnemyKind.NormalZombie:
                {
                    startingHealth = newHealth;
                    health = newHealth;
                    damage = newDamage;

                    pathFinder.speed = newSpeed;
                    enemyRenderer.material.color = skinColor;
                    break;
                }
            case EnemyKind.FastZombie:
                {
                    startingHealth = newHealth * 0.5f;
                    health = newHealth * 0.5f;
                    damage = newDamage;

                    pathFinder.speed = newSpeed * 2f;
                    enemyRenderer.material.color = skinColor;
                    break;
                }
        }
    }

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    public void Restart()
    {
        if (!gameObject)
            return;

        pathFinder.enabled = true;
        StartCoroutine(UpdatePath());
    }


    private void Update()
    {
        enemyAnimator.SetBool("HasTarget", hasTarget);
        //if(!dead)
        //{
        //    switch (enemyStates)
        //    {
        //        case EnemyStates.IDLE:
        //            enemyAnimator.SetBool("HasTarget", false);
        //            break;
        //        case EnemyStates.SAERCH:
        //            enemyAnimator.SetBool("HasTarget", true);
        //            break;
        //    }

        //}
    }

    private IEnumerator UpdatePath()
    {
        while (pathFinder.enabled == false) yield return null;
        

        while (!dead)
        {
            targetEntity = null;
            pathFinder.isStopped = true;

            Collider[] colliders = Physics.OverlapSphere(transform.position, 10f, whatIsTarget);
            float shortestDistance = Mathf.Infinity;
            for (int i = 0; i < colliders.Length; i++)
            {
                LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                
                if (livingEntity != null)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, livingEntity.transform.position);
                    if (!livingEntity.dead && distanceToEnemy <= shortestDistance)
                    {
                        targetEntity = livingEntity;
                        pathFinder.SetDestination(targetEntity.transform.position);
                        pathFinder.isStopped = false;
                        
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            enemyAudioPlayer.PlayOneShot(hitSound);
        }

        base.OnDamage(damage, hitPoint, hitNormal);
    }

    public override void Die()
    {
        if (pathFinder.enabled == false)
            return;
        base.Die();

        //Collider[] enemyColliders = GetComponents<Collider>();
        //for (int i = 0; i < enemyColliders.Length; i++)
        //{
        //    enemyColliders[i].enabled = true;
        //}



        pathFinder.isStopped = true;
        pathFinder.enabled = false;

        enemyAnimator.SetTrigger("Die");

        enemyAudioPlayer.PlayOneShot(deathSound);
    }

    // Enemy Animator event 로 사용중
    private void EndAttack()
    {
        //yield return new WaitForSeconds(0.1f);
        enemyAnimator.SetBool("Attack", false);
        isAttack = false;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //Vector3 hitPosition = Vector3.zero;
    //    //Vector3 hitPoint = Vector3.zero;

    //    //// 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행  
    //    //if (other.CompareTag("Player") && Time.time >= lastAttackTime + timeBetAttack)
    //    //{
    //    //    lastAttackTime = Time.time;
    //    //    targetEntity.OnDamage(damage, hitPoint, hitPosition);
    //    //}

    //    if(!dead && Time.time >= lastAttackTime + timeBetAttack && isAttack == false)
    //    {
    //        LivingEntity attackTarget = collision.gameObject.GetComponent<LivingEntity>();

    //        if(attackTarget != null && attackTarget == targetEntity)
    //        {
    //            enemyAnimator.SetTrigger("New Trigger");
    //            enemyAnimator.SetBool("Attack", true);
    //            isAttack = true;
    //            lastAttackTime = Time.time;
    //            Vector3 hitPoint = collision.collider.ClosestPoint(transform.position);
    //            Vector3 hitNormal = transform.position - collision.transform.position;

    //            attackTarget.OnDamage(damage, hitPoint, hitNormal);
    //        }
    //    }
    //}

    private void OnTriggerStay(Collider collider)
    {
        //Vector3 hitPosition = Vector3.zero;
        //Vector3 hitPoint = Vector3.zero;

        //// 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행  
        //if (other.CompareTag("Player") && Time.time >= lastAttackTime + timeBetAttack)
        //{
        //    lastAttackTime = Time.time;
        //    targetEntity.OnDamage(damage, hitPoint, hitPosition);
        //}

        if (!dead && Time.time >= lastAttackTime + timeBetAttack && isAttack == false)
        {
            LivingEntity attackTarget = collider.gameObject.GetComponent<LivingEntity>();

            if (attackTarget != null && attackTarget == targetEntity)
            {
                enemyAnimator.SetTrigger("New Trigger");
                enemyAnimator.SetBool("Attack", true);
                isAttack = true;
                lastAttackTime = Time.time;
                Vector3 hitPoint = collider.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - collider.transform.position;

                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }

    // 데미지 받는거 종류별로 스킬별로 있는데 이부분은 어떻게 컴포넌트화 시킬지?
    //public void HitByGrenade(Vector3 explosionPos)
    //{
    //    Vector3 reactVec = transform.position - explosionPos;
    //    StartCoroutine(GrenadeOnDamage(reactVec));
    //    base.OnDamage(100, reactVec, reactVec);
    //}

    //IEnumerator GrenadeOnDamage(Vector3 reactVec)
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    reactVec = reactVec.normalized;
    //    reactVec += Vector3.up * 0.2f;
       
    //    enemyRigidbody.AddForce(reactVec * 5, ForceMode.Impulse);
    //}
}