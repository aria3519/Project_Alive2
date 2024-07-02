using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelController : MonoBehaviour
{
    public Animator anim;
    private bool wheelSelected = false;
    public Image selectedChar;
    public Sprite noImage;
    public static int charID;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            wheelSelected = !wheelSelected;
        }

        if(wheelSelected)
        {
            anim.SetBool("OpenWheel", true);
        }
        else
        {
            anim.SetBool("OpenWheel", false);
        }

        switch (charID)
        {
            case 0:
                selectedChar.sprite = noImage;
                break;
            case 1:
                Debug.Log("char1");
                break;
            case 2:
                Debug.Log("char2");
                break;
            case 3:
                Debug.Log("char3");
                break;
        }
            
    }
}
