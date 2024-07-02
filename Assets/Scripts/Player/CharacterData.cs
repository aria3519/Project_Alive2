using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharData", menuName = "Scriptable Object/CharData", order = int.MaxValue)]
public class CharacterData : ScriptableObject
{
    // Character 관련 모든 유닛들 풀링 데이터
    [SerializeField] int maxPlayerCharacter;
    public int MaxPlayerCharacter { get { return maxPlayerCharacter; } }

    [SerializeField] int maxPlayerUnit;
    public int MaxPlayerUnit { get { return maxPlayerUnit; } }

    [SerializeField] int maxPlayerTurret;
    public int MaxPlayerTurret { get { return maxPlayerTurret; } }

    [SerializeField] Character[] charPrefabs;
    public Character[] CharPrefabs {  get { return charPrefabs; } }

    [SerializeField] Character[] unitPrefabs;
    public Character[] UnitPrefabs { get { return unitPrefabs; } }

    [SerializeField] Character[] turretPrefabs;
    public Character[] TurretPrefabs { get { return turretPrefabs; } }

    // 캐릭터 선택 데이터 추가
    [SerializeField] Character[] charPool;
    public Character[] CharPool { get { return charPool; } }
    // 소환 유닛들의 체력,공격력,공격속도
    // 터렛도 마찬가지

    //[SerializeField] AudioClip getDamaged;
}
