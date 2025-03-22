using UnityEngine;

  [RequireComponent(typeof(Controller), typeof(CollisionDataRetriever), typeof(Rigidbody2D))]
    public class Jump : MonoBehaviour
    {
       [SerializeField, Range(0f, 10f)] private float _jumpHeight = 3f;
[SerializeField, Range(0, 5)] private int _maxAirJumps = 0;
[SerializeField, Range(0f, 10f)] private float _downwardGravityMultiplier = 8f; // Up from 6f for faster falls
[SerializeField, Range(0f, 5f)] private float _upwardGravityMultiplier = 3f; // Up from 2.5f for sharper jumps
[SerializeField, Range(0f, 0.3f)] private float _coyoteTime = 0.15f; // Decrease from 0.2f
[SerializeField, Range(0f, 0.3f)] private float _jumpBufferTime = 0.15f; // Up from 0.1f for easier timing
        private Controller _controller;
        private Rigidbody2D _body;
        private CollisionDataRetriever _ground;
        private Vector2 _velocity;

        private int _jumpPhase;
        private float _defaultGravityScale, _jumpSpeed, _coyoteCounter, _jumpBufferCounter;

        private bool _desiredJump, _onGround, _isJumping, _isJumpReset;


        // Start is called before the first frame update
       private void Awake()
{
    _body = GetComponent<Rigidbody2D>();
    _ground = GetComponent<CollisionDataRetriever>();
    _controller = GetComponent<Controller>(); // This should always exist due to RequireComponent

    _isJumpReset = true;
    _defaultGravityScale = 1f;

    // 🔹 Extra safety check to prevent crashes
    if (_controller == null || _controller.input == null)
    {
        Debug.LogError("Jump.cs: Controller or InputController is missing! Make sure the Player prefab has the correct input setup.");
    }
}


        // Update is called once per frame
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
            else if(_jumpBufferCounter > 0)
            {
                _jumpBufferCounter -= Time.deltaTime;
            }
            else if(!_desiredJump)
            {
                _isJumpReset = true;
            }

            if(_jumpBufferCounter > 0)
            {
                JumpAction();
            }

            if (_body.linearVelocity.y < 0)
    {
        // Falling - much stronger gravity
        _body.gravityScale = _downwardGravityMultiplier;
    }
    else if (_body.linearVelocity.y > 0 && !_controller.input.RetrieveJumpInput())
    {
        // Released jump button while rising - cut the jump short
        _body.gravityScale = _downwardGravityMultiplier; 
    }
    else if (_body.linearVelocity.y > 0)
    {
        // Rising with jump button held
        _body.gravityScale = _upwardGravityMultiplier;
    }
    else
    {
        // Not moving vertically
        _body.gravityScale = _defaultGravityScale;
    }

    _body.linearVelocity = _velocity;
        }
        private void JumpAction()
        {
            if (_coyoteCounter > 0f || (_jumpPhase < _maxAirJumps && _isJumping))
            {
                if(_isJumping)
                {
                    _jumpPhase += 1;
                }

                _jumpBufferCounter = 0;
                _coyoteCounter = 0;
                _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight * _upwardGravityMultiplier);
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