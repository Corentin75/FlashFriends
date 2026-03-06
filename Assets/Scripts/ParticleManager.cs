using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [Header("References")]
    public GameObject playerInstance;

    [Header("Particles Prefabs")]
    public GameObject scoreParticles;


    private void Awake()
    {
        // singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnParticles(GameObject prefab)
    {
        Vector3 spawnPos = playerInstance.transform.position + playerInstance.transform.forward * 4f + Vector3.up * 2f;
        Instantiate(prefab, spawnPos, playerInstance.transform.rotation);
    }
}
