using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGunMG : TestGunController
{
    private void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
    }
}
