using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Transform shootPoint;
    public float projectileForce = 500f;
    public ProjectilePooler pooler;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject projectile = pooler.GetPooledProjectile();
            projectile.transform.position = shootPoint.position;
            projectile.transform.rotation = shootPoint.rotation;
            projectile.SetActive(true);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero; // reset any existing motion
            rb.AddForce(shootPoint.forward * projectileForce, ForceMode.Impulse);
        }
    }
}
