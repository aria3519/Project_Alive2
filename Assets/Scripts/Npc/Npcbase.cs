using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class Npcbase : LivingEntity
{
    [SerializeField] protected TextAsset NpcContent; // 메모장 받는곳
    [SerializeField] protected TextAsset NpcYes; // 메모장 받는곳
    [SerializeField] protected TextAsset NpcNo; // 메모장 받는곳

    [SerializeField] protected Text npcName;
    [SerializeField] protected Text npcInfo;

    protected BoxCollider interCollider;

   
    protected bool isYes;
    protected bool setOnce;

    protected abstract void Yes();
    protected abstract void No();
    protected abstract void Exit();

    protected void Awake()
    {
        setOnce = true;
    }

    private void Update()
    {
        if (npcName)
        {
            npcName.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2.8f);
            npcInfo.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2.3f);
        }

        

    }

    protected virtual void SetUiManager()
    {
        if (!isYes)
        {
           
            UIManager.instance.SetText(NpcContent.text, (null != NpcYes) ? NpcYes.text : "", (null != NpcNo) ? NpcNo.text : "");
            UIManager.instance.Yes += Yes;
            UIManager.instance.No += No;
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !UIManager.instance.isPressE)
            UIManager.instance.OnNpcHotkey();
        if (GameManager.instance.isInteractkeyDowned && other.tag == "Player")
        {
            SetUiManager();
            GameManager.instance.isInteractkeyDowned = false;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        UIManager.instance.OffNpcHotKey();
        UIManager.instance.Nulltext();
        Exit();
    }
}
