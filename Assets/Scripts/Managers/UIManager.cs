using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance;

    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }
    [SerializeField] private GameObject status;
    [SerializeField] private GameObject status_Battle;
    [SerializeField] private GameObject GameObjectDetail;

    [SerializeField] private List<TextAsset> details;
    [SerializeField] private Text Windowdetail;

    public Image[] images;
    [SerializeField] private Sprite[] selectSprites;
   
    [SerializeField] private Sprite[] defaultSprites;
    [SerializeField] private Sprite[] FormationIcons;

    public Text ammoText;
    public Text scoreText; // kill로 변경 
    public Text waveText;
    public Text grenadeText;
    public Text flareText;
    public Text reloadText;
    public Text waveNoticeText;
    public Text bombReadyText;
    public Text turretReadyText;
    public Text healthText;
    public Text spText;
    public Text BossHealth;
    public Text rescuedNpcText;

    public Text formationText;
    public Text formationTextDistance;
    public Image formationDispaly;

    public Text interactKey;
    public Image interactImage;

    public Text gameClearText1;
    public Text gameClearText2;
    private float leftClearTime;
    Hellicopter rescueHellicopter;

    public Text pressE;
    public bool isPressE { get; set; }

    public Text reloadAlarm;
    public Text clearCubeText;
    public Text characterSwapTimeText;
    public Text characterSwapReadyText;

    //-----------NPC 관련 변수 -----
    public GameObject NPCtext;
    public Text NPCTalk; // NPC 대사창 
    public Image NPCImage; // npc 이미지 

    private string NpcContent; // 메모장 받는곳
    private string NpcYes; // 메모장 받는곳
    private string NpcNo; // 메모장 받는곳
    StringReader reader;
    public bool isYes = false;
    public event System.Action Yes;
    public event System.Action No;

    private string Npctext;

    private Coroutine coroutine;
    public GameObject Store;
    [SerializeField] private GameObject gameObjectUniqueStore;

    //----------------

    // 캐릭터 상태
    [SerializeField] private List<Sprite> characterIcon;
    [SerializeField] private Image CharIcon;
    [SerializeField] private Image CharIcon1;
    [SerializeField] private Image CharIcon2;
    [SerializeField] private Sprite CharIconEmpty;
    [SerializeField] private Image HpBar;
    [SerializeField] private Image SpBar;
    private float HpMax;
    private float HpNow;
    private float SpMax;
    private float SpNow;
    private string playerID;


    //-----------Skill UI -----
    [SerializeField] private List<Sprite> publicSkillIcon;
    [SerializeField] private List<Sprite> listAlice;
    [SerializeField] private List<Sprite> listBomber;
    [SerializeField] private List<Sprite> listSoldier;
    [SerializeField] private Image privateSkill;
    [SerializeField] private Image publicSkill1;
    [SerializeField] private Image publicSkill2;
    [SerializeField] private Image publicSkill3;
    [SerializeField] private Text privateSkillTimeText;
    [SerializeField] private Text privateSkillOnText;

    // 게임 화면 
    [SerializeField] private GameObject clearSence;
    [SerializeField] private GameObject gameObjectGameOver;
    [SerializeField] private Text overTime;
    [SerializeField] private Text overbossKillCount;


    // 데미지 관련 변수 
    private int maxTextCount = 200;
    private float moveSpeed = 2.0f;
    private float alphaSpeed;
    private float destroyTime;

    Queue<string> DMreader = new Queue<string>();
    Queue<Text> DMtexts = new Queue<Text>();
    Queue<DamageText> DamageTextQueue = new Queue<DamageText>();
    [SerializeField] private DamageText damageText;

    private bool isOnDamage;
    private float onDamageValue;

    // 보상 쳬계 관련 
    [SerializeField] private Text killbossCountText;
    int killbossCount;
    [SerializeField] private Text clearTime;
    private float timerSec;
    private int timerMin;
    [SerializeField] private Text clearBullet;

    [SerializeField] private Text Mylv;
    [SerializeField] private Text ShowMylv;
    [SerializeField] private Text MyEXP;
    [SerializeField] private Text Mybullet;

    private int PlayerLv;
    private int PlayerSkillKind;






    private void Start()
    {
        for (int i = 0; i < maxTextCount; i++)
        {
            DamageText text = Instantiate(damageText, transform);
            text.gameObject.SetActive(false);
            DamageTextQueue.Enqueue(text);
        }
    }
    public void DoDetailmain()
    {
        GameObjectDetail.SetActive(true);
        Windowdetail.text = details[0].text; // 마을 설명(0) 
    }

    public void DoDetailstage()
    {
        GameObjectDetail.SetActive(true);
        Windowdetail.text = details[1].text; // stage 설명(1) 
    }

    public void DeleteDetail()
    {
        GameObjectDetail.SetActive(false);
    }

    public void BattleTrue()
    {
        status_Battle.SetActive(true);
    }
    public void Battlefalse()
    {
        status_Battle.SetActive(false);
    }
    public void StatusTrue()
    {
        status.SetActive(true);
    }
    public void StatusFalse()
    {
        status.SetActive(false);
    }

    public void selectWeapon(int index)
    {
        images[index].sprite = selectSprites[index];
        switch (index)
        {
            case 0:
                defaultWeapon(1, 2);
                break;
            case 1:
                defaultWeapon(0, 2);
                break;
            case 2:
                defaultWeapon(0, 1);
                break;
        }
    }
    public void defaultWeapon(int index, int index2)
    {
        images[index].sprite = defaultSprites[index];
        images[index2].sprite = defaultSprites[index2];
    }
    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        ammoText.text = string.Format("{0} / {1}", magAmmo, remainAmmo);
    }

    public void UpdateGrenadeText(int maxGrenade, int remainGrenade)
    {
        grenadeText.text = remainGrenade + "/" + maxGrenade;
    }

    public void UpdateFlareText(int maxFlare, int remainFlare)
    {
        flareText.text = remainFlare + "/" + maxFlare;
    }

    public void activateReloadingText()
    {
        reloadText.gameObject.SetActive(true);
    }

    public void deactivateReloadingText()
    {
        reloadText.gameObject.SetActive(false);
    }

    public void activateWaveNotice()
    {
        waveNoticeText.gameObject.SetActive(true);
    }

    public void deactivateWaveNotice()
    {
        waveNoticeText.gameObject.SetActive(false);
    }

    public void activateBombReady()
    {
        bombReadyText.gameObject.SetActive(true);
    }

    public void deactivateBombReady()
    {
        bombReadyText.gameObject.SetActive(false);
    }

    public void activateTurretReady()
    {
        turretReadyText.gameObject.SetActive(true);
    }

    public void deactivateTurretReady()
    {
        turretReadyText.gameObject.SetActive(false);
    }

    // 점수 텍스트 갱신
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score : " + newScore;
    }

    // 적 웨이브 텍스트 갱신
    public void UpdateWaveText(int waves, int count)
    {
        waveText.text = "Wave : " + waves + "\nEnemy Left : " + count;
    }

    public void UpdateHealthText(float health, float Maxhealth)
    {
        healthText.text = "Health : " + health;
        HpMax = Maxhealth;
        HpNow = health;

    }
    public void UpdateSpText(float spPoint, float maxSpPoint)
    {
        spText.text = "SP : " + spPoint;
        SpMax = maxSpPoint;
        SpNow = spPoint;
    }

    // 게임 오버 UI 활성화
    public void SetActiveGameoverUI(bool active)
    {
        gameObjectGameOver.SetActive(active);
        overbossKillCount.text = killbossCount.ToString();
        StopTimer();
        overTime.text = string.Format("{0:D2}:{1:D2}", timerMin, (int)timerSec);
        Invoke("FalseGameover", 3f);
    }
    public void FalseGameover()
    {
        gameObjectGameOver.SetActive(false);
        SceneManager.LoadScene("MainTown");
        TestUnitManager.Instance.currentPlayerTrue();
    }

    public void UpdateBossHealth(float health)
    {
        BossHealth.text = "BossHealth : " + health;
    }

    public void UpdateInteract(Vector3 onPos)
    {
        interactKey.gameObject.SetActive(true);
        interactImage.gameObject.SetActive(true);
        interactKey.transform.position = onPos;
        interactImage.transform.position = onPos;

        interactKey.text = "E키 상호작용\n<color=yellow>안나</color>";
    }

    // 메뉴로 복귀
    public void GetBackMenu()
    {
        SceneManager.LoadScene("Menu");
    }


    // 게임 클리어 T/F
    public void ClearTrue()
    {
        StopTimer();
        clearSence.SetActive(true);

    }

    public void ClearFalse()
    {
        clearSence.SetActive(false);
    }

    //------NPC 관련 함수 ------
    public void Nulltext()
    {
        if (null != coroutine) StopCoroutine(coroutine);
        coroutine = null;
        NpcContent = null;
        NpcYes = null;
        NpcNo = null;

        NPCtext.SetActive(false);
    }
    public void SetText(string Content, string yes, string no)
    {
        Yes = null;
        No = null;

        //text.text = NpcName.text;
        if (NpcContent == null)
        {
            NpcContent = Content;
            NpcYes = yes;
            NpcNo = no;
            NPCtext.SetActive(true);

            //if (null != coroutine) StopCoroutine(coroutine);
            coroutine = StartCoroutine(talking());
        }
    }
    private IEnumerator talking()
    {
        reader = new StringReader(NpcContent);


        yield return readText();
        yield return select();
    }
    private IEnumerator readText()
    {
        if (null == reader) yield break;

        while (true)
        {
            if (reader == null)
                yield break;
            Npctext = reader.ReadLine();
            if (Npctext == null) break;
            NPCTalk.text = Npctext;
            yield return waiting();
        }
    }

    private IEnumerator waiting()
    {
        while (true)
        {
            if (Input.GetKeyDown("e"))
            {
                //Npctext = reader.ReadLine();
                yield break;
            }

            yield return null;
        }
    }
    private IEnumerator select()
    {
        reader = null;
        while (true)
        {
            if (Input.GetKeyDown("y"))
            {
                isYes = true;
                if (!string.IsNullOrEmpty(NpcYes)) reader = new StringReader(NpcYes);
                break;
            }
            else if (Input.GetKeyDown("n"))
            {
                if (!string.IsNullOrEmpty(NpcNo)) reader = new StringReader(NpcNo);
                break;
            }

            yield return null;
        }

        yield return readText();

        if (isYes) Yes?.Invoke();
        else No?.Invoke();
    }
    // Invoke("함수이름",3f) 3초 뒤에 함수가 실행


    public void StoreTrue()
    {
        Store.SetActive(true);
    }
    public void StoreFalse()
    {
        Store.SetActive(false);
    }

    public void UniqueStoreTrue()
    {
        gameObjectUniqueStore.SetActive(true);
    }
    public void UniqueStoreFalse()
    {
        gameObjectUniqueStore.SetActive(false);
    }
    

    //------------


    // skill UI 함수
    public void SetPlayer(string ID, int Lv, int skill)
    {
        playerID = ID;
        PlayerLv = Lv;
        PlayerSkillKind = skill;
    }

    public void PutIcon(int bullet)
    {
       
        if (playerID == "CH001")
        {
            privateSkill.sprite = listAlice[PlayerSkillKind];
            CharIcon.sprite = GameManager.instance.GetPlayer().GetPlayerIcon();
            DisplayCharIcons();
            SkillManager.instance.SetCurrentSkill1(PlayerSkillKind);
        }
        else if (playerID == "CH002")
        {
            privateSkill.sprite = listBomber[PlayerSkillKind];
            CharIcon.sprite = GameManager.instance.GetPlayer().GetPlayerIcon();
            DisplayCharIcons();
            SkillManager.instance.SetCurrentSkill2(PlayerSkillKind);
        }
        else if(playerID == "CH003")
        {
            
            privateSkill.sprite = listSoldier[PlayerSkillKind];
            CharIcon.sprite = GameManager.instance.GetPlayer().GetPlayerIcon();
            DisplayCharIcons();
            SkillManager.instance.SetCurrentSkill3(PlayerSkillKind);
        }
        publicSkill1.sprite = publicSkillIcon[0];
        publicSkill2.sprite = publicSkillIcon[1];
        publicSkill3.sprite = publicSkillIcon[2];
        HpBar.fillAmount = HpNow / HpMax;
        SpBar.fillAmount = SpNow / SpMax;
        ShowMylv.text = PlayerLv.ToString();
        Mybullet.text = bullet.ToString();
        rescuedNpcText.text = "Rescued : " + NpcManager.instance.GetRescuedNpcCount().ToString();
    }

    public void DisplayCharIcons()
    {
        var playerList = TestUnitManager.Instance.GetPlayerLists();

        int index = playerList.Count;

        switch(index)
        {
            case 0:
                CharIcon1.sprite = CharIconEmpty;
                CharIcon2.sprite = CharIconEmpty;
                break;
            case 1:
                CharIcon1.sprite = playerList[0].GetPlayerIcon();
                CharIcon2.sprite = CharIconEmpty;
                break;
            case 2:
                CharIcon1.sprite = playerList[0].GetPlayerIcon();
                CharIcon2.sprite = playerList[1].GetPlayerIcon();
                break;
        }
    }

    public void PrivateSkillUse()
    {
        privateSkillTimeText.gameObject.SetActive(false);
        privateSkillOnText.gameObject.SetActive(true);
    }

    public void PrivateSkillCool()
    {
        privateSkillTimeText.gameObject.SetActive(true);
        privateSkillOnText.gameObject.SetActive(false);
    }

    public void DisplayFormationIcon(Formation _formation, string _formationText, float _formationDist)
    {
        switch(_formation)
        {
            case Formation.Circle:
                formationDispaly.sprite = FormationIcons[(int)Formation.Circle];
                formationText.text = _formationText;
                formationTextDistance.text = "Distance : " + _formationDist.ToString();
                break;
            case Formation.Square:
                formationDispaly.sprite = FormationIcons[(int)Formation.Square];
                formationText.text = _formationText;
                formationTextDistance.text = "Distance : " + _formationDist.ToString();
                break;
        }
    }

    // 데미지폰트 관련함수 
    public void CheckDamage(bool flag, float damage)
    {
        isOnDamage = flag;
        onDamageValue = damage;
        DMreader.Enqueue(onDamageValue.ToString("F1"));
    }


    public void InsertDamageText(DamageText text)
    {
        text.gameObject.SetActive(false);
        DamageTextQueue.Enqueue(text);
    }

    public DamageText GetDamageText(Transform _pos, float damage)
    {
        var text = DamageTextQueue.Dequeue();
        var pos = Camera.main.WorldToScreenPoint(_pos.position);
        text.transform.position = pos;
        text.GetInfo(damage);
        text.gameObject.SetActive(true);
        return text;
    }

    // NPC 정보 관련
    

    //public void flowDamage()
    //{
    //    DMtext.transform.Translate(new Vector3(0, moveSpeed*Time.deltaTime, 0));
    //    /* pos.y = moveSpeed * Time.deltaTime; 
    //     DMtext.transform.Translate(pos.normalized);*/

    //    Invoke("waitingTime", 3f);
    //}

    //private void waitingTime(float time)
    //{
    //    DMtext.transform.Translate(0, 0, 0);
    //    DMreader = new StringReader("0");
    //    DMtext.text = DMreader.ReadLine();

    //}


    // 보상 체계 관련 함수 
    public void SetBullet(int addbullet)
    {
        clearBullet.text = addbullet.ToString();
    }

    public void setClearData(int lv,float exp)
    {
        // 게임 매니저 쪽에서 계산 해야함
        //clearTime;
        Mylv.text = lv.ToString();
        
        MyEXP.text = exp.ToString("F1");
        
        killbossCountText.text = killbossCount.ToString();

    }
    // 보스가 죽을때 마다 카운터 +1
    public void addBossKill()
    {
        killbossCount += 1;
        //killbossCountText.text = killbossCount.ToString("F1");
    }

    // 보상체계 시간관련
    public void InitTimer()
    {
        timerSec = 0;
        timerMin = 0;
    }
    public void UpdateTimer()
    {
        timerSec += Time.deltaTime;

        if((int)timerSec > 59)
        {
            timerSec = 0;
            timerMin++;
        }
    }
    public void StopTimer()
    {
        GameManager.instance.isTimerOn = false;
        clearTime.text = string.Format("{0:D2}:{1:D2}", timerMin, (int)timerSec);
    }

    // 게임 클리어 타이머
    public void OnGameClearTimer(int clearTime)
    {
        leftClearTime = clearTime;
        gameClearText1.gameObject.SetActive(true);
        gameClearText2.gameObject.SetActive(true);

    }

    public void OffGameClearTimer()
    {
        gameClearText1.gameObject.SetActive(false);
        gameClearText2.gameObject.SetActive(false);

    }
    public void UpdateClearTimeText(float time)
    {
        if (leftClearTime == 12)
        {
            Vector3 rescuePoint = GameManager.instance.GetPlayer().transform.position + -transform.forward * 100;
            rescuePoint += Vector3.up * 15f;

            rescueHellicopter = ItemManager.instance.CreateRescueHelli();
            rescueHellicopter.SetPosition(rescuePoint, transform.forward);
        }
        if (leftClearTime == 9)
            rescueHellicopter.PlayClip();
        if (leftClearTime == 5)
            if (rescueHellicopter.CheckDistance(GameManager.instance.GetPlayer().transform.position))
                rescueHellicopter.StopMove();

        leftClearTime -= time;
        float tempTime = leftClearTime;

        int min = (int)((tempTime / 60) % 60);
        int sec = (int)(tempTime % 60);

        gameClearText2.text = string.Format("{0:D2}:{1:D2}", min, sec);
    }

    // npc
    public void OnNpcHotkey()
    {
        pressE.gameObject.SetActive(true);
    }

    public void OffNpcHotKey()
    {
        pressE.gameObject.SetActive(false);
        UIManager.instance.isPressE = false;
    }

    // character swap timer
    public void UpdateCharacterSwapTime(float timer)
    {
        characterSwapTimeText.gameObject.SetActive(true);
        characterSwapReadyText.gameObject.SetActive(false);
        StartCoroutine(CharacterSwapTimer(timer));
    }

    private IEnumerator CharacterSwapTimer(float timer)
    {
        var display = timer;
        for(int i = 0; i < timer; i++)
        {
            display -= 1;
            characterSwapTimeText.text = ": " + display + "sec";
            yield return new WaitForSeconds(1f);
        }
        characterSwapTimeText.gameObject.SetActive(false);
        characterSwapReadyText.gameObject.SetActive(true);
}
}
