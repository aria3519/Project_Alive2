using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*class charInfor
{
    public string playerName;
    public string CharacterID;
    public string Level;
    public string attack;
    public string health;
    public int i;
    public float f;
    

    public charInfor()
    {
      
        
    }
}

public class XMLtest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<charInfor> t = new List<charInfor>();
        t.Add(new charInfor());
        XML<charInfor>.Write("charInfor", t);
    }
}
*/

/*public struct CharacterInfo
{
    public string name;
    public  float level;
    public float exp;
    public float health;
}*/
public class XMLtest : MonoBehaviour
{
    /*void Start()
    {
        CreateXml();
    }*/

  /*  public XmlDocument CreateXml()
    {
        //string chname,float chhealth
        XmlDocument xmlDoc = new XmlDocument();
        // Xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다.)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
        return xmlDoc; 
    }
    public XmlNode CreatRoot(XmlDocument xmlDoc,string filename)
    {
        // 루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, filename, string.Empty);
        xmlDoc.AppendChild(root);
        return root;
    }*/

    /*public void XmlSave(XmlDocument xmlDoc, XmlNode root, CharacterInfo charname )
    {
        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Character", string.Empty);
        root.AppendChild(child);

        // 자식 노드에 들어갈 속성 생성
        XmlElement name = xmlDoc.CreateElement("Name");
        name.InnerText = charname.name;
        child.AppendChild(name);

        XmlElement lv1 = xmlDoc.CreateElement("Level");
        lv1.InnerText = charname.level.ToString("F1");
        child.AppendChild(lv1);

        XmlElement exp = xmlDoc.CreateElement("Experience");
        exp.InnerText = charname.exp.ToString("F1"); ;
        child.AppendChild(exp);

        XmlElement health = xmlDoc.CreateElement("health");
        health.InnerText = charname.health.ToString("F1");
        child.AppendChild(health);
        // 속성값 넣기 

        xmlDoc.Save("./Assets/Resources/Character.xml");
    }*/
}



