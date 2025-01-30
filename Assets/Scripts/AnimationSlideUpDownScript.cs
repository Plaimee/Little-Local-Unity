using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSlideUpDownScript : MonoBehaviour
{
    public float moveDistance = 0.1f;  // Distance to move up and down
    public float moveSpeed = 0.25f;    // Speed of the movement
    public float rotateAngle = 5f;    // Max angle to rotate left and right
    public float rotateSpeed = 2f;     // Speed of the rotation
    public bool moveUpFirst = true;    // Start moving up first

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingUp;
    private float rotationTime;
    private float positionOffset;      // Random offset for position animation
    private float rotationOffset;      // Random offset for rotation animation

    void Start()
    {
        // Initialize start position
        startPosition = transform.position;
        movingUp = moveUpFirst;

        // Apply random offsets
        positionOffset = Random.Range(0f, moveDistance); // Randomize vertical position offset
        rotationOffset = Random.Range(0f, Mathf.PI * 2); // Randomize rotation phase (0 to 2π)

        // Set the initial position based on the offset
        transform.position = startPosition + Vector3.up * positionOffset;

        // Determine the initial target position
        targetPosition = movingUp
            ? startPosition + Vector3.up * moveDistance
            : startPosition - Vector3.up * moveDistance;
    }

    void Update()
    {
        // Move the object towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Switch direction when reaching the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            movingUp = !movingUp;
            targetPosition = movingUp
                ? startPosition + Vector3.up * moveDistance
                : startPosition - Vector3.up * moveDistance;
        }

        // Add slight left-right rotation with random start phase
        rotationTime += Time.deltaTime * rotateSpeed;
        float rotationZ = Mathf.Sin(rotationTime + rotationOffset) * rotateAngle;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
}
