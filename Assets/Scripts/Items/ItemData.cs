using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/ItemData", order = int.MaxValue)]
public class ItemData : ScriptableObject
{
    // 원거리 무기
    [SerializeField] GunController[] allGuns;
    public GunController[] AllGuns { get { return allGuns; } }

    // 근접 무기
    [SerializeField] MeleeController[] allMelee;
    public MeleeController[] AllMelee { get { return allMelee; } }

    // 투척 무기
    [SerializeField] public int maxGrenade = 10;
    [SerializeField] public int maxFlare = 10;
    [SerializeField] public int maxJetBomber = 2;
    [SerializeField] public int maxSkillGrenade = 10;
    [SerializeField] public int maxBullet = 200;
    [SerializeField] public int maxBossGrenade = 3;
    [SerializeField] public int maxFlameBullet = 20;
    [SerializeField] public int maxItemSupply = 3;


    [SerializeField] ItemGrenade grenade;
    public ItemGrenade Grenade { get { return grenade; } }

    [SerializeField] ItemFlareGun flare;
    public ItemFlareGun Flare { get { return flare; } }

    [SerializeField] JetBomber jetBomber;
    public JetBomber JetBomber { get { return jetBomber; } }

    [SerializeField] ItemBulletGrenade bulletGrenade;

    public ItemBulletGrenade BulletGrenade { get { return bulletGrenade; } }

    [SerializeField] ItemBullet[] bullet;
    public ItemBullet[] Bullet { get { return bullet; } }

    [SerializeField] BossGrenade bossGrenade;
    public BossGrenade BossGrenade { get { return bossGrenade; } }

    [SerializeField] FlameBullet flameBullet;
    public FlameBullet FlameBullet { get { return flameBullet; } }

    [SerializeField] ItemSupply itemSupply;
    public ItemSupply ItemSupply { get { return itemSupply; } }
    // 잡템

}
