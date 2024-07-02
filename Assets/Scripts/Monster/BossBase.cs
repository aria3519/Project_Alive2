using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� �ڵ�
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
    protected LivingEntity playerEntity; // �÷��̾� ��ġ 
    [SerializeField] protected float hasPlayerRange = 1;
    [SerializeField] protected float attackRange = 1;

    [SerializeField] protected ParticleSystem hitEffect; // �ǰݽ� ����� ��ƼŬ ȿ��
    [SerializeField] protected AudioClip deathSound; // ����� ����� �Ҹ�
    [SerializeField] protected AudioClip hitSound; // �ǰݽ� ����� �Ҹ�

    protected Animator bossAnimator; // �ִϸ����� ������Ʈ
    protected AudioSource bossAudioPlayer; // ����� �ҽ� ������Ʈ
    protected Renderer bossRenderer; // ������ ������Ʈ
    protected CapsuleCollider bossCapsuleCollider;

    protected float lastStayTime; // ���������� ����ð� 
    protected float stayTime = 3f; // stay 3�� ���� 
    protected float lastRangeAttackTime; // ���������� ���� ���� �� �ð� 
    protected float lastAttackTime; // ���������� �������� �� �ð� 
    protected float rangeAttackTime = 4f; // 3�ʸ��� ���� ���� ���� 
    protected float attackTime = 3f; // 5�ʸ��� ���� ���� ���� 

    //[SerializeField] protected GameObject bossSkill1; // ���� ��ų 1
    //[SerializeField] protected GameObject bossSkill2; // ���� ��ų 2

    [SerializeField] protected RangeSkill bossRangeSkill; // ���� Range��ų

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
    



    /*[SerializeField] protected Slider Bosshealth; // ü���� ǥ���� UI �����̴�*/

    /*protected virtual void OnEnable()
    {
        health = startingHealth;
        // ���� �Ŵ������� �÷��̾� ���� �����;��� 
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

    //        // ���°� ���� �Ǳ� ���� ���� 
    //        // ��Ʈ Ÿ�� ������ �־���� 
    //            PlayerPoint = PlayerEntity.transform.position; // �÷��̾� ��ġ
    //            PlayerPoint.y = 0.1f;
    //            // �÷��̾� ���� ���� �ϴ� �ڵ� 

    //            var dist = Vector3.Distance(PlayerPoint, transform.position);
    //        if (hasPlayerRange >= dist) // ������ ���� ���� �÷��̾ ���� �ϰ� 
    //        {
    //            if (Lookat)
    //            {
    //                // rising Ȱ��ȭ 
    //                Debug.Log("rising");
    //                //BossAnimator.SetBool("Lookat", true);
    //                Lookat = false;
    //            }
    //            else
    //            {
    //                if (Time.time >= RangelastAttackTime + RangeAttackTime && AttackRange <= dist) // ���� ������ �⺻
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
        // LivingEntity�� Die()�� �����Ͽ� �⺻ ��� ó�� ����
        base.Die();
        bossInfo.gameObject.SetActive(false);
        
        // ���� ���� Ƚ�� ī���� 
        UIManager.instance.addBossKill();
        bossCapsuleCollider.enabled = false;
        //��� �ִϸ��̼� ���
        //BossAnimator.SetBool("isDead",true);
        bossAnimator.SetTrigger("Dead");
        // ��� ȿ����
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
            // ���ݹ��� ������ �������� ��ƼŬ ȿ�� ��� 
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            UIManager.instance.GetDamageText(damageFontPos, damage);
            //UIManager.instance.CheckDamage(true, damage);
        }
        // LivingEntity�� OnDamage()�� �����Ͽ� ������ ����
        base.OnDamage(damage, hitPoint, hitNormal);
       

       /* BossAnimator.SetTrigger("OnDamage");*/

    }

    // unityEvent �� �÷��̾� ���� �޾ƿ��� �ڵ�
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
