using UnityEngine;
using TMPro;
using System.Collections;

public class PhotoFeedbackManager : MonoBehaviour
{
    public static PhotoFeedbackManager Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI commentText;

    public float displayTime = 1.5f;

    private Coroutine routine;

    void Awake()
    {
        Instance = this;
    }

    public void ShowFeedback(int score)
    {
        string comment = GetComment(score);

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FeedbackRoutine(score, comment));
    }

    string GetComment(int score)
    {
        if (score <= 0) return "Meh...";
        if (score < 20) return "Not bad!";
        if (score < 50) return "Nice shot!";
        if (score < 100) return "Great photo!";
        if (score < 200) return "Amazing!";
        return "MASTERPIECE!";
    }

    IEnumerator FeedbackRoutine(int score, string comment)
    {
        scoreText.text = "+" + score + " score";
        commentText.text = comment;

        SetAlpha(scoreText, 1);
        SetAlpha(commentText, 1);

        yield return new WaitForSeconds(displayTime);

        float fadeSpeed = 2f;

        while (scoreText.color.a > 0)
        {
            Fade(scoreText, fadeSpeed);
            Fade(commentText, fadeSpeed);
            yield return null;
        }
    }

    void Fade(TextMeshProUGUI text, float speed)
    {
        Color c = text.color;
        c.a -= speed * Time.deltaTime;
        text.color = c;
    }

    void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
