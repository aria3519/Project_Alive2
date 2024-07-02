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

        // ����� ���� �ΰ��� ����
        bulletLineRenderer.positionCount = 2;
        // ���� ������ ��Ȱ��ȭ
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

    // ���Ϸ��� �տ� ź�� �ٽ� �ѱ�
    private IEnumerator RocketActive()
    {
        yield return new WaitForSeconds(1f);
        instantRocket.gameObject.SetActive(true);
    }
    protected override IEnumerator ReloadRoutine()
    {
        // ���� ���¸� ������ �� ���·� ��ȯ
        state = State.RELOADING;

        // ������ �Ҹ� ���
        gunAudioPlayer.PlayOneShot(gunData.ReloadClip);

        // ������ �ҿ� �ð� ��ŭ ó���� ����
        yield return new WaitForSeconds(gunData.ReloadTime);
        
        int ammoToFill = gunData.MagCapacity - magAmmo;

        // źâ�� ä������ ź���� ���� ź�˺��� ���ٸ�
        // ä���� �� ź�� ���� ���� ź�� ���� ���� ���δ�.
        if (ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }

        // instnatiate �������� pooling ����ϱ�

        //źâ�� ä��
        magAmmo += ammoToFill;
        //���� ź�˿��� źâ�� ä�ŭ ź���� ����
        ammoRemain -= ammoToFill;

        state = State.READY;
    }

    private void OnDisable()
    {
        instantRocket.gameObject.SetActive(true);
    }

    // �ѿ� Collision�� ���θ� �ȵ�. ����� �����ؾ� �۵��� �Ѵ�. ���� �� �κ��� ���� ��ũ��Ʈ�� ����
    // RocketTrailEffect ������Ʈ�� ������Ʈ�� �־���.
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
