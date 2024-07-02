using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTrigger
{
    Alarm,
    Supply,
    Weapon
}
public class ItemCrateBox : MonoBehaviour
{
    [SerializeField] ItemTrigger itemTrigger;
    Character player;
    private bool isUsed = false;

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && !isUsed && Input.GetKeyDown("e"))
        {
            isUsed = true;

            switch (itemTrigger)
            {
                case ItemTrigger.Alarm:
                    Alarm();
                    break;

                case ItemTrigger.Supply:
                    Supply();
                    break;

                case ItemTrigger.Weapon:
                    Weapon();
                    break;
            }

        }
    }

    private void Alarm()
    {
        Debug.Log("Alarm");
        GameManager.instance.isWaveOn = true;
        player = EnemyManager.Instance.GetPlayer();
        player.GetAmmo(10);
    }

    private void Supply()
    {
        Debug.Log("Supply");
        player = EnemyManager.Instance.GetPlayer();
        player.GetAmmo(50);
    }

    private void Weapon()
    {
        Debug.Log("Weapon");
        player = EnemyManager.Instance.GetPlayer();
        player.GetAmmo(100);
    }
}
