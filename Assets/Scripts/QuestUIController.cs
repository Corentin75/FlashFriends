using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestUIController : MonoBehaviour
{
    public static QuestUIController Instance;

    [Header("Panels")]
    public GameObject questPanel;         // Panel principal de quêtes
    public GameObject questDetailPanel;   // Panel pour les détails
    public TextMeshProUGUI detailTitle;
    public TextMeshProUGUI detailDescription;
    public TextMeshProUGUI detailReward;

    [Header("Quest List")]
    public Transform questListContainer;  // parent pour les boutons de quêtes
    public GameObject questButtonPrefab;  // prefab bouton (TextMeshPro + Button)

    private List<GameObject> questButtons = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void OpenQuestPanel()
    {
        questPanel.SetActive(true);
        RefreshQuestList();
    }

    public void CloseQuestPanel()
    {
        questPanel.SetActive(false);
        questDetailPanel.SetActive(false);
    }

    public void RefreshQuestList()
    {
        // Supprime anciens boutons
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

    public void ShowQuestDetails(PhotoQuest quest)
    {
        questDetailPanel.SetActive(true);

        detailTitle.text = quest.questTitle;
        detailDescription.text = quest.description;
        detailReward.text = "Reward: " + quest.rewardPoints + " score!";
    }
}
