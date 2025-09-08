using UnityEngine;

public class TimingIndicatorSpawner : MonoBehaviour
{

    public void SpawnIndicator(GameObject prefabToSpawn)
    {
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
    }
}