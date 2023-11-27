using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float _movementInputDirection;
    [SerializeField] private float dashingVelocity = 14.0f;
    [SerializeField] private float dashingTime = 0.5f;
    
    private Vector2 _dashingDirection;
    private TrailRenderer _trailRenderer;
    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private AudioSource dashSoundEffect;
    

    private int _amountOfJumpsLeft;
    private int _facingDirection = 1;

    private bool _isFacingRight = true;
    private bool _isWalking;
    private bool _isGrounded;
    private bool _isTouchingWall;
    private bool _isWallSliding;
    private bool _canJump;
    private bool _isDashing;
    private bool _canDash;

    private Rigidbody2D _rb;
    private Animator _anim;

    public int amountOfJumps = 2;

    public float movementSpeed = 10.0f;
    public float jumpForce = 0.0f;
    public float groundCheckRadius = 0.14f;
    public float wallCheckDistance = 0.7f;
    public float wallSlideSpeed = 3.0f;
    public float airDragMultiplier = 1.0f;
    public float wallHopForce = 20.0f;
    public float wallJumpForce = 20.0f;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _amountOfJumpsLeft = amountOfJumps;
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.enabled = false;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
        ApplyMovement();
    }

    private void CheckIfWallSliding()
    {
        if (_isTouchingWall && !_isGrounded && _rb.velocity.y < 0)
        {
            _isWallSliding = true;
        }
        else
        {
            _isWallSliding = false;
        }
    }

    private void CheckSurroundings()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        _isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if(_isGrounded && _rb.velocity.y <= 0)
        {
            _amountOfJumpsLeft = amountOfJumps;
            _canJump = true;
            _canDash = true;
        }

        if(_amountOfJumpsLeft <= 0)
        {
            _canJump = false;
        }
      
    }

    private void CheckMovementDirection()
    {
        if(_isFacingRight && _movementInputDirection < 0)
        {
            Flip();
        }
        else if(!_isFacingRight && _movementInputDirection > 0)
        {
            Flip();
        }

        if(_rb.velocity.x != 0)
        {
            _isWalking = true;
        }
        else
        {
            _isWalking = false;
        }
    }

    private void UpdateAnimations()
    {
        _anim.SetBool("isWalking", _isWalking);
        _anim.SetBool("isGrounded", _isGrounded);
        _anim.SetFloat("yVelocity", _rb.velocity.y);
        _anim.SetBool("isWallSliding", _isWallSliding);
    }

    private void CheckInput()
    {
        _movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Dash") && _canDash)
        {
            SoundManager.instance.PlaySound(dashSoundEffect.clip);
            _isDashing = true;
            _canDash = false;
            _canJump = false;
            _trailRenderer.enabled = true;
            if (Input.GetAxisRaw("Horizontal") == 0.0f)
            {
                _dashingDirection = new Vector2(0.0f, 0.75f);
            }
            else if (Input.GetAxisRaw("Vertical") == 0.0f)
            {
                _dashingDirection = new Vector2(0.75f*_facingDirection, 0.0f);
            }
            else
            {
                _dashingDirection = new Vector2(0.5f*_facingDirection, 0.5f);
            }
            if(_dashingDirection == Vector2.zero)
            {
                _dashingDirection = new Vector2(_facingDirection, 0.0f);
            }
            StartCoroutine(StopDashing());
            
        }

        if (_isDashing)
        {
            _rb.velocity = _dashingDirection * dashingVelocity;
        }
        

        if (Input.GetButton("Jump") && _isGrounded)
        {
            jumpForce += 0.2f;
            movementSpeed = 0.0f;
        }

        if (Input.GetButtonUp("Jump") && _canJump)
        {
            if (jumpForce >= 25)
            {
                jumpForce = 25;
            }
            if (jumpForce <= 13.0f)
            {
                jumpForce = 13.0f;
            }
            movementSpeed = 10.0f;
            Jump();
            //movementSpeed = 10.0f;
            jumpForce = 0.0f;
        }
        
        if(_isWallSliding && Input.GetButtonUp("Jump"))
        {
            Jump();
        }

    }

    private void Jump()
    {
        SoundManager.instance.PlaySound(jumpSoundEffect.clip);
        if (_canJump && !_isWallSliding)
        {
            if(_isFacingRight)
            {
                _rb.velocity = new Vector2(jumpForce*2.0f, jumpForce*1);
            }
            else
            {
                _rb.velocity = new Vector2(-jumpForce*2.0f, jumpForce*1);
            }
            _canJump = false;
        }
        else if (_isWallSliding && _movementInputDirection == 0 && _amountOfJumpsLeft > 0) //Wall hop
        {
            _isWallSliding = false;
            _amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -_facingDirection, wallHopForce * wallHopDirection.y);
            _rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if((_isWallSliding || _isTouchingWall) && _movementInputDirection != 0 && _amountOfJumpsLeft > 0)
        {
            _isWallSliding = false;
            _amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -_facingDirection, wallJumpForce * wallJumpDirection.y);
            _rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        _isDashing = false;
        _trailRenderer.enabled = false;
    }

    private void ApplyMovement()
    {

        if (_isGrounded)
        {
            _rb.velocity = new Vector2(movementSpeed * _movementInputDirection, _rb.velocity.y);
        }
        else if(!_isGrounded && !_isWallSliding && _movementInputDirection != 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x * airDragMultiplier, _rb.velocity.y);
        }
        else if(!_isGrounded && !_isWallSliding && _movementInputDirection == 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x * airDragMultiplier, _rb.velocity.y);
        }

        if (_isWallSliding)
        {
            if(_rb.velocity.y < -wallSlideSpeed)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Flip()
    {
        if (!_isWallSliding)
        {
            _facingDirection *= -1;
            _isFacingRight = !_isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}