using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class ToLevel1 : MonoBehaviour
{
    [SerializeField] private int transitionSceneIndex = 2; // Set this to your transition scene's build index
    [SerializeField] private float delayTime = 30f; // 1 second delay
    
    
    
    void Start()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }
    
    
    
    IEnumerator LoadSceneAfterDelay()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delayTime);
        
        // Load the transition scene
        SceneManager.LoadScene(transitionSceneIndex);
    }
}

