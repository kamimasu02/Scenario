using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene", menuName = "Scene")]
public class SceneSO : ScriptableObject 
{
    public int id = -1;
    public string title = "";
    public string path = "";
}
