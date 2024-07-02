using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerWeapon : MonoBehaviour
{
    public Transform gunPivot;

    private PlayerInput playerInput;
    private Animator playerAnimator;

    public Transform weaponHold;

    //public Gun[] allGuns;
    //public Gun equippedGun;
    public GunController[] allGuns;
    public GunController equippedGun;

    public MeleeController[] allMelee;
    public MeleeController equippedMelee;
    
    public GameObject[] grenades;
    public int hasGrenades = 10;
    private int maxGrenades = 10;
    public GameObject grenadeObj;

    public int hasFlare = 10;
    private int maxFlare = 10;
    private bool isGunEquip;
    public GameObject flareObj;

    // 근접 콤보
    protected bool comboPossible;
    protected int comboStep;
    protected float lastAttackTime;
    [SerializeField] protected MeleeData meleeData;

    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }
    public void EquipGunWeapon(GunController gunToEquip)
    {
        equippedGun = gunToEquip;
    }
    public void EquipMeleeWeapon(MeleeController meleeToEquip)
    {
        equippedMelee = meleeToEquip;
    }
    public void EquipWeapon(int weaponIndex)
    {
        if (isGunEquip == true)
            EquipGunWeapon(allGuns[weaponIndex]);
        else
            EquipMeleeWeapon(allMelee[weaponIndex]);

        UIManager.instance.selectWeapon(weaponIndex);
    }

    // 근접 콤보
    public void ComboPossible()
    {
        comboPossible = true;
    }
    public void Combo()
    {
        if (comboStep == 2)
            playerAnimator.SetTrigger("Attack2");
        //if (comboStep == 3)
        //    playerAnimator.SetTrigger("Attack3");
    }
    public void ComboReset()
    {
        comboPossible = false;
        comboStep = 0;
    }


    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
        UIManager.instance.selectWeapon(0);

        if (equippedGun)
            isGunEquip = true;
        else
            isGunEquip = false;
    }

    private void OnEnable()
    {
        if (equippedGun)
            equippedGun.gameObject.SetActive(true);
        else
            equippedMelee.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (equippedGun)
            equippedGun.gameObject.SetActive(false);
        else
            equippedMelee.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (isGunEquip) // 총 발사
        {
            if (playerInput.fire)
            {
                equippedGun.Fire();
            }
            else if (playerInput.reload)
            {
                if (equippedGun.Reload())
                {
                    playerAnimator.SetTrigger("Reload");
                }
            }
        }
        else // 근접공격
        {
            if (playerInput.fire)
            {
                //equippedMelee.Attack(playerAnimator, comboPossible, comboStep);


                if (Time.time >= lastAttackTime + meleeData.TimeBetAttack)
                {
                    lastAttackTime = Time.time;

                    if (comboStep == 0)
                    {
                        playerAnimator.SetTrigger("Attack");
                        comboStep = 1;
                        return;
                    }
                    if (comboStep != 0)
                    {
                        if (comboPossible)
                        {
                            comboPossible = false;
                            comboStep += 1;
                        }
                    }
                }

                //if (comboStep == 0)
                //{
                //    playerAnimator.SetTrigger("Attack");
                //    comboStep = 1;
                //    return;
                //}
                //if (comboStep != 0)
                //{
                //    if (comboPossible)
                //    {
                //        comboPossible = false;
                //        comboStep += 1;
                //    }
                //}
            }
        }

        // 무기변경 안넣을예정, 대신 스킬로 쓸거
        if (playerInput.gunChange1)
        {
            OnDisable();
            EquipWeapon(0);
            playerAnimator.SetTrigger("Reload");
        }
        else if(playerInput.gunChange2)
        {
            OnDisable();
            EquipWeapon(1);
            playerAnimator.SetTrigger("Reload");
        }
        else if(playerInput.gunChange3)
        {
            OnDisable();
            EquipWeapon(2);
            playerAnimator.SetTrigger("Reload");
        }
        OnEnable();

        Grenade();
        Flare();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (equippedGun != null && UIManager.instance != null)
        {
            UIManager.instance.UpdateAmmoText(equippedGun.getMagAmmo(), equippedGun.getAmmoRemain());
            UIManager.instance.UpdateGrenadeText(maxGrenades, hasGrenades);
            UIManager.instance.UpdateFlareText(maxFlare, hasFlare);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        if (isGunEquip)
        {
            playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, equippedGun.leftHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, equippedGun.leftHandMount.rotation);
        }
        else
        {
            playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, equippedMelee.leftHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, equippedMelee.leftHandMount.rotation);
        }

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        if (isGunEquip)
        {
            playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, equippedGun.rightHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, equippedGun.rightHandMount.rotation);
        }
        else
        {
            playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, equippedMelee.rightHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, equippedMelee.rightHandMount.rotation);
        }
    }

    void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if (playerInput.throwGrenade)
        {
            Ray ray = playerInput.viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 50))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 5;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                //grenades[hasGrenades].SetActive(false);
            }
        }
    }


    void Flare()
    {
        if (hasFlare == 0)
            return;
        if(playerInput.flare)
        {
            Ray ray = playerInput.viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 20;

                GameObject instantFlare = Instantiate(flareObj, transform.position, transform.rotation);
                Rigidbody rigidFlare = instantFlare.GetComponent<Rigidbody>();
                rigidFlare.AddForce(nextVec, ForceMode.Impulse);
                rigidFlare.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasFlare--;
            }
        }
    }
}
