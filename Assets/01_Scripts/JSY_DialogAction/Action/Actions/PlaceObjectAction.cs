using UnityEngine;

public class PlaceObjectAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    
    public void StartAction()
    {
        _isComplete = false;
        SetObjects(ActionManager.Instance.currentDialog);
        _isComplete = true;
    }

    private void SetObjects(DialogData dialogData)
    {
        if (dialogData.objects != null)
        {
            foreach (GameObject prefab in dialogData.objects)
            {
                if (prefab != null)
                {
                    Instantiate(prefab);
                }
            }
        }
    }
}
