using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwap : MonoBehaviour
{
    // 게임 매니저에 넣을 스크립트
    // 메뉴창에서 캐릭터 선택 후 게임 입장

    private void Update()
    {
        // 현재 플레이 중인 캐릭터가 죽었을때 스왑 x
        

        // 캐릭터 스왑시 큐 FIFO 이므로 자동적으로 소환순서가 정해짐
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            
            UnitManager.instance.SwapCharacter();
        }
    }

}
