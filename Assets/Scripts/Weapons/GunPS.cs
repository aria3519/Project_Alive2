using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPS : GunController
{
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