using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int health = 1;
    private Rigidbody2D rb;
    [SerializeField] private float holdDownForce = .5f;
    [SerializeField] private float playerRotationAngleZ = 45;
    [SerializeField] private float playerRotationThreshold = .1f;
    [SerializeField] private float minPlayerY = 3.5f;
    [SerializeField] private float dampingFactor = 0.95f; // Adjust this value to control the damping effect

    private Vector3 originalScale;
    private GameManager gm;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = GameManager.Instance;
        originalScale = transform.localScale; // Store the original scale of the player
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            ObjectPooler.Instance.ReturnToPool(other.gameObject.tag, other.gameObject);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            ObjectPooler.Instance.ReturnToPool(other.gameObject.tag, other.gameObject);
            playerHit();
        }
    }

    private void playerHit()
    {
        health -= 1;
        if (health <= 0)
        {
            gm.isDead = true;
            gm.RestartGame(GameManager.Instance.isDead);
        }
    }

    private void PlayerDive()
    {
        if (rb.transform.position.y > -minPlayerY)
        {
            // Apply holdDownForce
            rb.velocity += new Vector2(0, -holdDownForce);

            // Apply damping to both x and y components of velocity
            //rb.velocity *= dampingFactor * Time.deltaTime;
        }
    }

    public void onPlayerHold()
    {
        if (Input.GetMouseButton(0))
        {
            PlayerDive();
        }
    }

    private void RotatePlayer(float height)
    {
        Vector3 eulerAngle = height > playerRotationThreshold ?
                             new Vector3(0, 0, playerRotationAngleZ) :
                             new Vector3(0, 0, -playerRotationAngleZ);

        Quaternion targetRotation = Quaternion.Euler(eulerAngle);

        // Adjust the interpolation factor to control the snappiness
        float interpolationFactor = 10f; // Experiment with this value to achieve desired snappiness

        // Use Quaternion.RotateTowards for a more immediate rotation change
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * interpolationFactor);
    }

    void FixedUpdate()
    {
        onPlayerHold();
        RotatePlayer(rb.velocity.y * 2f);

        // Debugging velocity
        Debug.Log("Velocity: " + rb.velocity);
        // Squish and stretch effect based on velocity
        //float stretchFactor = 0.2f; // Adjust how much to stretch based on velocity
        //float squashFactor = 0.2f; // Adjust how much to squash based on velocity

        //Vector3 currentScale = originalScale;
        //currentScale.x += Mathf.Abs(rb.velocity.x) * stretchFactor;
        //currentScale.y -= Mathf.Abs(rb.velocity.y) * squashFactor;

        //// Limit maximum squash to maintain aspect ratio
        //currentScale.y = Mathf.Clamp(currentScale.y, originalScale.y * 0.8f, originalScale.y);

        //transform.localScale = currentScale;
    }
}
