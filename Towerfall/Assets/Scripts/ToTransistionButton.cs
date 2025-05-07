using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class SceneTransitionButton : MonoBehaviour
{
    [SerializeField] private int transitionSceneIndex = 1; // Set this to your transition scene's build index
    [SerializeField] private float delayTime = 1f; // 1 second delay
    
    [SerializeField] private SmoothFreeFloatingCharacter mainMenuChar; 
    private Button button;
    
    void Start()
    {
        // Get the Button component
        button = GetComponent<Button>();
        
        // Add click listener
        button.onClick.AddListener(OnButtonClick);
    }
    
    void OnButtonClick()
    {
        // Disable the button to prevent multiple clicks
        button.interactable = false;
        
        // Play animation if animator is assigned
        mainMenuChar.driftSpeed = 3.0f;
        mainMenuChar.rotationSpeed = 15.0f;
        mainMenuChar.maxDriftDistance = 5.0f;
        
        // Start coroutine for delayed scene loading
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

