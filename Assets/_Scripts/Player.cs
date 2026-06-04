using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private int health = 1;
    private Rigidbody2D rb;
    [SerializeField] private float holdDownForce = .5f;
    [SerializeField] private float playerRotationAngleZ = 45;
    [SerializeField] private float rotationSpeed = 8f;      // how fast the tilt follows velocity
    [SerializeField] private float minPlayerY = 5f;         // depth cap — can't dive below -minPlayerY

    private Vector3 originalScale;
    private GameManager gm;
    private PlayerControls controls;
    private bool isDiving;

    private void Awake()
    {
        controls = new PlayerControls();

        // Subscribe to Dive action events — no polling needed
        controls.Gameplay.Dive.performed += _ => { isDiving = true;  Debug.Log("[DIVE] performed — isDiving = true");  };
        controls.Gameplay.Dive.canceled  += _ => { isDiving = false; Debug.Log("[DIVE] canceled  — isDiving = false"); };
    }

    private void OnEnable()  => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = GameManager.Instance;
        originalScale = transform.localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
            ObjectPooler.Instance.ReturnToPool(other.gameObject.tag, other.gameObject);

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
            rb.linearVelocity += new Vector2(0, -holdDownForce);
    }

    private void RotatePlayer()
    {
        // Normalise velocity.y to -1..1 using the max rotation angle as scale
        // → tilts when moving, goes flat (0°) when velocity is near zero (settled)
        float normalizedVel = Mathf.Clamp(rb.linearVelocity.y / playerRotationAngleZ, -1f, 1f);
        float targetAngleZ  = normalizedVel * playerRotationAngleZ;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngleZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void FixedUpdate()
    {
        if (isDiving) PlayerDive();
        RotatePlayer();
    }
}
