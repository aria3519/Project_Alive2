using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SceneList", menuName = "Scene Management/Scene List")]
public class SceneList : ScriptableObject
{
    public List<string> scenes = new List<string>();
}
