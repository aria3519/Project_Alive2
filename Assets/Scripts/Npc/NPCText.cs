using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

// ui�Ŵ����� ���� ���� 
/*public class NPCText : MonoBehaviour
{
    // Start is called before the first frame update
    private TextAsset NpcContent; // �޸��� �޴°�
    private TextAsset NpcYes; // �޸��� �޴°�
    private TextAsset NpcNo; // �޸��� �޴°�
    [SerializeField] private Text text; // �޾Ƽ� text�� ���� 

    public event System.Action Yes;
    public event System.Action No;

    private string Npctext;
    StringReader reader;
    //IEnumerator enumerator;
    
    
    public bool isYes = false;


    //public Rigidbody rigidbody;
    //public Transform target;




    public void setText(TextAsset Content, TextAsset yes, TextAsset no)
    {
        //text.text = NpcName.text;
        NpcContent = Content;
        NpcYes = yes;
        NpcNo = no; 

        if (!isYes)
            StartCoroutine(talking());



    }

    // Update is called once per frame
    private IEnumerator talking()
    {
        reader = new StringReader(NpcContent.text);


        yield return readText();

        yield return select();
    }


    private IEnumerator select()
    {
        reader = null;
        while(null == reader)
        {
            if (Input.GetKeyDown("y"))
            {
                isYes = true;
                reader = new StringReader(NpcYes.text);
            }
            else if (Input.GetKeyDown("n"))
            {
                reader = new StringReader(NpcNo.text);
            }
            yield return null;
        }

        yield return readText();

        if (isYes) Yes?.Invoke();
        else No?.Invoke();
    }
    // Invoke("�Լ��̸�",3f) 3�� �ڿ� �Լ��� ����




    private IEnumerator readText()
    {
        while (true)
        {
            Npctext = reader.ReadLine();
            if (Npctext == null) break;
            text.text = Npctext;
            yield return new WaitForSeconds(2f);
        }
    }
}*/
