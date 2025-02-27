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

    // XRGrabInteractable 컴포넌트 (수동으로 GameObject에 추가되어 있어야 합니다)
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        // XRGrabInteractable 컴포넌트를 가져옴 (자동 추가하지 않음)
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogError("MessageTrigger -> XRGrabInteractable 컴포넌트가 없습니다. GameObject에 수동으로 추가하세요.");
            return;
        }

        // 이벤트 등록
        grabInteractable.selectEntered.AddListener(OnObjectGrabbed);
        grabInteractable.selectExited.AddListener(OnObjectReleased);

        // 핸드 컨트롤러로 오브젝트를 잡아도 오브젝트가 움직이지 않도록 설정
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
        // 지연 시간 적용 취소 후 색상 복구
        CancelInvoke(nameof(ResetColor));
        ResetColor();

        // 지연 시간 적용 취소 후 패널 비활성화
        CancelInvoke(nameof(HideExplanation));
        HideExplanation();
    }

    // 현재 모드에 따라 메시지 출력 및 패널 활성화 처리
    private void ShowMessageByMode()
    {
        // 현재 모드 값을 인덱스로 변환
        int index = (int)currentMode;

        // 리스트가 유효하고, 해당 인덱스가 범위 내에 있는지 확인
        if (interactionSettings != null && interactionSettings.Count > index)
        {
            InteractionSetting setting = interactionSettings[index];

            if (setting.explanationPanel != null && setting.explanationText != null)
            {
                // 메시지 내용 적용 및 패널 활성화
                setting.explanationText.text = setting.message;
                setting.explanationPanel.SetActive(true);

                // 지연 시간 이후 패널 비활성화
                Invoke(nameof(HideExplanation), setting.delayTime);
            }
        }
    }

    // 현재 모드에 따라 오브젝트의 색상 변경 처리
    private void ChangeObjectColorByMode()
    {
        int index = (int)currentMode;
        if (objRenderer != null && interactionSettings != null && interactionSettings.Count > index)
        {
            InteractionSetting setting = interactionSettings[index];
            // 설정된 색상으로 변경
            objRenderer.material.color = setting.highlightColor;

            // 지연 시간 이후 원래 색상으로 복구
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
