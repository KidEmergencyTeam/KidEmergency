using UnityEngine;
using UnityEngine.SceneManagement;

public class JSYNPCController : MonoBehaviour
{
    public NpcRig[] npcs;
    private Vector3[] _originPos;
    private Vector3[] _originScale;
    
    private void Start()
    {
        _originPos = new Vector3[npcs.Length];
        _originScale = new Vector3[npcs.Length];
        for (int i = 0; i < npcs.Length; i++)
        {
            _originPos[i] = npcs[i].transform.localPosition;
           _originScale[i] = npcs[i].transform.localScale;
        }
        
        if (SceneManager.GetActiveScene().name == "Eq_School_1" ||
            SceneManager.GetActiveScene().name == "Eq_School_4")
        {
            SetNPCState("None");
        } 
        
        else if (SceneManager.GetActiveScene().name == "Eq_School_2" ||
              SceneManager.GetActiveScene().name == "Eq_School_3") 
        {
            SetNPCState("HoldBag");
        }
    }
    
    public void SetNPCState(string st)
    {
        for (int i = 0; i < npcs.Length; i++)
        {
            if (st == "None")
            {
                npcs[i].state = NpcRig.State.None;
                npcs[i].transform.localPosition = _originPos[i];
                npcs[i].transform.localScale = _originScale[i];
            }
			
            else if (st == "DownDesk")
            {
                npcs[i].state = NpcRig.State.DownDesk;
                Vector3 changePos = npcs[i].transform.localPosition;
                changePos.z += 0.4f;
                npcs[i].transform.localPosition = changePos;
                npcs[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
			
            else if (st == "HoldDesk")
            {
                npcs[i].state = NpcRig.State.HoldDesk;
            }

            else if (st == "HoldBag")
            {
                npcs[i].state = NpcRig.State.HoldBag;
            }
        }
    }
}
