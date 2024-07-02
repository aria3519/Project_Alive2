using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum State
{
    READY, // �߻� �غ��
    EMPTY, // źâ�� ��
    RELOADING // ������ ��
}

public enum WeaponType
{
    Melee,
    Range
}
public abstract class GunController : MonoBehaviour
{
    [SerializeField] protected GunData gunData;
    public State state { get; protected set; } // ���� ���� ����

    [SerializeField] protected Transform fireTransform; // �Ѿ��� �߻�� ��ġ
    [SerializeField] protected ParticleSystem muzzleFlashEffect; // �ѱ� ȭ�� ȿ��
    [SerializeField] protected ParticleSystem shellEjectEffect; // ź�� ���� ȿ��
    protected LineRenderer bulletLineRenderer; // �Ѿ� ������ �׸��� ���� ������
    protected AudioSource gunAudioPlayer; // �� �Ҹ� �����
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

    // AmmoPack ������ ammo ����
    public void getAmmoPack(int ammo)
    {
        ammoRemain += ammo;
    }
    // PlayerShooter�� UI�κ�
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
        // ���� ���°� �߻� ������ ����?
        // �׸��� ������ �� �߻� �������� timeBetFire �̻��� �ð��� ���� ��
        if (state == State.READY && Time.time >= lastFireTime + gunData.TimeBetFire)
        {
            // ������ �� �߻� ���� ����
            lastFireTime = Time.time;
            // ���� �߻� ó�� ����
            Shot(gunData.MaxHit);
        }
    }
    public bool Reload()
    {
        if (state == State.RELOADING || ammoRemain <= 0 || magAmmo > gunData.MagCapacity)
        {
            // �̹� ������ ���̰ų� ���� ź���� ���ų�
            // źâ�� ź���� �̹� ������ ��� ������ �Ұ�
            return false;
        }
        // ������ ó�� ����
        StartCoroutine(ReloadRoutine());
        return true;
    }

    protected virtual IEnumerator ReloadRoutine()
    {
        UIManager.instance.activateReloadingText();
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

        //źâ�� ä��
        magAmmo += ammoToFill;
        //���� ź�˿��� źâ�� ä�ŭ ź���� ����
        ammoRemain -= ammoToFill;

        state = State.READY;
        UIManager.instance.deactivateReloadingText();
    }

    protected virtual void Shot(int maxHit)
    {
        // RayCast �� ���� �浹 ������ �����ϴ� �����̳�
        RaycastHit[] hits;
        //ź���� ���� ���� ������ ����
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

                //������ ���� �κ�.�ٽ� Ȯ�ιٶ�.
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
        
        // �߻� ����Ʈ ��� ����
        StartCoroutine(ShotEffect(hitPositionSR, fireTransform, count));
        // ���� ź�� �� ����
        magAmmo--;

        if (magAmmo <= 0)
        {
            // źâ�� ���� ź���� ���ٸ� ���� ���� ���¸� Empty�� ����
            state = State.EMPTY;
            UIManager.instance.reloadAlarm.gameObject.SetActive(true);
        }
    }

    protected IEnumerator ShotEffect(Vector3[] hitPosition, Transform fireTransform, int count)
    {
        //�ѱ� ȭ�� ȿ�� ���
        muzzleFlashEffect.Play();
        //ź�� ���� ȿ�� ���
        shellEjectEffect.Play();

        //�ѰݼҸ� ���s
        gunAudioPlayer.PlayOneShot(gunData.ShotClip);
        
        //���� �������� �ѱ��� ��ġ
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        // ���� ������ �Է����� ���ƿ� �浹 ��ġ
        bulletLineRenderer.SetPosition(1, hitPosition[count-1]);
        //���� �������� Ȱ��ȭ�Ͽ� �Ѿ� ������ �׸���.
        bulletLineRenderer.enabled = true;

        // 0.03�� ���� ��� ó���� ���
        yield return new WaitForSeconds(0.03f);

        // ���� �������� ��Ȱ���Ͽ� �Ѿ� ������ �����
        bulletLineRenderer.enabled = false;
    }

    // Update is called once per frame
    protected void OnEnable()
    {
        // ���� źâ ���� ä���
        //magAmmo = gunData.MagCapacity;
        // ���� ���� ���¸� ���� �� �غ� �� ���·� ����
        if(state != State.EMPTY)
            state = State.READY;
        // ���������� ���� �� ������ �ʱ�ȭ
        lastFireTime = 0;
    }
}