using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;





public class NpcPortal : Npcbase
{
    //private Menu Menu;
    [SerializeField] GameObject stageSelect;
    protected override void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
            UIManager.instance.OnNpcHotkey();

        if (GameManager.instance.isInteractkeyDowned)
        {
            stageSelect.gameObject.SetActive(true);
            npcName.gameObject.SetActive(false);
            npcInfo.gameObject.SetActive(false);
            GameManager.instance.isInteractkeyDowned = false;
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        UIManager.instance.OffNpcHotKey();
        stageSelect.gameObject.SetActive(false);
        npcName.gameObject.SetActive(true);
        npcInfo.gameObject.SetActive(true);
    }
    protected override void Yes()
    {
    }

    protected override void No()
    {

    }

    protected override void Exit()
    {
    }
}
