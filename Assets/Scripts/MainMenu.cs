using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;


    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle fullScreenToggle;
    public int[] screenWidths;
    int activeScreenResIndex;


    [SerializeField] private GameObject LoadScene;

    private void Start()
    {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen = (PlayerPrefs.GetInt("fullscreen", 1) > 0);

        //volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        //volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        //volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        //for(int i = 0; i < resolutionToggles.Length; i++)
        //{
        //    resolutionToggles[i].isOn = i == activeScreenResIndex;
        //}

        //fullScreenToggle.isOn = isFullscreen;
        //UnitManager.instance.gameObject.SetActive(false);

    }



    public void Play()
    {


        SceneManager.LoadScene("MainTown");
        GameManager.instance.SetDetailOnce();
        UIManager.instance.StatusTrue();
        GameManager.instance.SetHUDTrue();
        GameManager.instance.GetSceneInfo(true);
        UIManager.instance.Battlefalse();
        GameManager.instance.SetNowScene((int)KindScene.KindScene_MainTown); // 1 maintown의 경우  
        //UnitManager.instance.gameObject.SetActive(true);

    }
    public void RePlay()
    {
        GameManager.instance.isRePlay = true;
        Play();

    }
    public void GoLoad()
    {
        LoadScene.SetActive(true);
    }
    public void Load1()
    {
      
        TestUnitManager.Instance.SetLoadData(GameManager.instance.SetNowFile("Save1")); // 안에서 로드함 
        Play();
    }
    public void load2()
    {
      
        TestUnitManager.Instance.SetLoadData(GameManager.instance.SetNowFile("Save2"));
        Play();
    }
    public void load3()
    {
       
        TestUnitManager.Instance.SetLoadData(GameManager.instance.SetNowFile("Save3"));
        Play();
    }

    public void backLoad()
    {
        LoadScene.SetActive(false);
    }


    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        GameManager.instance.isMenu = true;
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void SetMainMenu()
    {
        GameManager.instance.isMenu = true;
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }

    public void SetScreenResolution(int i)
    {
        if(resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        for(int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }

        if(isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenResIndex);
        }
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }
    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }
    public void SetSfxVolume(float value)
    {
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }
}
