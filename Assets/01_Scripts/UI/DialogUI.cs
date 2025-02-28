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
        Rob11ColorManager robotColor = GameObject.Find("Rob12").GetComponent<Rob11ColorManager>();
        colorImage.color = robotColor.predefinedColors[robotColor.colorIndex];
    }
    
}