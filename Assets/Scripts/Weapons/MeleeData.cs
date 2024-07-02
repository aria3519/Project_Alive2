using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeData", menuName = "Scriptable Object/MeleeData", order = int.MaxValue)]
public class MeleeData : ScriptableObject
{
    [SerializeField] WeaponType weaponType;
    public WeaponType WeaponType { get { return weaponType; } } // ���� / ���Ÿ� ����

    [SerializeField] AudioClip attackClip;
    public AudioClip AttackClip {  get { return attackClip; } }

    [SerializeField] float damage;
    public float Damage { get { return damage; } }

    [SerializeField] float lastAttackTime; // ���� ���������� �߻��� ����
    public float LastAttackTime { get { return lastAttackTime; } set { lastAttackTime = value; } }

    [SerializeField] float timeBetAttack; // �Ѿ� �߻� ����
    public float TimeBetAttack { get { return timeBetAttack; } }
}
