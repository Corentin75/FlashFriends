using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPhotoQuest", menuName = "PhotoQuest")]
public class PhotoQuest : ScriptableObject
{
    public string questTitle;
    [TextArea] public string description;

    [Header("Requirements")]
    public List<TagRequirement> requiredTags = new List<TagRequirement>();

    public int rewardPoints = 0;
    [HideInInspector] public bool completed = false;
}

[System.Serializable]
public class TagRequirement
{
    public string tag;     // e.g. "Male", "Female"
    public int count = 1;  // how many of this tag need to be in the photo
}
