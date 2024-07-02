using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 코드
public enum BossStates
{

    Stay,
    Attack,
    RangeAttack,
    SpecialAttack,
    Die
}

abstract public class BossBase : LivingEntity
{
    protected BossStates bossStates;
    protected LivingEntity playerEntity; // 플레이어 위치 
    [SerializeField] protected float hasPlayerRange = 1;
    [SerializeField] protected float attackRange = 1;

    [SerializeField] protected ParticleSystem hitEffect; // 피격시 재생할 파티클 효과
    [SerializeField] protected AudioClip deathSound; // 사망시 재생할 소리
    [SerializeField] protected AudioClip hitSound; // 피격시 재생할 소리

    protected Animator bossAnimator; // 애니메이터 컴포넌트
    protected AudioSource bossAudioPlayer; // 오디오 소스 컴포넌트
    protected Renderer bossRenderer; // 렌더러 컴포넌트
    protected CapsuleCollider bossCapsuleCollider;

    protected float lastStayTime; // 마지막으로 멈춘시간 
    protected float stayTime = 3f; // stay 3초 마다 
    protected float lastRangeAttackTime; // 마지막으로 범위 공격 한 시간 
    protected float lastAttackTime; // 마지막으로 근접공격 한 시간 
    protected float rangeAttackTime = 4f; // 3초마다 범위 공격 간격 
    protected float attackTime = 3f; // 5초마다 근접 공격 간격 

    //[SerializeField] protected GameObject bossSkill1; // 보스 스킬 1
    //[SerializeField] protected GameObject bossSkill2; // 보스 스킬 2

    [SerializeField] protected RangeSkill bossRangeSkill; // 보스 Range스킬

    //protected List<GameObject> listSkill1 = new List<GameObject>();
    //protected List<GameObject> listSkill2 = new List<GameObject>();

    protected Vector3 playerPos;


    protected bool useSpecialAttack;
    protected bool lookat = true;
    protected bool isPlayer = false;
    protected bool SPAttack = false;
    protected bool inRange = false;

    protected RaycastHit target;
    protected int SPAttackStack = 0;
    protected Vector3 skillDirection;
    public Transform damageFontPos;

   
    [SerializeField] protected Text bossInfo;
    



    /*[SerializeField] protected Slider Bosshealth; // 체력을 표시할 UI 슬라이더*/

    /*protected virtual void OnEnable()
    {
        health = startingHealth;
        // 게임 매니저에서 플레이어 정보 가져와야함 
        PlayerEntity = GameManager.instance.GetPlayer();
    }*/

    protected virtual void Start()
    {
        bossAnimator = GetComponent<Animator>();
        bossAudioPlayer = GetComponent<AudioSource>();
        bossRenderer = GetComponentInChildren<Renderer>();
        rigidbody = GetComponent<Rigidbody>();
        bossCapsuleCollider = GetComponent<CapsuleCollider>();

        bossStates = BossStates.Stay;
        health = startingHealth;
    }

    protected virtual void Stay(Vector3 direction)
    {   
        var rayPos = rigidbody.position + Vector3.up * 1f;
        var dist = Vector3.Distance(rigidbody.position, playerPos);
        inRange = dist <= hasPlayerRange ? true : false;


        if (Time.time >= lastStayTime + stayTime)
        {
            Debug.Log("Stay");

            if (inRange)
            {
                if (Physics.Raycast(rayPos, direction, out RaycastHit hit, attackRange, LayerMask.GetMask("Player", "NPC"))
                    && Time.time >= lastAttackTime + attackTime)
                {
                    lastAttackTime = Time.time;

                    Debug.DrawLine(rigidbody.position, rigidbody.position + direction * attackRange, Color.blue);
                    target = hit;
                    bossStates = BossStates.Attack;
                }
                else if (Physics.Raycast(rayPos, direction, hasPlayerRange, LayerMask.GetMask("Player", "NPC"))
                    && Time.time >= lastRangeAttackTime + rangeAttackTime)
                {
                    Debug.DrawLine(rigidbody.position, rigidbody.position + direction * hasPlayerRange, Color.blue);

                    lastRangeAttackTime = Time.time;

                    skillDirection = direction;
                    if (SPAttackStack >= 5)
                    {
                        SPAttackStack = 0;
                        bossStates = BossStates.SpecialAttack;
                    }
                    else
                        bossStates = BossStates.RangeAttack;
                }
            }
            lastStayTime = Time.time;
        }
    }

    protected virtual void Attack()
    {
        Debug.Log("Attack");

        lastAttackTime = Time.time;
        LivingEntity livingEntity = target.collider.GetComponent<LivingEntity>();

        bossAnimator.SetTrigger("Attack");
        if (livingEntity == null)
            return;

        livingEntity.OnDamage(20f, livingEntity.transform.position, rigidbody.transform.forward);
        bossStates = BossStates.Stay;
        SPAttackStack++;
        //bossStates = BossStates.Stay;
    }

    protected virtual void RangeAttack()
    {
        Debug.Log("RangeAttack");

        bossAnimator.SetTrigger("RangeAttack");
        //lastRangeAttackTime = Time.time;
        bossRangeSkill.transform.position = playerPos;
        bossRangeSkill.gameObject.SetActive(true);

        StartCoroutine(DisableRangeAttack(2.5f));

        bossStates = BossStates.Stay;
        SPAttackStack++;
    }

