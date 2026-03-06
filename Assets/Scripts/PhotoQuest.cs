using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPhotoQuest", menuName = "PhotoQuest")]
public class PhotoQuest : ScriptableObject
{
    public string questTitle;
    [TextArea] public string description;

    // Chaque string est le tag qu'on doit trouver dans la photo
    public List<string> requiredTags = new List<string>();

    public int rewardPoints = 0;

    [HideInInspector] public bool completed = false;
}
