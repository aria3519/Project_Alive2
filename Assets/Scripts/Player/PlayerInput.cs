using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 플레이어 캐릭터를 조작하기 위한 사용자 입력을 감지
// 감지된 입력값을 다른 컴포넌트들이 사용할 수 있도록 제공
public class PlayerInput : MonoBehaviour
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
    public string skillName1 = "Skill1";
    public string skillName2= "Skill2";
    public string skillName3 = "Skill3";
    public string pauseName = "Pause";
    public string dodgeName = "Dodge";
    public string swapName = "Swap";
    public string interactName = "Interact";
    public string formKeyName = "Formation";
    public string changeFormKeyName = "ChangeFormation";
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

    public bool formKey { get; private set; }
    public bool changeFormKey { get; private set; }

    public Camera viewCamera;
    //PlayerWeapon weaponController;
    Character weaponController;
    public Crosshairs crosshairs;
    private PlayerMovement controller;
    
    LineRenderer aimLine;
    Vector3 aimStartPos;

    private GameObject turret;
    private GameObject jetBomber;

    bool isPause = false;

    public Vector3 mousePoint;

    

    private void Start()
    {
        
        controller = GetComponent<PlayerMovement>();
        //weaponController = GetComponent<PlayerWeapon>();
        weaponController = GetComponent<Character>();
        aimLine = GetComponent<LineRenderer>();
        viewCamera = Camera.main;
    }

    private void OnEnable()
    {
        viewCamera = Camera.main;
    }

    public void SetFireFalse()
    {
        fire = false;
    }

    private void FixedUpdate()
    {
        //viewCamera = GameManager.instance.viewCamera;
        if (viewCamera == null)
            viewCamera = Camera.main;

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        //Plane groundPlane = new Plane(Vector3.up, Vector3.up * weaponController.GunHeight);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * weaponController.WeaponHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            controller.LookAt(point);
            crosshairs.transform.position = point; // 크로스헤어 이동
            crosshairs.DetectTargets(ray); // 적이 있는지 판별

            aimStartPos = transform.position + transform.forward;
            aimStartPos.y += 1f;

            aimLine.SetPosition(0, aimStartPos);
            aimLine.SetPosition(1, point);
        }
    }
    // 매프레임 사용자 입력을 감지
    private void Update() {
        // 게임오버 상태에서는 사용자 입력을 감지하지 않는다
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            moveVertical = 0;
            moveHorizontal = 0;
            fire = false;
            skill = false;
            reload = false;
            throwGrenade = false;
            pause = false;
            dodge = false;
            return;
        }

        /* // move에 관한 입력 감지
         moveVertical = Input.GetAxis(moveVerticalName);

         // move2 관한 입력 감지
         moveHorizontal = Input.GetAxis(moveHorizontalName);
         // fire에 관한 입력 감지
         fire = Input.GetButton(fireButtonName);*/
        fire = Input.GetButton(fireButtonName);

        if (Input.GetMouseButtonDown(0)) Moving();
        skill = Input.GetMouseButtonDown(1);
        // reload에 관한 입력 감지
        reload = Input.GetButtonDown(reloadButtonName);

        throwGrenade = Input.GetButtonDown(throwButtonName);

        flare = Input.GetButtonDown(flareButtonName);

        changeView = Input.GetButtonDown(changeViewButtonName);

        sprint = Input.GetButton(sprintName);

        pause = Input.GetButtonDown(pauseName);

        skillSet1 = Input.GetButtonDown(skillName1);
        skillSet2 = Input.GetButtonDown(skillName2);
        skillSet3 = Input.GetButtonDown(skillName3);

        dodge = Input.GetButtonDown(dodgeName);

        swap = Input.GetButtonDown(swapName);

        interact = Input.GetButtonDown(interactName);

        if (interact)
        {
            UIManager.instance.OffNpcHotKey();
            UIManager.instance.isPressE = true;
            GameManager.instance.isInteractkeyDowned = true;
            StartCoroutine(InteractWaitTime());
        }

        formKey = Input.GetButton(formKeyName);

        changeFormKey = Input.GetButtonDown(changeFormKeyName);

        // 게임 일시정지 기능
        if (pause)
        {
            isPause = !isPause;
            Debug.Log((isPause ? "Game Pause" : "Game Start"));
            Time.timeScale = isPause ? 0 : 1;
        }

        // 마우스포인터 + 스킬
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * weaponController.WeaponHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            mousePoint = ray.GetPoint(rayDistance);
            controller.LookAt(mousePoint);
        }

        if (formKey && Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            NpcManager.instance.ChangeFormationSpace(0.5f);
        }
        else if (formKey && Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            NpcManager.instance.ChangeFormationSpace(-0.5f);
        }

        if (changeFormKey)
            NpcManager.instance.ChangeFormation();
    }

    IEnumerator InteractWaitTime()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.isInteractkeyDowned = false;
    }


    private void Moving()
    {
        /*if(controller.canMove)
            StartCoroutine(controller.Moving(mousePoint));*/


        controller.SetTargetPos(mousePoint);

    }
}