using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrenade : Item
{
    private ParticleSystem effectObj;
    public AudioClip explosionClip;
    private  AudioSource explosionAudio;
    public Rigidbody rigid;

    private void Start()
    {
        explosionAudio = GetComponent<AudioSource>();
    }

    // Awake - OnEnable - Start ������
    // OnEnable �� ������Ʈ Ȱ��ȭ�� ���� ȣ���
    void OnEnable()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(2f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        effectObj = EffectManager.Instance.GetSmallExplosionEffect();
        effectObj.transform.position = transform.position;
        explosionAudio.PlayOneShot(explosionClip);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 5,
            Vector3.up, 0f, LayerMask.GetMask("Enemy", "Boss"));

        foreach (RaycastHit hitObj in rayHits)
        {
            var check = hitObj.transform.GetComponent<LivingEntity>();
            if (check != null)
                check.HitByGrenade(transform.position);
        }

        // Ǯ�� �ֱ�
        yield return new WaitForSeconds(1.5f);
        EffectManager.Instance.InsertSmallExplosionEffect(effectObj);
        ItemManager.instance.InsertQueue(this, ItemKind.ItemGrenade);
    }

    //IEnumerator Explosion()
    //{
    //    yield return new WaitForSeconds(2.5f);
    //    rigid.velocity = Vector3.zero;
    //    rigid.angularVelocity = Vector3.zero;
    //    meshObj.SetActive(false);
    //    effectObj.SetActive(true);
    //    explosionAudio.PlayOneShot(explosionClip);

    //    RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 5,
    //        Vector3.up, 0f, LayerMask.GetMask("Enemy"));

    //    foreach (RaycastHit hitObj in rayHits)
    //    {
    //        hitObj.transform.GetComponent<LivingEntity>().HitByGrenade(transform.position);
    //    }

    //    // Ǯ�� �ֱ�
    //    yield return new WaitForSeconds(1.5f);
    //    meshObj.SetActive(true);
    //    effectObj.SetActive(false);
    //    ItemManager.instance.InsertQueue(this, ItemKind.ItemGrenade);
    //}
    //IEnumerator Explosion()
    //{
    //    yield return new WaitForSeconds(3f);
    //    rigid.velocity = Vector3.zero;
    //    rigid.angularVelocity = Vector3.zero;
    //    meshObj.SetActive(false);
    //    effectObj.SetActive(true);
    //    explosionAudio.PlayOneShot(explosionClip);

    //    RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 5, 
    //        Vector3.up, 0f, LayerMask.GetMask("Enemy"));

    //    foreach(RaycastHit hitObj in rayHits)
    //    {
    //        hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
    //    }

    //    Destroy(gameObject, 1f);
    //}
}