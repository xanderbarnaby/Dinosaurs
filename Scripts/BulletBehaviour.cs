using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    PlayerController player;
    CapsuleCollider2D myBulletCollider;
    Animator myAnimator;
    float xSpeed;

    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] AudioClip bulletCollisionSFX;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBulletCollider = GetComponent<CapsuleCollider2D>();
        player = FindObjectOfType<PlayerController>();
        xSpeed = player.transform.localScale.x*bulletSpeed;
        myAnimator.SetBool("Muzzle", true);
        Invoke("StopMuzzle", 0.3f);
    }

    void StopMuzzle()
    {
        myAnimator.SetBool("Muzzle", false);
    }

    void Update()
    {
        myRigidbody.velocity = new Vector2(xSpeed, 0f);
        myAnimator.SetBool("ShootBullet", true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (myBulletCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) { return; }
        /*if (other.tag == "Enemy")
        {
            Destroy(other.gameObject); 
        }*/
        EndAnimation();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //AudioSource.PlayClipAtPoint(bulletCollisionSFX, Camera.main.transform.position);
        EndAnimation();
    }

    void EndAnimation()
    {
        //AudioSource.PlayClipAtPoint(bulletCollisionSFX, Camera.main.transform.position);
        myAnimator.SetBool("ShootBullet", false);
        myAnimator.SetTrigger("BulletCollision");
        Invoke("DestroyBullet", 0.15f);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
