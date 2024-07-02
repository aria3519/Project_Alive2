using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGrenade : Item
{
    private ParticleSystem effectObj;
    public AudioClip explosionClip;
    private AudioSource explosionAudio;
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
        yield return new WaitForSeconds(1f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        effectObj = EffectManager.Instance.GetBigExplosionEffect();
        effectObj.transform.position = transform.position;
        explosionAudio.PlayOneShot(explosionClip);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 5,
            Vector3.up, 0f, LayerMask.GetMask("Player"));

        foreach (RaycastHit hitObj in rayHits)
        {
            var check = hitObj.transform.GetComponent<LivingEntity>();
            if (check != null)
                check.HitByGrenade(transform.position);
        }

        // Ǯ�� �ֱ�
        yield return new WaitForSeconds(3f);
        EffectManager.Instance.InsertBigExplosionEffect(effectObj);
        ItemManager.instance.InsertBossGrenade(this);
    }
}
