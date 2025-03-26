using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSYPlayerController : MonoBehaviour
{
    private PlayerState state;

    private void Awake()
    {
        state = GetComponent<PlayerState>();
    }

    private void Start()
    {
        state = PlayerState.None;
    }
}
