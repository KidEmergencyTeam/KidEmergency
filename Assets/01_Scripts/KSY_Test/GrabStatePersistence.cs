using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

// 씬 전환 시에도 손수건 유지해 주는 스크립트
public class GrabStatePersistence : DisableableSingleton<GrabStatePersistence>
{
    [Header("손수건")]
    public Grabbable grabbableComponent;

    // 이벤트를 사용한 이유: 씬이 로드될 때마다 자동으로 이벤트가 호출되어 처리되기 때문에,
    // 별도의 수동 호출 없이도 새로운 씬에서 필요한 작업이 자동으로 처리됨
    // 따라서 매 씬 마다 별도의 처리를 할 필요가 없다.

    // 이벤트 등록
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 씬 로드 후, Hand 태그를 가진 새로운 Left Controller 오브젝트에 추가된 Grabber 컴포넌트를 찾아
    // 손수건 오브젝트에 추가된 Grabbable 컴포넌트에서 currentGrabber를 갱신
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (grabbableComponent != null)
        {
            // Hand 태그를 가진 새로운 Left Controller 오브젝트 찾기
            GameObject handObj = GameObject.FindGameObjectWithTag("Hand");
            if (handObj == null)
            {
                Debug.LogWarning("Hand 태그의 Left Controller 오브젝트를 찾을 수 없습니다.");
                return;
            }

            // Hand 태그를 가진 새로운 Left Controller 오브젝트에서
            // Grabber 컴포넌트 가져오기
            Grabber newGrabber = handObj.GetComponent<Grabber>();
            if (newGrabber == null)
            {
                Debug.LogWarning("Hand 태그의 Left Controller 오브젝트에서 Grabber 컴포넌트를 찾을 수 없습니다.");
                return;
            }

            // 손수건 오브젝트에 추가된
            // Grabbable 컴포넌트에서 currentGrabber를 갱신하면
            // Grabbable 컴포넌트에서 손수건의 위치와 방향을
            // Grabber 컴포넌트가 추가된 오브젝트의 위치와 방향으로 처리
            grabbableComponent.currentGrabber = newGrabber;

            // grabbableComponent.isGrabbable가 false일 경우에만 Grab 실행
            if (!grabbableComponent.isGrabbable)
            {
                //newGrabber.OnGrab(grabbableComponent);
            }
        }
    }

    // 이벤트 제거
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

//public void OnGrab(Grabbable grabbable)
//{
//    // null 체크 후 초기화
//    if (_handAnimation == null)
//    {
//        _handAnimation = FindObjectOfType<HandAnimation>();
//    }
//    if (_targetFollower == null)
//    {
//        _targetFollower = FindObjectOfType<TargetFollower>();
//    }

//    print("OnGrab");
//    rayInteractor.enabled = false;
//    grabbable.rb.useGravity = false;
//    grabbable.rb.isKinematic = true;
//    grabbable.isGrabbable = false;
//    _handAnimation.enabled = false;
//    currentGrabbedObject = grabbable;
//    if (isLeft) _handAnimation.animator.SetFloat("Left Trigger", 1);
//    else _handAnimation.animator.SetFloat("Right Trigger", 1);

//    if (currentGrabbedObject.isMoving)
//    {
//        currentGrabbedObject.isGrabbable = false;
//        currentGrabbedObject.currentGrabber = this;
//    }

//    else
//    {
//        if (isLeft)
//        {
//            _targetFollower.followTargets[2].target =
//                currentGrabbedObject.transform;
//            _targetFollower.followTargets[2].posOffset =
//                currentGrabbedObject.grabPosOffset;
//            _targetFollower.followTargets[2].rotOffset =
//                currentGrabbedObject.grabRotOffset;
//        }
//        else
//        {
//            _targetFollower.followTargets[3].target =
//                currentGrabbedObject.transform;
//            _targetFollower.followTargets[3].posOffset =
//                currentGrabbedObject.grabPosOffset;
//            _targetFollower.followTargets[3].rotOffset =
//                currentGrabbedObject.grabRotOffset;
//        }
//    }
//}