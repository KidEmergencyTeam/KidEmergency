using System.Collections;
using UnityEngine;
using XRController = UnityEngine.XR.Interaction.Toolkit.XRController;

public class FixingBagAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    
    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(Fixing());
    }

    public void StartMultiModeAction()
    {
        _isComplete = false;        
    }

    private IEnumerator Fixing()
    {
        Bag bag = FindObjectOfType<Bag>();
        while (!_isComplete)
        {
            bag.BagInteraction();
            if (bag.IsProtect())
            {
                _isComplete = true;
            }
            
            yield return null;
        }
    }
}
