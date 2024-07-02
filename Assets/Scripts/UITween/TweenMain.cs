using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenMain : MonoBehaviour
{
    private void Start()
    {
        transform.DOMove(Vector3.up, 5); // vec3, tweening time

        Material mat = GetComponent<MeshRenderer>().material;

        mat.DOColor(Color.cyan, 5);
    }
}
