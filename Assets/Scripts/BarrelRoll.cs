using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRoll : MonoBehaviour
{
    [SerializeField] GameObject minigunBarrel;
    private float rotateSpeed = 200f;
    void FixedUpdate()
    {
        minigunBarrel.transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));
    }
}
