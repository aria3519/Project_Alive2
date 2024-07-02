using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlow : LivingEntity
{
    public LayerMask whatIsTarget;
    private Vector3 targetTransform;
    public ParticleSystem hitEffect;
    public ParticleSystem bodyExplosion;
    public AudioClip deathSound;
    public AudioClip hitSound;
    private Animator enemyAnimator;
   // private RuntimeAnimatorController enemyAnimatorController;
    private CapsuleCollider capsuleCollider;
    [SerializeField] private SkinnedMeshRenderer rend;

    public float damage = 20f;
    public float timeBetAttack = 2f;
    private float lastAttackTime;
    public float moveSpeed = 2f;
    public bool checkDead = false;
    private bool isMove = false;
    private bool inRange = false;
    private bool hasTarget = false;

    private float maxDist = 10f;
    public Transform damageFontPos;


    private void Awake()
    {
        enemyAnimator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        //enemyAnimatorController = GetComponent<RuntimeAnimatorController>();
        //health = startingHealth;
    }

    public void Updates(Vector3 flowfieldDir, Transform playerPos)
    {
        if (checkDead == true)
            return;
        UpdateMovement(flowfieldDir, playerPos);

        enemyAnimator.SetBool("HasTarget", isMove);
        // 애니메이션 및 회전

        //Debug.Log(rigidbody.velocity);
    }

    private void UpdateMovement(Vector3 flowfieldDir, Transform playerPos)
    {
        // Player방향 계산

        //var dist = Vector3.Distance(rigidbody.position, playerPos.position);
        var rayPos = rigidbody.position + Vector3.up * 1f;
        //inRange = dist <= maxDist ? true : false;
        //var moveDirection = inRange ? direction : flowfieldDir;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 10f, Vector3.up, 0f, LayerMask.GetMask("Player", "NPC"));

        if (hits.Length > 0)
        {
            // RaycastAll 의 값들이 거리순대로 sorting이 되어있다면?
            isMove = true;
            hasTarget = true;
            var targetPos = hits[0].transform.position;
            var closeDirection = (targetPos - transform.position).normalized;
            closeDirection.y = 0;

            Debug.DrawLine(rigidbody.position, rigidbody.position + closeDirection * maxDist, Color.blue);

            //rigidbody.rotation = Quaternion.LookRotation(direction);
            if (Vector3.zero != closeDirection)
                rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.LookRotation(closeDirection), 0.15f);
            rigidbody.velocity = closeDirection * moveSpeed;

            if (!dead && Physics.Raycast(rayPos, closeDirection, out RaycastHit hit, 1f, LayerMask.GetMask("Player", "NPC")))
            {
                isMove = false;
                rigidbody.velocity = Vector3.zero;
                if (Time.time >= lastAttackTime + timeBetAttack)
                {
                    lastAttackTime = Time.time;
                    LivingEntity livingEntity = hit.collider.GetComponent<LivingEntity>();

                    enemyAnimator.SetTrigger("New Trigger");
                    enemyAnimator.SetBool("Attack", true);
                    if (livingEntity == null)
                        return;

                    livingEntity.OnDamage(damage, livingEntity.transform.position, closeDirection);
                    rigidbody.rotation = Quaternion.LookRotation(closeDirection);
                }
            }
        }
        else
        {
            if (GameManager.instance.isWaveOn)
            {
                isMove = true;
                rigidbody.velocity = flowfieldDir * moveSpeed;
                if (Vector3.zero != flowfieldDir)
                    rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.LookRotation(flowfieldDir), 0.15f);
            }
            else
                isMove = false;

            hasTarget = false;
        }
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        damage += GameManager.instance.DamagePlus();
        if (!dead)
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            UIManager.instance.GetDamageText(damageFontPos, damage);

            AudioManager.Instance.PlayEnemyDeathSound(hitSound);
        }

        base.OnDamage(damage, hitPoint, hitNormal);
    }

    public override void Die()
    {
        base.Die();

        if(isAttackedExplosive)
        {
            rend.enabled = false;
            isAttackedExplosive = false;
            bodyExplosion.Play();
        }
        rigidbody.useGravity = false;
        enemyAnimator.SetTrigger("Die");
        capsuleCollider.enabled = false;
        isMove = false;
        AudioManager.Instance.PlayEnemyDeathSound(hitSound);
        rigidbody.velocity = Vector3.zero;
        StartCoroutine(activeTime(this));

    }

    private void EndAttack()
    {
        enemyAnimator.SetBool("Attack", false);
    }

    public void SetData(float _damage, float _moveSpeed, RuntimeAnimatorController _animatorController)
    {
        damage = _damage;
        moveSpeed = _moveSpeed;
        enemyAnimator.runtimeAnimatorController = _animatorController;
        //enemyAnimatorController = _animatorController;
    }

    private IEnumerator activeTime(EnemyFlow enemy)
    {
        enemy.checkDead = true;
        yield return new WaitForSeconds(3f);
        rend.enabled = true;
        EnemyManager.Instance.InsertQueue(enemy); // enemyPool 에 반납
        EnemyManager.Instance.RemoveList(enemy); // 죽을때 Update용 리스트에서 삭제
        //unitsInGame.RemoveAt(listIndex);
    }
    
    public void InitInfo()
    {
        rigidbody.useGravity = true;
        dead = false;
        checkDead = false;
        health = startingHealth;
        capsuleCollider.enabled = true;
    }
}


