using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerKind
{
    CharOriginal,
    CharUnit,
    CharTurret
}

// Enemy 매니저는 EnemySpawner에서 관리하고있음 UnitManager는 플레이어 관련
// 유닛들만 관리하는 싱글톤매니저임
// 매니저 친구들은 기능적인 부분을 담당하지 않아야한다. 플레이어의 소환이라는 기능에 데이터를 보내주고
// 소환할 모든 데이터들을 가지고 풀링으로 관리하는 기능만 가지고있어야한다.

// 풀링 생성시 -> GetComponent 를 일일이 수행함으로 인한 비용문제 발생
// 따라서 enemy 같은 경우는 자체적인 큐 자료구조를 만들어 수행하고 플레이어, npc, 스킬 오브젝트, 이펙트, 아이템, 장비등은 
// 라이브러리화 한 오브젝트 풀링을 사용하도록 계획함
public class UnitManager : MonoBehaviour
{
    private static UnitManager m_instance;

    public static UnitManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UnitManager>();
            }

            return m_instance;
        }
    }
    
    [SerializeField] private CharacterData charData;
    [SerializeField] private Transform PlayerSpawnPoint;
    [SerializeField] private FuncCamera chagnedCam;
    private Dictionary<PlayerKind, List<Character>> charPool = new Dictionary<PlayerKind, List<Character>>();
    private Character player;

    [SerializeField] private CharacterEvent characterEvent;
    [SerializeField] private StringEvent stringEvent;

    private void Awake()
    {
        // 플레이어 오리지널 캐릭터 생성
        stringEvent.Raise("PlayerRespawned");
        var playerChar = new List<Character>();
        for (int i = 0; i < charData.MaxPlayerCharacter; i++)
        {
            Character charOrigin = Instantiate(charData.CharPrefabs[i], Vector3.zero, Quaternion.identity);
            //player.onDeath += () => InsertQueue(player, index);
            charOrigin.onDeath += () =>
            {
                //AddList(charOrigin, PlayerKind.CharOriginal);
                StartCoroutine(PlayerChagne());
            };

            playerChar.Add(charOrigin);
            charOrigin.gameObject.SetActive(false);
        }

        // 플레이어 하위 유닛 생성 
        var playerUnit = new List<Character>();
        for (int i = 0; i < charData.MaxPlayerUnit; i++)
        {
            if (charData.MaxPlayerUnit == 0)
                return;
            Character charUnit = Instantiate(charData.UnitPrefabs[(int)PlayerKind.CharUnit], Vector3.zero, Quaternion.identity);
            playerUnit.Add(charUnit);
            charUnit.gameObject.SetActive(false);
        }

        // 플레이어 터렛 생성
        var playerTurret = new List<Character>();
        for (int i = 0; i < charData.MaxPlayerTurret; i++)
        {
            if (charData.MaxPlayerTurret == 0)
                return; 
            Character charTurret = Instantiate(charData.TurretPrefabs[(int)PlayerKind.CharTurret], Vector3.zero, Quaternion.identity);
            playerTurret.Add(charTurret);
            charTurret.gameObject.SetActive(false);
        }

        //딕셔너리에 생성한 큐 추가
        charPool.Add(PlayerKind.CharOriginal, playerChar);
        charPool.Add(PlayerKind.CharUnit, playerUnit);
        charPool.Add(PlayerKind.CharTurret, playerTurret);
    }

    IEnumerator PlayerChagne()
    {
        if (!player)
            yield break;

        Vector3 pos;
        pos = player.transform.position;

        yield return new WaitForSeconds(4f);
        player.gameObject.SetActive(false);
        //GameManager.instance.EndGame(charData.MaxPlayerCharacter);

        if (charPool[PlayerKind.CharOriginal].Count == 0)
            yield break;
        else
            chagnedCam.TargetSet(GetList(PlayerKind.CharOriginal).transform);
        player.transform.position = pos;
        characterEvent.Raise(player);
    }

    // 풀링 가져오기
    // 죽으면 큐에 넣어주기
    public void AddList(Character player, PlayerKind index)
    {
        // 키값이 있는지? 큐가 있는지?
        if(charPool.ContainsKey(index) && !charPool[index].Contains(player))
        {
            charPool[index].Add(player);
        }
        player.gameObject.SetActive(false);
    }
    
    // 풀링 내보내기
    // 큐에서 꺼내와서 소환하기
    public Character GetList(PlayerKind index)
    {
        //Character player = charPool[index].Dequeue();
        //if(0 >= charPool[index].Count)
        //{
        //
        //}
        if(0 >= charPool[index].Count)
        {
            return null;
        }

        player = charPool[index][0];
        charPool[index].RemoveAt(0);
        player.gameObject.SetActive(true);

        return player;
    }

    public Character GetList(PlayerKind index, Vector3 pos)
    {
        if (0 >= charPool[index].Count)
        {
            return null;
        }

        Character player = charPool[index][0];
        charPool[index].RemoveAt(0);

        player.transform.position = pos;

        player.gameObject.SetActive(true);

        return player;
    }

    public void CreateCharacter()
    {
        var index = PlayerKind.CharOriginal;
        if (charPool[index].Count <= 0)
        {
            return;
        }

        Transform spawnPoint = PlayerSpawnPoint;

        //Character player = GetQueue(index);
        player = GetList(index);
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
        characterEvent.Raise(player);
        chagnedCam.TargetSet(player.transform);
    }

    public void SwapCharacter()
    {
        Vector3 pos;

        var index = PlayerKind.CharOriginal;
        if (charPool[index].Count <= 0)
        {
            return; 
        }

        pos = player.transform.position;
        
        AddList(player, index);

        player = GetList(index);
        player.transform.position = pos;
        chagnedCam.TargetSet(player.transform);
        characterEvent.Raise(player);
        //player.onDeath += () =>
        //{
        //    InsertQueue(player, index);
        //    GetQueue(index);
        //    player.transform.position = pos;
        //    chagnedCam.TargetSet(player.transform);
        //};
    }

    //bool isDead = charPool[index].Find(player => player.dead);
    //    if (isDead == true)
    //        return;

    //public bool CheckAnotherCharDie(PlayerKind index)
    //{
    //    var player = charPool[index];
    //}

    public Character GetPlayer()
    {
        return player;
    }

    //public Character[] GetCharPool()
    //{
    //    int count = charPool[PlayerKind.CharOriginal].Count + charPool[PlayerKind.CharTurret].Count;
    //    Character[] dd = new Character[count];

    //    for (int i = 0; i < count; i++)
    //        dd[i] = charPool[PlayerKind.CharOriginal][i];
    //}
}
