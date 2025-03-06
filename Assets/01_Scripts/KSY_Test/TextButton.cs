using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // IPointerEnterHandler, IPointerExitHandler 사용
using TMPro;
using System.Collections;

public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI displayText;
    public Button Button; // 인스펙터에서 할당한 버튼

    // 원래 텍스트를 저장할 변수
    private string originalText;
    // 텍스트 초기화 코루틴을 저장할 변수 (이미 실행중인지 확인하기 위함)
    private Coroutine resetCoroutine;

    void Start()
    {
        // 시작 시점에 Button, displayText가 잘 할당되었는지 로그 확인
        if (Button != null)
        {
            Debug.Log("[TextButton] Button이 정상 할당되었습니다: " + Button.name);
        }
        else
        {
            Debug.LogError("[TextButton] Button이 할당되지 않았습니다. Inspector에서 Button 필드를 확인하세요.");
        }

        if (displayText != null)
        {
            Debug.Log("[TextButton] displayText가 정상 할당되었습니다: " + displayText.name);
            // displayText의 원래 텍스트를 저장
            originalText = displayText.text;
        }
        else
        {
            Debug.LogError("[TextButton] displayText가 할당되지 않았습니다. Inspector에서 TextMeshProUGUI 필드를 확인하세요.");
        }
    }

    // PointerEnter 이벤트 발생 시 (레이/포인터가 UI 영역 위로 들어올 때)
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("[TextButton] OnPointerEnter 호출됨! pointerEnter = " + eventData.pointerEnter);

        // 인스펙터에서 할당한 버튼과 동일한 오브젝트인지 확인
        if (Button != null && eventData.pointerEnter == Button.gameObject)
        {
            Debug.Log("[TextButton] pointerEnter가 할당된 Button과 일치합니다. OnButtonClick() 호출 시작.");
            OnButtonClick();

            // 포인터가 버튼 위에 들어오면, 만약 실행중인 텍스트 초기화 코루틴이 있다면 중지함
            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
                resetCoroutine = null;
            }
        }
        else
        {
            if (Button == null)
            {
                Debug.LogWarning("[TextButton] Button이 null입니다. Inspector에서 Button 필드를 확인하세요.");
            }
            else
            {
                Debug.Log("[TextButton] pointerEnter(" + eventData.pointerEnter + ")가 할당된 Button("
                          + Button.gameObject + ")과 다릅니다. 이벤트 무시.");
            }
        }
    }

    // PointerExit 이벤트 발생 시 (레이/포인터가 UI 영역에서 벗어날 때)
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("[TextButton] OnPointerExit 호출됨! pointerExit = " + eventData.pointerEnter);

        // 포인터가 버튼 영역을 벗어나면 2초 후에 원래 텍스트로 초기화하는 코루틴을 시작
        if (displayText != null)
        {
            // 이미 실행중인 코루틴이 있다면 중지
            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
            resetCoroutine = StartCoroutine(ResetTextCoroutine());
        }
    }

    // 버튼(또는 PointerEnter)로 인해 실행되는 메서드
    void OnButtonClick()
    {
        Debug.Log("[TextButton] OnButtonClick() 메서드가 호출되었습니다.");

        if (displayText != null)
        {
            displayText.text = "버튼 클릭 처리 완료!";
            Debug.Log("[TextButton] displayText에 문구를 업데이트했습니다: " + displayText.text);
        }
        else
        {
            Debug.LogError("[TextButton] displayText가 null입니다. 텍스트 변경 불가.");
        }
    }

    // 2초 후에 텍스트를 원래대로 초기화하는 코루틴
    IEnumerator ResetTextCoroutine()
    {
        yield return new WaitForSeconds(2f);
        if (displayText != null)
        {
            displayText.text = originalText;
            Debug.Log("[TextButton] displayText가 원래 텍스트로 초기화되었습니다: " + originalText);
        }
        resetCoroutine = null;
    }
}
