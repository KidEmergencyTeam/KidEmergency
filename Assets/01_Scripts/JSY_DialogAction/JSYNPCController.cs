using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class JSYNPCController : MonoBehaviour
{
    public NpcRig[] npcs;
    private Vector3[] _originPos;
    private void Start()
    {
        for (int i = 0; i < npcs.Length; i++)
        {
           _originPos[i] = npcs[i].transform.position;
        }
        
        if (SceneManager.GetActiveScene().name == "JSY" ||
            SceneManager.GetActiveScene().name == "JSY_SchoolGround")
        {
            SetNPCState("None");
        } 
        else if (SceneManager.GetActiveScene().name == "JSY_SchoolHall" ||
              SceneManager.GetActiveScene().name == "JSY_StairEv") 
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
                npcs[i].transform.position = _originPos[i];
            }
			
            else if (st == "DownDesk")
            {
                npcs[i].state = NpcRig.State.DownDesk;
                Vector3 changePos = npcs[i].transform.position;
                changePos.z += 0.4f;
                npcs[i].transform.position = changePos;
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
