using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;

    [Header("Movement")] public float speed = 12f;

    [Header("Jump")] public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;
    public float jumpTopDelay = 0.1f;
    public float coyoteTime = .1f;
    public float jumpFallGravModifier = 1.5f;


    [Header("Ground Check")] public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    //Timers
    private float _jumpTopTimer;
    private float _coyoteTimer;


    private Vector2 _downwardVelocity;
    private Vector2 _lastDownwardVelocity = new Vector2(0f, 0f);

    private bool _isGrounded;
    private bool _isMoving;

    private Vector3 _movementInput;

    private Vector3 _lastPosition = new Vector3(0f, 0f, 0f);

    private bool _hasJumped;


    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Getting inputs
        _movementInput.x = Input.GetAxis("Horizontal");
        _movementInput.z = Input.GetAxis("Vertical");

        HandleJump();
        HandleTimers();
        HandleMovement();

        // Ground check
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (_isGrounded && !_hasJumped)
        {
            _coyoteTimer = coyoteTime;
        }
    }

    private void HandleMovement()
    {
        //Creating moving vector
        Vector3 move = transform.right * _movementInput.x + transform.forward * _movementInput.z;

        //Character movement
        _controller.Move(move * (speed * Time.deltaTime));


        if (_lastPosition != gameObject.transform.position && _isGrounded)
        {
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }

        _lastPosition = gameObject.transform.position;
    }

    private void HandleTimers()
    {
        if (_jumpTopTimer > 0)
        {
            _jumpTopTimer -= Time.deltaTime;
        }
        else
        {
            _jumpTopTimer = 0;
        }

        if (_coyoteTimer > 0)
        {
            _coyoteTimer -= Time.deltaTime;
        }
        else
        {
            _coyoteTimer = 0;
        }
    }

    private void HandleJump()
    {
        //if the coyote timer is higher than 0 that means that we are grounded or we left the ground very recently 
        if (Input.GetButtonDown("Jump") && _coyoteTimer > 0)
        {
            _hasJumped = true;
            _coyoteTimer = 0;
            _downwardVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        //not on the ground
        if (!_isGrounded)
        {
            _downwardVelocity.y += gravity * Time.deltaTime;
            //Falling Down
            if (_downwardVelocity.y < 0)
            {
                _downwardVelocity.y += (jumpFallGravModifier - 1) * gravity * Time.deltaTime;
            }
        }

        if (!_isGrounded && _hasJumped)
        {
            if (((int)Mathf.Sign(_lastDownwardVelocity.y) == 1 && (int)Mathf.Sign(_downwardVelocity.y) == -1) ||
                ((int)Mathf.Sign(_lastDownwardVelocity.y) == 0 && (int)Mathf.Sign(_downwardVelocity.y) == -1) ||
                ((int)Mathf.Sign(_lastDownwardVelocity.y) == 1 && (int)Mathf.Sign(_downwardVelocity.y) == 0))
            {
                _hasJumped = false;
                if (_jumpTopTimer == 0)
                {
                    //if the y velocity of the player is less than 
                    _jumpTopTimer = jumpTopDelay;
                }
            }
        }

        if (_isGrounded && !_hasJumped)
        {
            _downwardVelocity.y = 0;
        }

        if (_jumpTopTimer > 0)
        {
            _downwardVelocity.y = 0;
        }

        _lastDownwardVelocity = _downwardVelocity;

        _controller.Move(_downwardVelocity * Time.deltaTime);
        return;
    }

    public bool IsMoving()
    {
        return _isMoving;
    }


    private void OnDrawGizmos()
    {
        //visualizing ground check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);

        //visualizing jump height
        Gizmos.color = Color.green;
        //find the distance to the ground
        RaycastHit hit;
        Physics.Raycast(groundCheck.position, -transform.up, out hit, jumpHeight, groundMask);
        //if it is more than the jump height, clamp it to the feet
        var distanceToGround = hit.distance;
        if (distanceToGround >= jumpHeight) distanceToGround = jumpHeight;
        Gizmos.DrawWireCube(
            new Vector3(groundCheck.position.x, groundCheck.position.y + jumpHeight - distanceToGround,
                groundCheck.position.z),
            new Vector3(.5f, 0, .5f));
    }
}