using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    public int pointValue = 1;
    private Animator animator;
    private Collider2D myCollider2D;
    private bool isCollected = false;
    public AudioClip collectSound;
    private AudioSource audioSource;

    private void Start()
    {
        isCollected = false;
        animator = GetComponent<Animator>();
        myCollider2D = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;

            // Disable collider to ensure no further collisions
            myCollider2D.enabled = false;

            // Play collect sound
            if (collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }

            // Play the collected animation immediately
            if (animator != null)
            {
                animator.SetTrigger("Collected");
            }

            // Add points to the GameManager
            GameManager.instance.AddPoints(pointValue);

            // Wait for animation to complete before destroying the object
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        if (animator != null)
        {
            // Get the animation's runtime length
            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(animationState.length + 0.1f);
        }

        // Destroy the collectible object after the animation finishes
        Destroy(gameObject);
    }
}