//public void Updates(Vector3 flowfieldDir, Transform playerPos)
//{
//    if (checkDead == true)
//        return;

//    // Player방향 계산
//    var direction = (playerPos.position - rigidbody.position).normalized;
//    direction.y = 0;

//    var dist = Vector3.Distance(rigidbody.position, playerPos.position);
//    var rayPos = rigidbody.position + Vector3.up * 1f;
//    inRange = dist <= maxDist ? true : false;
//    var moveDirection = inRange ? direction : flowfieldDir;

//    if (inRange)
//    {
//        if (Physics.Raycast(rayPos, direction, maxDist, LayerMask.GetMask("Player")))
//        {
//            inRange = true;
//            //maxDist = 20f;
//            //Debug.DrawLine(rigidbody.position, rigidbody.position + moveDirection * maxDist, Color.blue);
//            isMove = true;
//            rigidbody.velocity = moveDirection * moveSpeed;
//            //rigidbody.rotation = Quaternion.LookRotation(moveDirection);
//        }
//    }
//    else
//    {
//        if (GameManager.instance.isWaveOn)
//        {
//            isMove = true;
//            rigidbody.velocity = moveDirection * moveSpeed;
//            //rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.LookRotation(moveDirection), 0.1f);
//        }
//        else if (dist > maxDist)
//        {
//            inRange = false;
//            isMove = false;
//            rigidbody.velocity = Vector3.zero;
//        }
//    }

//    if (Vector3.zero != moveDirection)
//        rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.LookRotation(moveDirection), 0.05f);
//    //Debug.DrawLine(rayPos, rayPos + direction, Color.green);
//    // 적 공격
//    if (!dead && Physics.Raycast(rayPos, direction, out RaycastHit hit, 1f, 1 << LayerMask.GetMask("Player") | LayerMask.GetMask("NPC")))
//    {
//        isMove = false;
//        rigidbody.velocity = Vector3.zero;
//        if (Time.time >= lastAttackTime + timeBetAttack)
//        {
//            lastAttackTime = Time.time;
//            LivingEntity livingEntity = hit.collider.GetComponent<LivingEntity>();

//            enemyAnimator.SetTrigger("New Trigger");
//            enemyAnimator.SetBool("Attack", true);
//            if (livingEntity == null)
//                return;

//            livingEntity.OnDamage(damage, livingEntity.transform.position, direction);
//            rigidbody.rotation = Quaternion.LookRotation(direction);
//        }
//    }

//    enemyAnimator.SetBool("HasTarget", isMove);
//    // 애니메이션 및 회전

//    //Debug.Log(rigidbody.velocity);
//}