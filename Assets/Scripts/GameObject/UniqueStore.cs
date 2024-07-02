using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public struct SkillInfo
{
    public int skillKind;
    public string charId;
}


public class UniqueStore : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectStore;
    [SerializeField] private Image SkillIcon1;
    [SerializeField] private Image SkillIcon2;
    [SerializeField] private Image SkillIcon3;
    [SerializeField] private Text Skill_text1;
    [SerializeField] private Text Skill_text2;
    [SerializeField] private Text Skill_text3;
    //[SerializeField] private List<Image> listImage;
   
    
    // Start is called before the first frame update
    // 원본데이터 
    [SerializeField] private List<TextAsset> listSkilText =new List<TextAsset>();
    [SerializeField] private List<Sprite> listSkillIcon = new List<Sprite>();
    private List<SkillInfo> listskillinfo = new List<SkillInfo>();
    // 원본데이터를 랜덤으로 뽑아 현재 팔 상품을 모음 
    private List<string> nowlistText = new List<string>();
    private List<Sprite> nowListSkill = new List<Sprite>();
    private List<SkillInfo> nowListSkilInfo = new List<SkillInfo>();

    private int RandomInt;
    private int maxSellingCount=3; // 상점 판매 최대수 
    private int maxSkillCount=12;
    private int maxCharcount = 3;

    private void Start()
    {
       
        SetSkillData();
        List<TextAsset> AllTexttemp = new List<TextAsset>(listSkilText);
        List<Sprite> privateSkillIcontemp = new List<Sprite>(listSkillIcon);
        List<SkillInfo> listSkillinfoTemp = new List<SkillInfo>(listskillinfo);
      

        for (int i=0; i< maxSellingCount; i++)
        {
            RandomInt = Random.Range(0, maxSkillCount - i); // 랜덤 범위를 설정 
            nowlistText.Add(AllTexttemp[RandomInt].text);
            AllTexttemp.Remove(AllTexttemp[RandomInt]);
            nowListSkill.Add (privateSkillIcontemp[RandomInt]);
            privateSkillIcontemp.Remove(privateSkillIcontemp[RandomInt]);
            nowListSkilInfo.Add(listSkillinfoTemp[RandomInt]);
            listSkillinfoTemp.Remove(listSkillinfoTemp[RandomInt]);
        }

        SkillIcon1.sprite = nowListSkill[0];
        SkillIcon2.sprite = nowListSkill[1];
        SkillIcon3.sprite = nowListSkill[2];
        

        Skill_text1.text = nowlistText[0];
        Skill_text2.text = nowlistText[1];
        Skill_text3.text = nowlistText[2];

        




    }
    private void SetSkillData()
    {
        // 각 캐릭터 1번 스킬 힐 2번 스킬 신속 3번 고유스킬1 4번 고유스킬2
        SkillInfo temp;
        int j = 0;
        for (int i = 1; i <= maxSkillCount; i++)
        {
            if (i % maxCharcount == 1) // 
            {
                j++;
                temp.charId = "CH001";
                if (j == 4)
                {
                    temp.skillKind = 3;
                }
                else
                    temp.skillKind = j;
                listskillinfo.Add(temp);
            }
            else if (i % maxCharcount == 2) // 
            {
                temp.charId = "CH002"; // 밤버 
                temp.skillKind = j;
                listskillinfo.Add(temp);
            }
            else if (i % maxCharcount == 0) // 
            {
                temp.charId = "CH003"; // 솔져 
                temp.skillKind = j;
                listskillinfo.Add(temp);
            }

        }
    }
    private void ClearList()
    {
        nowListSkill.Clear();
        nowlistText.Clear();
        listskillinfo.Clear();
        nowListSkilInfo.Clear();
    }
    public void PutSkill1()
    {
        GameManager.instance.WhatBuySkill(nowListSkilInfo[0].skillKind, nowListSkilInfo[0].charId);
        ClearList();
        BackStore();
        
    }
    public void PutSkill2()
    {
        GameManager.instance.WhatBuySkill(nowListSkilInfo[1].skillKind, nowListSkilInfo[1].charId);
        ClearList();
        BackStore();
       
    }
    public void PutSkill3()
    {
        GameManager.instance.WhatBuySkill(nowListSkilInfo[2].skillKind, nowListSkilInfo[2].charId);
        ClearList();
        BackStore();
       
    }
    public void BackStore()
    {
        gameObjectStore.SetActive(false);
    }
}
