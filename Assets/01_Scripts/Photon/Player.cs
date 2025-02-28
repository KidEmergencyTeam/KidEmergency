using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Networked] public int Token {get;set;}
}
