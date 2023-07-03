using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour, IPooledObject
{
    public float speed = 1f;
    private Rigidbody2D rb2d;

    // Start is called before the first frame update
    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }
    public void OnObjectSpawn()
    {
        // Add Code to cause behaviour when Object is Spawned
    }

    

    // Update is called once per frame
    void Update()
    {
        UpdateSpeedX(new Vector2(-speed, 0));

        if(isOutOfBounds()){
            DestroyObject();
        }
    }

    private void UpdateSpeedX(Vector2 objSpeed){
        rb2d.velocity = objSpeed;
    }

    private void DestroyObject(){
        Debug.Log("Destory");
        ObjectPooler.Instance.ReturnToPool(gameObject.tag, gameObject);
    }

    private bool isOutOfBounds(){
        return transform.position.x < -20;
    }
}
