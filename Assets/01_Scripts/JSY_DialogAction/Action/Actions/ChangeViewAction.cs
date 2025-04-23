using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        StartCoroutine(SetView(ActionManager.Instance.beforeDialog.changePos, ActionManager.Instance.beforeDialog.changeRot));
    }
    
    
    private IEnumerator SetView(Vector3 newPos, Vector3 newRot)
    {
        yield return StartCoroutine(OVRScreenFade.Instance.Fade(0f, 1f));

        while (!_isComplete)
        {
            RobotController seti = FindObjectOfType<RobotController>();
            JSYNPCController npcCtrl = FindObjectOfType<JSYNPCController>();
            
            if (SceneManager.GetActiveScene().name == "Eq_School_1")
            {
                if (newPos == _originPos)
                {
                    SetNewView(newPos, newRot, PlayerRig.State.None);
                    UIManager.Instance.DialogPosReset(0);
                    // UIManager.Instance.WarningPosReset(1);
                    seti.SetRobotPos(seti.setiPos[0]);
                    
                    GameObject bag = GameObject.Find("Bags");
                    bag.transform.GetChild(0).gameObject.SetActive(true);
                    
                    if (npcCtrl != null)
                    {
                        npcCtrl.SetNPCState("None");
                    }
                }
                
                            
                else if (newPos != _originPos && (ActionManager.Instance.beforeDialog.name == "School4_Dialog" || ActionManager.Instance.beforeDialog.name == "School3_Dialog"))
                {
                    // 지진 학교 - 책상 다리로 시점 변경
                    SetNewView(newPos, newRot, PlayerRig.State.Down);
                    UIManager.Instance.DialogPosReset(1);
                    // UIManager.Instance.WarningPosReset(0);
                    seti.SetRobotPos(seti.setiPos[1]);
                    if (npcCtrl != null)
                    {
                        npcCtrl.SetNPCState("DownDesk");
                    }
                }
            }

            if (SceneManager.GetActiveScene().name == "Eq_Home_1")
            {
                 if (newPos == _originPos) 
                 { 
                     SetNewView(newPos, newRot, PlayerRig.State.None); 
                     UIManager.Instance.DialogPosReset(0); 
                     // UIManager.Instance.WarningPosReset(1); 
                     seti.SetRobotPos(seti.setiPos[0]); 
                 }
                 
                 else if (newPos != _originPos && ActionManager.Instance.beforeDialog.name == "EqHome5_Dialog") 
                 { 
                     // 지진 집 - 책상 다리로 시점 변경
                     SetNewView(newPos, newRot, PlayerRig.State.Down); 
                     UIManager.Instance.DialogPosReset(1); 
                     // UIManager.Instance.WarningPosReset(0); 
                     seti.SetRobotPos(seti.setiPos[1]); 
                 }
                 
                 else if (newPos != _originPos && ActionManager.Instance.beforeDialog.name == "EqHome7_Dialog") 
                 { 
                     // 지진 집 - 가스 밸브 앞으로 시점 변경
                     SetNewView(newPos, newRot, PlayerRig.State.None); 
                     UIManager.Instance.DialogPosReset(2); 
                     seti.SetRobotPos(seti.setiPos[2]); 
                 }
                 
                 else if (newPos != _originPos && ActionManager.Instance.beforeDialog.name == "EqHome9_Dialog") 
                 { 
                     // 지진 집 - 전기 차단기 앞으로 시점 변경
                     SetNewView(newPos, newRot, PlayerRig.State.None);
                     UIManager.Instance.DialogPosReset(3);
                     seti.SetRobotPos(seti.setiPos[3]);
                 }
            }

            if(SceneManager.GetActiveScene().name == "Eq_Home_2")
            {
                if (player == null)
                {
                    player = GameObject.Find("VR + Player");
                    SetNewView(newPos, newRot, PlayerRig.State.None);
                    UIManager.Instance.DialogPosReset(5);
                    // UIManager.Instance.WarningPosReset(3); 
                    seti.SetRobotPos(seti.setiPos[1]);
                }
                else
                {
                    SetNewView(newPos, newRot, PlayerRig.State.None);
                    // UIManager.Instance.WarningPosReset(3); 
                    seti.SetRobotPos(seti.setiPos[1]);
                }
            }
            
            yield return StartCoroutine(OVRScreenFade.Instance.Fade(1f, 0f));
            _isComplete = true;
        }
    }

    private void SetNewView(Vector3 pos, Vector3 rot, PlayerRig.State state)
    {
        PlayerRig playerState = FindObjectOfType<PlayerRig>();
        player.transform.localPosition = pos;
        player.transform.localRotation = Quaternion.Euler(rot);
        playerState.currentState = state;
    }
}
