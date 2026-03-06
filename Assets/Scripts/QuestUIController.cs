using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestUIController : MonoBehaviour
{
    public static QuestUIController Instance;

    [Header("Panels")]
    public GameObject questPanel;         // main quest panel
    public GameObject questDetailPanel;   // panel showing details
    public TextMeshProUGUI detailTitle;
    public TextMeshProUGUI detailDescription;
    public TextMeshProUGUI detailReward;

    [Header("Quest List")]
    public Transform questListContainer;  // parent for quest buttons
    public GameObject questButtonPrefab;  // prefab with TMP + Button

    private List<GameObject> questButtons = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Opens the quest panel and refreshes the list
    public void OpenQuestPanel()
    {
        questPanel.SetActive(true);
        RefreshQuestList();
    }

    // Closes quest panels
    public void CloseQuestPanel()
    {
        questPanel.SetActive(false);
        questDetailPanel.SetActive(false);
    }

    // Populates the list of active quests
    public void RefreshQuestList()
    {
        // Removes old buttons
        foreach (GameObject btn in questButtons)
            Destroy(btn);

        questButtons.Clear();

        foreach (PhotoQuest quest in QuestManager.Instance.activeQuests)
        {
            GameObject newBtn = Instantiate(questButtonPrefab, questListContainer);
            TextMeshProUGUI text = newBtn.GetComponentInChildren<TextMeshProUGUI>();
            text.text = quest.questTitle;

            Button btn = newBtn.GetComponent<Button>();
            btn.onClick.AddListener(() => ShowQuestDetails(quest));

            questButtons.Add(newBtn);
        }
    }

    // Shows details of a single quest
    public void ShowQuestDetails(PhotoQuest quest)
    {
        questDetailPanel.SetActive(true);

        detailTitle.text = quest.questTitle;
        detailDescription.text = quest.description;
        detailReward.text = "Reward: " + quest.rewardPoints + " score!";
    }
}
