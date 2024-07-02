using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rid;
    [SerializeField] protected float hasPlayerRange = 1;

    // Start is called before the first frame update
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        var tag = other.tag;
        if (tag.Equals("Player"))
        {
            LivingEntity Player = other.GetComponent<LivingEntity>();
            Vector3 hitPoint = Player.transform.position;
            Vector3 hitNormal = (transform.position - hitPoint).normalized;
            // ���Ϳ� �÷��̾� ��ġ�� ������ ���� ���� -> ���Ͱ� �÷��̾� ���� ����
            Player.OnDamage(100f, hitPoint, hitNormal);
            gameObject.SetActive(false);
        }
    }
    public void onFire(Vector3 pos,Vector3 dir, float force)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = pos;
        gameObject.transform.Rotate(dir.normalized);
        rid.velocity = Vector3.zero;
        rid.AddForce(dir.normalized * force);
    }
    public void Pooling(Vector3 pos)
    {
       gameObject.transform.position = pos;
       gameObject.SetActive(false);
    }
   /* private void OnDrawGizmosSelected()
    {
        // ���� �׷��ִ� �ڵ� 
        Gizmos.DrawWireSphere(transform.position, hasPlayerRange);
        // Gizmos.DrawWireSphere(transform.position, AttackRange);
    }*/
}
