using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUpDown : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 3f;
    private Vector3 startingPosition;
    private bool movingUp = true;
    private bool stopMovement = false;

    void Start()
    {
        // Store the initial position
        startingPosition = transform.position;
    }

    void Update()
    {
        // Stop enemy movement if collision occurs
        if (stopMovement)
        {
            return;
        }

        // Calculate the target position
        Vector3 targetPosition = startingPosition + (movingUp ? Vector3.up : Vector3.down) * moveDistance;

        // Move the enemy towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the enemy has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            movingUp = !movingUp;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop the enemy's movement
            stopMovement = true;

            // Stop the player's movement
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
                playerRb.isKinematic = true;
            }

            // Trigger the player's death animation
            PlayerMovement playerScript = other.GetComponent<PlayerMovement>();
            if (playerScript != null)
            {
                playerScript.Die();
            }

            // Delay scene reset to allow death animation to play
            StartCoroutine(RestartSceneWithDelay(playerRb));
        }
    }

    private IEnumerator RestartSceneWithDelay(Rigidbody2D playerRb)
    {
        yield return new WaitForSeconds(1.5f);

        // Reactivate the player's Rigidbody for the next scene
        if (playerRb != null)
        {
            playerRb.isKinematic = false;
        }

        // Restart the scene
        GameManager.instance.RestartScene();
    }
}
