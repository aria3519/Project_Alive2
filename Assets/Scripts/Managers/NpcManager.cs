using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NpcKind
{
    NpcRescue,
    NpcQuest,
    NpcUnit
}

public enum Formation
{
    Circle,
    Square,
    Triangle
}

public class NpcManager : MonoBehaviour
{
    private static NpcManager m_instance;
    public static NpcManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<NpcManager>();
            }

            return m_instance;
        }
    }

    // 필수 npcUnit
    //[SerializeField] private 

    // 랜덤 npcUnit
    [SerializeField] private Transform[] randomSpawnPoints;
    [SerializeField] private NpcUnit[] randomNpcUnitPrefabs;
    [SerializeField] private NpcUnit[] mainNpcUnitPrefabs;
    [SerializeField] private int maxRandomNpcUnit;

    public Formation formation { get; set; }

    private List<NpcUnit> npcUnitPool = new List<NpcUnit>();
    private List<NpcUnit> aliveUnits = new List<NpcUnit>();
    private List<Vector3> formationVertices = new List<Vector3>();
    private float defaultSpace = 2f;
    private void Start()
    {
        //formation = Formation.Circle;
        formation = Formation.Circle;


        //for(int i = 0; i < mainNpcUnitPrefabs.Length; i++)
        //{
        //    npcUnitPool.Add(mainNpcUnitPrefabs[i]);
        //}
        //for(int i = 0; i < maxRandomNpcUnit; i++)
        //{
        //    var unit = Instantiate(randomNpcUnitPrefabs[Random.Range(0, randomNpcUnitPrefabs.Length)], transform);
        //    unit.transform.position = randomSpawnPoints[Random.Range(0, randomSpawnPoints.Length)].position;
        //    npcUnitPool.Add(unit);
        //}

    }

    public void UpdateFormation(Transform _playerPos)
    {
        for (int i = 0; i < aliveUnits.Count; i++)
        {
            aliveUnits[i].Updates(_playerPos.position + formationVertices[i]);
        }
    }

    public void MakeFormation(float value)
    {
        switch (formation)
        {
            case Formation.Circle:
                CircleFormation(value);
                UIManager.instance.DisplayFormationIcon(Formation.Circle, "Circle Formation", value);
                break;
            case Formation.Square:
                SquareFormation(value);
                UIManager.instance.DisplayFormationIcon(Formation.Square, "Square Formation", value);
                break;
            case Formation.Triangle:
                break;
        }
    }

    public void ChangeFormation()
    {
        formation = (formation == Formation.Square) ? Formation.Circle : Formation.Square;
        MakeFormation(defaultSpace);
    }

    private void CircleFormation(float spaceValue)
    {
        int segment = aliveUnits.Count;
        if (0 >= segment) return;

        float x, y = 0f, z;
        float angleDiv = 360f / (float)segment;
        float angle = angleDiv;
        formationVertices.Clear();
        for (int i = 0; i < segment; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle);
            z = Mathf.Sin(Mathf.Deg2Rad * angle);

            formationVertices.Add(new Vector3(x, y, z) * spaceValue);
            angle += angleDiv;
        }
    }

    private void SquareFormation(float spaceValue)
    {
        int columns = 4;
        formationVertices.Clear();

        for (int i = 0; i < aliveUnits.Count; i++)
        {
            float posX = (i % columns) * spaceValue - 2;
            float posZ = (i / columns) * spaceValue - 2;
            formationVertices.Add(new Vector3(posX, 0f, posZ));
        }
    }

    public void ChangeFormationSpace(float _value)
    {
        defaultSpace += _value;
        if (defaultSpace > 4) defaultSpace = 4;
        else if (defaultSpace < 1) defaultSpace = 1;

        MakeFormation(defaultSpace);
    }

    public void AddRescuedUnit(NpcUnit unit)
    {
        if (!aliveUnits.Contains(unit))
        {
            aliveUnits.Add(unit);

            MakeFormation(defaultSpace);
        }
    }

    public void RemoveRescuedUnit(NpcUnit unit)
    {
        aliveUnits.Remove(unit);
    }

    // 구출한 Npc 수 return
    public int GetRescuedNpcCount()
    {
        return aliveUnits.Count;
    }

    public void InitRescuedList()
    {
        aliveUnits.Clear();
    }
    
}
