using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharData", menuName = "Scriptable Object/CharData", order = int.MaxValue)]
public class CharacterData : ScriptableObject
{
    // Character ���� ��� ���ֵ� Ǯ�� ������
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

    // ĳ���� ���� ������ �߰�
    [SerializeField] Character[] charPool;
    public Character[] CharPool { get { return charPool; } }
    // ��ȯ ���ֵ��� ü��,���ݷ�,���ݼӵ�
    // �ͷ��� ��������

    //[SerializeField] AudioClip getDamaged;
}
