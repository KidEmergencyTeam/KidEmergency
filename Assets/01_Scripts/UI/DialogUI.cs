using TMPro;
using UnityEngine;

public class DialogUI : MonoBehaviour
{
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    
    private void Awake()
    {
        dialogPanel.SetActive(false);
    }
    
}
