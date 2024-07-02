using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBullet : MonoBehaviour
{
    private float lastDamageTime;
    private float damageTime = 0.08f;
    //private void OnEnable()
    //{
    //    StartCoroutine(flameTime());
    //}
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Enemy" && Time.time >= lastDamageTime + damageTime)
        {
            lastDamageTime = Time.time;
            other.transform.GetComponent<LivingEntity>().OnDamageByFlame(50f, 5f, 5, Vector3.zero, Vector3.zero);
        }   
    }
    //IEnumerator flameTime()
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    ItemManager.instance.InsertFlameBullet(this);
    //}
}
