using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjectAction : MonoBehaviour, IActionEffect
{
    public GameObject bag;
    public GameObject pencilCase;
    public GameObject textbook;
    
    private bool isComplete = false;
    public bool IsActionComplete => isComplete;
    public void StartAction()
    {
       SetObject();
       isComplete = false;
    }

    public void StopAction()
    {
        isComplete = true;
    }


    private void SetObject()
    {
        Instantiate(bag);
        Instantiate(pencilCase);
        Instantiate(textbook);
    }
}
