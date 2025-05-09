using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private  string sceneToTransition;

    private void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag(playerTag)){
            Debug.Log("Player Collision Detected");
            SceneManager.LoadScene(sceneToTransition);
        }
    }

}
