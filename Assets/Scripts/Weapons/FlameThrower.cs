using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : TestGunController
{
    [SerializeField] private ParticleSystem muzzleEffect;
    [SerializeField] private AudioClip flameThrowerClip;
    private float fireTime = 0.1f;
    private void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();

    }

    protected override void Start()
    {
        
    }

    public override void Fire()
    {
        if (Time.time >= lastFireTime + fireTime)
        {
            lastFireTime = Time.time;

            gunAudioPlayer.PlayOneShot(flameThrowerClip);
            muzzleEffect.transform.position = fireTransform.position;
            muzzleEffect.transform.rotation = fireTransform.rotation;
            muzzleEffect.gameObject.SetActive(true);
            muzzleEffect.Play();

            //var bullet = ItemManager.instance.GetFlameBullet();
            //bullet.transform.position = fireTransform.position;
            //bullet.transform.rotation = fireTransform.rotation;
            //bullet.gameObject.SetActive(true);
            //gunAudioPlayer.PlayOneShot(gunData.ShotClip);
        }
    }
}
