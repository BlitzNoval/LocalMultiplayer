// Title: The ULTIMATE 2D Character CONTROLLER in UNITY
// Author: Shinjingi
// Date: 01 March  2025
// Availability: https://www.youtube.com/watch?v=lcw6nuc2uaU

// Title: Smooth Wall Climb In Corners
// Author: ChatGPT
// Date: 09 March  2025
// Used to smooth the wall climb , becuase when hitting a corner and wall at the same time caused the player to stop and not move at all 


using UnityEngine;

    [RequireComponent(typeof(Controller), typeof(CollisionDataRetriever), typeof(Rigidbody2D))]
    public class WallInteractor : MonoBehaviour
    {
        public bool WallJumping { get; private set; }

        [Header("Wall Slide")]
        [SerializeField, Range(0.1f, 5f)] private float _wallSlideMaxSpeed = 4f; 
        [Header("Wall Jump")]
       [SerializeField, Range(0.05f, 0.5f)] private float _wallStickTime = 0.1f; 

        [SerializeField] private Vector2 _wallJumpClimb = new Vector2(6f, 14f); 
        [SerializeField] private Vector2 _wallJumpBounce = new Vector2(12f, 12f); 
        [SerializeField] private Vector2 _wallJumpLeap = new Vector2(16f, 14f); 
        
        private CollisionDataRetriever _collisionDataRetriever;
        private Rigidbody2D _body;
        private Controller _controller;

        private Vector2 _velocity;
        private bool _onWall, _onGround, _desiredJump, _isJumpReset;
        private float _wallDirectionX,  _wallStickCounter;

    
        private void Awake()
{
    _collisionDataRetriever = GetComponent<CollisionDataRetriever>();
    _body = GetComponent<Rigidbody2D>();
    _controller = GetComponent<Controller>(); 

    _isJumpReset = true;

    
    if (_controller == null || _controller.input == null)
    {
        Debug.LogError("WallInteractor: Controller or InputController is missing! Make sure the Player prefab has the correct input setup.");
    }
}


    
        void Update()
        {

                _desiredJump = _controller.input.RetrieveJumpInput();
        
        }

        private void FixedUpdate()
        {
            _velocity = _body.linearVelocity;
            _onWall = _collisionDataRetriever.OnWall;
            _onGround = _collisionDataRetriever.OnGround;
            _wallDirectionX = _collisionDataRetriever.ContactNormal.x;

            #region Wall Slide
            if(_onWall)
            {
                if(_velocity.y < -_wallSlideMaxSpeed)
                {
                    _velocity.y = -_wallSlideMaxSpeed;
                }
            }
            #endregion

                        #region Wall Stick
            if(_collisionDataRetriever.OnWall && !_collisionDataRetriever.OnGround && !WallJumping)
            {
                if(_wallStickCounter > 0)
                {
                    _velocity.x = 0;

                    if(_controller.input.RetrieveMoveInput() != 0 &&
   Mathf.Sign(_controller.input.RetrieveMoveInput()) == Mathf.Sign(_collisionDataRetriever.ContactNormal.x))
                    {
                        _wallStickCounter -= Time.deltaTime;
                    }
                    else
                    {
                        _wallStickCounter = _wallStickTime;
                    }
                }
                else
                {
                    _wallStickCounter = _wallStickTime;
                }
            }
            #endregion

            #region Wall Jump

            if((_onWall && _velocity.x == 0) || _onGround)
            {
                WallJumping = false;
            }

             if(_onWall && !_onGround)
            {

            if(_desiredJump && _isJumpReset)
            {
                 if(_controller.input.RetrieveMoveInput() == 0)
                {
                    _velocity = new Vector2(_wallJumpBounce.x * _wallDirectionX, _wallJumpBounce.y);
                    WallJumping = true;
                    _desiredJump = false;
                     _isJumpReset = false;
                }

                else if(Mathf.Sign( -_wallDirectionX) == Mathf.Sign(_controller.input.RetrieveMoveInput()))
                {
                    _velocity = new Vector2(_wallJumpClimb.x * _wallDirectionX, _wallJumpClimb.y);
                    WallJumping = true;
                    _desiredJump = false;
                     _isJumpReset = false;
                    
                }
            
                else
                {
                    _velocity = new Vector2(_wallJumpLeap.x * _wallDirectionX, _wallJumpLeap.y);
                    WallJumping = true;
                    _desiredJump = false;
                     _isJumpReset = false;
                }
            }
            else if (!_desiredJump){
                _isJumpReset = true;
            }
            }
            #endregion

            

            _body.linearVelocity = _velocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _collisionDataRetriever.EvaluateCollision(collision);
             _isJumpReset = false;

            if(_collisionDataRetriever.OnWall && !_collisionDataRetriever.OnGround && WallJumping)
            {
                _body.linearVelocity = Vector2.zero;
            }
        }
    }