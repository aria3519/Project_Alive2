using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCam : MonoBehaviour
{
    private Camera _my;
    [SerializeField] private Transform target;
    [SerializeField] private float y = 100f;
    void Start()
    {
        _my = GetComponent<Camera>();
        Init();
    }
    void Init()
    {
        
    }

    private void FixedUpdate()
    {
        FollowTarget();
    }


    void FollowTarget()
    {
        if (GameManager.instance.GetPlayer() == null) return;

        target = GameManager.instance.GetPlayer().transform;

        transform.position = new Vector3(target.position.x, y, target.transform.position.z);
        //transform.rotation = new Quaternion(90,0,0,0);
        
    }

    
}
