using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public string moveVerticalName = "Vertical"; // 앞뒤 움직임을 위한 입력축
    public string moveHorizontalName = "Horizontal"; // 좌우 움직임을 위한 입력축
    public string fireButtonName = "Fire1"; // 발사를 위한 입력 버튼
    public string skillButtonName = "Skill";
    public string reloadButtonName = "Reload"; // 재장전을 위한 입력 버튼
    public string throwButtonName = "Throw";
    public string flareButtonName = "Flare";
    public string changeViewButtonName = "changeView";
    public string sprintName = "Sprint";
    public string weaponChangeName1 = "WeaponChange1";
    public string weaponChangeName2 = "WeaponChange2";
    public string weaponChangeName3 = "WeaponChange3";
    public string skillName1 = "Skill1";
    public string skillName2 = "Skill2";
    public string skillName3 = "Skill3";
    public string pauseName = "Pause";
    public string dodgeName = "Dodge";
    public string swapName = "Swap";
    public string interactName = "Interact";
    // 값 할당은 내부에서만 가능
    public float moveVertical { get; private set; } // 앞 뒤 움직임
    public float moveHorizontal { get; private set; } // 왼쪽 오른쪽 움직임
    public bool fire { get; private set; } // 감지된 발사 입력값

    public bool skill { get; private set; }
    public bool reload { get; private set; } // 감지된 재장전 입력값

    public bool throwGrenade { get; private set; }

    public bool flare { get; private set; }

    public bool changeView { get; private set; }

    public bool sprint { get; private set; }
    public bool gunChange1 { get; private set; }
    public bool gunChange2 { get; private set; }
    public bool gunChange3 { get; private set; }

    public bool skillSet1 { get; private set; }

    public bool skillSet2 { get; private set; }
    public bool skillSet3 { get; private set; }
    public bool pause { get; private set; }

    public bool dodge { get; private set; }

    public bool swap { get; private set; }

    public bool interact { get; private set; }
}
