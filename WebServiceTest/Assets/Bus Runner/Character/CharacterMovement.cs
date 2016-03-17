using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] public float maxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] public float jumpForce = 400f;                  // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Transform m_CeilingCheck;   // A position marking where to check for ceilings
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private bool jumpStarted = false;
    private float jumpTimer = 0;
    public float jumpTimerMax = 0.3f;
    public float jumpBoost = 40.0f;
    private GameObject movingPlatform = null;
    private GameObject lastPlatform = null;

    public float startingSpeed;
    public float speedUpFactor = 1.0f;
    private bool pendingDeath = false;
    private bool accelerating = true;
    public float currentSpeed = 0;


    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_CeilingCheck = transform.Find("CeilingCheck");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        startingSpeed = maxSpeed;
    }


    private void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        //Physics2D.ra
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                //Debug.Log("Grounded");
            }
            //else
            //{
            //    Debug.Log("Flying!");
            //}
            
            
        }
        //if (!m_Grounded) {
        //    Debug.Log("Flying!");
        //}
        m_Anim.SetBool("Ground", m_Grounded);

        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

        if (transform.position.y < -100) {
            Die();
        }
    }


    public void Move(float move, bool crouch, bool jump)
    {
        //if (pendingDeath && m_Rigidbody2D.velocity.y <= 0)
        //{
        //    Die();
        //    return;
        //}

        if (accelerating) {
            currentSpeed += Time.deltaTime * 2.5f;
            if (currentSpeed >= maxSpeed)
                accelerating = false;
        }
        else
        {
            maxSpeed += Time.deltaTime * speedUpFactor;
            currentSpeed = maxSpeed;
        }


        

        // If crouching, check to see if the character can stand up
        if (!crouch && m_Anim.GetBool("Crouch"))
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        // Set whether or not the character is crouching in the animator
        m_Anim.SetBool("Crouch", crouch);

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // Reduce the speed if crouching by the crouchSpeed multiplier
            move = (crouch ? move*m_CrouchSpeed : move);

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            m_Anim.SetFloat("Speed", Mathf.Abs(move));

            // Move the character
            if(movingPlatform != null)
                m_Rigidbody2D.velocity = new Vector2(currentSpeed + movingPlatform.GetComponent<BusMovementScript>().speed, m_Rigidbody2D.velocity.y);
            else
                m_Rigidbody2D.velocity = new Vector2(move * currentSpeed, m_Rigidbody2D.velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
                // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }

        if (CrossPlatformInputManager.GetButtonUp("Jump"))
        {
            jumpStarted = false;
            //Debug.Log("Jump stopped");
        }

        if (jumpStarted && !m_Grounded)
        {
            jumpTimer += Time.deltaTime;
            m_Rigidbody2D.AddForce(new Vector2(0f, jumpBoost));
            if (jumpTimer >= jumpTimerMax)
            {
                jumpStarted = false;
            }
            //Debug.Log("Jumping!");
        }

        // If the player should jump...
        if (m_Grounded && jump && m_Anim.GetBool("Ground"))
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Anim.SetBool("Ground", false);
            m_Rigidbody2D.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            jumpStarted = true;
            jumpTimer = 0;
            //Debug.Log("Jump started");
        }
        
        
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlatformScript>() != null)
        {
            lastPlatform = other.gameObject;

        }
    }

    //void OnCollisionExit2D(Collision2D other)
    //{
    //    if (other.gameObject.tag == "MovingPlatform" && movingPlatform == other.gameObject)
    //    {
    //        transform.SetParent(null);
    //        movingPlatform = null;
    //    }
    //}

    public void Die(){
        if (lastPlatform != null)
        {
            transform.position = new Vector3(lastPlatform.GetComponent<PlatformScript>().GetRect().xMin, lastPlatform.GetComponent<PlatformScript>().GetRect().yMax + 2, 0);
            m_Rigidbody2D.velocity = Vector2.zero;
        }
        else {
            transform.position = GameObject.Find("PlayerSpawnPoint").transform.position;
            m_Rigidbody2D.velocity = Vector2.zero;
        }
        Brake();
    }

    public void TryPlatformKill() {
        if (m_Rigidbody2D.velocity.y <= 0)
            Die();
        //else
        //    pendingDeath = true;
    }

    public void Brake() {
        currentSpeed = 0;
        accelerating = true;
        maxSpeed = startingSpeed;
    }

    //public void Unkill() {
    //    pendingDeath = false;
    //}
}

