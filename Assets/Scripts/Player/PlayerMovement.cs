using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _mainSpriteRenderer;

    private Rect _screenBounds;
    private Vector2 _halfSize;

    private bool _isInFocusMode = false;
    private Rigidbody2D _rigidBody;
    private PlayerBaseStats _playerBaseStats;

    private void Start()
    {
        _halfSize = _mainSpriteRenderer.bounds.extents;
        _rigidBody = GetComponent<Rigidbody2D>();
        _playerBaseStats = GetComponent<PlayerStatsHolder>().PlayerStats;
        var bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));

        _screenBounds = new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }

    private void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, _screenBounds.xMin + _halfSize.x, _screenBounds.xMax - _halfSize.x),
                                         Mathf.Clamp(transform.position.y, _screenBounds.yMin + _halfSize.y, _screenBounds.yMax - _halfSize.y),
                                         transform.position.z);
    }

    public void HandleMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        input.Normalize();
        _rigidBody.linearVelocity = input * ((!_isInFocusMode) ? _playerBaseStats.MovementSpeed : _playerBaseStats.FocusModeMovementSpeed);
    }

    public void HandleFocusMode(InputAction.CallbackContext context)
    {
        if(context.started || context.canceled)
        {
            _isInFocusMode = context.started;
        }
    }
}
