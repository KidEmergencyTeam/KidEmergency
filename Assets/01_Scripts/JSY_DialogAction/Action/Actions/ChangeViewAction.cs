using System.Collections;
using UnityEngine;

public class ChangeViewAction : MonoBehaviour, IActionEffect
{
    public GameObject camOffset;
    private bool _isComplete = false;
    private Vector3 _originPos;
    public bool IsActionComplete => _isComplete;

    private void Start()
    {
        _originPos = camOffset.transform.localPosition;
    }

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(SetView(ActionManager.Instance.beforeDialog.changePos));
    }
    
    
    private IEnumerator SetView(Vector3 newPos)
    {
        yield return StartCoroutine(FadeInOut.Instance.FadeOut());

        while (camOffset.transform.localPosition != newPos)
        {
            JSYNPCController npcCtrl = FindObjectOfType<JSYNPCController>();
            
            if (newPos == _originPos)
            {
                camOffset.transform.localPosition = newPos;
                npcCtrl.SetNPCState("None");
            }
            
            else if (newPos != _originPos)
            {
                camOffset.transform.localPosition = newPos;
                npcCtrl.SetNPCState("DownDesk");
            }
            
            yield return StartCoroutine(FadeInOut.Instance.FadeIn());
            _isComplete = true;
        }
    }
    
}
