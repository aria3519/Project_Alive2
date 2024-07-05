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

        // ����� ���� �ΰ��� ����
        bulletLineRenderer.positionCount = 2;
        // ���� ������ ��Ȱ��ȭ
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
}
