using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy[] enemyPrefabs;
    public Transform[] spawnPoints;

    public float damageMax = 40f;
    public float damageMin = 20f;

    public float healthMax = 200f;
    public float healthMin = 100f;

    public float speedMax = 3f;
    public float speedMin = 2f;

    public Color strongEnemyColor = Color.red;

    Dictionary<EnemyKind, Queue<Enemy>> enemyPool;
    private int wave;
    private int enemyNum;

    private float timeBetSpawn = 1f;
    private float lastSpawnTime = 0f;

    private float timeBetWave = 20f;
    private float lastWaveTime = 0f;

    [SerializeField] private EnemyData enemyData;

    private bool isUsed = true;

    public void Stop()
    {
        isUsed = false;
    }
    
    private void Start()
    {
        enemyPool = new Dictionary<EnemyKind, Queue<Enemy>>();
        var normal = new Queue<Enemy>();
        var fast = new Queue<Enemy>();
        for(int i = 0; i < enemyData.NormalZombie; i++)
        {
            Enemy enemy = Instantiate(enemyPrefabs[(int)EnemyKind.NormalZombie], Vector3.zero, Quaternion.identity);
            normal.Enqueue(enemy);
            enemy.gameObject.SetActive(false);
        }
        for (int i = 0; i < enemyData.FastZombie; i++)
        {
            Enemy enemy = Instantiate(enemyPrefabs[(int)EnemyKind.FastZombie], Vector3.zero, Quaternion.identity);
            fast.Enqueue(enemy);
            enemy.gameObject.SetActive(false);
        }
        enemyPool.Add(EnemyKind.NormalZombie, normal);
        enemyPool.Add(EnemyKind.FastZombie, fast);
    }

    public void InsertQueue(Enemy enemy, EnemyKind index)
    {
        if (enemyPool.ContainsKey(index) && !enemyPool[index].Contains(enemy)) // 중복 체크 , 모든 컨테이너가 다들고있음
        {
            enemyPool[index].Enqueue(enemy);
        }
        enemy.gameObject.SetActive(false);
    }

    public Enemy GetQueue(EnemyKind index)
    {
        Enemy enemy = enemyPool[index].Dequeue();
        enemy.gameObject.SetActive(true);
        return enemy;
    }

    private void Update()
    {
       
        if (!isUsed)
            return;

        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        if (enemyPool.Count <= 0)
        {
            return;
        }

        if (Time.time >= lastWaveTime + timeBetWave && isUsed == true)
        {
            lastWaveTime = Time.time;
            SpawnWave();
        }
        if (Time.time >= lastSpawnTime + timeBetSpawn && isUsed == true)
        {
            lastSpawnTime = Time.time;
            Spawn();
        }
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        enemyNum = enemyPool[EnemyKind.NormalZombie].Count + enemyPool[EnemyKind.FastZombie].Count;
        //UIManager.instance.UpdateWaveText(enemyData.MaxEnemy - enemyNum);
    }

    private void Spawn()
    {
        float enemyIntensity = Random.Range(0f, 1f);

        CreateEnemy(enemyIntensity);
    }

    private void SpawnWave()
    {
        enemyNum = enemyPool[EnemyKind.NormalZombie].Count + enemyPool[EnemyKind.FastZombie].Count;
        wave++;

        int spawnCount = Mathf.RoundToInt(wave * 100f);
        spawnCount = Mathf.Min(enemyNum, spawnCount);

        for (int i = 0; i < spawnCount; i++)
        {
            float enemyIntensity = Random.Range(0f, 1f);

            CreateEnemy(enemyIntensity);
            StartCoroutine(timeWaveNotice());
        }
    }

    private IEnumerator timeWaveNotice()
    {
        UIManager.instance.activateWaveNotice();
        yield return new WaitForSeconds(4f);
        UIManager.instance.deactivateWaveNotice();
    }

    private void CreateEnemy(float intensity)
    {
        var index = (EnemyKind)Random.Range((int)EnemyKind.NormalZombie, (int)EnemyKind.End);
        if (enemyPool[index].Count <= 0)
        {
            return;
        }

        float health = Mathf.Lerp(healthMin, healthMax, intensity);
        float damage = Mathf.Lerp(damageMin, damageMax, intensity);
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Enemy enemy = GetQueue(index);
        enemy.Setup(health, damage, speed, skinColor, enemy.enemyKind);
        enemy.transform.position = spawnPoint.position;
        enemy.transform.rotation = spawnPoint.rotation;
        enemy.Restart();

        //Enemy enemy;

        //if (listEnemy.Count <= 20)
        //{
        //    enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        //    enemy.Setup(health, damage, speed, skinColor);
        //    enemy.gameObject.SetActive(true);
        //    listEnemy.Add(enemy);
        //    count++;
        //    //enemy.onDeath += () => listEnemy.Remove(enemy);
        //    //enemy.onDeath += () => Destroy(enemy.gameObject, 10f);
        //}
        //else
        //{
        //    if (listEnemy.Find(b => !b.gameObject.activeSelf))
        //    {
        //        enemy = listEnemy.Find(b => !b.gameObject.activeSelf);
        //        enemy.Setup(health, damage, speed, skinColor);

        //        enemy.gameObject.SetActive(true);
        //        count++;
        //    }
        //    else
        //        return;
        //}


        // event는 꼭 할당 해제 해주자.. onDeath에 null로 해줬음
        /* enemy.onDeath += () => GameManager.instance.AddScore(100);*/
        enemy.onDeath += () => StartCoroutine(activeTime(enemy, index));
    }

    private IEnumerator activeTime(Enemy enemy, EnemyKind index)
    {
        yield return new WaitForSeconds(3f);
        InsertQueue(enemy, index);
    }
}
