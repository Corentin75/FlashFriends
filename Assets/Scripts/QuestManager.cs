using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<PhotoQuest> activeQuests = new List<PhotoQuest>();
    public List<PhotoQuest> completedQuests = new List<PhotoQuest>();

    public int goodVibesScore = 0;

    private void Awake()
    {
        Instance = this;

        // Resets all quests at start
        foreach (PhotoQuest quest in activeQuests)
            quest.completed = false;
    }

    // Checks a photo against all active quests and calculate score
    public int CheckPhotoAndReturnScore(List<GameObject> objectsInPhoto)
    {
        int photoScore = 0;

        // Adds NPC pose scores
        foreach (GameObject obj in objectsInPhoto)
        {
            if (obj.TryGetComponent<NPCController>(out NPCController npc))
                photoScore += npc.GetPoseScore();
        }

        goodVibesScore += photoScore;

        // Copy to avoid modifying list while iterating
        List<PhotoQuest> questsCopy = new List<PhotoQuest>(activeQuests);

        foreach (PhotoQuest quest in questsCopy)
        {
            if (quest.completed || quest.requiredTags == null || quest.requiredTags.Count == 0)
                continue;

            bool questCompleted = true;

            // All tag requirements must be satisfied
            foreach (TagRequirement req in quest.requiredTags)
            {
                int countFound = 0;

                foreach (GameObject obj in objectsInPhoto)
                {
                    if (obj.CompareTag(req.tag))
                        countFound++;
                }

                if (countFound < req.count)
                {
                    questCompleted = false;
                    break;
                }
            }

            if (questCompleted)
            {
                CompleteQuest(quest);
                photoScore += quest.rewardPoints;
            }
        }

        return photoScore;
    }

    // Marks quest as completed
    private void CompleteQuest(PhotoQuest quest)
    {
        quest.completed = true;
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        goodVibesScore += quest.rewardPoints;

        ParticleManager.Instance.SpawnParticles(ParticleManager.Instance.scoreParticles);

        Debug.Log("Quest completed: " + quest.questTitle);

        // Ends the game if there are no active quests left
        if (activeQuests.Count == 0)
            GameManager.Instance.EndGame();
    }

    // Checks if a tag is relevant to any active quest
    public bool IsTagRelevant(string tag)
    {
        foreach (PhotoQuest quest in activeQuests)
        {
            foreach (TagRequirement req in quest.requiredTags)
            {
                if (req.tag == tag) return true;
            }
        }
        return false;
    }
}
