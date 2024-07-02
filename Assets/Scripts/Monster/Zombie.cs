using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //

public class Zombie : LivingEntity
{
    

    public LayerMask whatIsTarget; // ���� ��� ���̾�

    private LivingEntity targetEntity; // ������ ���
    private NavMeshAgent pathFinder; // ��ΰ�� AI ������Ʈ

    public ParticleSystem hitEffect; // �ǰݽ� ����� ��ƼŬ ȿ��
    public AudioClip deathSound; // ����� ����� �Ҹ�
    public AudioClip hitSound; // �ǰݽ� ����� �Ҹ�

    private Animator enemyAnimator; // �ִϸ����� ������Ʈ
    private AudioSource enemyAudioPlayer; // ����� �ҽ� ������Ʈ
    private Renderer enemyRenderer; // ������ ������Ʈ

    public float damage = 20f; // ���ݷ�
    public float timeBetAttack = 0.5f; // ���� ����
    private float lastAttackTime; // ������ ���� ����

    // ������ ����� �����ϴ��� �˷��ִ� ������Ƽ
    private bool hasTarget
    {
        get
        {
            // ������ ����� �����ϰ�, ����� ������� �ʾҴٸ� true
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }

            // �׷��� �ʴٸ� false
            return false;
        }
    }

    private void Awake()
    {
        // �ʱ�ȭ
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();

        enemyRenderer = GetComponentInChildren<Renderer>();
    }

    // �� AI�� �ʱ� ������ �����ϴ� �¾� �޼���
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {
        // ü�� ����
        startingHealth = newHealth;
        health = newHealth;

        // ���ݷ� ����
        damage = newDamage;
        // Renderer�� ������� Material �� �÷� ����, 
        enemyRenderer.material.color = skinColor;

    }

    private void Start()
    {
        // ���� ������Ʈ Ȱ��ȭ�� ���ÿ� AI�� ���� ��ƾ ����
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        // ���� ����� ���� ���ο� ���� �ٸ� �ִϸ��̼��� ���
        enemyAnimator.SetBool("HasTarget", hasTarget);
    }

    // �ֱ������� ������ ����� ��ġ�� ã�� ��θ� ����
    private IEnumerator UpdatePath()
    {
        // ����ִ� ���� ���� ����
        while (!dead)
        {

            if (hasTarget)
            {
                // ���� ��� ���� : ��θ� ���� �ϰ� AI �̵��� ��� ����
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            }
            else
            {
                // ���� ��� ���� :AI �̵� ����
                pathFinder.isStopped = true;
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f,
                    whatIsTarget);

                for (int i = 0; i < colliders.Length; i++)
                {
                    // �ݶ��̴��� ���� LivingEntity Component ��������
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    //LivingEntity Component�� �����ϸ�, �ش� LivingEntity �� ��� �ִٸ�
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        // ����������� �ٸ� ����ִ� ����� ���� 
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }
            // 0.25�� �ֱ�� ó�� �ݺ�
            yield return new WaitForSeconds(0.25f);
        }
    }

    // �������� �Ծ����� ������ ó��
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            // ���ݹ��� ������ �������� ��ƼŬ ȿ�� ��� 
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
        }
        // LivingEntity�� OnDamage()�� �����Ͽ� ������ ����
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    // ��� ó��
    public override void Die()
    {
        // LivingEntity�� Die()�� �����Ͽ� �⺻ ��� ó�� ����
        base.Die();

        Collider[] enemyColliders = GetComponents<Collider>();

        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].enabled = true;
        }

        // AI ������ ���� �ϰ� ���񿡼� ������Ʈ ��Ȱ��ȭ
        pathFinder.isStopped = true;
        pathFinder.enabled = false;

        //��� �ִϸ��̼� ���
        enemyAnimator.SetTrigger("Die");
        // ��� ȿ����
        enemyAudioPlayer.PlayOneShot(deathSound);

    }

    private void OnTriggerStay(Collider other)
    {
        // Ʈ���� �浹�� ���� ���� ������Ʈ�� ���� ����̶�� ���� ����   
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            // �������� ���� LivingEnitity  Ÿ���� �������� �õ�
            LivingEntity attackTarget
                = other.GetComponent<LivingEntity>();

            // ������ LivingEntity �� �ڽ��� ���� ����̶�� ���� ����
            if (attackTarget != null && attackTarget == targetEntity)
            {
                // �ֱ� ���� �ð��� ����
                lastAttackTime = Time.time;

                // ������ �ǰ� ��ġ�� �ǰ� ������ �ٻ����� ���
                Vector3 hitPoint
                    = other.ClosestPoint(transform.position);
                Vector3 hitNormal
                    = transform.position - other.transform.position;

                // ���� ����
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }
}

