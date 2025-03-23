using UnityEngine;

[RequireComponent(typeof(Controller), typeof(CollisionDataRetriever), typeof(Rigidbody2D))]
public class Jump : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float _jumpHeight = 3f;
    [SerializeField, Range(0, 5)] private int _maxAirJumps = 1; // Set to 1 for double jump
    [SerializeField, Range(0f, 10f)] private float _downwardGravityMultiplier = 6f;
    [SerializeField, Range(0f, 5f)] private float _upwardGravityMultiplier = 2.5f;
    [SerializeField, Range(0f, 0.3f)] private float _coyoteTime = 0.15f;
    [SerializeField, Range(0f, 0.3f)] private float _jumpBufferTime = 0.1f;
    [SerializeField, Range(0f, 1f)] private float _airJumpHeightMultiplier = 0.5f; // New field for air jump height

    private Controller _controller;
    private Rigidbody2D _body;
    private CollisionDataRetriever _ground;
    private Vector2 _velocity;

    private int _jumpPhase;
    private float _defaultGravityScale, _jumpSpeed, _coyoteCounter, _jumpBufferCounter;

    private bool _desiredJump, _onGround, _isJumping, _isJumpReset;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<CollisionDataRetriever>();
        _controller = GetComponent<Controller>();

        _isJumpReset = true;
        _defaultGravityScale = 1f;

        if (_controller == null || _controller.input == null)
        {
            Debug.LogError("Jump.cs: Controller or InputController is missing! Make sure the Player prefab has the correct input setup.");
        }
    }

    void Update()
    {
        _desiredJump = _controller.input.RetrieveJumpInput();
    }

    private void FixedUpdate()
    {
        _onGround = _ground.OnGround;
        _velocity = _body.linearVelocity;

        if (_onGround && _body.linearVelocity.y == 0)
        {
            _jumpPhase = 0;
            _coyoteCounter = _coyoteTime;
            _isJumping = false;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }

        if (_desiredJump && _isJumpReset)
        {
            _isJumpReset = false;
            _desiredJump = false;
            _jumpBufferCounter = _jumpBufferTime;
        }
        else if (_jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
        else if (!_desiredJump)
        {
            _isJumpReset = true;
        }

        if (_jumpBufferCounter > 0)
        {
            JumpAction();
        }

        if (_body.linearVelocity.y < 0)
        {
            _body.gravityScale = _downwardGravityMultiplier;
        }
        else if (_body.linearVelocity.y > 0 && !_controller.input.RetrieveJumpInput())
        {
            _body.gravityScale = _downwardGravityMultiplier;
        }
        else if (_body.linearVelocity.y > 0)
        {
            _body.gravityScale = _upwardGravityMultiplier;
        }
        else
        {
            _body.gravityScale = _defaultGravityScale;
        }

        _body.linearVelocity = _velocity;
    }

    private void JumpAction()
    {
        if (_coyoteCounter > 0f || (_jumpPhase < _maxAirJumps && _isJumping))
        {
            if (_isJumping)
            {
                _jumpPhase += 1;
            }

            _jumpBufferCounter = 0;
            _coyoteCounter = 0;

            // Determine if this is an air jump
            bool isAirJump = !_onGround && _jumpPhase > 0;

            // Calculate effective jump height
            float effectiveJumpHeight = isAirJump ? (_jumpHeight * _airJumpHeightMultiplier) : _jumpHeight;

            // Calculate jump speed using effectiveJumpHeight
            _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * effectiveJumpHeight * _upwardGravityMultiplier);

            _isJumping = true;

            if (_velocity.y > 0f)
            {
                _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
            }
            else if (_velocity.y < 0f)
            {
                _jumpSpeed += Mathf.Abs(_body.linearVelocity.y);
            }
            _velocity.y += _jumpSpeed;
        }
    }
}