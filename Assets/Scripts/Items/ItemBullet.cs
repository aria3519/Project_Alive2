using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Gun,
    MiniGun
}
public class ItemBullet : MonoBehaviour
{
    [SerializeField] private BulletType bulletType;
    [SerializeField] private int maxPenetrationCount;
    private Rigidbody rigidbody;
    public GameObject effectObj;
    public float velocity = 50f;

    private int penetrationCount = 0;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Active(Transform firePos)
    {
        rigidbody.velocity = firePos.forward * velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (bulletType)
        {
            case BulletType.Gun:
                if (other && other.tag == "Enemy")
                {
                    LivingEntity attackTarget = other.gameObject.GetComponent<LivingEntity>();
                    attackTarget.OnDamage(20f, other.transform.position, other.transform.forward);
                }
                else if(other && (other.tag == "Player" || other.tag == "NPC" || other.tag == "Bullet"))
                {
                    return;
                }

                ItemManager.instance.InsertBullet(this, bulletType);
                break;
            case BulletType.MiniGun:
                if (other && other.tag == "Enemy")
                {
                    LivingEntity attackTarget = other.gameObject.GetComponent<LivingEntity>();
                    attackTarget.OnDamage(20f, other.transform.position, other.transform.forward);
                    penetrationCount++;
                }
                else if (other && (other.tag == "Player" || other.tag == "NPC" || other.tag == "Bullet"))
                {
                    return;
                }
                ItemManager.instance.InsertBullet(this, bulletType);

                if (penetrationCount >= maxPenetrationCount)
                    ItemManager.instance.InsertBullet(this, bulletType);
                break;
        }
    }
}
