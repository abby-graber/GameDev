using UnityEngine;

public class SmoothFreeFloatingCharacter : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] public float driftSpeed = 1.0f;
    [SerializeField] public float rotationSpeed = 10f;
    [SerializeField] public float maxDriftDistance = 2f;
    
    [Header("Noise Settings")]
    [SerializeField] private float noiseFrequency = 0.3f;
    [SerializeField] private float verticalNoiseFrequency = 0.2f;
    [SerializeField] private float verticalAmplitude = 0.5f;

    
    
    private Vector3 startPosition;
    private Vector3 noiseOffset;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private float rotationBlend = 0f;
    private float rotationChangeTimer;
    
    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        targetRotation = Random.rotation;
        
        // Use random offsets to ensure different characters move differently
        noiseOffset = new Vector3(
            Random.Range(0f, 100f),
            Random.Range(0f, 100f),
            Random.Range(0f, 100f)
        );
        
        // Set a random timer for first rotation change
        rotationChangeTimer = Random.Range(3f, 6f);
    }
    
    private void Update()
    {
        
        float time = Time.time * driftSpeed;
        
        // Calculate smooth position offsets using Perlin noise
        float xOffset = Mathf.PerlinNoise(time * noiseFrequency + noiseOffset.x, noiseOffset.y) * 2 - 1;
        float zOffset = Mathf.PerlinNoise(noiseOffset.z, time * noiseFrequency + noiseOffset.x) * 2 - 1;
        float yOffset = Mathf.PerlinNoise(time * verticalNoiseFrequency + noiseOffset.y, noiseOffset.z) * 2 - 1;
        
        
        Vector3 offset = new Vector3(
            xOffset * maxDriftDistance,
            yOffset * verticalAmplitude,
            zOffset * maxDriftDistance
        );
        
        transform.position = startPosition + offset;
        
        
        rotationChangeTimer -= Time.deltaTime;
        if (rotationChangeTimer <= 0)
        {
            // Set new target rotation
            targetRotation = Random.rotation;
            rotationBlend = 0f;
            
            
            rotationChangeTimer = Random.Range(5f, 10f);
        }
        
        
        rotationBlend += Time.deltaTime * rotationSpeed * 0.1f;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationBlend * Time.deltaTime);
    }
}