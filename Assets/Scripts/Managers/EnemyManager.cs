using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyKind
{
	NormalZombie,
	FastZombie,
	SlowZombie,
	End
}

[System.Serializable]
public class EnemyManager : MonoBehaviour
{
	private static EnemyManager m_instance;
	public static EnemyManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = FindObjectOfType<EnemyManager>();
			}

			return m_instance;
		}
	}

	public GridController gridController;
	//public GameObject unitPrefab;
	//public EnemyFlow[] unitsInGame;
	public int numUnitsPerSpawn;
	public float moveSpeed;
	private int enemyNum;

	private Character player;
	[SerializeField] private EnemyData enemyData;

	private List<EnemyFlow> unitsInGame;
	private Queue<EnemyFlow> enemyPool = new Queue<EnemyFlow>();
	//private Dictionary<EnemyKind, Queue<EnemyFlow>> enemyPool;

	// ������
	[SerializeField] int enemyPoolCount;
	[SerializeField] int unitsInGameCount;

	private void OnEnable()
    {
		player = GameManager.instance.GetPlayer();
	}

    private void Start()
    {
		//      enemyPool = new Dictionary<EnemyKind, Queue<EnemyFlow>>();

		//var normal = new Queue<EnemyFlow>();
		//for (int i = 0; i < enemyData.NormalZombie; i++)
		//{
		//	EnemyFlow enemy = Instantiate(enemyData.EnemyPrefabs[(int)EnemyKind.NormalZombie], Vector3.zero, Quaternion.identity);
		//	normal.Enqueue(enemy);
		//	enemy.gameObject.SetActive(false);
		//}

		//var fast = new Queue<EnemyFlow>();
		//for (int i = 0; i < enemyData.FastZombie; i++)
		//      {
		//          EnemyFlow enemy = Instantiate(enemyData.EnemyPrefabs[(int)EnemyKind.FastZombie], Vector3.zero, Quaternion.identity);
		//          fast.Enqueue(enemy);
		//          enemy.gameObject.SetActive(false);
		//      }

		//var slow = new Queue<EnemyFlow>();
		//for (int i = 0; i < enemyData.SlowZombie; i++)
		//{
		//	EnemyFlow enemy = Instantiate(enemyData.EnemyPrefabs[(int)EnemyKind.SlowZombie], Vector3.zero, Quaternion.identity);
		//	slow.Enqueue(enemy);
		//	enemy.gameObject.SetActive(false);
		//}

		//enemyPool.Add(EnemyKind.NormalZombie, normal);
		//enemyPool.Add(EnemyKind.FastZombie, fast);
		//enemyPool.Add(EnemyKind.SlowZombie, slow);

		CreateUnits(enemyData.MaxEnemy);
		unitsInGame = new List<EnemyFlow>(); // Update�� ���� ����Ʈ
		SpawnUnits(); // enemyPool ���� ������ unitsInGame �� �־������
    }

	void Update()
	{
		enemyPoolCount = enemyPool.Count;
		unitsInGameCount = unitsInGame.Count;
        

		/* player = UnitManager.instance.GetPlayer();*/
		//Vector3 pos = player.transform.position;

		if (gridController.curFlowField == null) { return; }

		if (enemyPool == null || unitsInGame == null)
			return;

		foreach (EnemyFlow unit in unitsInGame)
		{
			Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(unit.transform.position);
			Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
			unit.Updates(moveDirection, player.transform);	

			//Rigidbody unitRB = unit.GetComponent<Rigidbody>();
			//unitRB.velocity = moveDirection * moveSpeed;
		}

		//UpdateUI();
	}

	//private void FixedUpdate()
	//{
	//	/* player = UnitManager.instance.GetPlayer();*/
	//	Vector3 pos = player.transform.position;

	//	if (gridController.curFlowField == null) { return; }
		
	//	if (enemyPool == null || unitsInGame == null)
	//		return;

	//	foreach (EnemyFlow unit in unitsInGame)
	//	{
	//		//Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(unit.transform.position);
	//		//Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
	//		unit.Updates(pos);

	//		//Rigidbody unitRB = unit.GetComponent<Rigidbody>();
	//		//unitRB.velocity = moveDirection * moveSpeed;
	//	}
	//}

	private void CreateUnits(int maxValue)
    {
		for (int i = 0; i < maxValue; i++)
		{
			int rand = Random.Range(0, 3);
			EnemyFlow enemy = Instantiate(enemyData.EnemyPrefab[rand], Vector3.zero, Quaternion.identity);

			// Data Setting
			var index = (EnemyKind)Random.Range((int)EnemyKind.NormalZombie, (int)EnemyKind.End);
			switch (index)
			{
				case EnemyKind.SlowZombie:
					enemy.SetData(10, 1.5f, enemyData.AnimatorControllers[0]);
					break;
				case EnemyKind.NormalZombie:
					enemy.SetData(15, 2.5f, enemyData.AnimatorControllers[1]);
					break;
				case EnemyKind.FastZombie:
					enemy.SetData(20, 3.5f, enemyData.AnimatorControllers[2]);
					break;
			}

			enemy.gameObject.SetActive(false);
			enemyPool.Enqueue(enemy);
		}
	}

	private void SpawnUnits()
	{
		Vector2Int gridSize = gridController.gridSize;
		float nodeRadius = gridController.cellRadius;
		Vector2 maxSpawnPos = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius, gridSize.y * nodeRadius * 2 + nodeRadius);
		int colMask = LayerMask.GetMask("Impassible", "Enemy"); // ��Ʈ�������� �迭�ι޾ƿͼ� ���尡��
		// ���⼭ �ǹ� Layer�� Impassible�� �������ָ鼭 ���� BoxCollider�� �������� �����۵���.
		Vector3 newPos;
		//enemyNum = enemyPool[EnemyKind.NormalZombie].Count + enemyPool[EnemyKind.FastZombie].Count + enemyPool[EnemyKind.SlowZombie].Count;

		for (int i = 0; i < enemyData.MaxEnemy; i++)
		{
			if (enemyPool.Count <= 0)
				break;

			EnemyFlow enemy = GetQueue();
			enemy.transform.parent = transform;

			// ��ȯ�Ǵ� ��ǥ�� �Ȱ�ġ�� ���ؼ� �۵�
			do
			{
				newPos = new Vector3(Random.Range(0, maxSpawnPos.x), 0, Random.Range(0, maxSpawnPos.y));
				enemy.transform.position = newPos;
			}
			while (Physics.OverlapSphere(newPos, 1f, colMask).Length > 0);
			
			unitsInGame.Add(enemy); // Pooling�� ������Ʈ �־��� EnemyFlow���� ������ �ٽ� ��������
		}
	}

	// BOSS3 �� ���� ��ų
	public void SpawnUnitsSkill(Transform[] spawnPos, int maxSpawnCount)
    {
		if (enemyPool.Count < maxSpawnCount)
		{
			maxSpawnCount = enemyPool.Count <= 0 ? maxSpawnCount : maxSpawnCount - enemyPool.Count;
		}

		CreateUnits(maxSpawnCount);
		StartCoroutine(SpawnTime(spawnPos, maxSpawnCount));
	}

	private IEnumerator SpawnTime(Transform[] spawnPos, int maxSpawnCount)
    {
		for (int i = 0; i < maxSpawnCount; i++)
		{
			int rand = Random.Range(0, 2);
			EnemyFlow enemy = GetQueue();
			enemy.transform.position = spawnPos[rand].position;
			unitsInGame.Add(enemy);
			yield return new WaitForSeconds(1f);
		}
	}

	public void InsertQueue(EnemyFlow enemy)
	{
		//if (enemyPool.ContainsKey(index) && !enemyPool[index].Contains(enemy)) // �ߺ� üũ , ��� �����̳ʰ� �ٵ������
		if (!enemyPool.Contains(enemy)) // �ߺ� üũ , ��� �����̳ʰ� �ٵ������
		{
			enemyPool.Enqueue(enemy);
		}

		enemy.InitInfo();
		enemy.gameObject.SetActive(false);
	}

	public EnemyFlow GetQueue()
	{
		EnemyFlow enemy = enemyPool.Dequeue();
		enemy.gameObject.SetActive(true);
		return enemy;
	}

	public void RemoveList(EnemyFlow enemy)
    {
		if (!unitsInGame.Contains(enemy))
			return;
		unitsInGame.Remove(enemy);
    }

	public Character GetPlayer()
    {
		return player;
    }

	// UnityEvent �� ���� Player�� ���� �޾ƿ�
	// UnitManager CharacterEvent ����
	public void SetPlayer(Character getPlayer)
	{
		player = getPlayer;
	}

	private void UpdateUI()
	{
		//enemyNum = enemyPool[EnemyKind.NormalZombie].Count + enemyPool[EnemyKind.FastZombie].Count + enemyPool[EnemyKind.SlowZombie].Count;
		//UIManager.instance.UpdateWaveText(enemyData.MaxEnemy - enemyNum);
	}
}