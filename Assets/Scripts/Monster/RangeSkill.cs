using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SkillKind
{
    Default,
    Flame,
    Bind
}
// ���� ��ų 
public class RangeSkill : MonoBehaviour
{
    // �̳� ��Ÿ , ��Ÿ
    SphereCollider sphereCollider;
    BoxCollider boxCollider;
    [SerializeField] private SkillKind type;
    [SerializeField] private float damage = 20f; // ���ݷ�
    [SerializeField] private float flameTickDamage = 5f; // ���ݷ�
    [SerializeField] private int tickTime = 10; // ���ݷ�

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
        // �÷��̾ �¾����� �÷��̾��� ü���� ���� ��Ŵ    
        //other.GetComponent<LivingEntity>().OnDamage();
        // ����ġ�� ��Ÿ or ��Ÿ (���� ������ ������� ����)

        //gameObject.SetActive(true)
        // Debug.Log("RangeAttack");

        if (other && (other.tag == "Player" || other.tag == "NPC"))
        {
            var attackTarget = other.GetComponent <LivingEntity>();
            Vector3 hitPoint = attackTarget.transform.position;
            Vector3 hitNormal = (transform.position - hitPoint).normalized; // ���Ϳ� �÷��̾� ��ġ�� ������ ���� ���� -> ���Ͱ� �÷��̾� ���� ����

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
