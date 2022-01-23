using UnityEngine;

public class RaptorMovement : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    BoxCollider2D floorCollider;
    PolygonCollider2D myBodyCollider;
    SpriteRenderer mySpriteRenderer;
    [Header("Enemy Attributes")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float scaleDino = 1f;

    [Header("Death Animation")]
    [SerializeField] float fadeSpeed = 0.01f;
    [SerializeField] AudioClip raptorAttackSFX;
    [SerializeField] AudioClip raptorDeathSFX;
    [SerializeField] AudioClip bulletCollisionSFX;
    bool fadeDeathCheck = false;
    float alphaLevel = 1f;

    public static object Alpha { get; private set; }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        floorCollider = GetComponent<BoxCollider2D>();
        myBodyCollider = GetComponent<PolygonCollider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        mySpriteRenderer.color = new Color(1f, 1f, 1f, alphaLevel);
    }

    void Update()
    {
        myRigidbody.velocity = new Vector2(moveSpeed, 0f);
        myAnimator.SetBool("IsWalking", true);
        FadeDeathAnimation();  //Is he dead?  If so, start the death animation
    }

    void OnTriggerExit2D(Collider2D other)  //So, if the toe collider hits the edge of the plaform, flip the sprite & go the opposite direction
    {
        moveSpeed = -moveSpeed;
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidbody.velocity.x))*scaleDino, 1f*scaleDino);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ammo")  //If the enemy is hit by a bullet
        {
            AudioSource.PlayClipAtPoint(bulletCollisionSFX, Camera.main.transform.position);
            StopAnimation();  //Stop all animation commands
            myAnimator.SetBool("IsDead", true);
            moveSpeed = moveSpeed * -0.0001f;  //The dino should stop moving when its dead
            Invoke("FadeDeathTrue", 0.5f);
        }
    }

    void FadeDeathTrue()
    {
        fadeDeathCheck = true;
    }

    void StopAnimation()
    {
        AudioSource.PlayClipAtPoint(raptorDeathSFX, Camera.main.transform.position);
        myAnimator.SetBool("IsWalking", false);
        myAnimator.SetBool("IsRunning", false);
        myAnimator.SetBool("IsAttacking", false);

    }

    void FadeDeathAnimation()
    {
        if (!fadeDeathCheck) { return; }
        Debug.Log("This is working");
        alphaLevel = alphaLevel - fadeSpeed;
        mySpriteRenderer.color = new Color(1f, 1f, 1f, alphaLevel);
        if (alphaLevel < 0.05)
        {
            fadeDeathCheck = false;
            Destroy(gameObject);
        }
    }
}
