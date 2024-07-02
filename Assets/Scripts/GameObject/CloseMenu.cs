using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseMenu : MonoBehaviour
{
    [SerializeField] private GameObject goMeun;
    [SerializeField] private GameObject goMain;
    // Start is called before the first frame update

    private void Update()
    {
        
       
            if (Input.GetKeyDown(KeyCode.Escape)
                && GameManager.instance.GetNowScene() == (int)KindScene.KindScene_Stage)
            {

                goMain.SetActive(true);

            }
            else if (Input.GetKeyDown(KeyCode.Escape)
                && GameManager.instance.GetNowScene() == (int)KindScene.KindScene_MainTown)
            {
                goMeun.SetActive(true);
            } 
        
    }
    public void GoMain()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("MainTown");
        GameManager.instance.SetHUDTrue();
        GameManager.instance.isWaveOn = false;
        goMain.SetActive(false);
        GameManager.instance.SetNowScene((int)KindScene.KindScene_MainTown);
        NpcManager.instance.InitRescuedList();
        GameManager.instance.InitDeathCount();

        TestUnitManager.Instance.ResetPlayerList();
    }
    public void BackGoMain()
    {
        goMain.SetActive(false);

    }
    public void GoMeun()
    {
        SceneManager.LoadScene("Menu");
        GameManager.instance.SetHUDTrue();
        goMeun.SetActive(false);
        GameManager.instance.SetNowScene((int)KindScene.KindScene_Menu);
        TestUnitManager.Instance.InitFirstState();
    }
    public void BackGoMeun()
    {

        goMeun.SetActive(false);

    }
}
