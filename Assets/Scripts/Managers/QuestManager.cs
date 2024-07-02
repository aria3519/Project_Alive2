using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager m_instance;
    public static QuestManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<QuestManager>();
            }

            return m_instance;
        }
    }
}
