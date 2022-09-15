using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    public float movementSpeed;
    public Transform orientation;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool m_ReadyToJump;
    private float m_HorizontalInput;
    private float m_VerticalInput;
    private Vector3 m_MoveDirection;
    private Rigidbody m_Rb;

    [Header("Ground Check")] 
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool m_Grounded;

    [Header("KeyBinds")] 
    public KeyCode jumpKey = KeyCode.Space;
    
    
    
    // Start is called before the first frame update
    private void Start()        
    {
        m_Rb = GetComponent<Rigidbody>();
        m_Rb.freezeRotation = true;
        ResetJump();
    }

    // Update is called once per frame
    private void Update()       
    {
        GroundCheck();
        MyInput();
        SpeedControl();
        HandleDrag();
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
    }
    
    private void MovePlayer()      
    {
        // CALCULATES THE PLAYER'S MOVEMENT
        m_MoveDirection = (orientation.forward * m_VerticalInput) + (orientation.right * m_HorizontalInput);
        switch (m_Grounded)
        {
            // ON GROUND
            case true:
                m_Rb.AddForce(m_MoveDirection.normalized * (movementSpeed * 10f), ForceMode.Force);
                break;
            // IN AIR
            case false:
                m_Rb.AddForce(m_MoveDirection.normalized * (movementSpeed * 10f * airMultiplier), ForceMode.Force);
                break;
                
        }

    }

    private void SpeedControl()     
    {   
        // LIMITS THE VELOCITY OF THE PLAYER'S MOVEMENT
        var cachedVelocity = m_Rb.velocity;
        var flatVelocity = new Vector3(cachedVelocity.x, 0f, cachedVelocity.z);
        if (flatVelocity.magnitude > movementSpeed)
        {
            var limitedVelocity = flatVelocity.normalized * movementSpeed;
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
}
