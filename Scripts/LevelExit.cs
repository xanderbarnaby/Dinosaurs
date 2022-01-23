using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 1f;
    [SerializeField] AudioClip celebrationSFX;
    
    void OnTriggerEnter2D(Collider2D other)  //If I touch the exit, I go to the next stage
    {
        AudioSource.PlayClipAtPoint(celebrationSFX, Camera.main.transform.position);
        StartCoroutine(LoadNextLevel());
    }

    IEnumerator LoadNextLevel()  //This coroutine allows there to be a time delay
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;  //So, do the math & go to the next numerical level
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)  //If I have hit the last leve, start the game over
        {
            nextSceneIndex = 0;
        }

        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(nextSceneIndex);
    }


}
