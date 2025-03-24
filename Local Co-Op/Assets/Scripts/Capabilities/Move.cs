                    
        // Title: The ULTIMATE 2D Character CONTROLLER in UNITY
        // Author: Shinjingi
        // Date: 01 March  2025
        // Availability: https://www.youtube.com/watch?v=lcw6nuc2uaU
        
        using UnityEngine;

        [RequireComponent(typeof(Controller), typeof(CollisionDataRetriever), typeof(Rigidbody2D))]
        public class Move : MonoBehaviour
        {
            
        [SerializeField, Range(0f, 100f)] private float _maxSpeed = 10f;
        [SerializeField, Range(0f, 100f)] private float _maxAcceleration = 100f; 
        [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 60f; 
        [SerializeField, Range(0f, 100f)] private float _maxDeceleration = 100f; 
        [SerializeField, Range(0f, 100f)] private float _maxAirDeceleration = 50f; 
        [SerializeField, Range(0f, 100f)] private float _instantStopThreshold = 0.1f; 
        [SerializeField] private Transform _spriteTransform; 
        [SerializeField, Range(0f, 15f)] private float _tiltAngle = 8f; 
        [SerializeField, Range(1f, 20f)] private float _tiltSpeed = 10f; 
        [SerializeField, Range(0f, 2f)] private float _initialBoostMultiplier = 1.5f; 
[SerializeField, Range(0f, 0.5f)] private float _initialBoostTime = 0.1f; 
        private float _directionChangeTime; 
        private int _lastMoveDirection = 0; 
        [SerializeField, Range(0f, 1f)] private float _airControlFactor = 0.8f; 

    
        private Controller _controller;
        private Vector2 _direction, _desiredVelocity, _velocity;
        private Rigidbody2D _body;
        private CollisionDataRetriever _collisionDataRetriever;
        private WallInteractor _wallInteractor;

        private float _maxSpeedChange, _acceleration;
        private bool _onGround;

        private void Awake()
        {
            
            _body = GetComponent<Rigidbody2D>();
            _collisionDataRetriever = GetComponent<CollisionDataRetriever>();
            _controller = GetComponent<Controller>(); 
            _wallInteractor = GetComponent<WallInteractor>();

           
            if (_controller == null || _controller.input == null)
            {
                Debug.LogError("Controller or InputController is missing! Make sure the Player prefab has the correct input setup.");
            }
        }


        private void Update()
        {
              _direction.x = _controller.input.RetrieveMoveInput();
    
    
        int currentMoveDirection = Mathf.RoundToInt(Mathf.Sign(_direction.x));
        if (_lastMoveDirection != currentMoveDirection && currentMoveDirection != 0)
        {
            _directionChangeTime = Time.time;
        }
        _lastMoveDirection = currentMoveDirection;
        
       
        float boostMultiplier = 1f;
        if (Time.time < _directionChangeTime + _initialBoostTime)
        {
            boostMultiplier = _initialBoostMultiplier;
        }

        if (!_onGround)
        {
            float currentVelSign = Mathf.Sign(_body.linearVelocity.x);
            float desiredVelSign = Mathf.Sign(_desiredVelocity.x);
            
           
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

          
            if (_onGround)
            {
                // If trying to stop and moving slowly, come to a complete stop
                if (Mathf.Abs(_direction.x) < 0.01f && Mathf.Abs(_velocity.x) < _instantStopThreshold)
                {
                    _velocity.x = 0f;
                }
                else
                {
                    
                    _maxSpeedChange = _maxAcceleration * Time.deltaTime;
                    _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
                }
            }
            else
            {
                // Air movement
                _maxSpeedChange = _maxAirAcceleration * Time.deltaTime;
                _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
            }

            _body.linearVelocity = _velocity;
        }
            }