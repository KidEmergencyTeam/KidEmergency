using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

// 모드를 열거형으로 나타냄
public enum InteractionMode
{
    화재모드_초급,
    화재모드_고급
}

// 메시지, 지연 시간, 색상, 패널 및 텍스트를 묶어 관리
[Serializable]
public class InteractionSetting
{
    [Header("메시지, 지연 시간, 적용 색상")]
    [TextArea(3, 10)]
    public string message;         
    public int delayTime;       
    public Color highlightColor;  

    [Header("패널 및 텍스트")]
    public GameObject explanationPanel;    
    public TextMeshProUGUI explanationText; 
}

public class MessageTrigger : MonoBehaviour
{
    [Header("모드별 설정")]
    public List<InteractionSetting> interactionSettings;

    // 현재 모드 (기본값은 화재모드_초급)
    public InteractionMode currentMode = InteractionMode.화재모드_초급;

    // 렌더러 컴포넌트 및 원래 색상
    private Renderer objRenderer;
    private Color originalColor;

    // XRGrabInteractable 컴포넌트
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        // XRGrabInteractable 컴포넌트를 가져오거나 없으면 추가
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
            Debug.Log("MessageTrigger -> XRGrabInteractable 컴포넌트가 자동으로 추가되었습니다.");
        }

        // 이벤트 등록
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnObjectGrabbed);
            grabInteractable.selectExited.AddListener(OnObjectReleased);
        }

        // 핸드 컨트롤러로 오브젝트를 잡아도
        // 오브젝트가 움직이지 않도록 설정
        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;
        grabInteractable.throwOnDetach = false;

    }

    private void OnDestroy()
    {
        // 이벤트 해제
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnObjectGrabbed);
            grabInteractable.selectExited.RemoveListener(OnObjectReleased);
        }
    }

    private void Start()
    {
        // 렌더러 컴포넌트 가져오기 및 원래 색상 저장
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            originalColor = objRenderer.material.color;
        }

        // 패널 비활성화
        HideExplanation();
    }

    // 오브젝트를 잡았을 때 
    private void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        // 메시지 출력
        ShowMessageByMode();

        // 색상 변경
        ChangeObjectColorByMode();
    }

    // 오브젝트를 놓았을 때 
    private void OnObjectReleased(SelectExitEventArgs args)
    {
        // 지연 시간 적용x
        CancelInvoke(nameof(ResetColor));
        // 색상 복구
        ResetColor();

        // 지연 시간 적용x
        CancelInvoke(nameof(HideExplanation));
        // 패널 비활성화
        HideExplanation();
    }

    // 현재 모드에 따라 메시지 출력 및 패널 활성화 처리
    private void ShowMessageByMode()
    {
        // 열거형(currentMode) 값을 정수형 인덱스(index) 로 변환하는 과정
        // 목적:  리스트 내에서 해당 모드에 해당하는 요소의 위치를 찾기 위함
        int index = (int)currentMode;

        // 리스트가 유효성 및
        // 현재 선택한 모드가 리스트에 있는지 확인
        if (interactionSettings != null && interactionSettings.Count > index)
        {
            // 현재 모드의 설정 값 가져오기
            InteractionSetting setting = interactionSettings[index];

            // 리스트가 유효하고 인덱스 범위 내인지 확인
            if (setting.explanationPanel != null && setting.explanationText != null)
            {
                // 메시지 내용 적용
                setting.explanationText.text = setting.message;

                // 패널 활성화
                setting.explanationPanel.SetActive(true);

                // 지연 시간 이후 패널 비활성화
                Invoke(nameof(HideExplanation), setting.delayTime);
            }
        }
    }

    // 현재 모드에 따라 오브젝트의 색상 변경 처리
    private void ChangeObjectColorByMode()
    {
        // 열거형(currentMode) 값을 정수형 인덱스(index) 로 변환하는 과정
        // 목적:  리스트 내에서 해당 모드에 해당하는 요소의 위치를 찾기 위함
        int index = (int)currentMode;

        // 렌더러와 리스트가 유효성 및
        // 현재 선택한 모드가 리스트에 있는지 확인
        if (objRenderer != null && interactionSettings != null && interactionSettings.Count > index)
        {
            // 현재 모드의 설정 값 가져오기
            InteractionSetting setting = interactionSettings[index];

            // 색상 변경
            objRenderer.material.color = setting.highlightColor;

            // 지연 시간 이후 색상 복구
            Invoke(nameof(ResetColor), setting.delayTime);
        }
    }

    // 현재 모드의 패널을 비활성화 처리
    private void HideExplanation()
    {
        int index = (int)currentMode;

        if (interactionSettings != null && interactionSettings.Count > index)
        {
            InteractionSetting setting = interactionSettings[index];
            if (setting.explanationPanel != null)
            {
                setting.explanationPanel.SetActive(false);
            }
        }
    }

    // 오브젝트의 색상을 원래대로 복구
    private void ResetColor()
    {
        if (objRenderer != null)
        {
            objRenderer.material.color = originalColor;
        }
    }
}
