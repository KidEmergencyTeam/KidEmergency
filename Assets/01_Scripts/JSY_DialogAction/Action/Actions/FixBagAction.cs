using System.Collections;
using UnityEngine;

public class FixBagAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    
    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(Fixing());
    }
    
    private IEnumerator Fixing()
    {
        Bag bag = FindObjectOfType<Bag>();
        JSYNPCController npcCtrl = FindObjectOfType<JSYNPCController>();
        if(npcCtrl != null)
        {
            npcCtrl.SetNPCState("HoldBag");   
        }
        
        bag.isGrabbable = true;
        bag.BagInteraction();

        while (!_isComplete)
        {
            if (bag.IsProtect())
            {
                _isComplete = true;
            }
            
            yield return null;
        }
    }
}
