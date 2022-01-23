using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;
    bool checkCrouch;
    bool playerHasHorizontalSpeed;
    bool isAlive = true;
    bool isInvulnerable = false;
    


    [Header("Various Movement Speeds")]
    [SerializeField] public float playerHealth = 3f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float meleeAnimation = 0.5f;
    [SerializeField] AudioClip walkSoundSFX;
    [SerializeField] AudioClip jumpSoundSFX;
    [SerializeField] AudioClip knifeSoundSFX;
    [SerializeField] AudioClip slideSoundSFX;
    [Header("Climbing Movement Details")]
    [SerializeField] float climbSpeed = 4f;
    [SerializeField] float climbAnimationSpeed = 1f;
    [SerializeField] AudioClip climbSoundSFX;
    [Header("Getting Hit - Knocking Back Details")]
    [SerializeField] float verticalKnockback = 5f;
    [SerializeField] float horizontalKnockback = 5f;
    [SerializeField] AudioClip hurtSoundSFX;
    [SerializeField] AudioClip deathSoundSFX;
    [Header("Throw Projectile Details")]
    [SerializeField] GameObject throwProjectile;
    [SerializeField] Transform handThrow;
    [SerializeField] float throwDelay = 0.45f;
    [SerializeField] AudioClip throwProjectileSFX;
    [Header("Standing Gun Projectile Details")]
    [SerializeField] GameObject standGunProjectile;
    [SerializeField] Transform standGun;
    [SerializeField] float standGunDelay = 0.45f;
    [SerializeField] bool bulletCreated = false;
    [SerializeField] AudioClip gunSoundSFX;
    [Header("Crouching Gun Projectile Details")]
    [SerializeField] GameObject crouchGunProjectile;
    [SerializeField] Transform crouchGun;



    void Start() //This is where I call all of the items attached to the player
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (!isAlive) { return; }
        Walk();
        FlipSprite();
        ClimbLadder();
        IsGrounded();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>(); //this is to translate the key movement
        moveSpeed = walkSpeed; //This is to dictate the "normal" walking speed
       
    }

    void OnRun(InputValue value)
    {
       if (!isAlive) { return; }  //If I am hurt, I have no control for a brief time

       if (value.isPressed)
        {
            moveSpeed = runSpeed;  //fast speed
        }
        else
        {
            moveSpeed = walkSpeed;  //slow speed
        }
    }

    void OnCrouch(InputValue value)
    {
        if (!isAlive) { return; } //If I am hurt, I have no control for a brief time
        if (value.isPressed)  //If I click the button
        {
            if(playerHasHorizontalSpeed)  //Am I moving or standing still?
            {
                //AudioSource.PlayClipAtPoint(slideSoundSFX, Camera.main.transform.position);  //This works...it just sounds annoying.  Fix one day...
                myAnimator.SetBool("IsSliding", playerHasHorizontalSpeed);
                Invoke("StopSliding", 0.25f);  //I only want this to last a few seconds

            } else
            {
                moveSpeed = 0f;  //to make sure I am not moving
                myAnimator.SetBool("IsCrouching", true);
                checkCrouch = true;
            }
        }
    }

    void OnMelee(InputValue value)
    {
        if (!isAlive) { return; }
        if (value.isPressed)
        {
            AudioSource.PlayClipAtPoint(knifeSoundSFX, Camera.main.transform.position);
            if (checkCrouch)  //Should the knife swipe be standing or crouching?
            {
                moveSpeed = 0f;
                myAnimator.SetBool("IsCrouchingMelee", true);
            }
            else
            {
                moveSpeed = 0f;
                myAnimator.SetBool("IsMelee", true);
            }
            Invoke("StopMelee", meleeAnimation);
        }
    }


    void StopMelee()
    {
        myAnimator.SetBool("IsMelee", false);
        myAnimator.SetBool("IsCrouchingMelee", false);
    }

    void StopSliding()
    {
        //AudioSource.PlayClipAtPoint(slideSoundSFX, Camera.main.transform.position);
        myAnimator.SetBool("IsSliding", false);
    }

    void Walk()
    {
 
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, myRigidbody.velocity.y);  //This actually calculating my speed
        myRigidbody.velocity = playerVelocity;  //This is updating my speed

        playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;  //The "Abs" part means it doesn't matter if I am going left or right...
        
        if (playerHasHorizontalSpeed)
        {
            StopMelee();
            checkCrouch = false;
            //AudioSource.PlayClipAtPoint(walkSoundSFX, Camera.main.transform.position);
        }
        
        if (moveSpeed == runSpeed)  //This is changing the animation based on walking or running speed
        {
            myAnimator.SetBool("IsRunning", playerHasHorizontalSpeed);
            myAnimator.SetBool("IsCrouching", false);
        }

        if (moveSpeed == walkSpeed)
        {
            
            myAnimator.SetBool("IsWalking", playerHasHorizontalSpeed);
            myAnimator.SetBool("IsRunning", false);
            myAnimator.SetBool("IsCrouching", false);
        }

    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;  //assigning an absolute value

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f); //flipping the sprite, based on the direction of movement
        }
    }

    void IsGrounded()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && !myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))  //am I on the ground or on a ladder?
        {
            myAnimator.SetBool("IsJumping", true);
            
        }
        else
        {
            myAnimator.SetBool("IsJumping", false);
        }
    }

    void OnJump(InputValue value)
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && !myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) { return; }

        if (value.isPressed)  //What do I do when I press the "jump" button
        {
            AudioSource.PlayClipAtPoint(jumpSoundSFX, Camera.main.transform.position);
            JumpingScript();
        }

    }

    void OnThrow(InputValue value)
    {
        if (!isAlive) { return; }
        myAnimator.SetTrigger("IsThrowing");  //Start the animation
        Invoke("ThrowProjectile", throwDelay);  //The throw should happen when the release occurs. so I call the creation of the projectile when the hand is in the correct position
    }


    void ThrowProjectile()
    {
        Instantiate(throwProjectile, handThrow.position, transform.rotation);  //Create the projectile
        AudioSource.PlayClipAtPoint(throwProjectileSFX, Camera.main.transform.position);  //create the sound
    }

    void OnShoot(InputValue value)
    {
        if (!isAlive) { return; }  //You can't shoot if you are hurt
        if (!bulletCreated)
        {
            bulletCreated = true;  //So now that the bullet has been created, where is it coming from?
            if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                myAnimator.SetTrigger("IsWalkShoot");  //am I walking & shooting?
            }
            if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                myAnimator.SetTrigger("IsJumpShoot");  //Am I jumping and shooting?
                Debug.Log("jumpshoot");
            }
            AudioSource.PlayClipAtPoint(gunSoundSFX, Camera.main.transform.position);  //Play the sound
            Invoke("ShootProjectile", standGunDelay);  //Sync the animation to when the gun goes off 
        }

    }

    void ShootProjectile()
    {
       if (checkCrouch)  //This is to check the height at which the projectile should be release from
       {
            Instantiate(crouchGunProjectile, crouchGun.position, transform.rotation);  //The first value is the sprite of the bullet, the second is the height & the third is to dictate the motion
       }
       else
       {
            Instantiate(standGunProjectile, standGun.position, transform.rotation);
       }
       bulletCreated = false;
    }

    void JumpingScript()
    {
        myRigidbody.velocity += new Vector2(0f, jumpSpeed);  //This is calculating my jump speed in relation to gravity
    }

    void ClimbLadder()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))  //Are my feet touching the ladder?
        {
            bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon; //to give an absolute value
            Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);  //to figure out climbing speed
            myRigidbody.velocity = climbVelocity;
            myRigidbody.gravityScale = 0f;  //we need this, or will slide off the ladder

            if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                if (playerHasVerticalSpeed)
                {
                    //Debug.Log("Climbing");
                    myAnimator.SetBool("IsClimbing", playerHasVerticalSpeed);  ///If the value is zero, I am not moving & the value is true/false
                    myAnimator.SetBool("IsJumping", false);
                    myAnimator.SetFloat("climbingSpeed", climbAnimationSpeed); //this is to play the animation
                    //AudioSource.PlayClipAtPoint(climbSoundSFX, Camera.main.transform.position);
                }
                if (!playerHasVerticalSpeed)
                {
                    //Debug.Log("Standing on Ladder");
                    myAnimator.SetFloat("climbingSpeed", 0f);  //this is to freeze the animation
                }

            }
        }
        else
        {
            myRigidbody.gravityScale = gravityScaleAtStart;  //Once I am off the ladder, gravity should go back to normal
            myAnimator.SetBool("IsClimbing", false);

        }

    }
    
    public void IncreaseHealth()
    {
        playerHealth += 1;
    }

    void Die()
    {
        if (isInvulnerable) { return; }  //This means a player can't be hit over & over.....
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            ResetBool();
            myRigidbody.gravityScale = 0f;
            myAnimator.SetBool("IsDead", true);
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")))
        {
            isAlive = false;
            playerHealth -= 1; //This subtracts 1 health
            isInvulnerable = true;  //This prevents further damage for the time being
            FindObjectOfType<GameSession>().ProcessPlayerHealth();
            if (playerHealth > 0)
            {
                ResetBool();  //This is a script where I set all of my "myAnimator.SetBool..." options to false, to guarantee that the only animation that plays is the injured one
                AudioSource.PlayClipAtPoint(hurtSoundSFX, Camera.main.transform.position);
                myRigidbody.gravityScale = gravityScaleAtStart; //This is to ensure that if the player is on a ladder, they obey the laws of physics, not the ladder...
                myAnimator.SetBool("IsHurt", true); //start animation
                myRigidbody.AddForce(transform.up*verticalKnockback, ForceMode2D.Impulse); //vertical knockback
                myRigidbody.AddForce(transform.right * horizontalKnockback, ForceMode2D.Impulse);  //horizontal knockback
                Invoke("RemoveInvulnerable", 2f);   //resets all variables with the hurt animation, so we can go back to normal
                Debug.Log(playerHealth);
            }
            else
            {
                ResetBool();
                myAnimator.SetBool("IsDead", true);
                //Invoke("RestartLevel", 3f);  //See below
                AudioSource.PlayClipAtPoint(deathSoundSFX, Camera.main.transform.position);
                FindObjectOfType<GameSession>().ProcessPlayerDeath();
            }           
        }
    }

    void ResetBool()  //This is to reset all animation bools
    {
        myAnimator.SetBool("IsJumping", false);
        myAnimator.SetBool("IsClimbing", false);
        myAnimator.SetBool("IsMelee", false);
    }

    void RemoveInvulnerable()  //This whole thing is to set things back to normal
    {
        isAlive = true;
        isInvulnerable = false;
        myAnimator.SetBool("IsHurt", false);
        myAnimator.SetBool("IsWalking", false);
        moveSpeed = 0f;  //this is needed to ensure you start over with the "idle" animation
    }

  
}
