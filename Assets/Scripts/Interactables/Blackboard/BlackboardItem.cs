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
    private SpriteRenderer _spriteRenderer;
    private Collider _collider;
    private PlayerController _playerController;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _sprite = _spriteRenderer.sprite;
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _playerController = PlayerController.Instance;
    }

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(isInteracting && IsOnBlackboard) StartCoroutine(CheckMouseHold());
    }

    private IEnumerator CheckMouseHold()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        if(Input.GetMouseButton(0)) HoldItem();
        else LookItem();
    }

    private void LookItem()
    {
        _isLooking = true;
        _isHolding = false;

        _currentZRotation = transform.eulerAngles.z;
        UIManager.Instance.ShowBlackboardImage(true, _sprite, _currentZRotation);
        _isLooking = true;
        SetupComponentsForLook();
        StartCoroutine(WaitForMouseInput());
    }

    private void SetupComponentsForLook()
    {
        _playerController.FreezePlayerMovement = _isLooking;
        _playerController.FreezePlayerRotation = _isLooking;
        _playerController.ActivateDepthOfField(_isLooking);
        _spriteRenderer.enabled = !_isLooking;
        _collider.enabled = !_isLooking;
    }

    private IEnumerator WaitForMouseInput()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1));

        //Create UI for this
        if (Input.GetMouseButtonDown(0)) transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, _currentZRotation);
        UIManager.Instance.ShowBlackboardImage(false);
        _isLooking = false;
        SetupComponentsForLook();
    }

    public void HoldItem(bool isFirstPlacement = false)
    {
        _isLooking = false;
        _isHolding = true;

        _playerController.FreezePlayerMovement = true;
        _collider.enabled = false;

        float delay = 0;
        if (isFirstPlacement) delay = 0.1f;
        StartCoroutine(WaitForMouseUp(delay));
    }

    private IEnumerator WaitForMouseUp(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        yield return new WaitUntil(()=> Input.GetMouseButtonUp(0));
        _collider.enabled = true;
        _playerController.FreezePlayerMovement = false;
        _isHolding = false;
        _isLooking = false;
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
            Ray ray = new()
            {
                origin = _playerController.Camera.position,
                direction = _playerController.Camera.forward
            };

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _playerController.PlayerData.InteractDistance, _playerController.PlayerData.InteractLayer))
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