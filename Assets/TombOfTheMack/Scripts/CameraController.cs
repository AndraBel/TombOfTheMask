using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public bool followX = true;
    public bool followY = false;
    public float offset = 0f;

    void Update()
    {
        if (player != null)
        {
            // Get the current camera position
            Vector3 cameraPosition = transform.position;

            // Update the camera position based on the player position
            if (followX)
                cameraPosition.x = player.position.x + offset;

            if (followY)
                cameraPosition.y = player.position.y + offset;

            // Apply the updated camera position
            transform.position = cameraPosition;
        }
    }
}
