using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcStore : Npcbase
{
    protected override void Yes()
    {
        UIManager.instance.NPCtext.SetActive(false);
        UIManager.instance.StoreTrue();
        npcName.gameObject.SetActive(false);
        npcInfo.gameObject.SetActive(false);
    }

    protected override void No()
    {

    }

    protected override void Exit()
    {
        // 상점창 끄기 기능 추가 예정 
        UIManager.instance.StoreFalse();
        //npcName.gameObject.SetActive(true);
        npcInfo.gameObject.SetActive(true);
    }
}

