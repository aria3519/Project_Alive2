using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeSence : MonoBehaviour
{
    // Start is called before the first frame update
   

    public void SceneChange()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("Stage5");
        GameManager.instance.SetHUDTrue();
        UIManager.instance.BattleTrue();
        GameManager.instance.SetDetailOnce(GameManager.instance.playerNum);
        GameManager.instance.SetNowScene((int)KindScene.KindScene_Stage); // 2 stage의 경우  
        UIManager.instance.OffNpcHotKey();
        //UnitManager.instance.gameObject.SetActive(true);

    }
    public void SceneChange2()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("Stage6");
        UIManager.instance.BattleTrue();
        GameManager.instance.SetHUDTrue();
        GameManager.instance.SetNowScene((int)KindScene.KindScene_Stage); // 2 stage의 경우  
        //UnitManager.instance.gameObject.SetActive(true);
    }

    public void SceneChange3()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("Stage7");
        UIManager.instance.BattleTrue();
        GameManager.instance.SetHUDTrue();
        GameManager.instance.SetNowScene((int)KindScene.KindScene_Stage); // 2 stage의 경우  
        //UnitManager.instance.gameObject.SetActive(true);
    }
    public void SceneChange4()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("Stage8");
        UIManager.instance.BattleTrue();
        GameManager.instance.SetHUDTrue();
        GameManager.instance.SetNowScene((int)KindScene.KindScene_Stage); // 2 stage의 경우  
        //UnitManager.instance.gameObject.SetActive(true);
    }
    public void Back()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("MainTown");
        GameManager.instance.SetHUDTrue();
        UIManager.instance.Battlefalse();
        GameManager.instance.SetNowScene((int)KindScene.KindScene_MainTown); // 1 main의 경우  
        UIManager.instance.OffNpcHotKey();
    }


}
