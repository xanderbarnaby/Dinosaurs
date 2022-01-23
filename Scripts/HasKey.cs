using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasKey : MonoBehaviour
{
    [SerializeField] AudioClip hasKeyPickupSFX;

    public bool hasKey = false;
    void Update()
    {
        transform.Rotate(0, 2, 0);  //This is to animate the key
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !hasKey)
        {
            hasKey = true;
            FindObjectOfType<GameSession>().CollectKey();  //Update the GUI
            AudioSource.PlayClipAtPoint(hasKeyPickupSFX, Camera.main.transform.position);  //Play the SFX
            gameObject.SetActive(false);
            Destroy(gameObject);  //have the key disappear
        }
    }
}
