using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwap : MonoBehaviour
{
    // ���� �Ŵ����� ���� ��ũ��Ʈ
    // �޴�â���� ĳ���� ���� �� ���� ����

    private void Update()
    {
        // ���� �÷��� ���� ĳ���Ͱ� �׾����� ���� x
        

        // ĳ���� ���ҽ� ť FIFO �̹Ƿ� �ڵ������� ��ȯ������ ������
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            
            UnitManager.instance.SwapCharacter();
        }
    }

}
