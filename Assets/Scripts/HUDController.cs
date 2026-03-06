using UnityEngine;
using TMPro;
using System.IO;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI photoText;
    public TextMeshProUGUI questText;

    private string photoFolder;

    private void Awake()
    {
        Instance = this;
        photoFolder = Path.Combine(Application.persistentDataPath, "Photos");
    }

    private void Update()
    {
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (QuestManager.Instance == null)
            return;

        // Score
        scoreText.text = "Score: " + QuestManager.Instance.goodVibesScore;

        // Photos
        int photoCount = 0;
        if (Directory.Exists(photoFolder))
        {
            photoCount = Directory.GetFiles(photoFolder, "*.png").Length;
        }

        photoText.text = "Photos taken: " + photoCount;

        // Quests
        int questCount = QuestManager.Instance.activeQuests.Count;

        questText.text = "Quests left: " + questCount;
    }
}
