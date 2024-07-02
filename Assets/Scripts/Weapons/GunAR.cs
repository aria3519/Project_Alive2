using System.Collections;
using UnityEngine;

public class GunAR : GunController
{
    // Start is called before the first frame update
    private void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        // ����� ���� �ΰ��� ����
        bulletLineRenderer.positionCount = 2;
        // ���� ������ ��Ȱ��ȭ
        bulletLineRenderer.enabled = false;
    }
}
