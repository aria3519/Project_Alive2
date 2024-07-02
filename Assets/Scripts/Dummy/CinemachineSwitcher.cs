using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

public class CinemachineSwitcher : MonoBehaviour
{
    private PlayerInput action;
    [SerializeField]
    private CinemachineVirtualCamera vcam1;
    [SerializeField]
    private CinemachineVirtualCamera vcam2; //followcam

    private bool overworldCamera = true;
    //private float timeBetSwitch = 8f;

    private void Start()
    {
        action = GetComponent<PlayerInput>();
        //vcam2.enabled = true;
        //vcam1.enabled = false;
    }

    private void Update()
    {
        if (action)
        {
            if (true == action.changeView)
                SwitchPriority();
                //StartCoroutine(SwitchPriority(timeBetSwitch));
        }
        else
            Debug.Log("action is null");
    }

    private void SwitchPriority()
    {
        if (overworldCamera)
        {
            vcam1.Priority = 0;
            vcam2.Priority = 1;
        }
        else
        {
            vcam1.Priority = 1;
            vcam2.Priority = 0;
        }
        overworldCamera = !overworldCamera;
    }

    //private IEnumerator SwitchPriority(float time)
    //{
    //    Debug.Log("camera changed");
    //    vcam2.enabled = false;
    //    vcam1.enabled = true;

    //    yield return new WaitForSeconds(time);

    //    vcam1.enabled = false;
    //    vcam2.enabled = true;
    //}
}
