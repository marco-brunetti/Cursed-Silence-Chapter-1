using System.Collections;
using UnityEngine;

public class BlackboardItem : MonoBehaviour, IBehaviour
{
    public bool IsOnBlackboard;
    public Collider BlackboardCollider;
    private bool _isLooking;
    private bool _isHolding;
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
            StartCoroutine(CheckMouseHold());
        }
    }

    private IEnumerator CheckMouseHold()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        if(Input.GetMouseButton(0))
        {
            _isLooking = false;
            _isHolding = true;
        }
        else
        {
            _isLooking = true;
            _isHolding = false;
        }

        if(_isLooking) LookItem();
        else if(_isHolding) HoldItem();
    }

    private void LookItem()
    {
        _currentZRotation = transform.eulerAngles.z;
        UIManager.Instance.ShowBlackboardImage(true, _sprite, _currentZRotation);
        _isLooking = true;
        SetupComponentsForLook();
        StartCoroutine(WaitForMouseInput());
    }

    private void HoldItem()
    {
        PlayerController.Instance.FreezePlayerMovement = true;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(WaitForMouseUp());
    }

    private IEnumerator WaitForMouseUp()
    {
        yield return new WaitUntil(()=> Input.GetMouseButtonUp(0));
        GetComponent<Collider>().enabled = true;
        PlayerController.Instance.FreezePlayerMovement = false;
        _isHolding = false;
        _isLooking = false;
    }

    private IEnumerator WaitForMouseInput()
    {
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(()=> Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1));

        //Create UI for this
        if(Input.GetMouseButtonDown(0)) transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, _currentZRotation);
        UIManager.Instance.ShowBlackboardImage(false);
        _isLooking = false;
        SetupComponentsForLook();
    }

    private void SetupComponentsForLook()
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
        if(_isLooking)
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                _currentZRotation += Input.mouseScrollDelta.y * _rotateSpeed;
                UIManager.Instance.ShowBlackboardImage(true, zAngle: _currentZRotation);
            }
        }

        if(_isHolding)
        {
            var playerController = PlayerController.Instance;

            Ray ray = new()
            {
                origin = playerController.Camera.position,
                direction = playerController.Camera.forward
            };

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, playerController.PlayerData.InteractDistance, playerController.PlayerData.InteractLayer))
            {
                if (hit.collider == BlackboardCollider)
                {
                    transform.position = hit.point;
                    transform.rotation = Quaternion.Euler(new Vector3(hit.normal.x, hit.normal.y + 90, _currentZRotation));
                }
            }
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
