using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Turret : Character
{
    public GunData gunData;
    public Transform target;
    public string enemyTag = "Enemy";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    [SerializeField] protected Transform fireTransform; // �Ѿ��� �߻�� ��ġ
    [SerializeField] ParticleSystem muzzleFlashEffect; // �ѱ� ȭ�� ȿ��
    [SerializeField] ParticleSystem shellEjectEffect; // ź�� ���� ȿ��
    protected LineRenderer bulletLineRenderer; // �Ѿ� ������ �׸��� ���� ������
    protected AudioSource gunAudioPlayer; // �� �Ҹ� �����

    private float lastFireTime;
    [SerializeField] private BulletType setBulletType;

    LineRenderer aimLine;
    Vector3 aimStartPos;
    private void Start()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        aimLine = GetComponent<LineRenderer>();

        // ����� ���� �ΰ��� ����
        bulletLineRenderer.positionCount = 2;
        // ���� ������ ��Ȱ��ȭ
        bulletLineRenderer.enabled = false;
        InvokeRepeating("UpdateTarget", 0f, 0.5f);

        gameObject.transform.position = SkillManager.instance.GetTurretPos();
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach(GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if(distanceToEnemy <= shortestDistance)
            {
                var check = enemy.transform.GetComponent<LivingEntity>();
                if (check != null && !check.isDead() && check.tag != "Player")
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }
        }
        
        target = null;
        if (nearestEnemy != null && shortestDistance <= gunData.FireDistance)
        {
            target = nearestEnemy.transform;

            var targetEnemy = target.GetComponent<EnemyFlow>();
            if (null != targetEnemy && true == targetEnemy.isDead())
            {
                target = null;
            }
        }
    }

    //public void Fire()
    //{
    //    float lastFireTime = gunData.LastFireTime;

    //    if (target != null && Time.time >= lastFireTime + gunData.TimeBetFire)
    //    {
    //        //lastFireTime = Time.time;
    //        lastFireTime = Time.time;
    //        Shot(gunData.MaxHit);
    //    }
    //}

    public virtual void Fire()
    {
        // ���� ���°� �߻� ������ ����?
        // �׸��� ������ �� �߻� �������� timeBetFire �̻��� �ð��� ���� ��
        if (Time.time >= lastFireTime + gunData.TimeBetFire)
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
            gunAudioPlayer.PlayOneShot(gunData.ShotClip);
        }
    }

    //public void Shot(int maxHit)
    //{
    //    RaycastHit[] hits;
    //    //ź���� ���� ���� ������ ����
    //    Vector3[] hitPositionSR;

    //    //hits = Physics.RaycastAll(fireTransform.position, fireTransform.forward, gunData.FireDistance);
    //    hits = Physics.RaycastAll(fireTransform.position, fireTransform.forward, gunData.FireDistance, LayerMask.GetMask("Enemy")).OrderByDescending(hits => hits.distance).ToArray();


    //    int count = Mathf.Clamp(hits.Length, 1, maxHit);
    //    hitPositionSR = new Vector3[count];

    //    for (int i = 0; i < count; i++)
    //    {
    //        //Mathf.Clamp(0 * , 40, 80);
    //        if (0 < hits.Length)
    //        {
    //            IDamageable checkWall = hits[count - 1].collider.GetComponent<IDamageable>();
    //            LivingEntity target = hits[i].collider.GetComponent<LivingEntity>();
    //            hitPositionSR[i] = hits[i].point;

    //            // ������ ���� �κ�. �ٽ� Ȯ�ιٶ�.
    //            if (null == checkWall)
    //            {
    //                hitPositionSR[i] = hits[count - 1].point;
    //                break;
    //            }

    //            if (null != target)
    //            {
    //                target.OnDamage(gunData.Damage, hits[i].point, hits[i].normal);
    //            }
    //        }
    //        else
    //            hitPositionSR[i] = fireTransform.position + fireTransform.forward * gunData.FireDistance;

    //    }

    //    // �߻� ����Ʈ ��� ����
    //    StartCoroutine(ShotEffect(hitPositionSR, fireTransform));
    //}

    //private IEnumerator ShotEffect(Vector3[] hitPosition, Transform fireTransform)
    //{
    //    //�ѱ� ȭ�� ȿ�� ���
    //    muzzleFlashEffect.Play();
    //    //ź�� ���� ȿ�� ���
    //    shellEjectEffect.Play();

    //    //�ѰݼҸ� ���s
    //    gunAudioPlayer.PlayOneShot(gunData.ShotClip);

    //    //���� �������� �ѱ��� ��ġ
    //    bulletLineRenderer.SetPosition(0, fireTransform.position);
    //    // ���� ������ �Է����� ���ƿ� �浹 ��ġ
    //    bulletLineRenderer.SetPosition(1, hitPosition[0]);
    //    //���� �������� Ȱ��ȭ�Ͽ� �Ѿ� ������ �׸���.
    //    bulletLineRenderer.enabled = true;

    //    // 0.03�� ���� ��� ó���� ���
    //    yield return new WaitForSeconds(0.03f);

    //    // ���� �������� ��Ȱ���Ͽ� �Ѿ� ������ �����
    //    bulletLineRenderer.enabled = false;
    //    gunData.LastFireTime = Time.time;
    //}

    //private IEnumerator gunShotPlay()
    //{
    //    gunAudioPlayer.PlayOneShot(gunData.ShotClip);
    //}

    private void Update()
    {
        if (target == null)
            return;

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // lineRenderer �ΰ��ϰ��� ���?
        //aimLine.SetPosition(0, fireTransform.position);
        //aimLine.SetPosition(1, dir);
        UpdateTarget();
        Fire();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gunData.FireDistance);
    }

    protected override void OnEnable()
    {
        ////healthSlider.gameObject.SetActive(true);
        //healthSlider.maxValue = startingHealth;
        //healthSlider.value = health;

        gameObject.SetActive(true);

        gunData.LastFireTime = 0;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        //healthSlider.value = health;
    }

    public override void Die()
    {
        //healthSlider.gameObject.SetActive(false);
        gameObject.SetActive(false);
        health = startingHealth;
        TestUnitManager.Instance.ReturnTurret(this);
    }
}
