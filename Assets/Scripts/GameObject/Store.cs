using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Store : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectStore;
   
    Buff buff;
    // CheckBuff(buffStep,buffKind); (�ܰ�, ����)
    // ���� 0 : attack , 1: health , 2: sp
    // ĳ���� ���ݷ� ����
    public void AttackUp1()
    {
        buff.buffKind = 0;
        buff.buffStep = 1;
        
        BuyBuff();

    }
    public void AttackUp2()
    {
        buff.buffKind = 0;
        buff.buffStep = 2;
        
        BuyBuff();
    }
    public void AttackUp3()
    {
        buff.buffKind = 0;
        buff.buffStep = 3;
        
        BuyBuff();
    }

    // ĳ���� ü�� ����

    public void HealthUp1()
    {
        buff.buffKind = 1;
        buff.buffStep = 1;
        
        BuyBuff();
    }
    public void HealthUp2()
    {
        buff.buffKind = 1;
        buff.buffStep = 2;
       
        BuyBuff();
    }
    public void HealthUp3()
    {
        buff.buffKind = 1;
        buff.buffStep = 3;
       
        BuyBuff();
    }

    // ĳ���� Ȱ�� ���� 

    public void SpUp1()
    {
        buff.buffKind = 2;
        buff.buffStep = 1;
       
        BuyBuff();
    }
    public void SpUp2()
    {
        buff.buffKind = 2;
        buff.buffStep = 2;
        
        BuyBuff();
    }
    public void SpUp3()
    {
        buff.buffKind = 2;
        buff.buffStep = 3;
       
        BuyBuff();
    }

    private void BuyBuff()
    {
        buff.cost = 200 * buff.buffStep;
        if (GameManager.instance.checkBullet(buff.cost))
        {
            GameManager.instance.CheckBuff(buff);
            gameObjectStore.SetActive(false);
        }
        /*else
        {
            cantBuyScene.SetActive(true);
        }*/

    }
    public void BackStore()
    {
        gameObjectStore.SetActive(false);
    }
}
