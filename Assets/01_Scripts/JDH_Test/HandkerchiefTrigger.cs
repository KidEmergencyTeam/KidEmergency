using UnityEngine;

public class HandkerchiefTrigger : MonoBehaviour
{
    // FireBeginner 스크립트를 Inspector에서 할당합니다.
    public FireBeginner fireBeginner;

    // 손수건 오브젝트에 "Handkerchief" 태그가 설정되어 있어야 합니다.
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 손수건인지 확인
        if (other.gameObject.CompareTag("Handker"))
        {
            // FireBeginner의 손수건 획득 여부를 true로 변경
            if (fireBeginner != null)
            {
                fireBeginner.hasHandkerchief = true;
                Debug.Log("손수건 획득: hasHandkerchief가 true로 설정되었습니다.");
            }
            else
            {
                Debug.LogWarning("FireBeginner 스크립트가 할당되지 않았습니다.");
            }
        }
    }
}
