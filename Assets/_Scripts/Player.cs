using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int health = 1;
    private Rigidbody2D rb;
    [SerializeField]private float holdDownForce = .5f;
    [SerializeField]private float playerRotationAngleZ = 45;
    [SerializeField]private float playerRotationThreshold = .1f;
    [SerializeField]private float minPlayerY = 3.5f;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        rb =  GetComponent<Rigidbody2D>();
        gm = GameManager.Instance;
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Coin")){
            ObjectPooler.Instance.ReturnToPool(other.gameObject.tag, other.gameObject);
        }

        if(other.gameObject.CompareTag("Enemy")){
            ObjectPooler.Instance.ReturnToPool(other.gameObject.tag, other.gameObject);
            playerHit();
        }

    }

    private void playerHit(){
        health -= 1;
        if(health <= 0){
            gm.isDead = true;
            gm.RestartGame(GameManager.Instance.isDead);
        }
    }

    private void PlayerDive(){
        if(rb.transform.position.y > -minPlayerY){
            rb.velocity += new Vector2(0, -holdDownForce);
        }
    }

    public void onPlayerHold(){
        if(Input.GetMouseButton(0)){
            PlayerDive();
            
        }
    }
    
    private void RotatePlayer(float height){
        Vector3 eulerAngle = new Vector3(0, 0, playerRotationAngleZ);
        // quaternion.EulerAngles = eulerAngle;
        if(height > playerRotationThreshold){
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(eulerAngle), Time.deltaTime * .5f);
        }

        if(height < playerRotationThreshold){
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(-eulerAngle), Time.deltaTime * .5f);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        onPlayerHold();
        // RotatePlayer(rb.velocity.y);
        // Debug.Log("Velocity: "+rb.velocity);
        // if(rb.transform.position.y < -4){
        //         transform.Rotate(Vector3.up, -playerRotationRate * Time.deltaTime);
        //     }

        // if(Input.GetMouseButtonUp(0) ==  true){
        //     transform.Rotate(Vector3.back, -playerRotationRate * Time.deltaTime);
        // }
    }
}
