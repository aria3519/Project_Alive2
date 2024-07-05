using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemKind
{
    ItemGrenade,
    ItemFlare,
    ItemJetBomber,
    ItemBulletGrenade
}

public class ItemManager : MonoBehaviour
{
    // 수류탄, 플레어, 지원박스등, 잡템, 무기 등
    private static ItemManager m_instance;
    public static ItemManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<ItemManager>();
            }

            return m_instance;
        }
    }

    // 아이템 데이터
    [SerializeField] private ItemData itemData;
    private Dictionary<ItemKind, Queue<Item>> itemPool;

    // 총알
    private Queue<ItemBullet> itemBullet = new Queue<ItemBullet>();
    private Queue<ItemBullet> itemBulletMiniGun = new Queue<ItemBullet>();
    private Queue<BossGrenade> bossGrenadePool = new Queue<BossGrenade>();
    private Queue<FlameBullet> flameBulletPool = new Queue<FlameBullet>();
    private Queue<ItemSupply> itemSupplyPool = new Queue<ItemSupply>();
    private Item item;
    public Hellicopter rescueHellicopter;

    private void Start()
    {
        itemPool = new Dictionary<ItemKind, Queue<Item>>();

        var grenadeQueue = new Queue<Item>();
        for (int i = 0; i < itemData.maxGrenade; i++)
        {
            ItemGrenade grenade = Instantiate(itemData.Grenade, transform);
            grenade.gameObject.SetActive(false);
            grenadeQueue.Enqueue(grenade);
        }

        var flareQueue = new Queue<Item>();
        for (int i = 0; i < itemData.maxFlare; i++)
        {
            ItemFlareGun flare = Instantiate(itemData.Flare, transform);
            flare.gameObject.SetActive(false);
            flareQueue.Enqueue(flare);
        }

        var jetBomberQueue = new Queue<Item>();
        for (int i = 0; i < itemData.maxJetBomber; i++)
        {
            JetBomber jet = Instantiate(itemData.JetBomber, transform);
            jet.gameObject.SetActive(false);
            jetBomberQueue.Enqueue(jet);
        }

        var bulletGrenadeQueue = new Queue<Item>();
        for (int i = 0; i < itemData.maxGrenade; i++)
        {
            ItemBulletGrenade skillGrenade = Instantiate(itemData.BulletGrenade, transform);
            skillGrenade.gameObject.SetActive(false);
            bulletGrenadeQueue.Enqueue(skillGrenade);
        }

        // Dictionary 사용 안하는 Queue 들
        for (int i = 0; i < itemData.maxBullet; i++)
        {
            var bullet = Instantiate(itemData.Bullet[0], transform);
            bullet.gameObject.SetActive(false);
            itemBullet.Enqueue(bullet);
        }
        for (int i = 0; i < itemData.maxBullet; i++)
        {
            var bullet = Instantiate(itemData.Bullet[1], transform);
            bullet.gameObject.SetActive(false);
            itemBulletMiniGun.Enqueue(bullet);
        }
        for (int i = 0; i < itemData.maxBossGrenade; i++)
        {
            var bossGrenade = Instantiate(itemData.BossGrenade, transform);
            bossGrenade.gameObject.SetActive(false);
            bossGrenadePool.Enqueue(bossGrenade);
        }
        for (int i = 0; i < itemData.maxItemSupply; i++)
        {
            var itemSupply = Instantiate(itemData.ItemSupply, transform);
            itemSupply.gameObject.SetActive(false);
            itemSupplyPool.Enqueue(itemSupply);
        }
        //for(int i = 0; i < itemData.maxFlameBullet; i++)
        //{
        //    var flameBullet = Instantiate(itemData.FlameBullet, transform);
        //    flameBullet.gameObject.SetActive(false);
        //    flameBulletPool.Enqueue(flameBullet);
        //}

        rescueHellicopter = Instantiate(rescueHellicopter, transform);
        rescueHellicopter.gameObject.SetActive(false);

        itemPool.Add(ItemKind.ItemGrenade, grenadeQueue);
        itemPool.Add(ItemKind.ItemFlare, flareQueue);
        itemPool.Add(ItemKind.ItemJetBomber, jetBomberQueue);
        itemPool.Add(ItemKind.ItemBulletGrenade, bulletGrenadeQueue);
    }

    // 풀링 가져오기
    public void InsertQueue(Item item, ItemKind index)
    {
        if(itemPool.ContainsKey(index) && !itemPool[index].Contains(item))
        {
            itemPool[index].Enqueue(item);
        }
        item.gameObject.SetActive(false);
    }

    // 풀링 내보내기
    public Item GetQueue(ItemKind index, Transform transform)
    {
        //Debug.LogError("GetQueue"+ index +"Transform " + transform.position);
        var item = itemPool[index].Dequeue();
        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;
        item.gameObject.SetActive(true);

        return item;
    }

    public Item GetQueue(ItemKind index, Vector3 point, Quaternion LookRotation)
    {
        var item = itemPool[index].Dequeue();
        item.transform.position = point;
        item.transform.rotation = LookRotation;
        item.gameObject.SetActive(true);

        return item;
    }

    public void InsertBullet(ItemBullet bullet, BulletType type)
    {
        bullet.gameObject.SetActive(false);
        switch(type)
        {
            case BulletType.Gun:
                itemBullet.Enqueue(bullet);
                break;
            case BulletType.MiniGun:
                itemBulletMiniGun.Enqueue(bullet);
                break;
        }
    }
    public ItemBullet GetBullet(BulletType type)
    {
        ItemBullet bullet;
        if (BulletType.MiniGun == type)
            bullet = itemBulletMiniGun.Dequeue();
        else
            bullet = itemBullet.Dequeue();

        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void InsertBossGrenade(BossGrenade grenade)
    {
        grenade.gameObject.SetActive(false);
        bossGrenadePool.Enqueue(grenade);
    }

    public BossGrenade GetBossGrenade(Vector3 firePos)
    {
        var grenade = bossGrenadePool.Dequeue();
        grenade.transform.position = firePos;
        grenade.gameObject.SetActive(true);

        return grenade;
    }

    public void InsertFlameBullet(FlameBullet flame)
    {
        flame.gameObject.SetActive(false);
        flameBulletPool.Enqueue(flame);
    }

    public FlameBullet GetFlameBullet()
    {
        var flame = flameBulletPool.Dequeue();
        return flame;
    }

    public void InsertItemSupply(ItemSupply supply)
    {
        supply.gameObject.SetActive(false);
        itemSupplyPool.Enqueue(supply);
    }

    public ItemSupply GetItemSupply()
    {
        var supply = itemSupplyPool.Dequeue();
        return supply;
    }

    public bool IsJetBomb()
    {
        if (itemPool[ItemKind.ItemJetBomber].Count == 0)
            return false;
        else
            return true;
    }

    public Hellicopter CreateRescueHelli()
    {
        rescueHellicopter.gameObject.SetActive(true);
        return rescueHellicopter;
    }

    public void DisableRescueHelli()
    {
        rescueHellicopter.gameObject.SetActive(false);
    }
}
