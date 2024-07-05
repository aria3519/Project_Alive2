using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnitManager : MonoBehaviour
{
    private static TestUnitManager m_instance;
    public static TestUnitManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<TestUnitManager>();
            }

            return m_instance;
        }
    }

    [SerializeField] private CharacterData charData;
    [SerializeField] private Transform PlayerSpawnPoint;
    [SerializeField] private FuncCamera chagnedCam;

    [SerializeField] private CharacterEvent characterEvent;
    private List<Character> playerList = new List<Character>();
    private List<Character> tempPlayerList = new List<Character>();
    private List<Character> turretList = new List<Character>();
    private List<Character> useTurretList = new List<Character>();
    private Character currentPlayer;

    // 테스트 용
    [SerializeField] private string playerName1 = "Test_Alice";
    [SerializeField] private string playerName2 = "Test_Bomber";
    [SerializeField] private string playerName3 = "Test_Soldier";
    private string turretName = "TurretA";

    // 디버그용
    [SerializeField] private bool isDebug = false;

    // 스왑 쿨타임
    private float lastSwapTime;
    private float swapTime = 5f;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 터렛 풀링
        int turretCount = ObjectPoolingManager.instance.GetPoolCount(turretName);

        for (int i = 0; i < turretCount; i++)
        {
            turretList.Add(ObjectPoolingManager.instance.GetObject<Character>(turretName));
            turretList[i].gameObject.SetActive(false);
        }

        // 플레이어 풀링
        playerList.Add(ObjectPoolingManager.instance.GetObject<Character>(playerName1));
        playerList.Add(ObjectPoolingManager.instance.GetObject<Character>(playerName2));
        playerList.Add(ObjectPoolingManager.instance.GetObject<Character>(playerName3));
       


        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].gameObject.SetActive(false);
            playerList[i].onDeath += () =>
            {
                StartCoroutine(PlayerChangeWhenDie());
            };
        }
        // 테스트용
        if (isDebug == true)
        {
            currentPlayer = GetPlayerList();
            currentPlayer.gameObject.SetActive(true);
            currentPlayer.transform.position = PlayerSpawnPoint.position;
            characterEvent.Raise(currentPlayer);
            chagnedCam.TargetSet(currentPlayer.transform);
        }
    }

    public void CreatePlayer(int num = 0)
    {
        if (PlayerSpawnPoint)
        {
            currentPlayer = GetPlayerList();
            currentPlayer.gameObject.SetActive(true);
            currentPlayer.transform.position = PlayerSpawnPoint.position;
            characterEvent.Raise(currentPlayer);
            chagnedCam.TargetSet(currentPlayer.transform);

            for (int i = 0; i < num; i++)
            {
                Vector3 pos;
                pos = currentPlayer.transform.position;
                currentPlayer.ResetSP();
                ReturnPlayerList(currentPlayer);
                currentPlayer.ResetSP();
                ReturnPlayerList(currentPlayer);
                currentPlayer = GetPlayerList();
                currentPlayer.transform.position = pos;
                chagnedCam.TargetSet(currentPlayer.transform);
                characterEvent.Raise(currentPlayer);
                UIManager.instance.reloadAlarm.gameObject.SetActive(false);
            }

        }
    }

    public void InitFirstState()
    {
        //ReturnPlayerList(currentPlayer);
        List<Character> tempCharList = new List<Character>();
        tempCharList.Add(currentPlayer);
        for(int i = 0; i < playerList.Count; i++)
        {
            tempCharList.Add(playerList[i]);
        }

        playerList.Clear();
        playerList = tempCharList;

        currentPlayer = null;
    }

    public void UpdateCurrentPlayerInfo()
    {
        characterEvent.Raise(currentPlayer);
        
    }

    IEnumerator PlayerChangeWhenDie()
    {
        if (!currentPlayer)
            yield break;

        Vector3 pos;
        pos = currentPlayer.transform.position;

        yield return new WaitForSeconds(4f);
        currentPlayer.gameObject.SetActive(false);
        GameManager.instance.EndGame(charData.MaxPlayerCharacter);

        if (playerList.Count <= 0 && currentPlayer.dead)
        {
            tempPlayerList.Add(currentPlayer);
            ResetPlayerList();
            yield break;
        }
        else
        {
            tempPlayerList.Add(currentPlayer);
            currentPlayer = GetPlayerList();
            chagnedCam.TargetSet(currentPlayer.transform);
        }

        SkillManager.instance.isCharChanged = true;
        StartCoroutine(currentPlayer.InvincibleTime());
        currentPlayer.transform.position = pos;
        characterEvent.Raise(currentPlayer);
    }

    public void SwapCharacter()
    {
        if (playerList.Count <= 0) return;

        if (Time.time >= lastSwapTime + swapTime)
        {
            UIManager.instance.UpdateCharacterSwapTime(swapTime);
            SkillManager.instance.isCharChanged = true;
            lastSwapTime = Time.time;
            Vector3 pos;
            pos = currentPlayer.transform.position;
            currentPlayer.ResetSP();
            ReturnPlayerList(currentPlayer);

            currentPlayer = GetPlayerList();
            currentPlayer.transform.position = pos;
            chagnedCam.TargetSet(currentPlayer.transform);
            characterEvent.Raise(currentPlayer);
            UIManager.instance.reloadAlarm.gameObject.SetActive(false);
        }
    }

    private Character GetPlayerList()
    {
        if (playerList.Count <= 0) return null;

        currentPlayer = playerList[0];
        playerList.RemoveAt(0);
        currentPlayer.gameObject.SetActive(true);

        return currentPlayer;
    }

    private void ReturnPlayerList(Character player)
    {
        if (!playerList.Contains(player)) playerList.Add(player);
        player.gameObject.SetActive(false);
    }

    public void SpawnTurret(Vector3 pos)
    {
        if (turretList.Count <= 0) return;

        Character turret = turretList[0];
        turretList.RemoveAt(0);

        turret.transform.position = pos;
        turret.gameObject.SetActive(true);

        useTurretList.Add(turret);
    }

    public void ReturnTurret(Character turret)
    {
        if (!turretList.Contains(turret))
            turretList.Add(turret);
        turret.gameObject.SetActive(false);

        useTurretList.RemoveAt(0);
    }

    public void ResetTurret()
    {
        for (int i = 0; i < useTurretList.Count; i++)
            turretList.Add(useTurretList[i]);

        useTurretList.Clear();
    }

    public void GetStartPoint(Transform StartPoint)
    {
        currentPlayer.SetStart();
        for (int i = 0; playerList.Count > i; i++)
        {
            playerList[i].SetStart();
        }
        currentPlayer.transform.position = StartPoint.position;
    }

    public void UseSupplySkill(int ammo)
    {
        currentPlayer.GetAmmo(ammo);
    }

    public void ResetPlayerList()
    {
        if (currentPlayer.dead) // 다죽었을 경우
        {
            for(int i = 0; i < tempPlayerList.Count; i++)
            {
                ReturnPlayerList(tempPlayerList[i]);
            }
            tempPlayerList.Clear();
            SetOnDeathAll();
            currentPlayer = GetPlayerList();
            chagnedCam.TargetSet(currentPlayer.transform);
            currentPlayerFalse();
        }
        else
        {
            for (int i = 0; i < tempPlayerList.Count; i++)
            {
                tempPlayerList[i].onDeath += () => StartCoroutine(PlayerChangeWhenDie());
                ReturnPlayerList(tempPlayerList[i]);
            }
            tempPlayerList.Clear();
        }

        // 체력 및 탄약회복
        currentPlayer.ResetCharacter();
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].ResetCharacter();
        }
        characterEvent.Raise(currentPlayer);
        GameManager.instance.isGameover = false;
    }

    private void SetOnDeathAll()
    {
        for(int i = 0; i < playerList.Count; i++)
            playerList[i].onDeath += () => StartCoroutine(PlayerChangeWhenDie());
    }

    public void currentPlayerTrue()
    {
        currentPlayer.gameObject.SetActive(true);
    }

    public void currentPlayerFalse()
    {
        currentPlayer.gameObject.SetActive(false);
    }
    public void AfterClear(int addExp)
    {
        // 게임 클리어시 보상  
        currentPlayer.setClearData(addExp);
        for (int i = 0; playerList.Count > i; i++)
        {
            playerList[i].setClearData(addExp);
        }
        //

        // 게임 클리어시 저장  
        GameManager.instance.SetXML(currentPlayer.SendCharData());
        for (int i = 0; playerList.Count > i; i++)
        {
            GameManager.instance.SetXML(playerList[i].SendCharData());
        }

    }


    public void InputBuff(Buff buff)
    {
        currentPlayer.CharBuff(buff);
        for (int i = 0; playerList.Count > i; i++)
        {
            playerList[i].CharBuff(buff);
        }
    }

    public void SetLoadData(List<CharData> listcharData)
    {
        List<Character> tempPlayerList = new List<Character>();
        if (currentPlayer != null)
            tempPlayerList.Add(currentPlayer);
        for (int i = 0; i < playerList.Count; i++)
        {
            tempPlayerList.Add(playerList[i]);
        }
        /* tempPlayerList.Sort((a, b) => string.Compare(a.name, b.name));*/

        for (int i = 0; i < tempPlayerList.Count; i++)
        {
            for (int j = 0; j < listcharData.Count; j++)
            {

                if (tempPlayerList[i].CheckID(listcharData[j].name))
                {
                    tempPlayerList[i].SetCharData(listcharData[j]);
                    break;
                }
            }
        }
    }

        public void GetBuySKill(int skill, string id)
        {
            if (currentPlayer.CheckID(id))
            {
                currentPlayer.SetMySkill(skill);
            }
            else
            {
                for (int i = 0; playerList.Count > i; i++)
                {
                    if (playerList[i].CheckID(id))
                    {
                        playerList[i].SetMySkill(skill);
                    }
                }
            }


        }

    public List<Character> GetPlayerLists()
    {
        var tempList = playerList;

        return tempList;
    }


}

    



