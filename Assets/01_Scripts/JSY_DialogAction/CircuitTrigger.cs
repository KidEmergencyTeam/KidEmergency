using UnityEngine;

public class CircuitTrigger : MonoBehaviour
{
    [SerializeField] private bool _isLever; // true면 레버, false먄 버튼

    private bool _isBtTrigger = false;
    private bool _isLvTrigger = false;
    public OpenCBAction box;
    public LowerCLAction lever;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ActionManager.Instance != null)
            {
                if (!_isBtTrigger && !_isLever && ActionManager.Instance.currentAction == ActionType.OpenCircuitBox)
                {
                    _isBtTrigger = true;
                    print("두꺼비집 버튼 트리거 ~");
                    box.isButtonTriggered = true;
                    print($"여긴 트리거 스크립트~ 박스의 버튼 트리거는 {box.isButtonTriggered}!");
                }
                
                else if (!_isLvTrigger && _isLever && ActionManager.Instance.currentAction == ActionType.LowerCircuitLever)
                {
                    _isLvTrigger = true;
                    print("두꺼비집 레버 트리거~");
                    lever.isLeverTriggered = true;
                    print($"여긴 트리거 스크립트~ 레버의 레버 트리거는 {lever.isLeverTriggered}!");
                }

                else return;
            }
        }
    }
}