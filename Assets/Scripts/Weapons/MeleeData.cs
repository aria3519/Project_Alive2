using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeData", menuName = "Scriptable Object/MeleeData", order = int.MaxValue)]
public class MeleeData : ScriptableObject
{
    [SerializeField] WeaponType weaponType;
    public WeaponType WeaponType { get { return weaponType; } } // 근접 / 원거리 구분

    [SerializeField] AudioClip attackClip;
    public AudioClip AttackClip {  get { return attackClip; } }

    [SerializeField] float damage;
    public float Damage { get { return damage; } }

    [SerializeField] float lastAttackTime; // 총을 마지막으로 발사한 시점
    public float LastAttackTime { get { return lastAttackTime; } set { lastAttackTime = value; } }

    [SerializeField] float timeBetAttack; // 총알 발사 간격
    public float TimeBetAttack { get { return timeBetAttack; } }
}
