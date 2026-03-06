using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [Header("References")]
    public GameObject playerInstance;

    [Header("Particle Prefabs")]
    public GameObject scoreParticles;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Spawns a particle effect near the player
    public void SpawnParticles(GameObject prefab)
    {
        Vector3 spawnPos = playerInstance.transform.position + playerInstance.transform.forward * 4f + Vector3.up * 2f;
        Instantiate(prefab, spawnPos, playerInstance.transform.rotation);
    }
}
