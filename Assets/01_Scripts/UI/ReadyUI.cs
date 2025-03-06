using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ReadyUI : MonoBehaviour
{
    public int currentPeople;
    private int _maxPeople = 8;

    public Image cirlceImage;
    public GameObject checkImage;
    public TextMeshProUGUI peopleText;

    private void Start()
    {
        // if(싱글) peopletext 비활성화
    }

    private void Update()
    {
        peopleText.text = currentPeople + " / " + _maxPeople;
        SetPeopleReady();
}

    private void SetPeopleReady()
    {
        // if(멀티) if(플레이어가 준비 중이면) 컬러 변화 및 체크 이미지 활성화
        // if(싱글) if(준비를 누르면) 컬러 변화 및 체크 이미지 활성화
        
        // 테스트용 키임
        if (Input.GetKeyDown(KeyCode.F)) // 사람이 들어오면
        {
            if (currentPeople < _maxPeople)
            {
                currentPeople++;
            }

            else return;
        }

        if (Input.GetKeyDown(KeyCode.G)) // 준비 버튼(각 그립 버튼)을 눌렀을 때
        {
            cirlceImage.color = Color.green;
            checkImage.SetActive(true);
        }
    }
}
