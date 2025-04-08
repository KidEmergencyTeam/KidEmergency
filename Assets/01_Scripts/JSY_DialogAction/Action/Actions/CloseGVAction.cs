using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CloseGVAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _target; // 가스 밸브
    [SerializeField] private GameObject[] _hand; // 0 왼, 1 오
    [SerializeField] private GameObject _highlighter;
    
    private float _limitRot = 90f;
    private XRGrabInteractable _grabInteractable;
    private bool _isComplete = false;
    
    public bool IsActionComplete => _isComplete;

    private void Awake()
    {
        _grabInteractable = _target.GetComponent<XRGrabInteractable>();
    }

    private void Start()
    {
        _grabInteractable.enabled = false;
        _target.GetComponent<BaseOutlineObject>().enabled = false;
    }

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(TryCloseGV());
    }

    private IEnumerator TryCloseGV()
    {
        while (!_isComplete)
        {
            _highlighter.SetActive(true);
            _target.GetComponent<BaseOutlineObject>().enabled = true;
            bool isInteractable = Vector3.Distance(_target.transform.position, _hand[0].transform.position) < 0.05f
                                  || Vector3.Distance(_target.transform.position, _hand[1].transform.position) < 0.05f;
            if (isInteractable)
            {
                _grabInteractable.enabled = true;
                
                float currentZRotation = _target.transform.rotation.eulerAngles.z;

                if (currentZRotation == _limitRot)
                {
                    _grabInteractable.enabled = false;
                    _target.transform.rotation = Quaternion.Euler(0, 0, _limitRot);

                    _target.GetComponent<BaseOutlineObject>().enabled = false;
                    _highlighter.SetActive(false);
                    _isComplete = true;
                }
            }

            else
            {
                _grabInteractable.enabled = false;
            }
            
            yield return null;

        }
    }
}
