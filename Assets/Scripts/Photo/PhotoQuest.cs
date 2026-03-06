using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPhotoQuest", menuName = "PhotoQuest")]
public class PhotoQuest : ScriptableObject
{
    public string questTitle;

    [TextArea]
    public string description;

    [Header("Requirements")]
    public List<TagRequirement> requiredTags = new List<TagRequirement>();

    public int rewardPoints = 0;

    [HideInInspector]
    public bool completed = false; // tracks if the quest is done
}

// Defines how many of a specific tag must be in the photo
[System.Serializable]
public class TagRequirement
{
    public string tag;     // ex: "Male", "Female", "Fountain"
    public int count = 1;  // required number of objects with this tag
}
