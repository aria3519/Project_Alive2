using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine.AI; // AI, 내비게이션 시스템 관련 코드를 가져오기

public class BossMonster : LivingEntity
{
   /* public enum States 
    { 
         
        Stay, 
        Attack, 
        Die
    }*/

   
    [SerializeField] public LayerMask whatIsTarget; // 추적 대상 레이어

    private LivingEntity targetEntity; // 추적할 대상
    //private NavMeshAgent pathFinder; // 경로계산 AI 에이전트

    [SerializeField] private ParticleSystem hitEffect; // 피격시 재생할 파티클 효과
    [SerializeField] private AudioClip deathSound; // 사망시 재생할 소리
    [SerializeField] private AudioClip hitSound; // 피격시 재생할 소리

    private Animator BossAnimator; // 애니메이터 컴포넌트
    private AudioSource BossAudioPlayer; // 오디오 소스 컴포넌트
    private Renderer BossRenderer; // 렌더러 컴포넌트

    [SerializeField] private float FineRange = 1;
    [SerializeField] private float damage = 20f; // 공격력
    [SerializeField] private float timeBetAttack = 3f; // 공격 간격
    private float lastAttackTime; // 마지막 공격 시점

    [SerializeField] public GameObject BossSkill1; // 보스 스킬 1
    private List<GameObject> listSkill1;

    private void Awake()
    {
        //pathFinder = GetComponent<NavMeshAgent>();
        BossAnimator = GetComponent<Animator>();
        BossAudioPlayer = GetComponent<AudioSource>();

        BossRenderer = GetComponentInChildren<Renderer>();

        listSkill1 = new List<GameObject>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
    {
        BossAnimator.SetBool("HasTarget", hasTarget);
    }

    private bool hasTarget
    {
        
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true 
            if (targetEntity != null && !targetEntity.dead )//&& FineRange >= targetEntity.transform.position
            {
                return true;
            }

            // 그렇지 않다면 false
            return false;
        }
    }

    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {
        // 체력 설정
        startingHealth = newHealth;
        health = newHealth;

        // 공격력 설정
        damage = newDamage;
       

    }

    private void OnDrawGizmosSelected()
    {
        // 범위 그려주는 코드 
        Gizmos.DrawWireSphere(transform.position, FineRange);
    }

    private IEnumerator UpdatePath()
    {
        
        
        // 살아있는 동안 무한 루프
        while (!dead)
        {
            if (hasTarget)
            {
                //pathFinder.isStopped = false;
                
                Vector3 hitPoint = targetEntity.transform.position;
                Vector3 hitNormal = (transform.position - hitPoint).normalized; // 몬스터와 플레이어 위치를 뺀값의 단위 백터 -> 몬스터가 플레이어 보는 방향  

                if (!dead && Time.time >= lastAttackTime + timeBetAttack)
                {
                    lastAttackTime = Time.time;
                    // 공격 실행
                  

                    if (FineRange >= Vector3.Distance(hitPoint, transform.position)) // 지정된 범위 내에 플레이어만 공격 하게 
                    {

                        // 범위에 플레이어가 들어오면 스킬을 사용함 비활성화된 장판 찾기
                        SkillAttack(hitPoint);
                    }
                }

            }
            else
            {
                // 대상이 존내에 없으면 stay 상태 
                //pathFinder.isStopped = true;
                Collider[] colliders = Physics.OverlapSphere(transform.position, FineRange, whatIsTarget);
                for (int i = 0; i < colliders.Length; i++)
                {
                    // 콜라이더로 부터 LivingEntity Component 가져오기
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    //LivingEntity Component가 존재하며, 해당 LivingEntity 가 살아 있다면
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        // 추적대상으로 다른 살아있는 존재로 변경 
                        targetEntity = livingEntity;
                        break;
                    }
                }


            }

            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            // 공격받은 지점과 방향으로 파티클 효과 재생 
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
        }
        // LivingEntity의 OnDamage()를 실행하여 데미지 적용
        base.OnDamage(damage, hitPoint, hitNormal);
    }

   /* private void OnTriggerStay(Collider other)
    {
        // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행   
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            // 상대방으로 부터 LivingEnitity  타입을 가져오기 시도
            LivingEntity attackTarget
                = other.GetComponent<LivingEntity>();

            // 상대방의 LivingEntity 사 자신의 추적 대상이라면 공격 실행
            if (attackTarget != null && attackTarget == targetEntity)
            {
                // 최근 공격 시간을 갱신
                lastAttackTime = Time.time;

                // 상대방의 피격 위치와 피격 방향을 근삿값으로 계산
                Vector3 hitPoint
                    = other.ClosestPoint(transform.position);
                Vector3 hitNormal
                    = transform.position - other.transform.position;

                // 공격 실행
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }*/

    public override void Die()
    {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        //사망 애니메이션 재생
        BossAnimator.SetTrigger("Die");
        // 사망 효과음
        BossAudioPlayer.PlayOneShot(deathSound);

    }

    private void SkillAttack(Vector3 hitPoint)
    {

        //var Skill = GameObject.FindGameObjectsWithTag("Skill");
        // 비활성화가 된 스킬이 있는 경우 사용 없으면 생성 
       if (0 < listSkill1.Count)
        {
            listSkill1[0].transform.position = hitPoint;
            listSkill1[0].SetActive(true);
        }
        else
        {
            var skill = Instantiate(BossSkill1);
            listSkill1.Add(skill);
            skill.transform.position = hitPoint;
            skill.SetActive(true);
        }
    }
}
