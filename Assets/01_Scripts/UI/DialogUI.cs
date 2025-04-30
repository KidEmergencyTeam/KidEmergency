using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public Image colorImage;

    private void Awake()
    {
        dialogPanel.SetActive(false);
    }
    

    private void Update()
    {
        SetColorSameRobot();
    }

    private void SetColorSameRobot()
    {
        Rob11ColorManager seti = FindObjectOfType<Rob11ColorManager>();
        colorImage.color = seti.predefinedColors[seti.colorIndex];
    }
    
}