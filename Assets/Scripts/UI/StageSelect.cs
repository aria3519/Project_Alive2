using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelect : MonoBehaviour
{
    [SerializeField] private Image _npcImage;
    [SerializeField] private TextMeshProUGUI _npcText;
    
    private static StageSelect m_instance;
    public static StageSelect instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<StageSelect>();
            }

            return m_instance;
        }
    }




}
