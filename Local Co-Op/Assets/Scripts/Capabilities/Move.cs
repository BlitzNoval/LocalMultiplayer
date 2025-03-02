using UnityEngine;

        [RequireComponent(typeof(Controller), typeof(CollisionDataRetriever), typeof(Rigidbody2D))]
    public class Move : MonoBehaviour
    {
        [SerializeField, Range(0f, 100f)] private float _maxSpeed = 4f;
        [SerializeField, Range(0f, 100f)] private float _maxAcceleration = 35f;
        [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 20f;
        

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
            _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(_maxSpeed - _collisionDataRetriever.Friction, 0f);
        }

        private void FixedUpdate()
        {
            _onGround = _collisionDataRetriever.OnGround;
            _velocity = _body.linearVelocity;

            _acceleration = _onGround ? _maxAcceleration : _maxAirAcceleration;
            _maxSpeedChange = _acceleration * Time.deltaTime;
            _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);


            _body.linearVelocity = _velocity;
        }
    }