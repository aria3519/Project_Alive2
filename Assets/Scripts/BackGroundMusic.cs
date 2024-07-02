using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    [SerializeField] private AudioClip[] Music;
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
            RandomPlay();
    }

    private void RandomPlay()
    {
        audioSource.clip = Music[Random.Range(0, Music.Length)];
        audioSource.Play();
    }
}
