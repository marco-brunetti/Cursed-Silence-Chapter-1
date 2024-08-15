using System;
using System.Collections;
using UnityEngine;

public class BlackboardItem : MonoBehaviour, IBehaviour
{
    [field: SerializeField] public int PageNumber {  get; private set; }

    [NonSerialized] public Collider BlackboardCollider;
    private float _currentZRotation;
    private Vector3 _moveOffset;
    private BlackboardItemState _currentState;
    private Collider _collider;
    private SpriteRenderer _spriteRenderer;
    private PlayerController _playerController;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider>();
        _currentZRotation = transform.eulerAngles.z;
    }

    private void Start()
    {
        _playerController = PlayerController.Instance;
    }

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(isInteracting && BlackboardController.Instance.BlackboardItems.Contains(this))
        {
            StartCoroutine(CheckMouseHold());
        }
    }

    private IEnumerator CheckMouseHold()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        if(Input.GetMouseButton(0))
        {
            HoldItem();
        }
        else
        {
            _currentState = BlackboardItemState.Looking;
            _currentZRotation = transform.eulerAngles.z;
            UIManager.Instance.ShowBlackboardImage(sprite: _spriteRenderer.sprite, zAngle: _currentZRotation);
            SetupComponentsForLook(isLooking: true);
            StartCoroutine(WaitForMouseInput());
        }
    }

    public void HoldItem(bool isFirstPlacement = false, Collider blackboardCollider = null)
    {
        if(isFirstPlacement) BlackboardCollider = blackboardCollider;
        _currentState = BlackboardItemState.Moving;
        _playerController.FreezePlayerMovement = true;
        _collider.enabled = false;
        StartCoroutine(WaitForMouseUp(isFirstPlacement));
    }

    private IEnumerator WaitForMouseInput()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1));

        //Create UI for this
        if (Input.GetMouseButtonDown(0)) transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, _currentZRotation);
        UIManager.Instance.ShowBlackboardImage(false);
        SetupComponentsForLook(isLooking: false);
        _currentState = BlackboardItemState.None;
    }

    private void SetupComponentsForLook(bool isLooking)
    {
        _playerController.FreezePlayerMovement = isLooking;
        _playerController.FreezePlayerRotation = isLooking;
        _playerController.ActivateDepthOfField(isLooking);
        _spriteRenderer.enabled = !isLooking;
        _collider.enabled = !isLooking;
    }

    private IEnumerator WaitForMouseUp(bool isFirstPlacement)
    {
        if(isFirstPlacement) yield return new WaitForSecondsRealtime(0.1f);
        yield return new WaitUntil(()=> Input.GetMouseButtonUp(0));
        _collider.enabled = true;
        _playerController.FreezePlayerMovement = false;
        _moveOffset = Vector3.zero;
        _currentState = BlackboardItemState.None;
    }

    private void Update()
    {
        switch(_currentState)
        {
            case BlackboardItemState.Looking:
                if (Input.mouseScrollDelta.y != 0)
                {
                    var rotateSpeed = 2f;
                    _currentZRotation += Input.mouseScrollDelta.y * rotateSpeed;
                    UIManager.Instance.ShowBlackboardImage(zAngle: _currentZRotation);
                }
                break;
            case BlackboardItemState.Moving:
                Ray ray = new() { origin = _playerController.Camera.position, direction = _playerController.Camera.forward };
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, _playerController.PlayerData.InteractDistance, _playerController.PlayerData.InteractLayer))
                {
                    if (hit.collider == BlackboardCollider)
                    {
                        if (_moveOffset == Vector3.zero) _moveOffset = transform.position - hit.point;

                        transform.position = hit.point + _moveOffset;
                        transform.eulerAngles = new Vector3(hit.normal.x, hit.normal.y + 90, _currentZRotation);
                    }
                }
                break;
        }
    }

    public bool IsInspectable() { return false; }
    public bool IsInteractable() { return true; }
}

public enum BlackboardItemState
{
    None,
    Looking,
    Moving,
    Joined
}