using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGrenadeLauncher : GunController
{
    private GameObject player;
    private PlayerInput playerInput;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        // 사용할 점을 두개로 변경
        bulletLineRenderer.positionCount = 2;
        // 라인 렌더러 비활성화
        bulletLineRenderer.enabled = false;

        ammoRemain = gunData.AmmoRemain;
        magAmmo = gunData.MagAmmo;
        lastFireTime = gunData.LastFireTime;

        playerInput = player.GetComponent<PlayerInput>();
    }

    public override void Fire()
    {
        if (magAmmo == 0)
        {
            UIManager.instance.reloadAlarm.gameObject.SetActive(true);
            return;
        }

        if (state == State.READY && Time.time >= lastFireTime + gunData.TimeBetFire)
        {
            lastFireTime = Time.time;
            muzzleFlashEffect.Play();
            shellEjectEffect.Play();
            Shot(gunData.MaxHit);
        }
    }

    protected override void Shot(int maxHit)
    {
        //Debug.LogError("Shot");
        Ray ray = playerInput.viewCamera.ScreenPointToRay(Input.mousePosition);
        
        
        Vector3 nextVec =  transform.forward *5f;
        nextVec.y = 5;
       
        Item grenade = ItemManager.instance.GetQueue(ItemKind.ItemBulletGrenade, transform);
        Rigidbody rigidGrenade = grenade.GetComponent<Rigidbody>();
        rigidGrenade.position = fireTransform.position;
        rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
        //rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);
        /*if (Physics.Raycast(ray, out rayHit, Mathf.Abs(playerInput.viewCamera.nearClipPlane * 2)))
        {
            Debug.LogError("Shot???");
            Vector3 nextVec = rayHit.point - transform.position;
            nextVec.y = 5;

            Item grenade = ItemManager.instance.GetQueue(ItemKind.ItemBulletGrenade, transform);
            Rigidbody rigidGrenade = grenade.GetComponent<Rigidbody>();
            rigidGrenade.position = fireTransform.position;
            rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
            rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);
        }*/
        gunAudioPlayer.PlayOneShot(gunData.ShotClip);
        magAmmo--;

        //Debug.LogError("Shot end");
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
}
