using System;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum KindScene
{
    KindScene_Menu,
    KindScene_MainTown,
    KindScene_Stage,

}
public struct CharData
{
    public string name;
    public int level;
    public float exp;
    public float health;
}

public struct Buff
{
    public int buffStep;
    public int buffKind;
    public int healthAdd;
    public int spAdd;
    public int cost;
}

public class GameManager : MonoBehaviour
{
    private int score = 0;
    private int deathCheck = 0;
    public bool isMenu;
    public bool isGameover { get; set; }
    public bool playerInteract { get; set; }
    public bool isWaveOn { get; set; }

    public bool isInteractkeyDowned { get; set; }

    public bool isChangeStage;

    public bool isStageStarted { get; set; }
    public bool isRePlay;

    public Camera viewCamera;
    

    public GameObject HUD;
    [SerializeField]
    private Character player;

    // -----XML 정보 관리 관련 변수----
    private XmlDocument file;
    private XmlNode CharInfoRoot;
    private List<CharData> listCharData = new List<CharData>();
    public bool isSave;

    private string nowSaveFile;


    // 마우스 커서 관리 
    [SerializeField] private Texture2D cursorTexture;
    //텍스처의 중심을 마우스 좌표로 할 것인지 체크박스로 입력받습니다.
    [SerializeField] private bool hotSpotIsCenter = false;
    //텍스처의 어느부분을 마우스의 좌표로 할 것인지 텍스처의
    //좌표를 입력받습니다.
    [SerializeField] private Vector2 adjustHotSpot = Vector2.zero;
    private Vector2 hotSpot;

   



    // 보상체계 관련 변수


    private int attackPlus;
    private int totalBullet = 200; // 자원


    // 타이머 관련 변수 
    public bool isTimerOn { get; set; }


    // 현재 상황 저장 변수
    private int whatNowScene;

    // 유니크 상점 관련 변수

    // 마을 
    private bool detailOnce;
    private Coroutine coroutine;


    // 카메라 각도 
    //Vector3(3.40992856,16.2222519,-7.74717855)
    //Vector3(61.9999962,315,0)


    private bool isLobby = false;

    public int playerNum { private set; get; } = -1;

