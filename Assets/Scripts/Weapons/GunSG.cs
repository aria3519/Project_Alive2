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


        // ����� ���� �ΰ��� ����
        bulletLineRenderer.positionCount = 2;
        // ���� ������ ��Ȱ��ȭ
        bulletLineRenderer.enabled = false;
    }

    public override void Fire()
    {
        // ���� ���°� �߻� ������ ����?
        // �׸��� ������ �� �߻� �������� timeBetFire �̻��� �ð��� ���� ��
        if (state == State.READY && Time.time >= lastFireTime + gunData.TimeBetFire)
        {
            // ������ �� �߻� ���� ����
            lastFireTime = Time.time;
            // ���� �߻� ó�� ����
            for (int i = 0; i < gunData.MaxShotCount; i++)
            {
                Shot(gunData.MaxHit);
            }
            //Shot(gunData.MaxHit);
        }
    }

    private IEnumerator ShotEffect(Vector3[] hitPosition, Transform fireTransform, int count)
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
        bulletLineRenderer.SetPosition(1, hitPosition[count - 1]);
        //���� �������� Ȱ��ȭ�Ͽ� �Ѿ� ������ �׸���.
        bulletLineRenderer.enabled = true;

        // 0.03�� ���� ��� ó���� ���

        yield return new WaitForSeconds(0.03f);

        // ���� �������� ��Ȱ���Ͽ� �Ѿ� ������ �����
        bulletLineRenderer.enabled = false;
    }

    protected override void Shot(int maxHit)
    {
        float radius = Random.Range(0, scaleLimit);
        float angle = Random.Range(0, 10 * Mathf.PI);
        Vector3 direction = new Vector3(radius * Mathf.Cos(angle), -0.04f);
        direction = transform.TransformDirection(direction);

        // RayCast �� ���� �浹 ������ �����ϴ� �����̳�
        RaycastHit[] hits;
        //ź���� ���� ���� ������ ����
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

        // �߻� ����Ʈ ��� ����
        StartCoroutine(ShotEffect(hitPositionSR, fireTransform, count));

        // ���� ź�� �� ����
        magAmmo--;

        if (magAmmo <= 0)
        {
            // źâ�� ���� ź���� ���ٸ� ���� ���� ���¸� Empty�� ����
            state = State.EMPTY;
        }
    }
}