using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneGroup", menuName = "SceneGroup")]
public class SceneGroupSO : ScriptableObject
{
    public string category = "";
    public SceneSO[] scenes;
}
