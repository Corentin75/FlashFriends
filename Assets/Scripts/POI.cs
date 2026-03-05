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

    [Tooltip("How long NPCs stay here")]
    public float waitTime = 5f;

    [Tooltip("Radius around the POI where NPCs can move")]
    public float radius = 3f;

    public Vector3 GetRandomPoint()
    {
        Vector2 random = Random.insideUnitCircle * radius;
        Vector3 point = transform.position + new Vector3(random.x, 0, random.y);
        return point;
    }
}
