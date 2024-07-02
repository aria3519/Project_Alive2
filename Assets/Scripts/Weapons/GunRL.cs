using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRL : GunController
{
    [SerializeField] private GameObject instantRocket;
    private ParticleSystem muzzleEffect;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        muzzleEffect = EffectManager.Instance.RocketTrailEffect;

        // 사용할 점을 두개로 변경
        bulletLineRenderer.positionCount = 2;
        // 라인 렌더러 비활성화
        bulletLineRenderer.enabled = false;

        ammoRemain = gunData.AmmoRemain;
        magAmmo = gunData.MagAmmo;
        lastFireTime = gunData.LastFireTime;

        instantRocket.gameObject.SetActive(true);
    }

    public override void Fire()
    {
        if (magAmmo == 0)
            return;

        if (state == State.READY && Time.time >= lastFireTime + gunData.TimeBetFire)
        {
            lastFireTime = Time.time;
            instantRocket.SetActive(false);
            Shot(gunData.MaxHit);
        }
    }

    protected override void Shot(int maxHit)
    {
        //Ray ray = playerInput.viewCamera.ScreenPointToRay(Input.mousePosition);
        //RaycastHit rayHit;
        //if(Physics.Raycast(ray, out rayHit, 50))
        //{
        //    Vector3 nextVec = rayHit.point - transform.position;
        //    nextVec.x = 10;

        //    gunData.MagAmmo--;


        //}
        muzzleEffect.transform.position = fireTransform.position;
        muzzleEffect.transform.rotation = fireTransform.rotation;
        muzzleEffect.gameObject.SetActive(true);
        muzzleEffect.Play();
        gunAudioPlayer.PlayOneShot(gunData.ShotClip);
        magAmmo--;

        StartCoroutine(RocketActive());
        //muzzleEffect.Pause();
    }

    // 로켓런쳐 앞에 탄두 다시 켜기
    private IEnumerator RocketActive()
    {
        yield return new WaitForSeconds(1f);
        instantRocket.gameObject.SetActive(true);
    }
    protected override IEnumerator ReloadRoutine()
    {
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

        // instnatiate 하지말고 pooling 사용하기

        //탄창을 채움
        magAmmo += ammoToFill;
        //남은 탄알에서 탄창에 채운만큼 탄알을 뺀다
        ammoRemain -= ammoToFill;

        state = State.READY;
    }

    private void OnDisable()
    {
        instantRocket.gameObject.SetActive(true);
    }

    // 총에 Collision을 놔두면 안됨. 대상이 존재해야 작동을 한다. 따라서 이 부분은 따로 스크립트를 만들어서
    // RocketTrailEffect 오브젝트의 컴포넌트에 넣었음.
    //private void OnParticleCollision(GameObject other)
    //{
    //    Debug.Log("Rocket Collision");
    //    explosionEffect.transform.position = other.transform.position;
    //    explosionEffect.gameObject.SetActive(true);
    //    explosionEffect.Play();

    //    RaycastHit[] rayHits = Physics.SphereCastAll(other.transform.position, 3, Vector3.up
    //        , 0f, LayerMask.GetMask("Enemy"));

    //    foreach (RaycastHit hitObj in rayHits)
    //    {
    //        hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
    //    }
    //}
}
