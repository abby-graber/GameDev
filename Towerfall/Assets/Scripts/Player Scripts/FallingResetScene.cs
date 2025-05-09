using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    [SerializeField] private ManaBar manaBar;
    void Update()
    {
        if (transform.position.y < 0f)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
            manaBar.AddMana(100);// I have added this to reset mana to maximum
        }
    }
}
