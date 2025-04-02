using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class ChangeViewAction : MonoBehaviour, IActionEffect
{
    public GameObject player;
    private bool _isComplete = false;
    private Vector3 _originPos;
    public bool IsActionComplete => _isComplete;

    private void Start()
    {
        _originPos = player.transform.localPosition;
    }

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(SetView(ActionManager.Instance.beforeDialog.changePos));
    }
    
    
    private IEnumerator SetView(Vector3 newPos)
    {
        yield return StartCoroutine(FadeInOut.Instance.FadeOut());

        while (player.transform.localPosition != newPos)
        {
            JSYNPCController npcCtrl = FindObjectOfType<JSYNPCController>();
            PlayerRig playerState = FindObjectOfType<PlayerRig>();
            if (newPos == _originPos)
            {
                player.transform.localPosition = newPos;
                npcCtrl.SetNPCState("None");
                playerState.currentState = PlayerRig.State.None;
            }
            
            else if (newPos != _originPos)
            {
                player.transform.localPosition = newPos;
                npcCtrl.SetNPCState("DownDesk");
                playerState.currentState = PlayerRig.State.Down;
            }
            
            yield return StartCoroutine(FadeInOut.Instance.FadeIn());
            _isComplete = true;
        }
    }
    
}
