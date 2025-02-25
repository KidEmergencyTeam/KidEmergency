using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogSystem")]
public class DialogData : ScriptableObject
{
     public int nodeID;
     [TextArea]
     public string dailogs;
     
}
