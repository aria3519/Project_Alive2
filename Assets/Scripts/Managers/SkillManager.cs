using UnityEngine;
using System.Collections.Generic;

public enum SkillState
{ 
    TURRET,
    BOMB,
    BOMB_MULTI,
    ALPHA,
    ROCKET,
    MINIGUN,
    HEAL,
    FAST,
    SUPPLY,
    FLAMETHROWER
}

public class SkillManager : MonoBehaviour
{
    private static SkillManager m_instance;

    public static SkillManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<SkillManager>();
            }

            return m_instance;
        }
    }
    [SerializeField] ItemData itemData;
 

    private Vector3 bombPos;
    private Vector3 turretPos;
    private bool isClicked = false;

    // 폭격관련
    public bool isSkillUsed { get; set; }
    public bool isBomb { get; set; }
    public bool isCharChanged { get; set; }

    public SkillState skillState { get; set; }
    public SkillState currentSkillState { get; set; }

    private float bombSkillTime;
    private float alphaSkillTime;
    private float rocketSkillTime;
    private float minigunSkillTime;
    private float healSkillTime;
    private float fastSkillTime;
    private float supplySkillTime;
    private float flameThrowerSkillTime;

    // 신 스킬관련
    [SerializeField] private List<ParticleSystem> Skills_;


    public void SetTurretPos(Vector3 pos)
    {
        turretPos = pos;
    }

    public Vector3 GetTurretPos()
    {
        if (turretPos == null)
            return transform.position.normalized;
        else
            return turretPos;
    }

    public JetBomber GetJetBomberToAid()
    {
        return itemData.JetBomber;
    }

    public bool clickCheck()
    {
        return isClicked;
    }
    public void setClickCheck(bool flag)
    {
        isClicked = flag;
    }

    // 폭격
    public void SetBombPos(Vector3 pos)
    {
        bombPos = pos;
    }

    public Vector3 GetBombPos()
    {
        if (bombPos == null)
            return transform.position.normalized;
        else
            return bombPos;
    }

    public int GetMaxBombSkillCount()
    {
        return itemData.maxJetBomber;
    }

    // SkillCool 전달
    public void SetSkillTime(SkillState state, float value)
    {
        switch(state)
        {
            case SkillState.BOMB:
                bombSkillTime = value;
                break;
            case SkillState.ALPHA:
                alphaSkillTime = value;
                break;
            case SkillState.ROCKET:
                rocketSkillTime = value;
                break;
            case SkillState.MINIGUN:
                minigunSkillTime = value;
                break;
            case SkillState.HEAL:
                healSkillTime = value;
                break;
            case SkillState.FAST:
                fastSkillTime = value;
                break;
            case SkillState.SUPPLY:
                supplySkillTime = value;
                break;
            case SkillState.FLAMETHROWER:
                flameThrowerSkillTime = value;
                break;
        }
    }

    public void SetCurrentSkill1(int skillIndex)
    {
        switch(skillIndex)
        {
            case 0:
                break;
            case 1:
                currentSkillState = SkillState.HEAL;
                break;
            case 2:
                currentSkillState = SkillState.FAST;
                break;
            case 3:
                currentSkillState = SkillState.ALPHA;
                break;
            case 4:
                currentSkillState = SkillState.ALPHA;
                break;
        }
    }

    public void SetCurrentSkill2(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:
                break;
            case 1:
                currentSkillState = SkillState.HEAL;
                break;
            case 2:
                currentSkillState = SkillState.FAST;
                break;
            case 3:
                currentSkillState = SkillState.ROCKET;
                break;
            case 4:
                currentSkillState = SkillState.SUPPLY;
                break;
        }
    }

    public void SetCurrentSkill3(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:
                break;
            case 1:
                currentSkillState = SkillState.HEAL;
                break;
            case 2:
                currentSkillState = SkillState.FAST;
                break;
            case 3:
                currentSkillState = SkillState.MINIGUN;
                break;
            case 4:
                currentSkillState = SkillState.FLAMETHROWER;
                break;
        }
    }

    public float GetSkillTime(SkillState state)
    {
        switch(state)
        {
            case SkillState.BOMB:
                return bombSkillTime;
            case SkillState.ALPHA:
                return alphaSkillTime;
            case SkillState.ROCKET:
                return rocketSkillTime;
            case SkillState.MINIGUN:
                return minigunSkillTime;
            case SkillState.HEAL:
                return healSkillTime;
            case SkillState.FAST:
                return fastSkillTime;
            case SkillState.SUPPLY:
                return supplySkillTime;
            case SkillState.FLAMETHROWER:
                return flameThrowerSkillTime;
        }
        return 0;
    }



    public void UseSkill(Vector3 playerPos, Vector3 pos , in int num , in float attack)
    {
        int tempNum = num;
        if (tempNum >= Skills_.Count)
        { 
            tempNum = 0;
            Debug.Log("그런 스킬 없음 tempNum:" + tempNum);
            return;
        }
        Skills_[tempNum].gameObject.SetActive(true);
        Skills_[tempNum].transform.position = pos;
        Skills_[tempNum].transform.forward =(playerPos - pos);


        Skills_[tempNum].Play();
    }
}
