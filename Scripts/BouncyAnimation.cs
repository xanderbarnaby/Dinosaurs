using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D bouncyRigidbody;
    Animator bouncyAnimation;
    CircleCollider2D bouncyCollider;
    AudioSource bouncyAudioSource;
    [SerializeField] AudioClip bounceSFX;
    void Start()
    {
        bouncyRigidbody = GetComponent<Rigidbody2D>();
        bouncyAnimation = GetComponent<Animator>();
        bouncyCollider = GetComponent<CircleCollider2D>();

    }

    void Update()
    {
        checkBounce();
    }

    void checkBounce()
    {
        if(!bouncyCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            return;
        }
        bouncyAnimation.SetBool("IsTrampoline", true);
        AudioSource.PlayClipAtPoint(bounceSFX, Camera.main.transform.position);
        Invoke("stopAnimation", 0.5f);
    }

    void stopAnimation()
    {
        bouncyAnimation.SetBool("IsTrampoline", false);
    }

}
