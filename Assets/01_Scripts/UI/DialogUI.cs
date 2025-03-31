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
        // dialogPanel.SetActive(false);
    }

    private void Update()
    {
        SetColorSameRobot();
    }

    private void SetColorSameRobot()
    {
        Rob11ColorManager rob11Color = GameObject.Find("Seti").GetComponent<Rob11ColorManager>();
        colorImage.color = rob11Color.predefinedColors[rob11Color.colorIndex];
    }
    
}