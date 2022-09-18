using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    private float m_HorizontalInput;
    private float m_VerticalInput;
    private Vector3 m_MoveDirection;
    private Rigidbody m_Rb;

    [Header("Movement")] 
    public float walkSpeed;
    public float sprintSpeed;
    private float m_MovementSpeed;
    public Transform orientation;
    public float groundDrag;
    public MovementState state;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool m_ReadyToJump;

    [Header("Crouch")] 
    public float crouchSpeed;
    public float crouchYScale;
    private float m_StartYScale;
    
    
    [Header("Ground Check")] 
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool m_Grounded;

    [Header("KeyBinds")] 
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    public enum MovementState
    {
        Sprinting,
        Walking,
        Crouching,
        Air
    }
    
    // Start is called before the first frame update
    private void Start()        
    {
        m_Rb = GetComponent<Rigidbody>();
        m_Rb.freezeRotation = true;
        m_StartYScale = transform.localScale.y;
        ResetJump();
    }

    // Update is called once per frame
    private void Update()       
    {
        GroundCheck();
        MyInput();
        SpeedControl();
        HandleDrag();
        StateHandler();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        // GETTING WASD INPUTS 
        m_HorizontalInput = Input.GetAxisRaw("Horizontal");       
        m_VerticalInput = Input.GetAxisRaw("Vertical");
        
        // JUMPING WHEN CONDITIONS ARE MET
        if (Input.GetKey(jumpKey) && m_ReadyToJump && m_Grounded)
        {
            m_ReadyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        Crouching();
    }
    
    private void MovePlayer()      
    {
        // CALCULATES THE PLAYER'S MOVEMENT
        m_MoveDirection = (orientation.forward * m_VerticalInput) + (orientation.right * m_HorizontalInput);
        switch (m_Grounded)
        {
            // ON GROUND
            case true:
                m_Rb.AddForce(m_MoveDirection.normalized * (m_MovementSpeed * 10f), ForceMode.Force);
                break;
            // IN AIR
            case false:
                m_Rb.AddForce(m_MoveDirection.normalized * (m_MovementSpeed * 10f * airMultiplier), ForceMode.Force);
                break;
                
        }

    }

    private void SpeedControl()     
    {   
        // LIMITS THE VELOCITY OF THE PLAYER'S MOVEMENT
        var cachedVelocity = m_Rb.velocity;
        var flatVelocity = new Vector3(cachedVelocity.x, 0f, cachedVelocity.z);
        if (flatVelocity.magnitude > m_MovementSpeed)
        {
            var limitedVelocity = flatVelocity.normalized * m_MovementSpeed;
            m_Rb.velocity = new Vector3(limitedVelocity.x, cachedVelocity.y, limitedVelocity.z);
        }

        
    }

    private void Jump()
    {   
        //RESETS THE Y VELOCITY
        var cachedVelocity = m_Rb.velocity;
        m_Rb.velocity = new Vector3(cachedVelocity.x, 0f, cachedVelocity.z);
        m_Rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() => m_ReadyToJump = true;

    private void GroundCheck() => m_Grounded = Physics.Raycast(transform.position, 
                                        Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

    private void HandleDrag()
    {
        if (m_Grounded)
            m_Rb.drag = groundDrag;
        else
            m_Rb.drag = 0;
    }

    private void StateHandler()
    {
        // UPDATES THE PLAYER'S CURRENT MOVEMENT STATE AND APPLIES MOVE SPEED
        if (m_Grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.Sprinting;
            m_MovementSpeed = sprintSpeed;
        }
        else if (m_Grounded)
        {
            state = MovementState.Walking;
            m_MovementSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.Air;
        }
        
        // ONLY ALLOW THE SPEED TO BE APPLIED WHEN GROUNDED
        if (m_Grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.Crouching;
            m_MovementSpeed = crouchSpeed;
        }
    }

    private void Crouching()
    {
        var cachedPlayerScale = transform.localScale;
        // FORCES THE PLAYER DOWN WHEN CROUCHING INSTEAD OF CROUCHING MIDAIR
        if (m_Grounded && Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(cachedPlayerScale.x, crouchYScale, cachedPlayerScale.z);
            m_Rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        //ALLOWS THE PLAYER TO CROUCH MIDAIR WITHOUT HAVING FORCE APPLIED DOWNWARD
        else if (!m_Grounded && Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(cachedPlayerScale.x, crouchYScale, cachedPlayerScale.z);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(cachedPlayerScale.x, m_StartYScale, cachedPlayerScale.z);
        }
    }
}
