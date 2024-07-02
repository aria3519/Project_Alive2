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
    // ��ȭ ���
    // �÷��̾� ���󰡱�

    // Quest
    // ��ȭ ���

    // Unit
    // ��ȭ ���
    // �÷��̾� ���󰡱�
    // ����, ü��, ���ݼ��� ai, �����̼� ai

    // ���� ������
    //public NpcKind npcKind { get; set; }

    // �� ����
    private float lastFireTime;
    [SerializeField] protected Transform fireTransform; // �Ѿ��� �߻�� ��ġ
    [SerializeField] ParticleSystem muzzleFlashEffect; // �ѱ� ȭ�� ȿ��
    [SerializeField] ParticleSystem shellEjectEffect; // ź�� ���� ȿ��
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
            // RaycastAll �� ������ �Ÿ������ sorting�� �Ǿ��ִٸ�?
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
        // ���� ���°� �߻� ������ ����?
        // �׸��� ������ �� �߻� �������� timeBetFire �̻��� �ð��� ���� ��
        if (Time.time >= lastFireTime + gunData.TimeBetFire && magAmmo > 0 && !isReloading)
        {
            // ������ �� �߻� ���� ����
            lastFireTime = Time.time;
            // ���� �߻� ó�� ����
            var bullet = ItemManager.instance.GetBullet(setBulletType);
            bullet.transform.position = fireTransform.position;
            bullet.transform.rotation = fireTransform.rotation;
            bullet.Active(fireTransform);
            //�ѱ� ȭ�� ȿ�� ���
            muzzleFlashEffect.gameObject.SetActive(true);
            muzzleFlashEffect.Play();
            //ź�� ���� ȿ�� ���
            shellEjectEffect.Play();

            //�ѰݼҸ� ���s
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
