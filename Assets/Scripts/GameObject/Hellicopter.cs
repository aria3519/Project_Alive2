using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hellicopter : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    Vector3 position;

    [SerializeField] private GameObject effectObj;
    [SerializeField] private AudioClip HellicopterClip;
    [SerializeField] private AudioClip HellicopterEngineClip;
    Rigidbody myrigidbody;
    private AudioSource HellicopterAudio;
    bool isMove = true;

    private void Start()
    {
        myrigidbody = GetComponent<Rigidbody>();
        HellicopterAudio = GetComponent<AudioSource>();
    }

    public void SetPosition(Vector3 pos, Vector3 rotation)
    {
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(rotation);
    }

    public void PlayClip()
    {
        HellicopterAudio.PlayOneShot(HellicopterClip);
        HellicopterAudio.PlayOneShot(HellicopterEngineClip);
    }

    public bool CheckDistance(Vector3 playerPos)
    {
        if (0 <= Vector3.Distance(playerPos, transform.position))
            return true;
        else
            return false;
    }

    public void StopMove()
    {
        isMove = false;
        
        myrigidbody.velocity = Vector3.zero;
    }

    private void Update()
    {
        if(isMove)
            myrigidbody.velocity = moveSpeed * transform.forward;
        
    }
}
