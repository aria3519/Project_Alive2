using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class JetBomber : Item
{
    float moveSpeed = 15f;
    Vector3 position;
    Vector3 bombPos;

    public AudioClip explosionClip;
    public AudioClip JetBomberClip;
    private AudioSource JetBomberAudio;
    //private Rigidbody rigid;

    void Start()
    {
        JetBomberAudio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        position = transform.position;
        //rigid = GetComponent<Rigidbody>();

        //rigid.velocity = transform.forward * moveSpeed;
        //
        bombPos = SkillManager.instance.GetBombPos();


        StartCoroutine(Aid());
    }

    IEnumerator Aid()
    {
        yield return new WaitForSeconds(1f);
        JetBomberAudio.PlayOneShot(JetBomberClip);

        StartCoroutine(Bomb());

        yield return new WaitForSeconds(11f);
        ItemManager.instance.InsertQueue(this, ItemKind.ItemJetBomber);
    }

    private IEnumerator Bomb()
    {
        //yield return new WaitForSeconds(6.0f);
        //effectObj.SetActive(true);
        //effectObj = Instantiate(effectObj, bombPos, transform.rotation);
        //JetBomberAudio.PlayOneShot(explosionClip);

        //RaycastHit[] rayHits = Physics.SphereCastAll(bombPos, 8,
        //   Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        //foreach (RaycastHit hitObj in rayHits)
        //{
        //    hitObj.transform.GetComponent<Enemy>().HitByGrenade(bombPos);
        //}
        //Destroy(effectObj, 3.9f);

        //      if (skillManager.skillState == SkillState.BOMB_MULTI)
        //      {
        //          yield return new WaitForSeconds(6.0f);

        //          effectObj.SetActive(true);
        //          for (int i = 1; i <= 3; i++)
        //          {
        //              //bombPos.x += Random.Range(- 10, 10);
        //              //bombPos.z += Random.Range(- 2, 2);
        //              //bombPos.x += i+5;
        //              bombPos += transform.forward * (i * 5); // bombPos 도 같이 더해줘야 마우스포인터위치 기준이된다.
        //              effectObj = Instantiate(effectObj, bombPos, transform.rotation);
        //              JetBomberAudio.PlayOneShot(explosionClip);

        //              RaycastHit[] rayHits = Physics.SphereCastAll(bombPos, 8,
        //Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        //              foreach (RaycastHit hitObj in rayHits)
        //              {
        //                  hitObj.transform.GetComponent<Enemy>().HitByGrenade(bombPos);
        //              }
        //              Destroy(effectObj, 2f);
        //              yield return new WaitForSeconds(0.2f);
        //          }
        //      }
        //else if (skillManager.skillState == SkillState.BOMB)
        //{
        yield return new WaitForSeconds(6.0f);
        var effectObj = EffectManager.Instance.GetBigExplosionEffect();
        effectObj.transform.position = bombPos;
        effectObj.transform.rotation = transform.rotation;
        JetBomberAudio.PlayOneShot(explosionClip);

        RaycastHit[] rayHits = Physics.SphereCastAll(bombPos, 8,
           Vector3.up, 0f, LayerMask.GetMask("Enemy", "Boss"));

        foreach (RaycastHit hitObj in rayHits)
        {
            var check = hitObj.transform.GetComponent<LivingEntity>();
            if (check != null)
                check.HitByGrenade(transform.position);
        }

        yield return new WaitForSeconds(4f);
        EffectManager.Instance.InsertBigExplosionEffect(effectObj);
    }
    private void Update()
    {
        position += transform.forward * moveSpeed * Time.deltaTime;
        transform.position = position;
    }
}
