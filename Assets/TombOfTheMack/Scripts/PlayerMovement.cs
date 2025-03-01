using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 25f;
    private Vector2 direction;
    private bool canMove = true;

    public LayerMask whatIsWall;
    public LayerMask whatIsSpike;

    private Rigidbody2D rb;
    private Animator animator;
    public AudioClip hurtSound;
    public AudioClip jumpSound;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Allow movement input only if grounded
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                SetDirection(Vector2.up);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                SetDirection(Vector2.down);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                SetDirection(Vector2.left);
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                SetDirection(Vector2.right);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void FixedUpdate()
    {
        // Apply velocity for continuous movement
        if (!canMove)
        {
            rb.velocity = direction * speed;

            // Update the IsMoving parameter based on velocity
            animator.SetBool("IsMoving", rb.velocity.magnitude > 0.1f);

            // Continuously check for collisions
            CheckCollision();
        }
        else
        {
            // If no input, set IsMoving to false
            animator.SetBool("IsMoving", false);
        }
    }

    private void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
        canMove = false;

        // Play jump sound
        PlaySound(jumpSound);

        // Immediately set IsMoving to true
        animator.SetBool("IsMoving", true);
    }

    private void CheckCollision()
    {
        float rayLength = 0.6f;
        Debug.DrawRay(transform.position, direction * rayLength, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayLength, whatIsWall | whatIsSpike);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Walls"))
            {
                // Snap the player to the nearest grid position based on collision point
                Vector3 hitPosition = hit.point - direction * 0.1f;

                // Rotate player to face the wall
                FaceWall(hit.normal);

                rb.velocity = Vector2.zero;
                animator.SetBool("IsMoving", false);
                canMove = true;
            }
            else if (hit.collider.CompareTag("Spikes"))
            {
                // Trigger death animation and restart the scene
                Die();
            }
        }
    }

    private void FaceWall(Vector2 wallNormal)
    {
        float angle = Mathf.Atan2(wallNormal.y, wallNormal.x) * Mathf.Rad2Deg;

        // Rotate the player so that its feet align with the wall
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    public void Die()
    {
        // Stop the player's movement
        rb.velocity = Vector2.zero;
        canMove = false;

        // Rotate the player so the feet are on the ground
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Play the death animation
        animator.SetBool("IsDead", true);

        // Play the hurt sound
        if (hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        // Start a coroutine to reset the scene after a delay
        StartCoroutine(PlayDeathAnimationAndRestart());
    }

    private IEnumerator PlayDeathAnimationAndRestart()
    {
        // Wait for the death animation to complete
        yield return new WaitForSeconds(1.4f);

        // Reset the death animation parameter
        animator.SetBool("IsDead", false);

        // Restart the scene using the GameManager
        GameManager.instance.RestartScene();
    }
}
