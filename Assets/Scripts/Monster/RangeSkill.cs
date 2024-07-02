using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SkillKind
{
    Default,
    Flame,
    Bind
}
// 범위 스킬 
public class RangeSkill : MonoBehaviour
{
    // 이넘 단타 , 연타
    SphereCollider sphereCollider;
    BoxCollider boxCollider;
    [SerializeField] private SkillKind type;
    [SerializeField] private float damage = 20f; // 공격력
    [SerializeField] private float flameTickDamage = 5f; // 공격력
    [SerializeField] private int tickTime = 10; // 공격력

    private void Start()
    {
        if (type == SkillKind.Flame)
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;
        }
        else
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.enabled = false;
        }
    }

    public void OnCollider()
    {
        if (type == SkillKind.Flame)
            boxCollider.enabled = true;
        else
            sphereCollider.enabled = true;
    }

    public void OffCollider()
    {
        if (type == SkillKind.Flame)
            boxCollider.enabled = false;
        else
            sphereCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 맞았으면 플레이어의 체력을 감소 시킴    
        //other.GetComponent<LivingEntity>().OnDamage();
        // 스위치문 단타 or 연타 (몇초 단위로 대미지를 줄지)

        //gameObject.SetActive(true)
        // Debug.Log("RangeAttack");

        if (other && (other.tag == "Player" || other.tag == "NPC"))
        {
            var attackTarget = other.GetComponent <LivingEntity>();
            Vector3 hitPoint = attackTarget.transform.position;
            Vector3 hitNormal = (transform.position - hitPoint).normalized; // 몬스터와 플레이어 위치를 뺀값의 단위 백터 -> 몬스터가 플레이어 보는 방향

            if (type == SkillKind.Flame)
                attackTarget.OnDamageByFlame(damage, flameTickDamage, tickTime, hitPoint, hitNormal);
            else
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            
            if(type == SkillKind.Bind)
                attackTarget.BindAttacked();
        }
    }
    /*private void OnDisable()
    {
        GameObject parent = transform.parent.gameObject;
        parent.SetActive(false);
    }*/
}
