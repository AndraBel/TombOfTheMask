using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FinishTile : MonoBehaviour
{
    public float upwardSpeed = 10f;
    public float rotationSpeed = 360f;
    public string nextSceneName = "Level2";
    public float transitionDelay = 2f;

    private bool levelCompleted = false;

    [Header("UI Elements")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI nextLevelText;

    [Header("Final Level Settings")]
    public bool isFinalLevel = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !levelCompleted)
        {
            levelCompleted = true;

            // Display final score and next level name
            DisplayScoreAndLevelName();

            // Start moving the player upward
            StartCoroutine(MoveAndRotatePlayerUpAndTransition(other.transform));
        }
    }

    private void DisplayScoreAndLevelName()
    {
        // Hide existing score text to prevent overlap
        if (GameManager.instance.scoreText != null)
        {
            GameManager.instance.scoreText.gameObject.SetActive(false);
        }

        // Show final score and appropriate message
        if (finalScoreText != null && nextLevelText != null)
        {
            finalScoreText.text = $"Final Score: {GameManager.instance.GetScore()}";

            if (isFinalLevel)
            {
                nextLevelText.text = "End of Game";
            }
            else
            {
                nextLevelText.text = $"Next Level: {nextSceneName}";
            }

            finalScoreText.gameObject.SetActive(true);
            nextLevelText.gameObject.SetActive(true);
        }
    }

    private IEnumerator MoveAndRotatePlayerUpAndTransition(Transform player)
    {
        float elapsedTime = 0f;

        // Continuously move and rotate the player upward
        while (elapsedTime < transitionDelay)
        {
            if (player != null)
            {
                // Move the player upwards
                player.position += Vector3.up * upwardSpeed * Time.deltaTime;

                player.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Transition to the next level
        SceneManager.LoadScene(nextSceneName);
    }
}
