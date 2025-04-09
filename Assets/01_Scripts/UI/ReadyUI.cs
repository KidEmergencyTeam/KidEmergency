using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ReadyUI : MonoBehaviour
{
    public Image cirlceImage;
    public GameObject checkImage;
    public string nextScene;
    private bool _isReady = false;
    private bool _isLoading = false;
    private ActionBasedController _leftCtrl;
    private ActionBasedController _rightCtrl;

    private void Awake()
    {
        _leftCtrl = GameObject.Find("Left Controller").GetComponent<ActionBasedController>();
        _rightCtrl = GameObject.Find("Right Controller").GetComponent<ActionBasedController>();
    }

    private void Update()
    {
        SetPeopleReady();
    }

    private void SetPeopleReady()
    {
        // 준비 버튼(각 그립 버튼)을 눌렀을 때 / 테스트 용으로 오른쪽 버튼만 클릭
        if (_leftCtrl.selectAction.action.ReadValue<float>() >= 1 &&  _rightCtrl.selectAction.action.ReadValue<float>() >= 1) 
        {
            if (!_isReady)
            {
                print("준비 완료");
                cirlceImage.color = Color.green;
                checkImage.SetActive(true);

                _isReady = true;
                StartCoroutine(WaitForGameStart());
            }
        }
    }

    private IEnumerator WaitForGameStart()
    {
        while (!_isLoading && _isReady)
        {
            _isLoading = true;
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(OVRScreenFade.Instance.Fade(0, 1));
            
            AsyncOperation asyncChange = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
        
            while(!asyncChange.isDone)
            {
                yield return null;
            }
        }
    }
}
