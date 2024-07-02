using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, Sfx, Music };

    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    [SerializeField] private AudioSource sfxEnemySource;
    [SerializeField] AudioSource musicSources;
    int activeMusicSourceIndex;

    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }

            return instance;
        }
    }

    Transform audioListener;
    Transform playerT;

    private void Awake()
    {
        //musicSources = GetComponent<AudioSource>();
        //sfxEnemySource = GetComponent<AudioSource>();

        audioListener = FindObjectOfType<AudioListener>().transform;
        if (FindObjectOfType<PlayerMovement>() != null)
        {
            playerT = FindObjectOfType<PlayerMovement>().transform;
        }

        masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
        sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
        musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch(channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }

        //musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        //musicSources[1].volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
    }

    public void PlayEnemyDeathSound(AudioClip clip)
    {
        sfxEnemySource.PlayOneShot(clip);
    }
}
