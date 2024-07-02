using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Object/GunData", order = int.MaxValue)]
public class GunData : ScriptableObject
{
    [SerializeField] WeaponType weaponType;
    public WeaponType WeaponType { get { return weaponType; } } // 근접 / 원거리 구분

    [SerializeField] AudioClip shotClip; // 발사 소리
    public AudioClip ShotClip { get { return shotClip; } }

    [SerializeField] AudioClip reloadClip; // 장전 소리
    public AudioClip ReloadClip { get { return reloadClip; } }

    [SerializeField] float damage;
    public float Damage { get { return damage; } }

    [SerializeField] int ammoRemain; // 남은 전체 탄약
    public int AmmoRemain { get { return ammoRemain; } set { ammoRemain = value; } }

    [SerializeField] int magCapacity; // 탄창 용량
    public int MagCapacity { get { return magCapacity; } }

    [SerializeField] int magAmmo; // 현재 탄창에 남아있는 탄약
    public int MagAmmo { get { return magAmmo; } set { magAmmo = value; } }

    [SerializeField] float fireDistance; // 사정 거리
    public float FireDistance { get { return fireDistance; } }

    [SerializeField] float timeBetFire; // 총알 발사 간격
    public float TimeBetFire { get { return timeBetFire; } }

    [SerializeField] float reloadTime; // 재장전 소요 시간
    public float ReloadTime { get { return reloadTime; } }

    [SerializeField] float lastFireTime; // 총을 마지막으로 발사한 시점
    public float LastFireTime { get { return lastFireTime; } set { lastFireTime = value; } }

    [SerializeField] int penetNum; // raycast가 관통한 횟수
    public int PenetNum { get { return penetNum; } set { penetNum = value; } }

    [SerializeField] int maxHit; // 최대 관통 횟수 제한
    public int MaxHit { get { return maxHit; } }

    [SerializeField] int maxShotCount;
    public int MaxShotCount { get { return maxShotCount; } }
}
