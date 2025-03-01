using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRise : MonoBehaviour
{
    public float riseSpeed = 1f;
    public Vector3 initialScale;
    public Vector3 initialPosition;
    public float acceleration = 0.1f;
    public ParticleSystem waterParticles;
    public AudioClip riseSound;
    public AudioClip hurtSound;

    private AudioSource riseAudioSource;
    private AudioSource hurtAudioSource;
    private bool isGrowing = false;

    private void Start()
    {
        // Store the initial scale and position
        initialScale = transform.localScale;
        initialPosition = transform.position;

        if (waterParticles != null)
        {
            waterParticles.Clear();
        }

        // Setup the audio sources
        riseAudioSource = gameObject.AddComponent<AudioSource>();
        hurtAudioSource = gameObject.AddComponent<AudioSource>();

        // Configure rise sound audio source
        if (riseSound != null)
        {
            riseAudioSource.clip = riseSound;
            riseAudioSource.loop = true;
            riseAudioSource.volume = 0.3f;
        }

        // Configure hurt sound audio source
        hurtAudioSource.playOnAwake = false;
        hurtAudioSource.volume = 1f;
    }

    private void Update()
    {
        // Start the water growth when any arrow or WASD key is pressed
        if (!isGrowing && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
                           Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            StartGrowing();
        }

        if (isGrowing)
        {
            // Increase rise speed over time
            riseSpeed += acceleration * Time.deltaTime;

            // Increase the water's height by modifying its scale
            transform.localScale += new Vector3(0, riseSpeed * Time.deltaTime, 0);

            // Adjust the position to keep the bottom fixed
            transform.position = new Vector3(transform.position.x,
                                             initialPosition.y + (transform.localScale.y - initialScale.y) / 2,
                                             transform.position.z);

            // Synchronize the particle system's position
            if (waterParticles != null)
            {
                Vector3 particlePosition = transform.position;
                particlePosition.y += transform.localScale.y / 2 - 0.9f;
                waterParticles.transform.position = particlePosition;
            }
        }
    }

    private void StartGrowing()
    {
        isGrowing = true;

        // Start playing the rise sound
        if (riseSound != null && riseAudioSource != null)
        {
            riseAudioSource.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Play the hurt sound
            PlayHurtSound();

            // Get the player's animator and play the death animation
            Animator playerAnimator = other.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("IsDead", true);
            }

            StartCoroutine(RestartSceneAfterDelay(other));
        }
    }

    private void PlayHurtSound()
    {
        if (hurtSound != null && hurtAudioSource != null)
        {
            hurtAudioSource.PlayOneShot(hurtSound);
        }
    }

    private IEnumerator RestartSceneAfterDelay(Collider2D player)
    {
        yield return new WaitForSeconds(1.5f);

        // Reset the death animation to idle state
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsDead", false);
        }

        // Restart the scene through the GameManager
        GameManager.instance.RestartScene();
    }
}
