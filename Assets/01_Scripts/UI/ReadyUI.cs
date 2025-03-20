using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ReadyUI : MonoBehaviour
{
    public Image cirlceImage;
    public GameObject checkImage;
    private string _nextScene;
    private bool _isReady = false;
    [SerializeField] private ActionBasedController _leftCtrl;
    [SerializeField] private ActionBasedController _rightCtrl;

    private void Start()
    {
        StartCoroutine(FadeInOut.Instance.FadeIn());
    }

    private void Update()
    {
        SetPeopleReady();
    }

    private void SetPeopleReady()
    {
        if (_leftCtrl.selectAction.action.ReadValue<float>() > 0 && _rightCtrl.selectAction.action.ReadValue<float>() > 0) // 준비 버튼(각 그립 버튼)을 눌렀을 때
        {
            cirlceImage.color = Color.green;
            checkImage.SetActive(true);

            _isReady = true;
            StartCoroutine(WaitForGameStart());
        }
    }

    private IEnumerator WaitForGameStart()
    {
        while (_isReady)
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(_nextScene);
        }
    }
}
