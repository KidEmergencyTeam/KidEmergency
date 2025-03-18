using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// 어떤 이유로 컨트롤러와 오브젝트 간 상호 작용이 풀렸는지 디버그로 확인하는 스크립트
[RequireComponent(typeof(XRGrabInteractable))]
public class GrabStateChecker : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    // 여러 인터랙터가 있을 수 있으므로, 각 인터랙터별로 기록할 입력 상태 구조체
    private struct DeviceState
    {
        public bool triggerPressed;
        public bool gripPressed;
    }

    // 인터랙터별 이전 입력 상태 저장 (OnGrabbed에서 기록)
    private Dictionary<IXRInteractor, DeviceState> previousDeviceStates = new Dictionary<IXRInteractor, DeviceState>();

    // 오브젝트가 잡혔는지 여부
    public bool IsGrabbed { get; private set; }

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // 이벤트 구독
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    // 오브젝트가 잡혔을 때 호출
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        IsGrabbed = true;
        string interactorName = args.interactorObject.transform.name;
        Debug.Log($"{gameObject.name} : 잡혔습니다. (인터랙터: {interactorName})");

        // XRBaseControllerInteractor를 통한 입력 상태 기록
        XRBaseControllerInteractor controllerInteractor = args.interactorObject as XRBaseControllerInteractor;
        if (controllerInteractor != null && controllerInteractor.xrController is XRController xrController)
        {
            InputDevice device = xrController.inputDevice;
            DeviceState state = new DeviceState();
            device.TryGetFeatureValue(CommonUsages.triggerButton, out state.triggerPressed);
            device.TryGetFeatureValue(CommonUsages.gripButton, out state.gripPressed);

            // 현재 인터랙터에 대한 입력 상태 기록
            previousDeviceStates[args.interactorObject] = state;
        }
    }

    // 오브젝트가 놓였을 때 호출
    private void OnReleased(SelectExitEventArgs args)
    {
        IsGrabbed = false;

        string interactorName = args.interactorObject.transform.name;
        string interactorType = args.interactorObject.GetType().Name;
        string reason = "알 수 없는 이유";

        // 기본적으로 XRRayInteractor 체크
        XRRayInteractor rayInteractor = args.interactorObject as XRRayInteractor;
        if (rayInteractor != null)
        {
            reason = rayInteractor.isSelectActive ? "선택 상태 유지 중" : "선택 버튼 해제됨";
        }

        // XRBaseControllerInteractor를 통해 이전 상태와 현재 상태를 비교
        XRBaseControllerInteractor controllerInteractor = args.interactorObject as XRBaseControllerInteractor;
        if (controllerInteractor != null && controllerInteractor.xrController is XRController xrController)
        {
            InputDevice device = xrController.inputDevice;
            bool currentTrigger = false;
            bool currentGrip = false;
            device.TryGetFeatureValue(CommonUsages.triggerButton, out currentTrigger);
            device.TryGetFeatureValue(CommonUsages.gripButton, out currentGrip);

            // 이전 상태가 기록되어 있다면 비교
            if (previousDeviceStates.TryGetValue(args.interactorObject, out DeviceState prevState))
            {
                List<string> details = new List<string>();

                // 트리거 버튼 비교
                if (prevState.triggerPressed && !currentTrigger)
                {
                    details.Add("트리거 버튼 (사용자 입력에 의해 풀림)");
                }
                else if (prevState.triggerPressed && currentTrigger)
                {
                    details.Add("트리거 버튼 (코드 흐름상 해제됨)");
                }
                else
                {
                    details.Add("트리거 버튼 상태 변화 없음");
                }

                // 그립 버튼 비교
                if (prevState.gripPressed && !currentGrip)
                {
                    details.Add("그립 버튼 (사용자 입력에 의해 풀림)");
                }
                else if (prevState.gripPressed && currentGrip)
                {
                    details.Add("그립 버튼 (코드 흐름상 해제됨)");
                }
                else
                {
                    details.Add("그립 버튼 상태 변화 없음");
                }

                reason += " - " + string.Join(", ", details);
                // 기록 삭제
                previousDeviceStates.Remove(args.interactorObject);
            }
        }

        Debug.Log($"{gameObject.name} : 놓였습니다. (인터랙터: {interactorName}, 타입: {interactorType}, 이유: {reason})");
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}
