using System.Collections;
using UnityEngine;
using XRController = UnityEngine.XR.Interaction.Toolkit.XRController;

public class FixingBagAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    [SerializeField] private GameObject[] _bag;
    [SerializeField] GameObject _xrParent;
    public bool IsActionComplete => _isComplete;

    private void Start()
    {
        // 멀티일 시 고유 ID로 먼저 찾고 Canparent 찾기
        _xrParent = GameObject.Find("CamParent");
    }

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(TestAction());
        // _isComplete = true;
    }

    public void StartMultiModeAction()
    {
        _isComplete = false;        
    }

    private IEnumerator TestAction()
    {
        Bags bag = FindObjectOfType<Bags>();
        while (!_isComplete)
        {
            bag.BagInteraction();
            if (bag.IsProtect())
            {
                _isComplete = true;
            }
            
            yield return null;
        }
    }

    // private IEnumerator TestAction()
    // {
    //     // Transform headTransform = _xrParent.transform.GetChild(0);
    //     //
    //     // while (!_isComplete)
    //     // {
    //     //     Vector3 currentHeadPos = headTransform.position;
    //     //     Vector3 targetPos = currentHeadPos + Vector3.up * 0.5f;  // 값은 씬에서 테스트 해보고 결정
    //     //     
    //     //     for (int i = 0; i < _bag.Length; i++)
    //     //     {
    //     //         if (Vector3.Distance(_bag[i].transform.position, targetPos) < 0.1f)
    //     //         {
    //     //             _bag[i].transform.SetParent(_xrParent.transform);
    //     //             
    //     //             _isComplete = true;
    //     //             break;
    //     //         }
    //     //
    //     //         else
    //     //         {
    //     //             UIManager.Instance.SetWarningUI(warningSprite,warningText);
    //     //             UIManager.Instance.OpenWarningUI();
    //     //         }
    //     //     }
    //     //     
    //     //     yield return null;
    //     // }
    //     
    //     print("가방 고정됨");
    // }
}
