using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Portal Gothere;
    private bool isused;

    //[SerializeField] private LivingEntity Player;

    // Start is called before the first frame update
    /* void Start()
     {

     }*/

    // Update is called once per frame
    /* void Update()
     {
         if (Input.GetKeyDown("e"))
         {
             //eŰ ������ �Ա����� �ⱸ�� ����

         }
     }*/

    private void OnTriggerStay(Collider other)
    {
        isused = false;

        if (other.tag == "Player" && isused!=true && Input.GetKeyDown("e"))
        {
            isused = true;
            Gothere.isused = true;
            other.transform.position = Gothere.transform.position;
            StartCoroutine(UpdatePath());
        }
    }

    private IEnumerator UpdatePath()
    {
        // 3�еڿ� Ŭ���� �ڽ� ���� 
        yield return new WaitForSeconds(3f);
        isused = false;
        
    }
       
}
