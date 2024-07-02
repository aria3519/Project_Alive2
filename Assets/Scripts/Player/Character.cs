using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : LivingEntity
{
    //[SerializeField] protected string ID;
    [SerializeField] Slider healthSlider;

    [SerializeField] AudioClip deathClip;
    [SerializeField] AudioClip hitClip;
    //public AudioClip itemPickupClip;

    // �ڽĵ��� ���� ���� ������
    [SerializeField] protected Sprite playerIcon;
    [SerializeField] protected Transform weaponPivot;
    [SerializeField] protected Transform weaponHold;
    [SerializeField] protected ItemData itemData;
    [SerializeField] protected ParticleSystem flameEffect;
    [SerializeField] protected ParticleSystem healEffect;
    [SerializeField] protected ParticleSystem speedEffect;
    protected bool isSkillUse = false;
    [SerializeField] protected string ID;
    private int addHealth;
    private int addSpPoint;
    private bool startStge;


    public float WeaponHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }

    //���� ������Ʈ���� �ڽĵ��� ������Ʈ���� ������ ������Ʈ���� ��ũ�Ǳ⶧���� �ڽĿ� �ȳ־��
    protected AudioSource playerAudio;
    protected Animator playerAnimator;
    protected PlayerMovement playerMovement;
    protected PlayerWeapon playerWeapon;
    protected PlayerInput playerInput;
    protected CapsuleCollider playerCollider;

    //��ô���� ������
    protected int hasGrenades = 10;
    private int hasFlares = 10;

    // dodge
    public float dashSpeed;
    public float dashTime;

    // ȭ�� ����������
    private bool isTicked = false;

    // �̼ӽ�ų ����
    protected bool isFastSkill = false;

    // ������
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float sprintSpeed = 10f;

    [SerializeField] protected float spPoint;
    [SerializeField] protected float startingSpPoint = 100f;
    [SerializeField] protected float spRecoverRate = 0.1f;
    [SerializeField] protected float dodgeCost = 10f;
    [SerializeField] protected float runCost = 0.5f;

    // �ð� ����
    protected float spLastRecoverTime;
    protected float spRecoverTime = 0.5f;

    protected float runLastReduceTime;
    protected float runReduceTime = 0.05f;

    protected float bombLastSkillTime;
    protected float bombSkillTime = 10f;

    protected float healLastSkillTime;
    protected float healSkillTime = 5f;

    protected float speedLastSkillTime;
    protected float speedSkillTime = 5f;
    protected float speedUseTime = 30f;
    //---------

    // ĳ���� ���� ����
    protected int lv = 1;
    protected float exp = 0f;
    protected float maxExp = 100f;
    protected int uniqueSkillKind; // ĳ���Ͱ� ������ ���� ������ų ���� 

    

    public virtual void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

        playerMovement = GetComponent<PlayerMovement>();
        playerWeapon = GetComponent<PlayerWeapon>();
        playerInput = GetComponent<PlayerInput>();
        playerCollider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();

        health = startingHealth;
        spPoint = startingSpPoint;

        SkillManager.instance.SetSkillTime(SkillState.HEAL, healSkillTime);
        SkillManager.instance.SetSkillTime(SkillState.FAST, speedSkillTime);
        SkillManager.instance.SetSkillTime(SkillState.BOMB, bombSkillTime);
    }

    public string GetID()
    {
        return ID;
    }
    public void SetStart()
    {
        startStge = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        playerInput.enabled = true;
        playerMovement.enabled = true;

        if (startStge)
        {
            health = startingHealth;
            spPoint = startingSpPoint;
            startStge = false;
        }
                

        //playerWeapon.enabled = true;
     }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(!dead)
        {
            playerAudio.PlayOneShot(hitClip);
            base.OnDamage(damage, hitPoint, hitNormal);
        }
    }

    public override void OnDamageByFlame(float firstDamage, float tickDamage, int tickTime, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(!dead)
        {
            playerAudio.PlayOneShot(hitClip);
            base.OnDamage(firstDamage, hitPoint, hitNormal);
            if (!isTicked)
            {
                isTicked = true;
                StartCoroutine(flameDamageTime(tickTime, tickDamage));
            }
        }
    }

    private IEnumerator flameDamageTime(int tickTime, float tickDamage)
    {
        for(int i = 0; i < tickTime; i++)
        {
            flameEffect.Play();
            playerAudio.PlayOneShot(hitClip);
            base.OnDamage(tickDamage, Vector3.zero, Vector3.zero);
            yield return new WaitForSeconds(1f);
        }
        isTicked = false;
    }

    public override void Die()
    {
        base.Die();

        healthSlider.gameObject.SetActive(false);
        playerAnimator.SetTrigger("Die");

        playerInput.enabled = false;
        playerMovement.enabled = false;
        //playerWeapon.enabled = false;
        Cursor.visible = true;
    }

    protected void ThrowGrenade()
    {
        if (hasGrenades == 0)
            return;

        if(playerInput.throwGrenade)
        {
            playerAnimator.SetTrigger("Grenade");
            Ray ray = playerInput.viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            // clipping plane near, far �� ��ŭ raycast���� �Ÿ��� üũ�������
            // clipping plane near ��ŭ �ȵǸ� �Ÿ��� 50�̿������ ���� -100 �̱⶧����racast�� false�� ��
            // ���� �̰ɷ� �����Ÿ��� ������ �����غ���

            if (Physics.Raycast(ray, out rayHit, Mathf.Abs(playerInput.viewCamera.nearClipPlane * 2)))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 5;

                Item grenade = ItemManager.instance.GetQueue(ItemKind.ItemGrenade, transform);
                Rigidbody rigidGrenade = grenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
            }
        }
    }
    
    protected void ThrowFlare()
    {
        if (hasFlares == 0)
            return;

        if(playerInput.flare)
        {
            Ray ray = playerInput.viewCamera.ScreenPointToRay(playerInput.mousePoint);
            RaycastHit rayHit;

            if(Physics.Raycast(ray, out rayHit, Mathf.Abs(playerInput.viewCamera.nearClipPlane * 2)))
            {
                //Vector3 nextVec = rayHit.point - transform.position;
                Vector3 nextVec = transform.position;
                nextVec.y = 25;

                Item flare = ItemManager.instance.GetQueue(ItemKind.ItemFlare, transform);
                Rigidbody rigidFlare = flare.GetComponent<Rigidbody>();
                rigidFlare.AddForce(nextVec, ForceMode.Impulse);
                //rigidFlare.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasFlares--;
            }
        }
    }

    protected virtual void CharacterSwap()
    {
        if (dead || true == isSkillUse)
            return;

        if(playerInput.swap)
        {
            playerInput.SetFireFalse();
            //UnitManager.instance.SwapCharacter();
            TestUnitManager.Instance.SwapCharacter();
        }
    }

    protected virtual void UseSkill()
    {
        // ������ �ڿ�ȸ��

        // �޸��� �� ������ ����
        if (playerInput.sprint && spPoint > 0 && Time.time >= runLastReduceTime + runReduceTime)
        {
            runLastReduceTime = Time.time;
            spPoint -= runCost;
        }
        else
            RecoverSpPoint();

        if (playerInput.skillSet2)
        {
            if (!ItemManager.instance.IsJetBomb())
                return;

            //UIManager.instance.activateBombReady();
            SkillManager.instance.skillState = SkillState.BOMB;
            if (Time.time >= bombLastSkillTime + bombSkillTime &&
                (SkillManager.instance.skillState == SkillState.BOMB || SkillManager.instance.skillState == SkillState.BOMB_MULTI))
            {
                Debug.Log("Bomb Started");
                bombLastSkillTime = Time.time;
                //bomb skill
                Vector3 targetPoint = playerInput.mousePoint;
                //targetPoint.y = transform.position.y;

                Vector3 aidPoint = playerInput.mousePoint + -transform.forward * 100;
                aidPoint.y += 12;
                //aidPoint.x = transform.position.x - 100;
                //jetBomber.transform.position = new Vector3(0, transform.position.y + 12, 0);

                SkillManager.instance.SetBombPos(targetPoint);

                Item jet = ItemManager.instance.GetQueue(ItemKind.ItemJetBomber, aidPoint, Quaternion.LookRotation(transform.forward));

                //GameObject JetBomberToAid = SkillManager.instance.GetJetBomberToAid();
                //jetBomber = (GameObject)Instantiate(JetBomberToAid, aidPoint, Quaternion.LookRotation(transform.forward)); 
                // Quaternion.Euler(0, 90, 0)
                //jetBomber.transform.position += Vector3.forward * 50.0f;
                //UIManager.instance.deactivateBombReady();
                SkillManager.instance.isBomb = true;
                StartCoroutine(CoolBombSkill());
            }
        }
        else if (playerInput.skillSet3)
        {
            Debug.Log("Turret setted");
            SkillManager.instance.skillState = SkillState.TURRET;
            if (SkillManager.instance.skillState == SkillState.TURRET)
            {
                Vector3 targetPoint = playerInput.mousePoint;
                targetPoint.y = 0;

                SkillManager.instance.SetTurretPos(targetPoint);

                TestUnitManager.Instance.SpawnTurret(targetPoint);
            }
        }
        else if (playerInput.dodge && CheckDodgeSpPoint())
        {
            Debug.Log("Dodge Skill");

            playerAnimator.SetTrigger("Dodge");
            playerCollider.enabled = false;
            rigidbody.useGravity = false;
            StartCoroutine(Dodge(playerMovement.direction));
            spPoint -= dodgeCost;
        }

        //if (true == SkillManager.instance.clickCheck())
        //{
        //    if (skillManager.skillState == SkillState.TURRET)
        //    {
        //        // turret skill
        //        Vector3 buildPoint = mousePoint;
        //        buildPoint.y = transform.position.y;
        //        GameObject turretToBuild = skillManager.GetTurretToBuild();
        //        turret = (GameObject)Instantiate(turretToBuild, buildPoint, transform.rotation);
        //        skillManager.setClickCheck(false);
        //        UIManager.instance.deactivateTurretReady();
        //    }
        //    else if (skillManager.skillState == SkillState.BOMB || skillManager.skillState == SkillState.BOMB_MULTI)
        //    {
        //        // bomb skill
        //        Vector3 targetPoint = mousePoint;
        //        //targetPoint.y = transform.position.y;

        //        Vector3 aidPoint = mousePoint + -transform.forward * 100;
        //        aidPoint.y += 12;
        //        //aidPoint.y += 12;
        //        //aidPoint.x = transform.position.x - 100;
        //        //jetBomber.transform.position = new Vector3(0, transform.position.y + 12, 0);

        //        skillManager.SetBombPos(targetPoint);
        //        GameObject JetBomberToAid = skillManager.GetJetBomberToAid();
        //        jetBomber = (GameObject)Instantiate(JetBomberToAid, aidPoint, Quaternion.LookRotation(transform.forward)); // Quaternion.Euler(0, 90, 0)
        //                                                                                                                   //jetBomber.transform.position += Vector3.forward * 50.0f;
        //        skillManager.setClickCheck(false);
        //        UIManager.instance.deactivateBombReady();
        //    }
        //}
    }

    private IEnumerator CoolBombSkill()
    {
        yield return new WaitForSeconds(10f);
    }
    protected virtual IEnumerator Dodge(Vector3 moveinput)
    {
        float startTime = Time.time;


        while (Time.time < startTime + dashTime &&
            !Physics.Raycast(transform.position, moveinput, 5f, ~(1 << LayerMask.GetMask("Enemy"))))
        {
            rigidbody.MovePosition(rigidbody.position + moveinput * dashSpeed * Time.deltaTime);
            yield return null;
        }

        playerCollider.enabled = true;
        rigidbody.useGravity = true;
    }

    public virtual void GetAmmo(int ammo)
    {
        
    }


    public virtual void ResetCharacter()
    {
        health = startingHealth;
        spPoint = startingSpPoint;
        playerInput.enabled = true;
        playerMovement.enabled = true;
        dead = false;
        hasGrenades = 10;

        TestUnitManager.Instance.ResetTurret();
    }

    public IEnumerator InvincibleTime()
    {
        rigidbody.useGravity = false;
        playerCollider.enabled = false;
        yield return new WaitForSeconds(3f);
        rigidbody.useGravity = true;
        playerCollider.enabled = true;
    }

    public override void BindAttacked()
    {
        playerMovement.enabled = false;
        StartCoroutine(BindTime());
    }

    private IEnumerator BindTime()
    {
        yield return new WaitForSeconds(2.5f);
        playerMovement.enabled = true;
    }

    public float GetVelocity()
    {
        // ���� ���׿����� sprint Ű�Է� ��/���� �Ǻ� �� spPoint üũ
        return CheckSpPoint() ? (playerInput.sprint ? sprintSpeed : moveSpeed) : moveSpeed;
    }
    public void ResetSP()
    {
        spPoint = startingSpPoint;
    }

    private bool CheckDodgeSpPoint()
    {
        if (spPoint < dodgeCost)
            return false;
        else
            return true;
    }
    private bool CheckSpPoint()
    {
        if (spPoint <= 0)
            return false;
        else
            return true;
    }
    private void RecoverSpPoint()
    {
        if(Time.time >= spLastRecoverTime + spRecoverTime && spPoint <= startingSpPoint)
        {
            spLastRecoverTime = Time.time;
            spPoint += spRecoverRate;
        }
    }
    // XML ������ �۽�
    public CharData SendCharData()
    {
        CharData charData = new CharData();
        charData.name = ID;
        charData.level = lv;
        charData.exp = exp;
        charData.health = startingHealth;
        return charData;
    }
    // ������ ���� 
    public void SetCharData(CharData charData)
    {
        lv = charData.level;
        exp = charData.exp;
        startingHealth = charData.health;
        
    }

    public bool CheckID(string name)
    {
        if (ID == name)
            return true;
        return false;
    }

    // ĳ���� Exp ���� �Լ� 
    // �ٸ��ʿ��� exp�� �־��ֱ� ���� �뵵 
    public void setClearData(int addExp)
    {
        exp += addExp;
        checkLevelup();
        health = startingHealth;
        // Ŭ������ ����� ������ ����ġ�� uimanager�� ���� 
        UIManager.instance.setClearData(lv, exp);
    }

    protected void checkLevelup()
    {
        // ������ �Ѱ�� Ŭ����� ����ġ�� �ְ� üũ 
        if(maxExp==exp)
        {
            lv += 1;
            exp = 0;
            maxExp =lv * 100;
            startingHealth += 100;
        }
        
    }


    public void CharBuff(Buff buff)
    {
        startingHealth += buff.healthAdd;
        startingSpPoint += buff.spAdd;
        health += buff.healthAdd;
        spPoint += buff.spAdd;
        addHealth = buff.healthAdd;
        addSpPoint = buff.spAdd;
    }

    public void BuffRemove()
    {
        startingHealth -= addHealth;
        startingSpPoint -= addSpPoint;
    }

    // ���� ��ų ��
    protected void UseHealSKill()
    {
        healEffect.Play();

        var damagedHealth = startingHealth - health;
        var recoverHealth = startingHealth * 0.2f;

        health += Mathf.Min(damagedHealth, recoverHealth);
    }
    // ���� ��ų �ӷ¾�
    public float UseFastSKill(Vector3 rotation)
    {
        return isFastSkill ? 3f : 0;
    }
    
    public void SetMySkill(int skill)
    {
        uniqueSkillKind = skill;
    }
    public int GetMySKill()
    {
        return uniqueSkillKind;
    }

    public Sprite GetPlayerIcon()
    {
        return playerIcon;
    }
    
    //private void OnTriggerEnter(Collider other) // ������ �Դ� �ڵ�
    //{
    //    if (!dead)
    //    {
    //        IItem item = other.GetComponent<IItem>();

    //        if (item != null)
    //        {
    //            item.Use(gameObject);

    //            playerAudioPlayer.PlayOneShot(itemPickupClip);
    //        }
    //    }
    //}
}