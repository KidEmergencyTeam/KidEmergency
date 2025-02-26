using UnityEngine;

public class ButtonClickSound : MonoBehaviour
{
    public void OnMouseDown()
    {
        SoundManager.Instance.PlaySFX("Test_2", gameObject);
    }
}
