using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PortalData", menuName = "Scriptable Object/PortalData", order = int.MaxValue)]
public class PortalData : ScriptableObject
{
    [SerializeField] Portal[] portalList;

    public Portal[] PortalList {  get { return portalList; } }

    
}