    //protected virtual void Stay()
    //{
    //    if (!isPlayer)
    //        return;

    //    var dir = (PlayerPoint - transform.position).normalized;
    //    transform.rotation = Quaternion.LookRotation(dir);

    //    if (Time.time >= lastStayTime + StayTime)
    //    {
    //        Debug.Log("Stay");
    //       /* BossAnimator.SetFloat("State", 0);*/

    //        // 상태가 변경 되기 위한 조건 
    //        // 라스트 타입 가지고 있어야함 
    //            PlayerPoint = PlayerEntity.transform.position; // 플레이어 위치
    //            PlayerPoint.y = 0.1f;
    //            // 플레이어 방향 보게 하는 코드 

    //            var dist = Vector3.Distance(PlayerPoint, transform.position);
    //        if (hasPlayerRange >= dist) // 지정된 범위 내에 플레이어만 공격 하게 
    //        {
    //            if (Lookat)
    //            {
    //                // rising 활성화 
    //                Debug.Log("rising");
    //                //BossAnimator.SetBool("Lookat", true);
    //                Lookat = false;
    //            }
    //            else
    //            {
    //                if (Time.time >= RangelastAttackTime + RangeAttackTime && AttackRange <= dist) // 범위 공격이 기본
    //                {
    //                    bossStates = BossStates.RangeAttack;
    //                }
    //                else if (AttackRange >= dist && Time.time >= AttacklastAttackTime + AttackTime)
    //                {
    //                    bossStates = BossStates.Attack;
    //                }
    //            }
    //        }
    //        lastStayTime = Time.time;
    //    }
    //}
    //protected virtual void Attack()
    //{

    //    if (0 < listSkill2.Count)
    //    {
    //        listSkill2[0].transform.position = PlayerPoint;
    //        listSkill2[0].SetActive(true);
    //    }
    //    else
    //    {
    //        var skill = Instantiate(BossSkill2);
    //        listSkill2.Add(skill);
    //        skill.transform.position = PlayerPoint;
    //        skill.SetActive(true);

    //    }
    //    StartCoroutine(Disable(listSkill2));
    //    bossStates = BossStates.Stay;
    //    AttacklastAttackTime = Time.time;
    //}
    //protected virtual void RangeAttack()
    //{

    //    if (0 < listSkill1.Count)
    //    {
    //        listSkill1[0].transform.position = PlayerPoint;
    //        listSkill1[0].SetActive(true);

    //    }
    //    else
    //    {
    //        var skill = Instantiate(BossSkill1);
    //        listSkill1.Add(skill);
    //        skill.transform.position = PlayerPoint;
    //        skill.SetActive(true);

    //    }
    //    StartCoroutine(Disable(listSkill1));

    //    bossStates = BossStates.Stay;
    //    RangelastAttackTime = Time.time;
    //}
    protected abstract void SpecialAttack();

    protected virtual void Update()
    {
       
        if (playerEntity != null)
            playerPos = playerEntity.transform.position;

        var direction = (playerPos - transform.position).normalized;
        //direction.y = 0;

        if (!dead)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            switch (bossStates)
            {
                case BossStates.Stay:
                    Stay(direction);
                    break;
                case BossStates.Attack:
                    Attack();
                    break;
                case BossStates.RangeAttack:
                    RangeAttack();
                    break;
                case BossStates.SpecialAttack:
                    SpecialAttack();
                    break;
            }
            //UIManager.instance.UpdateBossHealth(health);
        }
    }

    public override void Die()
    {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();
        bossInfo.gameObject.SetActive(false);
        
        // 보스 잡은 횟수 카운터 
        UIManager.instance.addBossKill();
        bossCapsuleCollider.enabled = false;
        //사망 애니메이션 재생
        //BossAnimator.SetBool("isDead",true);
        bossAnimator.SetTrigger("Dead");
        // 사망 효과음
        bossAudioPlayer.PlayOneShot(deathSound);
        rigidbody.velocity = Vector3.zero;
        StartCoroutine(DieTime());
    }

    private IEnumerator DieTime()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        //if (lookat)
        //    return;
        damage += GameManager.instance.DamagePlus();
        if (!dead)
        {
            // 공격받은 지점과 방향으로 파티클 효과 재생 
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            UIManager.instance.GetDamageText(damageFontPos, damage);
            //UIManager.instance.CheckDamage(true, damage);
        }
        // LivingEntity의 OnDamage()를 실행하여 데미지 적용
        base.OnDamage(damage, hitPoint, hitNormal);
       

       /* BossAnimator.SetTrigger("OnDamage");*/

    }

    // unityEvent 로 플레이어 정보 받아오는 코드
    public virtual void SerchPlayer(Character player)
    {
        if (player)
        {
            playerEntity = player;
            isPlayer = true;
        }
       
    }

    protected virtual IEnumerator DisableRangeAttack(float time)
    {
        yield return new WaitForSeconds(time);
        bossRangeSkill.OnCollider();
        yield return new WaitForSeconds(0.1f);
        bossRangeSkill.OffCollider();
        bossRangeSkill.gameObject.SetActive(false);
    }
    //private IEnumerator Disable(List<GameObject> skillList)
    //{ 
    //    yield return new WaitForSeconds(3.25f);
    //    skillList[0].SetActive(false);
    //}

}
