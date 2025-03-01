using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private int score = 0;
    public TextMeshProUGUI scoreText;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        ResetScore();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void AddPoints(int points)
    {
        score += points;
        UpdateScoreText();
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Points: " + score.ToString();
        }
    }

    private void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

    public void RestartScene()
    {
        ResetScore();

        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reassign the scoreText reference when a new scene loads
        if (GameObject.Find("Points") != null)
        {
            scoreText = GameObject.Find("Points").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("Points object not found in the scene!");
        }

        // Update the score display after reassigning the reference
        UpdateScoreText();
    }
}
