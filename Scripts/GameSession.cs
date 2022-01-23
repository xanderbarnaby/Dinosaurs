using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{

    [SerializeField] int playerLives = 3;
    [SerializeField] int coinsCollected = 0;
    [SerializeField] int playerHealthDisplay = 3;
    [SerializeField] float initialTimeRemaining = 500f;
    public int keysCollected = 0;
    public float timeRemaining = 500f;
    


    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI keyText;

    void Awake()  //This section is to load as soon as the game starts
    {
        float playerHealthDisplay = FindObjectOfType<PlayerController>().playerHealth;
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1) ///So, should we start the count over or not/
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()  //all the starting info
    {
        livesText.text = playerLives.ToString();
        coinText.text = coinsCollected.ToString();
        healthText.text = playerHealthDisplay.ToString();
        timeText.text = timeRemaining.ToString();
        keyText.text = keysCollected.ToString();
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        } else
        {
            TakeLife();  //if you run out of time, you die
        }
        DisplayTime(timeRemaining);
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);  //this is to get the code in a readable way
    }

    public void AddToScore(int pointsToAdd)  //counting coins collected & displaying
    {
        coinsCollected += pointsToAdd; 
        coinText.text = coinsCollected.ToString();
    }


    public void ProcessPlayerHealth()  //decreasing health
    {
        playerHealthDisplay -= 1;
        healthText.text = playerHealthDisplay.ToString();
    }

    public void DisplayIncreasePlayerHealth()  //increasing health
    {
        playerHealthDisplay += 1;
        healthText.text = playerHealthDisplay.ToString();
    }

    public void CollectKey()  //Did I collect a key?
    {
        keysCollected += 1;
        keyText.text = keysCollected.ToString();
    }

    public void UseKey()  //Did I use a key?
    {
        keysCollected -= 1;
        keyText.text = keysCollected.ToString();
    }

    public void ProcessPlayerDeath()  //Do I start the level over, or the entire game?
    {
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            ResetGameSession();
        }
    }

    void TakeLife()  //lose life, both here & on the player
    {
        playerLives--;
        timeRemaining = initialTimeRemaining;
        livesText.text = playerLives.ToString();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        playerHealthDisplay = 3;
        healthText.text = playerHealthDisplay.ToString();
    }

    void ResetGameSession()  //starting the game over
    {
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
}
