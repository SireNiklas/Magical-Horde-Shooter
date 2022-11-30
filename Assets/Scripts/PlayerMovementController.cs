using UnityEngine;
using Rewired;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    private CharacterController _controller;
    [SerializeField] private int _playerId = 0;
    private Player _player;

    public float rotationSpeed = 27;

    [Header("Player")]
    [Tooltip("Walk speed of player moves.")]
    [SerializeField] private float _playerWalkSpeed;

    [Tooltip("Sprint speed if Player.")]
    [SerializeField] private float _playerSprintSpeed;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;
    
    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    private GameObject _mainCamera;
    
    private Transform _cameraTransform;
    private Transform _cameraReference;

    // Player
    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private bool IsCurrentDeviceMouse = true;
    
    private Vector3 _lastPos;

    private void Awake()
    {
        
        _cameraReference = new GameObject().transform;
        _cameraReference.name = "Camera Reference";
        
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Camera.main.transform.position = transform.position;
        _cameraTransform = Camera.main.transform;
        
        _player = ReInput.players.GetPlayer(_playerId);
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

    }

    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private Vector3 _playerMove;
    Vector3 velocity;
    public float gravity = -9.81f;

    void Move()
    {

        float moveHorizontal = _player.GetAxisRaw("Move Horizontal");
        float moveVertical = _player.GetAxisRaw("Move Vertical");

        Vector3 moveInput = new Vector3(moveHorizontal, 0, moveVertical).normalized;
        
        #region JUMP & GRAVITY
        // stop our velocity dropping infinitely when grounded
        // if (_verticalVelocity < 0.0f)
        // {
        //     _verticalVelocity = -2f;
        // }
        
        if(_player.GetButtonDown("Jump") && _controller.isGrounded)
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
        
        // if (_verticalVelocity < _terminalVelocity)
        // {
        //     _verticalVelocity += Gravity * Time.deltaTime;
        // }
        
        velocity.y += gravity * Time.deltaTime;
        _controller.Move(velocity * Time.deltaTime);
        #endregion
        
        if (moveInput.magnitude >= 0.1f)
        {

            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            _controller.Move(moveDir * (_playerWalkSpeed * Time.deltaTime) + new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
        }
    }

        private void JumpAndGravity()
        {

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
}
