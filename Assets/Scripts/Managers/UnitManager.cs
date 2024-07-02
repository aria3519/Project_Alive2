using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerKind
{
    CharOriginal,
    CharUnit,
    CharTurret
}

// Enemy �Ŵ����� EnemySpawner���� �����ϰ����� UnitManager�� �÷��̾� ����
// ���ֵ鸸 �����ϴ� �̱���Ŵ�����
// �Ŵ��� ģ������ ������� �κ��� ������� �ʾƾ��Ѵ�. �÷��̾��� ��ȯ�̶�� ��ɿ� �����͸� �����ְ�
// ��ȯ�� ��� �����͵��� ������ Ǯ������ �����ϴ� ��ɸ� �������־���Ѵ�.

// Ǯ�� ������ -> GetComponent �� ������ ���������� ���� ��빮�� �߻�
// ���� enemy ���� ���� ��ü���� ť �ڷᱸ���� ����� �����ϰ� �÷��̾�, npc, ��ų ������Ʈ, ����Ʈ, ������, ������ 
// ���̺귯��ȭ �� ������Ʈ Ǯ���� ����ϵ��� ��ȹ��
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
        // �÷��̾� �������� ĳ���� ����
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

        // �÷��̾� ���� ���� ���� 
        var playerUnit = new List<Character>();
        for (int i = 0; i < charData.MaxPlayerUnit; i++)
        {
            if (charData.MaxPlayerUnit == 0)
                return;
            Character charUnit = Instantiate(charData.UnitPrefabs[(int)PlayerKind.CharUnit], Vector3.zero, Quaternion.identity);
            playerUnit.Add(charUnit);
            charUnit.gameObject.SetActive(false);
        }

        // �÷��̾� �ͷ� ����
        var playerTurret = new List<Character>();
        for (int i = 0; i < charData.MaxPlayerTurret; i++)
        {
            if (charData.MaxPlayerTurret == 0)
                return; 
            Character charTurret = Instantiate(charData.TurretPrefabs[(int)PlayerKind.CharTurret], Vector3.zero, Quaternion.identity);
            playerTurret.Add(charTurret);
            charTurret.gameObject.SetActive(false);
        }

        //��ųʸ��� ������ ť �߰�
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

    // Ǯ�� ��������
    // ������ ť�� �־��ֱ�
    public void AddList(Character player, PlayerKind index)
    {
        // Ű���� �ִ���? ť�� �ִ���?
        if(charPool.ContainsKey(index) && !charPool[index].Contains(player))
        {
            charPool[index].Add(player);
        }
        player.gameObject.SetActive(false);
    }
    
    // Ǯ�� ��������
    // ť���� �����ͼ� ��ȯ�ϱ�
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
