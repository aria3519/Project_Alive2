using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;





public class NpcPortal : Npcbase
{
    //private Menu Menu;
    [SerializeField] private GameObject stageSelect;
    [SerializeField] private Image _myIamge;
   /* [SerializeField] private Camera _myCam;
    [SerializeField] private Animator _anim;



    private void Start()
    {
        _myCam = GameObject.Find("Camera").GetComponent<Camera>();
        _anim = GameObject.Find("Camera").GetComponent<Animator>();
    }*/



    // g키 클릭시 보여줄 애니메이션 
    /*private IEnumerator ShowWindow()
    {
        _anim.SetBool("AniStart", true);
        yield return new WaitForSeconds(2f);
        _anim.SetBool("AniStart", false);
    }*/


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
        //npcName.gameObject.SetActive(true);
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
