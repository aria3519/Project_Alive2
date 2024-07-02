using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCStates
{
    Idle,
    Attack,
    Move,
    Die
}


public class NpcUnit : Npcbase
{
    // Rescue
    // 대화 기능
    // 플레이어 따라가기

    // Quest
    // 대화 기능

    // Unit
    // 대화 기능
    // 플레이어 따라가기
    // 공격, 체력, 공격수행 ai, 포메이션 ai

    // 공통 데이터
    //public NpcKind npcKind { get; set; }

    // 총 관련
    private float lastFireTime;
    [SerializeField] protected Transform fireTransform; // 총알이 발사될 위치
    [SerializeField] ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    [SerializeField] ParticleSystem shellEjectEffect; // 탄피 배출 효과
    [SerializeField] GunData gunData;
    [SerializeField] private BulletType setBulletType;
    [SerializeField] protected Transform weaponPivot;
    [SerializeField] private AudioClip shotClip;
    [SerializeField] private TestGunController equippedGun;

    private SphereCollider sphereCollider;
    private CapsuleCollider capsuleCollider;
    private Animator npcAnimator;
    private AudioSource npcAudio;
    private GameObject npcText;
    private NPCStates npcStates;
    private LivingEntity playerEntity;

    private Vector3 targetPos;
    private Vector3 direction;

    private bool hasTarget = false;
    private bool isPlayer = false;
    private bool isMove = false;
    private bool isFollow = false;
    private bool isTicked = false;
    private bool isReloading = false;
    public string enemyTag = "Enemy";
    public Transform target;

    private int magAmmo;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float maxRange = 10f;
    [SerializeField] private float rotationSpeed = 0.15f;
    //private Vector3 playerPos;
    public void Awake()
    {
        npcAnimator = GetComponent<Animator>();
        npcAudio = GetComponent<AudioSource>();
        rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        health = startingHealth;
        magAmmo = gunData.MagAmmo;
    }

    protected override void Yes()
    {
    }

    protected override void No()
    {
    }

    protected override void Exit()
    {
    }

    protected override void SetUiManager()
    {
        UIManager.instance.SetText(NpcContent.text, (null != NpcYes) ? NpcYes.text : "", (null != NpcNo) ? NpcNo.text : "");
        isFollow = true;
        NpcManager.instance.AddRescuedUnit(this);
    }

    public void Updates(Vector3 movePos)
    {
        if (!dead && isFollow)
        {
            UpdateTarget();
            UpdateMovement(movePos);
        }
    }

    private void UpdateMovement(Vector3 movePos)
    {
        if (dead)
            return;

        //if (playerEntity != null)
        //    movePos = playerEntity.transform.position;

        var direction = (movePos - transform.position + Vector3.up * 1f);
        direction.y = 0;
        direction = direction.normalized;
        var rayPos = rigidbody.position + Vector3.up * 1f;
        var dist = Vector3.Distance(rigidbody.position, movePos);

        if (dist > 0.3f)
        {
            rigidbody.velocity = direction * moveSpeed;
            npcAnimator.SetBool("Move", true);
            if (!hasTarget)
                rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.LookRotation(direction), rotationSpeed);
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
            npcAnimator.SetBool("Move", false);
        }
    }
    private void UpdateTarget()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, maxRange, Vector3.up, 0f, LayerMask.GetMask("Enemy", "Boss"));

        if (hits.Length > 0)
        {
            // RaycastAll 의 값들이 거리순대로 sorting이 되어있다면?
            hasTarget = true;
            var targetPos = hits[0].transform.position;
            var direction = (targetPos - transform.position).normalized;
            direction.y = 0;

            Debug.DrawLine(rigidbody.position, rigidbody.position + direction * maxRange, Color.red);

            //rigidbody.rotation = Quaternion.LookRotation(direction);
            if (!dead)
                rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.LookRotation(direction), 0.15f);
            rigidbody.velocity = Vector3.zero;

            Fire();
        }
        else
            hasTarget = false;
    }

    public virtual void Fire()
    {
        // 현재 상태가 발사 가능한 상태?
        // 그리고 마지막 총 발사 시점에서 timeBetFire 이상의 시간이 지날 때
        if (Time.time >= lastFireTime + gunData.TimeBetFire && magAmmo > 0 && !isReloading)
        {
            // 마지막 총 발사 시점 갱신
            lastFireTime = Time.time;
            // 실제 발사 처리 실행
            var bullet = ItemManager.instance.GetBullet(setBulletType);
            bullet.transform.position = fireTransform.position;
            bullet.transform.rotation = fireTransform.rotation;
            bullet.Active(fireTransform);
            //총구 화연 효과 재생
            muzzleFlashEffect.gameObject.SetActive(true);
            muzzleFlashEffect.Play();
            //탄피 배출 효과 재생
            shellEjectEffect.Play();

            //총격소리 재생s
            npcAudio.PlayOneShot(shotClip);
            magAmmo--;
        }

        if(magAmmo <= 0)
        {
            isReloading = true;
            npcAnimator.SetTrigger("Reload");
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(gunData.ReloadTime);
        magAmmo = gunData.MagAmmo;
        isReloading = false;
    }

    public virtual void SerchPlayer(Character player)
    {
        if (player)
        {
            playerEntity = player;
            isPlayer = true;
        }

    }

    private void OnAnimatorIK(int layerIndex)
    {
        weaponPivot.position = npcAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        npcAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        npcAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        npcAnimator.SetIKPosition(AvatarIKGoal.LeftHand, equippedGun.leftHandMount.position);
        npcAnimator.SetIKRotation(AvatarIKGoal.LeftHand, equippedGun.leftHandMount.rotation);

        npcAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        npcAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        npcAnimator.SetIKPosition(AvatarIKGoal.RightHand, equippedGun.rightHandMount.position);
        npcAnimator.SetIKRotation(AvatarIKGoal.RightHand, equippedGun.rightHandMount.rotation);
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (dead && isFollow)
            return;

        base.OnTriggerStay(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (isFollow)
        {
            base.OnTriggerExit(other);
            sphereCollider.enabled = false;
        }
    }

    public override void Die()
    {
        base.Die();
        npcAnimator.SetTrigger("Die");
        capsuleCollider.enabled = false;
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;
        NpcManager.instance.RemoveRescuedUnit(this);
    }
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
    }
    public override void OnDamageByFlame(float firstDamage, float tickDamage, int tickTime, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            isTicked = true;
            base.OnDamage(firstDamage, hitPoint, hitNormal);
            if (isTicked)
            {
                StartCoroutine(flameDamageTime(tickTime, tickDamage));
            }
        }
    }

    private IEnumerator flameDamageTime(int tickTime, float tickDamage)
    {
        for (int i = 0; i < tickTime; i++)
        {
            base.OnDamage(tickDamage, Vector3.zero, Vector3.zero);
            yield return new WaitForSeconds(1f);
            isTicked = false;
        }
    }

    public override void BindAttacked()
    {
        rigidbody.velocity = Vector3.zero;
        //StartCoroutine(BindTime());
    }

    private IEnumerator BindTime()
    {
        yield return new WaitForSeconds(2.5f);
    }
}
