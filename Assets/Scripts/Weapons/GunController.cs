using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum State
{
    READY, // 발사 준비됨
    EMPTY, // 탄창이 빔
    RELOADING // 재장전 중
}

public enum WeaponType
{
    Melee,
    Range
}
public abstract class GunController : MonoBehaviour
{
    [SerializeField] protected GunData gunData;
    public State state { get; protected set; } // 현재 총의 상태

    [SerializeField] protected Transform fireTransform; // 총알이 발사될 위치
    [SerializeField] protected ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    [SerializeField] protected ParticleSystem shellEjectEffect; // 탄피 배출 효과
    protected LineRenderer bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러
    protected AudioSource gunAudioPlayer; // 총 소리 재생기
    public Transform leftHandMount;
    public Transform rightHandMount;

    protected int ammoRemain;
    protected int magAmmo;
    protected float lastFireTime;

    private void Start()
    {
        ammoRemain = gunData.AmmoRemain;
        magAmmo = gunData.MagAmmo;
        lastFireTime = gunData.LastFireTime;
    }

    // AmmoPack 먹으면 ammo 증가
    public void getAmmoPack(int ammo)
    {
        ammoRemain += ammo;
    }
    // PlayerShooter의 UI부분
    public int getMagAmmo()
    {
        return magAmmo;
    }
    public int getAmmoRemain()
    {
        return ammoRemain;
    }
    public WeaponType getWeaponType()
    {
        return gunData.WeaponType;
    }
    public void ResetAmmo()
    {
        magAmmo = gunData.MagAmmo;
        ammoRemain = gunData.AmmoRemain;
    }
    public virtual void Fire()
    {
        // 현재 상태가 발사 가능한 상태?
        // 그리고 마지막 총 발사 시점에서 timeBetFire 이상의 시간이 지날 때
        if (state == State.READY && Time.time >= lastFireTime + gunData.TimeBetFire)
        {
            // 마지막 총 발사 시점 갱신
            lastFireTime = Time.time;
            // 실제 발사 처리 실행
            Shot(gunData.MaxHit);
        }
    }
    public bool Reload()
    {
        if (state == State.RELOADING || ammoRemain <= 0 || magAmmo > gunData.MagCapacity)
        {
            // 이미 재장전 중이거나 남은 탄알이 없거나
            // 탄창에 탄알이 이미 가득한 경우 재장전 불가
            return false;
        }
        // 재장전 처리 시작
        StartCoroutine(ReloadRoutine());
        return true;
    }

    protected virtual IEnumerator ReloadRoutine()
    {
        UIManager.instance.activateReloadingText();
        // 현재 상태를 재장전 중 상태로 전환
        state = State.RELOADING;

        // 재장전 소리 재생
        gunAudioPlayer.PlayOneShot(gunData.ReloadClip);

        // 재장전 소요 시간 만큼 처리를 쉬기
        yield return new WaitForSeconds(gunData.ReloadTime);

        int ammoToFill = gunData.MagCapacity - magAmmo;

        // 탄창에 채워야할 탄알이 남은 탄알보다 많다면
        // 채워야 할 탄알 수를 남은 탄알 수에 맞춰 줄인다.
        if (ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }

        //탄창을 채움
        magAmmo += ammoToFill;
        //남은 탄알에서 탄창에 채운만큼 탄알을 뺀다
        ammoRemain -= ammoToFill;

        state = State.READY;
        UIManager.instance.deactivateReloadingText();
    }

    protected virtual void Shot(int maxHit)
    {
        // RayCast 에 의한 충돌 정보를 저장하는 컨테이너
        RaycastHit[] hits;
        //탄알이 맞은 곳을 저장할 변수
        Vector3[] hitPositionSR;

        //hits = Physics.RaycastAll(fireTransform.position, fireTransform.forward, gunData.FireDistance);
        hits = Physics.RaycastAll(fireTransform.position, fireTransform.forward, gunData.FireDistance).OrderBy(hits => hits.distance).ToArray();
        

        int count = Mathf.Clamp(hits.Length, 1, maxHit);
        hitPositionSR = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            if (0 < hits.Length)
            {
                //IDamageable checkWall = hits[count - 1].collider.GetComponent<IDamageable>();
                IDamageable target = hits[i].collider.GetComponent<IDamageable>();
                hitPositionSR[i] = hits[i].point;

                //벽관통 제한 부분.다시 확인바람.
                //if (null == checkWall)
                //{
                //    hitPositionSR[i] = hits[count - 1].point;
                //    break;
                //}

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
            UIManager.instance.reloadAlarm.gameObject.SetActive(true);
        }
    }

    protected IEnumerator ShotEffect(Vector3[] hitPosition, Transform fireTransform, int count)
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
        bulletLineRenderer.SetPosition(1, hitPosition[count-1]);
        //라인 렌더러를 활상화하여 총알 궤적을 그린다.
        bulletLineRenderer.enabled = true;

        // 0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.03f);

        // 라인 렌더러를 비활성하여 총알 궤적을 지운다
        bulletLineRenderer.enabled = false;
    }

    // Update is called once per frame
    protected void OnEnable()
    {
        // 현재 탄창 가득 채우기
        //magAmmo = gunData.MagCapacity;
        // 총의 현재 상태를 총을 쏠 준비가 된 상태로 변경
        if(state != State.EMPTY)
            state = State.READY;
        // 마지막으로 총을 쏜 시점을 초기화
        lastFireTime = 0;
    }
}