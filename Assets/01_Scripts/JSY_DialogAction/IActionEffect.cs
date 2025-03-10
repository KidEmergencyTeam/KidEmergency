using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionEffect
{
    void StartAction();
    bool IsActionComplete { get; }
}
