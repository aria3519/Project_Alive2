using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcUniqueStore : Npcbase
{
    private bool once;
    /*protected override void SetUiManager()
    {
        if (!isYes)
        {
           
            UIManager.instance.SetText(NpcContent.text, (null != NpcYes) ? NpcYes.text : "", (null != NpcNo) ? NpcNo.text : "");
            UIManager.instance.Yes += Yes;
            UIManager.instance.No += No;

        }
    }*/
    private void Start()
    {
        once = true;
    }

    protected override void Yes()
    {
        UIManager.instance.NPCtext.SetActive(false);
        UIManager.instance.UniqueStoreTrue();
        npcName.gameObject.SetActive(false);
        npcInfo.gameObject.SetActive(false);
    }
    protected override void No()
    {
        
        
       
    }

    protected override void Exit()
    {
        // 상점창 끄기 기능 추가 예정 
        UIManager.instance.UniqueStoreFalse();
        npcName.gameObject.SetActive(true);
        npcInfo.gameObject.SetActive(true);
    }

}






