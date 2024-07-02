using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GunSG : GunController
{
    public float scaleLimit = 1.0f;

    private void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();


        // 사용할 점을 두개로 변경
        bulletLineRenderer.positionCount = 2;
        // 라인 렌더러 비활성화
        bulletLineRenderer.enabled = false;
    }

    public override void Fire()
    {
        // 현재 상태가 발사 가능한 상태?
        // 그리고 마지막 총 발사 시점에서 timeBetFire 이상의 시간이 지날 때
        if (state == State.READY && Time.time >= lastFireTime + gunData.TimeBetFire)
        {
            // 마지막 총 발사 시점 갱신
            lastFireTime = Time.time;
            // 실제 발사 처리 실행
            for (int i = 0; i < gunData.MaxShotCount; i++)
            {
                Shot(gunData.MaxHit);
            }
            //Shot(gunData.MaxHit);
        }
    }

    private IEnumerator ShotEffect(Vector3[] hitPosition, Transform fireTransform, int count)
    {
        //총구 화연 효과 재생
        muzzleFlashEffect.Play();
        //탄피 배출 효과 재생
        shellEjectEffect.Play();

        //총격소리 재생s
        gunAudioPlayer.PlayOneShot(gunData.ShotClip);

        //선의 시작점은 총구의 위치
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        // 선의 끝점은 입력으로 돌아온 충돌 위치
        bulletLineRenderer.SetPosition(1, hitPosition[count - 1]);
        //라인 렌더러를 활상화하여 총알 궤적을 그린다.
        bulletLineRenderer.enabled = true;

        // 0.03초 동안 잠시 처리를 대기

        yield return new WaitForSeconds(0.03f);

        // 라인 렌더러를 비활성하여 총알 궤적을 지운다
        bulletLineRenderer.enabled = false;
    }

    protected override void Shot(int maxHit)
    {
        float radius = Random.Range(0, scaleLimit);
        float angle = Random.Range(0, 10 * Mathf.PI);
        Vector3 direction = new Vector3(radius * Mathf.Cos(angle), -0.04f);
        direction = transform.TransformDirection(direction);

        // RayCast 에 의한 충돌 정보를 저장하는 컨테이너
        RaycastHit[] hits;
        //탄알이 맞은 곳을 저장할 변수
        Vector3[] hitPositionSR;


        //for (int i = 0; i < gunData.MaxShotCount; i++)
        //{
        //    hits = Physics.RaycastAll(fireTransform.position, fireTransform.forward * radius, gunData.FireDistance).OrderBy(hits => hits.distance).ToArray();
        //    listOfHits.Add(hits);
        //}

        hits = Physics.RaycastAll(fireTransform.position, direction, gunData.FireDistance).OrderBy(hits => hits.distance).ToArray();

        //for(int i =0; i < hits.Length; i++)
        //Debug.Log(hits[i].collider.name);

        int count = Mathf.Clamp(hits.Length, 1, maxHit);
        hitPositionSR = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            if (0 < hits.Length)
            {
                IDamageable target = hits[i].collider.GetComponent<IDamageable>();
                hitPositionSR[i] = hits[i].point;

                if (null != target)
                {
                    target.OnDamage(gunData.Damage, hits[i].point, hits[i].normal);
                }
            }
            else
                hitPositionSR[i] = fireTransform.position + fireTransform.forward * gunData.FireDistance;
        }

        // 발사 이펙트 재생 시작
        StartCoroutine(ShotEffect(hitPositionSR, fireTransform, count));

        // 남은 탄알 수 차감
        magAmmo--;

        if (magAmmo <= 0)
        {
            // 탄창에 남은 탄알이 없다면 총의 현재 상태를 Empty로 갱신
            state = State.EMPTY;
        }
    }
}