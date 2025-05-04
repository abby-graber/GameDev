using System.Collections;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    public MonoBehaviour movementScript; 
    public MonoBehaviour cameraScript;
    public MonoBehaviour attackScript; 
    public float freezeDuration = 3f;

    void Start()
    {
        StartCoroutine(FreezePlayerAndCamera());
    }

    IEnumerator FreezePlayerAndCamera()
    {
        // Disable movement and camera
        movementScript.enabled = false;
        cameraScript.enabled = false;
        attackScript.enabled = false;

        yield return new WaitForSeconds(freezeDuration);

        // Re-enable movement and camera
        movementScript.enabled = true;
        cameraScript.enabled = true;
        attackScript.enabled = true;
    }
}
