using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSupply : Item
{
    private SphereCollider sphereCollider;
    [SerializeField] private ParticleSystem interactEffect;

    [SerializeField] private int supplyAmmo = 20;
    private int count = 0;

    private void OnTriggerStay(Collider other)
    {
        if(count == 0 && Input.GetKeyDown("e") && other.tag == "Player")
        {
            interactEffect.Play();
            TestUnitManager.Instance.UseSupplySkill(supplyAmmo);
            StartCoroutine(SupplySkillTime());
            count++;
        }
    }
    IEnumerator SupplySkillTime()
    {
        yield return new WaitForSeconds(5f);
        count = 0;
        UIManager.instance.PrivateSkillCool();
        ItemManager.instance.InsertItemSupply(this);
    }
}
