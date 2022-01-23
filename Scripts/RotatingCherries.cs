using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCherries : MonoBehaviour


{
    public float speed = 1;
    public float RotAngleZ = 45;
    [SerializeField] AudioClip fruitSFX;

    bool wasCollected = false;

    void Update()
    {
        float rZ = Mathf.SmoothStep(-RotAngleZ, RotAngleZ, Mathf.PingPong(Time.time * speed, 1));  //This dode is to rotate the cherries back & forth
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            FindObjectOfType<GameSession>().DisplayIncreasePlayerHealth();
            FindObjectOfType<PlayerController>().IncreaseHealth();
            AudioSource.PlayClipAtPoint(fruitSFX, Camera.main.transform.position);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}

