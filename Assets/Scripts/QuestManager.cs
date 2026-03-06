using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<PhotoQuest> activeQuests = new List<PhotoQuest>();
    public List<PhotoQuest> completedQuests = new List<PhotoQuest>();

    public int goodVibesScore = 0;

    void Awake()
    {
        Instance = this;

        // Réinitialise les quêtes à false au lancement
        foreach (PhotoQuest quest in activeQuests)
            quest.completed = false;
    }

    public int CheckPhotoAndReturnScore(List<GameObject> objectsInPhoto)
    {
        int photoScore = 0;

        // Score from NPCs
        foreach (GameObject obj in objectsInPhoto)
        {
            NPCController npc = obj.GetComponent<NPCController>();
            if (npc != null)
                photoScore += npc.GetPoseScore();
        }

        goodVibesScore += photoScore;

        // Copy active quests to avoid modifying the list while iterating
        List<PhotoQuest> questsCopy = new List<PhotoQuest>(activeQuests);

        foreach (PhotoQuest quest in questsCopy)
        {
            if (quest.completed || quest.requiredTags == null || quest.requiredTags.Count == 0)
                continue;

            bool questCompleted = true;

            // AND logic: all tag requirements must be satisfied
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
                    questCompleted = false; // requirement not met
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

    void CompleteQuest(PhotoQuest quest)
    {
        quest.completed = true;
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        goodVibesScore += quest.rewardPoints;

        ParticleManager.Instance.SpawnParticles(ParticleManager.Instance.scoreParticles);

        Debug.Log("Quest completed: " + quest.questTitle);

        // end game
        if (activeQuests.Count == 0)
            GameManager.Instance.EndGame();
    }

    // Determines if a tag is relevant for scoring bonus
    public bool IsTagRelevant(string tag)
    {
        foreach (PhotoQuest quest in activeQuests)
        {
            foreach (TagRequirement req in quest.requiredTags)
            {
                if (req.tag == tag)
                    return true;
            }
        }
        return false;
    }
}
