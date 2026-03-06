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

        foreach (GameObject obj in objectsInPhoto)
        {
            NPCController npc = obj.GetComponent<NPCController>();
            if (npc != null)
            {
                photoScore += npc.GetPoseScore();
            }

            if (IsTagRelevant(obj.tag))
            {
                photoScore += 10;
            }
        }

        // Mise à jour score global
        goodVibesScore += photoScore;

        // Vérification des quêtes
        List<PhotoQuest> questsCopy = new List<PhotoQuest>(activeQuests);
        foreach (PhotoQuest quest in questsCopy)
        {
            if (quest.completed) continue;
            if (quest.requiredTags == null || quest.requiredTags.Count == 0) continue;

            bool allConditionsMet = true;
            foreach (string tag in quest.requiredTags)
            {
                bool found = false;
                foreach (GameObject obj in objectsInPhoto)
                {
                    if (obj.CompareTag(tag))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    allConditionsMet = false;
                    break;
                }
            }

            if (allConditionsMet)
            {
                CompleteQuest(quest);
                photoScore += quest.rewardPoints; // bonus Good Vibes pour quête complétée
            }
        }

        Debug.Log("Photo Good Vibes score: " + photoScore + ", Total: " + goodVibesScore);

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
    }

    // Détermine si le tag d'un objet est pertinent pour le scoring bonus
    public bool IsTagRelevant(string tag)
    {
        foreach (PhotoQuest quest in activeQuests)
        {
            if (quest.requiredTags.Contains(tag))
                return true;
        }
        return false;
    }
}
