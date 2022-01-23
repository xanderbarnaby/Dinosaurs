using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningFruit : MonoBehaviour
{
    [SerializeField] AudioClip fruitSFX;

    bool wasCollected = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 5, 0);  //The animation code
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            FindObjectOfType<GameSession>().DisplayIncreasePlayerHealth();  //If I eat fruit, I need to change the health on the player AND on the interfaces
            FindObjectOfType<PlayerController>().IncreaseHealth();
            AudioSource.PlayClipAtPoint(fruitSFX, Camera.main.transform.position);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
