using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelliRoter : MonoBehaviour
{
    [SerializeField] private GameObject hellicopterRoterMain;
    [SerializeField] private GameObject hellicopterRoterBack;

    private float rotateSpeed = 600f;

    void FixedUpdate()
    {
        hellicopterRoterMain.transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        hellicopterRoterBack.transform.Rotate(new Vector3(rotateSpeed * Time.deltaTime, 0, 0));
    }
}
