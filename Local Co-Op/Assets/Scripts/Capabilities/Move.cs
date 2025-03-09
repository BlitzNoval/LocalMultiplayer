        using UnityEngine;

        [RequireComponent(typeof(Controller), typeof(CollisionDataRetriever), typeof(Rigidbody2D))]
        public class Move : MonoBehaviour
        {
            
        [SerializeField, Range(0f, 100f)] private float _maxSpeed = 4f;
        [SerializeField, Range(0f, 100f)] private float _maxAcceleration = 35f;
        [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 20f;
        [SerializeField] private Transform _spriteTransform; // Assign in inspector
[SerializeField, Range(0f, 15f)] private float _tiltAngle = 8f; // Max rotation when moving
[SerializeField, Range(1f, 20f)] private float _tiltSpeed = 10f; // How fast to tilt
[SerializeField, Range(0f, 2f)] private float _initialBoostMultiplier = 1.2f; // Brief speed boost
[SerializeField, Range(0f, 0.5f)] private float _initialBoostTime = 0.15f; // How long the boost lasts
private float _directionChangeTime; // Track when we changed direction
private int _lastMoveDirection = 0; // Last move direction
[SerializeField, Range(0f, 1f)] private float _airControlFactor = 0.8f; 

        [SerializeField, Range(0f, 100f)] private float _maxDeceleration = 45f; // Higher than acceleration
        [SerializeField, Range(0f, 100f)] private float _maxAirDeceleration = 25f; // Higher than air acceleration

        private Controller _controller;
        private Vector2 _direction, _desiredVelocity, _velocity;
        private Rigidbody2D _body;
        private CollisionDataRetriever _collisionDataRetriever;
        private WallInteractor _wallInteractor;

        private float _maxSpeedChange, _acceleration;
        private bool _onGround;

        private void Awake()
        {
            // Get necessary components from the GameObject
            _body = GetComponent<Rigidbody2D>();
            _collisionDataRetriever = GetComponent<CollisionDataRetriever>();
            _controller = GetComponent<Controller>(); // This should always exist because of RequireComponent
            _wallInteractor = GetComponent<WallInteractor>();

            // 🔹 Extra safety check (just in case something is wrong)
            if (_controller == null || _controller.input == null)
            {
                Debug.LogError("Controller or InputController is missing! Make sure the Player prefab has the correct input setup.");
            }
        }


        private void Update()
        {
              _direction.x = _controller.input.RetrieveMoveInput();
    
    // Check for direction changes
    int currentMoveDirection = Mathf.RoundToInt(Mathf.Sign(_direction.x));
    if (_lastMoveDirection != currentMoveDirection && currentMoveDirection != 0)
    {
        _directionChangeTime = Time.time;
    }
    _lastMoveDirection = currentMoveDirection;
    
    // Apply initial boost to desired velocity if we just changed direction
    float boostMultiplier = 1f;
    if (Time.time < _directionChangeTime + _initialBoostTime)
    {
        boostMultiplier = _initialBoostMultiplier;
    }

     if (!_onGround)
    {
        float currentVelSign = Mathf.Sign(_body.linearVelocity.x);
        float desiredVelSign = Mathf.Sign(_desiredVelocity.x);
        
        // If trying to move against current momentum, apply the air control factor
        if (currentVelSign != 0 && desiredVelSign != 0 && currentVelSign != desiredVelSign)
        {
            _desiredVelocity.x *= _airControlFactor;
        }
    }
    
    _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(_maxSpeed - _collisionDataRetriever.Friction, 0f) * boostMultiplier;


             if (_spriteTransform != null)
    {
        float targetAngle = _direction.x * _tiltAngle;
        Vector3 currentRotation = _spriteTransform.rotation.eulerAngles;
        currentRotation.z = Mathf.LerpAngle(currentRotation.z, targetAngle, Time.deltaTime * _tiltSpeed);
        _spriteTransform.rotation = Quaternion.Euler(currentRotation);
    }
        }

        private void FixedUpdate()
        {
            _onGround = _collisionDataRetriever.OnGround;
            _velocity = _body.linearVelocity;

            // Choose acceleration or deceleration based on input direction
            float currentAcceleration;
            if (_desiredVelocity.x == 0 || Mathf.Sign(_velocity.x) != Mathf.Sign(_desiredVelocity.x))
            {
                // We're stopping or changing direction - use deceleration
                currentAcceleration = _onGround ? _maxDeceleration : _maxAirDeceleration;
            }
            else
            {
                // We're accelerating in the same direction - use acceleration
                currentAcceleration = _onGround ? _maxAcceleration : _maxAirAcceleration;
            }
            
            _maxSpeedChange = currentAcceleration * Time.deltaTime;
            _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);

            _body.linearVelocity = _velocity;
        }
    }