using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Object/GunData", order = int.MaxValue)]
public class GunData : ScriptableObject
{
    [SerializeField] WeaponType weaponType;
    public WeaponType WeaponType { get { return weaponType; } } // ���� / ���Ÿ� ����

    [SerializeField] AudioClip shotClip; // �߻� �Ҹ�
    public AudioClip ShotClip { get { return shotClip; } }

    [SerializeField] AudioClip reloadClip; // ���� �Ҹ�
    public AudioClip ReloadClip { get { return reloadClip; } }

    [SerializeField] float damage;
    public float Damage { get { return damage; } }

    [SerializeField] int ammoRemain; // ���� ��ü ź��
    public int AmmoRemain { get { return ammoRemain; } set { ammoRemain = value; } }

    [SerializeField] int magCapacity; // źâ �뷮
    public int MagCapacity { get { return magCapacity; } }

    [SerializeField] int magAmmo; // ���� źâ�� �����ִ� ź��
    public int MagAmmo { get { return magAmmo; } set { magAmmo = value; } }

    [SerializeField] float fireDistance; // ���� �Ÿ�
    public float FireDistance { get { return fireDistance; } }

    [SerializeField] float timeBetFire; // �Ѿ� �߻� ����
    public float TimeBetFire { get { return timeBetFire; } }

    [SerializeField] float reloadTime; // ������ �ҿ� �ð�
    public float ReloadTime { get { return reloadTime; } }

    [SerializeField] float lastFireTime; // ���� ���������� �߻��� ����
    public float LastFireTime { get { return lastFireTime; } set { lastFireTime = value; } }

    [SerializeField] int penetNum; // raycast�� ������ Ƚ��
    public int PenetNum { get { return penetNum; } set { penetNum = value; } }

    [SerializeField] int maxHit; // �ִ� ���� Ƚ�� ����
    public int MaxHit { get { return maxHit; } }

    [SerializeField] int maxShotCount;
    public int MaxShotCount { get { return maxShotCount; } }
}
