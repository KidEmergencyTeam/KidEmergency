using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSYNPCController : MonoBehaviour
{
    public NpcRig[] NPCs;

    private void Awake()
    {
        NPCs = GetComponentsInChildren<NpcRig>();
    }

    public void SetNPCState(string st)
    {
        for (int i = 0; i < NPCs.Length; i++)
        {
            NPCs[i] = FindObjectOfType<NpcRig>();
				
            if (st == "None")
            {
                NPCs[i].state = NpcRig.State.None;
            }
			
            else if (st == "DownDesk")
            {
                NPCs[i].state = NpcRig.State.DownDesk;
            }
			
            else if (st == "HoldDesk")
            {
                NPCs[i].state = NpcRig.State.HoldDesk;
            }

            else if (st == "HoldBag")
            {
                NPCs[i].state = NpcRig.State.HoldBag;
            }
        }
    }
}
