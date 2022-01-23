using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    PlayerController player;
    CircleCollider2D myGrenadeCollider;
    Animator myAnimator;
    float xSpeed;
    float ySpeed;

    [SerializeField] float grenadeSpeedX = 5f;
    [SerializeField] float grenadeSpeedY = 5f;
    [SerializeField] AudioClip collisionExplosionSFX;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myGrenadeCollider = GetComponent<CircleCollider2D>();
        player = FindObjectOfType<PlayerController>();
        xSpeed = player.transform.localScale.x * grenadeSpeedX;
        ySpeed = player.transform.localScale.y * grenadeSpeedY;
        myRigidbody.velocity = new Vector2(xSpeed, ySpeed);
    }

    void Update()
    {
        //myRigidbody.velocity = new Vector2(xSpeed, ySpeed);
        //myAnimator.SetBool("ThrowProjectile", true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (myGrenadeCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) { return; }
        if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
        EndAnimation();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //if (myBulletCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) { return; }
        EndAnimation();
    }

    void EndAnimation()
    {
        AudioSource.PlayClipAtPoint(collisionExplosionSFX, Camera.main.transform.position);
        //myAnimator.SetBool("ThrowProjectile", false);
        myAnimator.SetTrigger("NotOnGround");
        Invoke("DestroyGrenade", 0.3f);
    }

    void DestroyGrenade()
    {
        Destroy(gameObject);
    }
}
