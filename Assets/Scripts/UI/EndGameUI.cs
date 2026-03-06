using UnityEngine;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI questsText;
    public TextMeshProUGUI photosText;

    // Updates the endgame stats
    public void UpdateStats()
    {
        int score = QuestManager.Instance.goodVibesScore;
        int quests = QuestManager.Instance.completedQuests.Count;
        int photos = PhotoManager.Instance.totalPhotosTaken;

        scoreText.text = "Final score: " + score;
        questsText.text = "Quests completed: " + quests;
        photosText.text = "Photos taken: " + photos;
    }
}
