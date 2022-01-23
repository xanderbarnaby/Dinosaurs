using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasChest : MonoBehaviour
{
    [SerializeField] AudioClip hasChestPickupSFX;
    [SerializeField] int pointsForCoinPickup = 10;
    int openChest = 0;
    Animator myAnimator;
    SpriteRenderer mySpriteRenderer;

    [SerializeField] float fadeSpeed = 0.01f;
    bool chestOpen = false;
    float alphaLevel = 1f;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        mySpriteRenderer.color = new Color(1f, 1f, 1f, alphaLevel);
    }

    void Update()
    {
        stopAnimation(); 
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            openChest = FindObjectOfType<GameSession>().keysCollected;
            if (openChest > 0)
            {
                chestOpen = true;
                myAnimator.SetBool("hasKey", true);
                FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickup);  //Reference the GUI to change the score & key amount
                FindObjectOfType<GameSession>().UseKey();
                AudioSource.PlayClipAtPoint(hasChestPickupSFX, Camera.main.transform.position);
                Invoke("stopAnimation", 2f);  //I want to add a delay to this 
                
            }

        }
    }


    void stopAnimation()
    {
        if (!chestOpen) { return; }
        Debug.Log("Chest is Changing");
        //This next session is about having the chest open up & then fade away
        alphaLevel = alphaLevel - fadeSpeed;  
        mySpriteRenderer.color = new Color(1f, 1f, 1f, alphaLevel);
        if (alphaLevel < 0.05)
        {
            chestOpen = false;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
