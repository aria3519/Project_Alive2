using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        // Testunit Manager 로 위치를 보내줘야함 
        if(transform)
            TestUnitManager.Instance.GetStartPoint(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other && other.tag == "Player")
        {
            
            TestUnitManager.Instance.UpdateCurrentPlayerInfo();
            UIManager.instance.InitTimer();
            GameManager.instance.isTimerOn = true;
        }
    }
}
