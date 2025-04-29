using UnityEngine;

public class RestrictCamera : MonoBehaviour
{
    public Vector3 roomCenter = Vector3.zero;
    public float roomRadius = 33f;
    public float minY = 1f;
    public float maxY = 10f;
    public float speed = 5f;

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = input.normalized * speed * Time.deltaTime;

        Vector3 nextPosition = transform.position + move;

        // Clamp to circular boundary
        Vector3 offset = nextPosition - roomCenter;
        Vector2 offsetXZ = new Vector2(offset.x, offset.z);

        if (offsetXZ.magnitude > roomRadius)
            offsetXZ = offsetXZ.normalized * roomRadius;

        float clampedY = Mathf.Clamp(nextPosition.y, minY, maxY);

        transform.position = new Vector3(
            roomCenter.x + offsetXZ.x,
            clampedY,
            roomCenter.z + offsetXZ.y
        );
    }
}
