using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Object/EnemyData", order = int.MaxValue)]
public class EnemyData : ScriptableObject
{
    [SerializeField] int maxEnemy;
    public int MaxEnemy { get { return maxEnemy; } }

    [SerializeField] int fastZombie;
    public int FastZombie { get { return fastZombie; } }

    [SerializeField] int normalZombie;
    public int NormalZombie { get { return normalZombie; } }

    [SerializeField] int slowZombie;
    public int SlowZombie { get { return slowZombie; } }

    [SerializeField] EnemyFlow[] enemyPrefab;

    public EnemyFlow[] EnemyPrefab {  get { return enemyPrefab; } }

    [SerializeField] int damage;

    public float Damage { get { return damage; } }
    
    [SerializeField] int moveSpeed;

    public float MoveSpeed { get { return moveSpeed; } }

    [SerializeField] RuntimeAnimatorController[] animatorControllers;

    public RuntimeAnimatorController[] AnimatorControllers { get {  return animatorControllers;} }
}