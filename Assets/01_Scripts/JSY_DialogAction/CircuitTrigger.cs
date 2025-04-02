using UnityEngine;

public class CircuitTrigger : MonoBehaviour
{
    [SerializeField] private bool _isLever; // true�� ���ܱ� ����, false�� ���ܱ�

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_isLever)
        {
            LowerCLAction lever = FindObjectOfType<LowerCLAction>();
            if (lever != null)
            {
                lever.TriggerLever();
            }
        }
        else
        {
            OpenCBAction box = FindObjectOfType<OpenCBAction>();
            if (box != null)
            {
                box.TriggerBox();
            }
        }
    }
}