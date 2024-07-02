using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeController : MonoBehaviour
{
    [SerializeField] protected MeleeData meleeData;
    [SerializeField] private GameObject Player;

    protected AudioSource meleeAudioPlayer;
    public Transform leftHandMount;
    public Transform rightHandMount;
    protected float lastAttackTime;

    private void Start()
    {
        lastAttackTime = meleeData.LastAttackTime;
    }
    public virtual void Attack(Animator playerAnimator, bool comboPossible, int comboStep)
    {
        if(Time.time >= lastAttackTime + meleeData.TimeBetAttack)
        {
            lastAttackTime = Time.time;

            if(comboStep == 0)
            {
                playerAnimator.SetTrigger("Attack");
                comboStep = 1;
                return;
            }
            if(comboStep != 0)
            {
                if(comboPossible)
                {
                    comboPossible = false;
                    comboStep += 1;
                }
            }

            //switch (comboStep)
            //{
            //    case 0:
            //        playerAnimator.SetTrigger("Attack");
            //        comboStep = 1;
            //        break;
            //    case 1:
            //        playerAnimator.SetTrigger("Attack2");
            //        comboStep = 2;
            //        break;
            //    case 2:
            //        playerAnimator.SetTrigger("Attack3");
            //        break;
            //}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other || other.tag != "Enemy")
            return;

        LivingEntity attackTarget = other.gameObject.GetComponent<LivingEntity>();
        attackTarget.OnDamage(meleeData.Damage, other.transform.position, other.transform.forward);
        //other.attachedRigidbody.AddForce(Player.transform.forward * 10.0f);
    }
}
