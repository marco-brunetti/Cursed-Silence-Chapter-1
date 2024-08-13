using System.Collections;
using UnityEngine;

public class BlackboardItem : MonoBehaviour, IBehaviour
{
    public bool IsOnBlackboard;

    private bool _isLooking;
    private float _currentZRotation;
    private float _rotateSpeed = 2;
    private Sprite _sprite;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>().sprite;
    }

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(isInteracting && IsOnBlackboard)
        {
            _currentZRotation = transform.eulerAngles.z;
            UIManager.Instance.ShowBlackboardImage(true, _sprite, _currentZRotation);
            _isLooking = true;
            SetupComponents();
            StartCoroutine(WaitForRightMouse());
        }
    }

    private IEnumerator WaitForRightMouse()
    {
        yield return new WaitUntil(()=> Input.GetMouseButtonDown(1));
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, _currentZRotation);
        UIManager.Instance.ShowBlackboardImage(false);
        _isLooking = false;
        SetupComponents();
    }

    private void SetupComponents()
    {
        var playerController = PlayerController.Instance;
        playerController.FreezePlayerMovement = _isLooking;
        playerController.FreezePlayerRotation = _isLooking;
        playerController.ActivateDepthOfField(_isLooking);
        GetComponent<SpriteRenderer>().enabled = !_isLooking;
        GetComponent<Collider>().enabled = !_isLooking;
    }

    private void Update()
    {
        if(Input.mouseScrollDelta.y != 0 && _isLooking)
        {
            _currentZRotation += Input.mouseScrollDelta.y * _rotateSpeed;
            UIManager.Instance.ShowBlackboardImage(true, zAngle: _currentZRotation);
        }
    }

    public bool IsInspectable()
    {
        return false;
    }

    public bool IsInteractable()
    {
        return true;
    }
}
