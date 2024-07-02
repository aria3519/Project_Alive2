using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileParticleCollision : MonoBehaviour
{
    [SerializeField]
    private GameObject missile;
    [SerializeField]
    private AudioClip explosionClip;
    private AudioSource audioPlayer;

    private ParticleSystem explosionEffect;
    private void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //if(missile.transform.position.x == crosshairs.transform.position.x)
        //{
        //    explosionEffect.transform.position = crosshairs.transform.position;
        //    explosionEffect.gameObject.SetActive(true);
        //    explosionEffect.Play();

        //    RaycastHit[] rayHits = Physics.SphereCastAll(crosshairs.transform.position, 3, Vector3.up
        //        , 0f, LayerMask.GetMask("Enemy"));

        //    foreach (RaycastHit hitObj in rayHits)
        //    {
        //        hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        //    }
        //}
    }

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Rocket Collision");
        if (other.tag == "Enemy")
        {
            explosionEffect = EffectManager.Instance.GetBigExplosionEffect();
            explosionEffect.transform.position = other.transform.position;
            explosionEffect.gameObject.SetActive(true);
            explosionEffect.Play();
            audioPlayer.PlayOneShot(explosionClip);

            RaycastHit[] rayHits = Physics.SphereCastAll(other.transform.position, 5, Vector3.up
                , 0f, LayerMask.GetMask("Enemy", "Boss"));

            foreach (RaycastHit hitObj in rayHits)
            {
                hitObj.transform.GetComponent<LivingEntity>().HitByGrenade(transform.position);
            }

            StartCoroutine(ExplosionTime(explosionEffect));
        }
    }

    private IEnumerator ExplosionTime(ParticleSystem effect)
    {
        yield return new WaitForSeconds(3f);
        EffectManager.Instance.InsertBigExplosionEffect(effect);
    }
}
