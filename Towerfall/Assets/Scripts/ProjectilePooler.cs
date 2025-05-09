using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int poolSize = 10;

    private List<GameObject> projectilePool;

    void Awake()
    {
        projectilePool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(projectilePrefab);
            proj.SetActive(false);
            projectilePool.Add(proj);
        }
    }

    public GameObject GetPooledProjectile()
    {
        foreach (GameObject proj in projectilePool)
        {
            if (!proj.activeInHierarchy)
            {
                return proj;
            }
        }

        // Optional: expand pool
        GameObject newProj = Instantiate(projectilePrefab);
        newProj.SetActive(false);
        projectilePool.Add(newProj);
        return newProj;
    }
}
