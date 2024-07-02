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

    [SerializeField] protected Transform fireTransform; // 총알이 발사될 위치
    [SerializeField] ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    [SerializeField] ParticleSystem shellEjectEffect; // 탄피 배출 효과
    protected LineRenderer bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러
    protected AudioSource gunAudioPlayer; // 총 소리 재생기

    private float lastFireTime;
    [SerializeField] private BulletType setBulletType;

    LineRenderer aimLine;
    Vector3 aimStartPos;
    private void Start()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        aimLine = GetComponent<LineRenderer>();

        // 사용할 점을 두개로 변경
        bulletLineRenderer.positionCount = 2;
        // 라인 렌더러 비활성화
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
        // 현재 상태가 발사 가능한 상태?
        // 그리고 마지막 총 발사 시점에서 timeBetFire 이상의 시간이 지날 때
        if (Time.time >= lastFireTime + gunData.TimeBetFire)
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
            gunAudioPlayer.PlayOneShot(gunData.ShotClip);
        }
    }

    //public void Shot(int maxHit)
    //{
    //    RaycastHit[] hits;
    //    //탄알이 맞은 곳을 저장할 변수
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

    //            // 벽관통 제한 부분. 다시 확인바람.
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

    //    // 발사 이펙트 재생 시작
    //    StartCoroutine(ShotEffect(hitPositionSR, fireTransform));
    //}

    //private IEnumerator ShotEffect(Vector3[] hitPosition, Transform fireTransform)
    //{
    //    //총구 화연 효과 재생
    //    muzzleFlashEffect.Play();
    //    //탄피 배출 효과 재생
    //    shellEjectEffect.Play();

    //    //총격소리 재생s
    //    gunAudioPlayer.PlayOneShot(gunData.ShotClip);

    //    //선의 시작점은 총구의 위치
    //    bulletLineRenderer.SetPosition(0, fireTransform.position);
    //    // 선의 끝점은 입력으로 돌아온 충돌 위치
    //    bulletLineRenderer.SetPosition(1, hitPosition[0]);
    //    //라인 렌더러를 활상화하여 총알 궤적을 그린다.
    //    bulletLineRenderer.enabled = true;

    //    // 0.03초 동안 잠시 처리를 대기
    //    yield return new WaitForSeconds(0.03f);

    //    // 라인 렌더러를 비활성하여 총알 궤적을 지운다
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

        // lineRenderer 두개일경우는 어떻게?
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
