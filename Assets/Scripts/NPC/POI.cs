using UnityEngine;

public class POI : MonoBehaviour
{
    public enum POIType
    {
        Stage,
        Food,
        Drink,
        Toilets,
        Fountain,
        GreenArea
    }

    public POIType type;

    [Tooltip("How long NPCs stay here in seconds")]
    public float waitTime = 5f;

    [Tooltip("Radius around the POI where NPCs can move")]
    public float radius = 3f;

    // Returns a random point within the radius around the POI
    public Vector3 GetRandomPoint()
    {
        Vector2 random = Random.insideUnitCircle * radius;
        return transform.position + new Vector3(random.x, 0, random.y);
    }
}
