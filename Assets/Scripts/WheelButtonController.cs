using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelButtonController : MonoBehaviour
{
    public int Id;
    private Animator anim;
    public string charName;
    public TextMeshProUGUI charText;
    public Image selectedChar;
    private bool selected = false;
    public Sprite icon;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(selected)
        {
            selectedChar.sprite = icon;
            charText.text = charName;
        }
    }

    public void Selected()
    {
        selected = true;
        WheelController.charID = Id;
    }

    public void Deselected()
    {
        selected = false;
        WheelController.charID = 0;
    }

    public void HoverEnter()
    {
        anim.SetBool("Hover", true);
        charText.text = charName;
    }

    public void HoverExit()
    {
        anim.SetBool("Hover", false);
        charText.text = "";
    }
}