    //--------------
    private static GameManager m_instance;
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }

            return m_instance;
        }
    }

    private void Awake()
    {
        isLobby = true;
        viewCamera = Camera.main;
        


        DontDestroyOnLoad(gameObject);
        if (instance != this)
        {
            Destroy(gameObject);
        }



    }

    private void Start()
    {
        whatNowScene = 0;
        isChangeStage = false;
        /* isMenu = true;*/

        //====XML 정보 처리====
        InitListCharData();
        isSave = false;
        nowSaveFile = "save1";
        createRoot(nowSaveFile);
        isRePlay = false;
        /* Load(nowSaveFile);*/

        /*  XmlSave();*/
        //=========
        /* nowSaveFile = "save1";
         Load(nowSaveFile);*/



        //UnitManager.instance.CreateCharacter();
        //FindObjectOfType<CharOriginal_Soldier>().onDeath += EndGame;
        //FindObjectOfType<CharOriginal_Alice>().onDeath += EndGame;
        //FindObjectOfType<PlayerHealth>().onDeath += EndGame;
    }

    private void Update()
    {
        if (isStageStarted)
        {
            TestUnitManager.Instance.CreatePlayer(playerNum);
            isStageStarted = false;
        }
        if (isMenu)
        {
            Cursor.visible = true;
            SetHUDFalse();
        }
        else
        {
           /* Cursor.visible = false;*/
            SetHUDTrue();
            UIManager.instance.PutIcon(totalBullet);
            //NpcManager.instance.MakeFormation(player.transform);
        }
        // 커서 보여주는 코드 
       /* if (hotSpotIsCenter)
        {
            hotSpot.x = cursorTexture.width / 2;
            hotSpot.y = cursorTexture.height / 2;
        }
        else
        {
            hotSpot = adjustHotSpot;
        }
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);*/
        //------
        // save 코드 
        if (isSave)
        {
            isSave = false;
            XmlSave(nowSaveFile);
        }
        // 타이머 코드
        if (isTimerOn)
        {
            UIManager.instance.UpdateTimer();
        }

        if (player)
            NpcManager.instance.UpdateFormation(player.transform);


        if (isRePlay)
        {
            if (nowSaveFile == null)
                nowSaveFile = "save1";
            Load(nowSaveFile);
        }
        if(detailOnce)
            ShowDetail();

       



        Cursor.visible = true;
    }

    private void ShowDetail()
    {
        switch(whatNowScene)
        {
            case (int)KindScene.KindScene_Menu: // 메뉴

                break;
            case (int)KindScene.KindScene_MainTown: // 마을 
                UIManager.instance.DoDetailmain();
                coroutine = StartCoroutine(KeyCheck());
                break;
            case (int)KindScene.KindScene_Stage: // 스테이지 
                UIManager.instance.DoDetailstage();
                coroutine = StartCoroutine(KeyCheck());
                break;
        }
    }

    private IEnumerator KeyCheck()
    {
        while (true)
        {
            if (Input.GetKeyDown("e"))
            {
                UIManager.instance.DeleteDetail();
                detailOnce = false;
                yield break;
            }
            yield return null;
        }
    }

    public void SetDetailOnce(in int num )
    {
        detailOnce = true;
        isLobby = false;
        playerNum = num;
    }
    

    public void AddScore(int newScore)
    {
        if (!isGameover)
        {
            score += newScore;

            UIManager.instance.UpdateScoreText(score);
        }
    }

    public void EndGame(int maxValue)
    {
        deathCheck++;
        if (deathCheck >= maxValue)
        {

            isGameover = true;
            SetNowScene((int)KindScene.KindScene_MainTown);
            UIManager.instance.Battlefalse();
            UIManager.instance.SetActiveGameoverUI(true);
            NpcManager.instance.InitRescuedList();
            isWaveOn = false;
            deathCheck = 0;
        }
    }

    public void InitDeathCount()
    {
        deathCheck = 0;
    }

    public void GetSceneInfo(bool _isChanged)
    {
        isStageStarted = _isChanged;
    }

    public bool ChangeStage()
    {
        isChangeStage = true;
        return isChangeStage;
    }

    // 
    public bool NotChangeStage()
    {
        isChangeStage = false;
        return isChangeStage;
    }

    public void SetHUDTrue()
    {
        if(isLobby)
        {
            viewCamera.transform.position = new Vector3(-1.5f, 5, -19.3799992f);
            viewCamera.transform.rotation = new Quaternion(0, 0, 0, 0);


            return;

        }

        
        if (player != null)
        {
            viewCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y +15f, player.transform.position.z);
            //viewCamera.transform.rotation = new Quaternion(61.9999962f, 315, 0, 0);
            viewCamera.transform.LookAt(player.transform.position);
        }
            
        isMenu = false;
        HUD.SetActive(true);
    }

    public void SetHUDFalse()
    {
        isMenu = true;
        HUD.SetActive(false);
    }

    public void SetPlayer(Character getPlayer)
    {
        player = getPlayer;
    }

    public Character GetPlayer()
    {
        return player;
    }


    // XML 관련 함수 
    public List<CharData> SetNowFile(string file)
    {
        nowSaveFile = file;
        Load(file);
        return listCharData;

    }
    private void createRoot(string fileName)
    {
        file = new XmlDocument();
        CharInfoRoot = file.CreateNode(XmlNodeType.Element, "playAllData", string.Empty);
        file.AppendChild(CharInfoRoot);
    }



    private void XmlSave(string fileName)
    {
        /*XmlNode child = file.CreateNode(XmlNodeType.Element, "SaveCharFile", string.Empty);
        CharInfoRoot.AppendChild(child);
        */
        for (int i = 0; listCharData.Count > i; i++)
        {
            XmlNode child = file.CreateNode(XmlNodeType.Element, "SaveCharFile", string.Empty);
            CharInfoRoot.AppendChild(child);

            var name = file.CreateAttribute("Name");
            name.Value = listCharData[i].name;
            child.Attributes.Append(name);

            var lv1 = file.CreateAttribute("Level");
            lv1.Value = listCharData[i].level.ToString();
            child.Attributes.Append(lv1);

            var exp = file.CreateAttribute("Experience");
            exp.Value = listCharData[i].exp.ToString("F1"); ;
            child.Attributes.Append(exp);

            var health = file.CreateAttribute("health");
            health.Value = listCharData[i].health.ToString("F1");
            child.Attributes.Append(health);

            CharInfoRoot.AppendChild(child);
        }

        fileName = System.IO.Path.ChangeExtension(fileName, ".xml");
        /*   var path = string.Format(@"{0}/Resources/{1}", Application.dataPath, name);*/
        var path = string.Format(@"{0}/{1}", Application.streamingAssetsPath, fileName);
        // 파일명을 제외한 최하위 폴더까지의 경로만을 구한다.
        var directoryPath = System.IO.Path.GetDirectoryName(path);
        // Assets 폴더의 바로 하위에 Resources 폴더가 없다면 생성.
        if (!System.IO.Directory.Exists(directoryPath)) System.IO.Directory.CreateDirectory(directoryPath);

        // 해당 프로젝트의 Resources/...폴더에 xml 파일 생성.
        file.Save(path);





    }
    private void Load(string name)
    {
        //name = System.IO.Path.GetFileNameWithoutExtension(name);
        //var xmlText = Resources.Load<TextAsset>(name);
        //if (!xmlText) throw new Exception(string.Format("Not Find TextAsset : {0}", name));

        /* StreamReader sr = new StreamReader(Application.dataPath + "/StreamingAssets" + "/" + "text.txt");*/
        file = new XmlDocument();
        name = System.IO.Path.ChangeExtension(name, ".xml");
        var path = string.Format(@"{0}/{1}", Application.streamingAssetsPath, name);
        file.Load(path);
        //file.LoadXml(xmlText.text); // text파일을 xml 데이터 형태로 변환.
        var nodes = file.DocumentElement;
        List<CharData> infos = new List<CharData>();


        foreach (XmlElement node in nodes.ChildNodes)
        {
            if (node.LocalName == "SaveCharFile")
            {
                CharData temp = new CharData();
                foreach (XmlAttribute nodechild in node.Attributes)
                {
                    switch (nodechild.Name)
                    {
                        case "Name":
                            temp.name = nodechild.Value;
                            break;
                        case "Level":
                            temp.level = System.Convert.ToInt16(nodechild.Value);
                            break;
                        case "Experience":
                            temp.exp = System.Convert.ToSingle(nodechild.Value);
                            break;
                        case "health":
                            temp.health = System.Convert.ToSingle(nodechild.Value);
                            infos.Add(temp);
                            break;
                    }
                }
            }
        }

        listCharData = infos;

    }

    // 프리팹 수정으로 체력할당해야하는데 여기서 다시 재정의해서 위험
    private void InitListCharData()
    {
        CharData temp;
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    temp.name = "CH001";
                    temp.level = 1;
                    temp.exp = 0;
                    temp.health = 1000;
                    listCharData.Add(temp);
                    break;
                case 1:
                    temp.name = "CH002";
                    temp.level = 1;
                    temp.exp = 0;
                    temp.health = 1000;
                    listCharData.Add(temp);
                    break;
                case 2:
                    temp.name = "CH003";
                    temp.level = 1;
                    temp.exp = 0;
                    temp.health = 1000;
                    listCharData.Add(temp);
                    break;
            }
        }
    }

    public void SetXML(CharData chData)
    {
        isSave = true;
        for (int i = 0; i < listCharData.Count; i++)
        {
            if (chData.name == listCharData[i].name)
            {
                listCharData[i] = chData;
            }
        }
    }

    // 없어도 아무 문제없음
    public void GetXMLData()
    {
        TestUnitManager.Instance.SetLoadData(listCharData);
    }

    // 보상 관련 함수 
    public void gameClear()
    {
        int add = 100;
        var npcAdd = NpcManager.instance.GetRescuedNpcCount() * 100;
        totalBullet += add + npcAdd;
        UIManager.instance.SetBullet(add + npcAdd);
        TestUnitManager.Instance.AfterClear(add);
        NpcManager.instance.InitRescuedList();
    }

    public int DamagePlus()
    {
        return attackPlus;
    }

    public void CheckBuff(Buff buff)
    {

        switch (buff.buffKind)
        {
            case 0: // attack
                attackPlus += buff.buffStep * 5 + 5 * (buff.buffStep - 1);
                break;
            case 1: // health
                buff.healthAdd = buff.buffStep * 100 + 100 * (buff.buffStep - 1);
                buff.spAdd = 0;
                break;
            case 2: // Sp
                buff.spAdd = buff.buffStep * 10 + 10 * (buff.buffStep - 1);
                buff.healthAdd = 0;
                break;
        }
        TestUnitManager.Instance.InputBuff(buff);

    }

    public bool checkBullet(int costBullet)
    {
        if (costBullet <= totalBullet)
        {
            totalBullet -= costBullet;
            return true;
        }
        return false;
    }

    // 종료메뉴
    public int GetNowScene()
    {
        return whatNowScene;
    }
    public void SetNowScene(int kindScene)
    {
        whatNowScene = kindScene;
    }


    // 유니크 상점 관련 함수
    public void WhatBuySkill(int Skillkind, string ID)
    {
        TestUnitManager.Instance.GetBuySKill(Skillkind, ID);
    }

    
    
}
