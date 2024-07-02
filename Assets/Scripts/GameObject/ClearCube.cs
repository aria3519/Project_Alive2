using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCube : MonoBehaviour
{
    [SerializeField] private int skillMaxSpawnCount;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] private float Range = 1;
    [SerializeField] GameObject wall;
    [SerializeField] ParticleSystem rescueEffect;
    private CapsuleCollider collider;

    private bool isEnter = false;
    private bool once = true;

    private int clearTime = 13;

    private void Start()
    {
        collider = GetComponent<CapsuleCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !isEnter)
        {
            if (once)
                UIManager.instance.clearCubeText.gameObject.SetActive(true);
            isEnter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isEnter = false;
            UIManager.instance.clearCubeText.gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        if (isEnter && Input.GetKeyDown("e")&&once)
        {
            collider.enabled = false;
            UIManager.instance.clearCubeText.gameObject.SetActive(false);
            once = false;
            StartCoroutine(UpdatePath());
        }
    }
    private void OnDisable()
    {
        isEnter = false;

    }

    private void OnDrawGizmosSelected()
    {
        // 범위 그려주는 코드 
        Gizmos.DrawWireSphere(transform.position, Range);
        //Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
    private IEnumerator UpdatePath()
    {
        GameManager.instance.isWaveOn = true;
        /*EnemyManager.Instance.SpawnUnitsSkill(spawnPoints, skillMaxSpawnCount);*/
        // 3분뒤에 클리어 박스 해제 
        rescueEffect.Play();
        wall.SetActive(true);
        UIManager.instance.OnGameClearTimer(clearTime);
        StartCoroutine(ClearTimer());
        yield return new WaitForSeconds(clearTime);
        UIManager.instance.OffGameClearTimer();
        GameManager.instance.gameClear();
        
        EnemyManager.Instance.SpawnUnitsSkill(spawnPoints, skillMaxSpawnCount);

        Debug.Log("GameClear");
        UIManager.instance.ClearTrue();
        TestUnitManager.Instance.ResetPlayerList();
        GameManager.instance.InitDeathCount();
        TestUnitManager.Instance.currentPlayerFalse();
        GameManager.instance.isWaveOn = false;
        wall.SetActive(false);
        yield return new WaitForSeconds(5f);

        collider.enabled = true;
        ItemManager.instance.DisableRescueHelli();
        UIManager.instance.ClearFalse();
        GameManager.instance.SetNowScene((int)KindScene.KindScene_MainTown); // 1 maintown의 경우  
        UIManager.instance.Battlefalse();
        SceneChange();
    }

    IEnumerator ClearTimer()
    {
        for(int i = 0; i < clearTime; i++)
        {
            UIManager.instance.UpdateClearTimeText(1f);
            yield return new WaitForSeconds(1f);
        }
    }


    public void SceneChange()
    {
        SceneManager.LoadScene("MainTown");
        GameManager.instance.NotChangeStage();
        TestUnitManager.Instance.currentPlayerTrue();
        // 메인타운으로 갈때마다 데이터 저장 

    }

}
