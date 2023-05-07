using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // Movement variables
    [SerializeField] public float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    public Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    public float horizontalInput;

    // Trail renderer variables for dashing
    private TrailRenderer _trailRenderer;

    // Dashing variables
    [SerializeField] private float _dashingVelocity = 8f;
    [SerializeField] private float _dashingTime = 0.35f;
    private Vector2 _dashingDir;
    private bool _isDashing;
    private bool _canDash = true;

    // Initialize the Rigidbody, Animator, and TrailRenderer components
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Get the player's horizontal input
        horizontalInput = Input.GetAxis("Horizontal");

        // Flip character while moving
        if (body.velocity.x > 0.01f)
            transform.localScale = Vector3.one;
        else if (body.velocity.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Set Animator param
        anim.SetBool("Walk", horizontalInput != 0);
        anim.SetBool("Grounded", IsGrounded());

        // Handle wall jump
        if (wallJumpCooldown > 0.2f)
        {
            // Move the player horizontally based on input
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            // Set gravity scale based on whether the player is on a wall
            if (onWall() && !IsGrounded())
            {
                body.gravityScale = 1;
                body.velocity = new Vector2(0, -0.5f);
            }
            else
                body.gravityScale = 3;

            // Handle jump and drop inputs
            if (Input.GetKey(KeyCode.Space))
                Jump();
            if (Input.GetKey(KeyCode.S))
                Drop();
        }
        else
            wallJumpCooldown += Time.deltaTime;

        // Handle dashing input
        var dashInput = Input.GetButtonDown("Dash");
        if (dashInput && _canDash)
        {
            _isDashing = true;
            _canDash = false;
            _trailRenderer.emitting = true;
            _dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
            if (_dashingDir == Vector2.zero)
            {
                _dashingDir = new Vector2(transform.localScale.x, 0);
            }

            StartCoroutine(stopDashing());
        }

        // Handle dashing movement
        if (_isDashing)
        {
            body.velocity = _dashingDir.normalized * _dashingVelocity;
            return;
        }

        // Allow the player to dash again when grounded
        if (IsGrounded())
        {
            _canDash = true;
        }
    }

    // Handle jumping and wall jumping
    public void Jump()
    {
        if (IsGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("Jump");
        }
        else if (onWall() && !IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 7);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            wallJumpCooldown = 0;
        }
        else if (!IsGrounded())
        {
            anim.ResetTrigger("Jump");
        }
    }

    // Handle dropping while on a wall
    private void Drop()
    {
        if (onWall() && !IsGrounded())
        {
            if (Input.GetKey(KeyCode.S))
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 5, -1);
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    // Check if the player is grounded
    public bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    // Check if the player is on a wall
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    // Check if the player can attack (not on a wall)
    public bool canAttack()
    {
        return !onWall();
    }

    // Coroutine to stop dashing after a set time
    private IEnumerator stopDashing()
    {
        yield return new WaitForSeconds(_dashingTime);
        _trailRenderer.emitting = false;
        _isDashing = false;
    }
}