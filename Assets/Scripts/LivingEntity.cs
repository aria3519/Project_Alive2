using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected float startingHealth; // 시작 체력
    /*protected float health { get; set; }// 현재 체력*/
    public float health;// 현재 체력
    public bool dead { get; protected set; } // 사망 상태
    public event Action onDeath; // 사망시 발동할 이벤트
    protected Rigidbody rigidbody;
    protected bool isAttackedExplosive = false;

    private void Awake()
    {
        //rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void OnEnable()
    {
        // 사망하지 않은 상태로 시작
        dead = false;
        // 체력을 시작 체력으로 초기화
        //health = startingHealth;
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // 데미지만큼 체력 감소
        health -= damage;

        // 체력이 0이하 && 아직 죽지않았다면 사망 처리 실행
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual bool isDead()
    {
        return dead;
    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            return;
        }

        health += newHealth;
    }

    public virtual void Die()
    {
        if (onDeath != null)
        {
            onDeath();
            onDeath = null;
        }
        dead = true;
    }

    public void HitByGrenade(Vector3 explosionPos)
    {   
        isAttackedExplosive = true;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(GrenadeOnDamage(reactVec));
        OnDamage(100, reactVec, reactVec);
    }

    IEnumerator GrenadeOnDamage(Vector3 reactVec)
    {
        yield return new WaitForSeconds(0.1f);
        reactVec = reactVec.normalized;
        reactVec += Vector3.up * 0.2f;

        rigidbody.AddForce(reactVec * 5, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        rigidbody.velocity = Vector3.zero; // Vector3.Lerp(reactVec ,Vector3.zero, Time.deltaTime);
    }

    public virtual void OnDamageByFlame(float firstDamage, float tickDamage, int tickTime, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            OnDamage(firstDamage, hitPoint, hitNormal);
            StartCoroutine(flameDamageTime(tickTime, tickDamage));
        }
    }

    private IEnumerator flameDamageTime(int tickTime, float tickDamage)
    {
        for (int i = 0; i < tickTime; i++)
        {
            OnDamage(tickDamage, Vector3.zero, Vector3.zero);
            yield return new WaitForSeconds(1f);
        }
    }

    public virtual void BindAttacked()
    {

    }
}
