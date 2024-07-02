using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    //public Slider healthSlider;

    //public AudioClip deathClip;
    //public AudioClip hitClip;
    //public AudioClip itemPickupClip;

    //private AudioSource playerAudioPlayer;
    //private Animator playerAnimator;

    //private PlayerMovement playerMovement;
    //private PlayerWeapon playerWeapon;

    //private void Awake()
    //{
    //    playerAnimator = GetComponent<Animator>();
    //    playerAudioPlayer = GetComponent<AudioSource>();

    //    playerMovement = GetComponent<PlayerMovement>();
    //    playerWeapon = GetComponent<PlayerWeapon>();
    //}

    ////protected override void OnEnable()
    ////{
    ////    base.OnEnable();

    ////    healthSlider.gameObject.SetActive(true);

    ////    healthSlider.maxValue = startingHealth;

    ////    healthSlider.value = health;

    ////    playerMovement.enabled = true;
    ////    playerWeapon.enabled = true;
    ////}

    //public override void RestoreHealth(float newHealth)
    //{
    //    base.RestoreHealth(newHealth);

    //    healthSlider.value = health;
    //}

    //public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    //{
    //    if (!dead)
    //    {
    //        playerAudioPlayer.PlayOneShot(hitClip);
    //    }
    //    base.OnDamage(damage, hitPoint, hitNormal);

    //    healthSlider.value = health;

    //}

    //public override void Die()
    //{
    //    base.Die();

    //    healthSlider.gameObject.SetActive(false);
    //    playerAnimator.SetTrigger("Die");

    //    playerMovement.enabled = false;
    //    playerWeapon.enabled = false;
    //    Cursor.visible = true;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!dead)
    //    {
    //        IItem item = other.GetComponent<IItem>();

    //        if (item != null)
    //        {
    //            item.Use(gameObject);

    //            playerAudioPlayer.PlayOneShot(itemPickupClip);
    //        }
    //    }
    //}
}
