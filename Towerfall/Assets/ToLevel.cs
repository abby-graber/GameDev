using UnityEngine;
using UnityEngine.SceneManagement;

public class ToLevel : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string sceneToLoad;
    

    private void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag(playerTag)){
            Debug.Log("Collision Detected");
            SceneManager.LoadScene(sceneToLoad);
        }

    }

}
