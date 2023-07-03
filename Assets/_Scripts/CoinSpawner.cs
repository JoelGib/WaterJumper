using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    ObjectPooler objectPooler;

    [SerializeField]private float spawnRate = 1f;

    private bool canSpawn = true;

    private void Start() {
        objectPooler = ObjectPooler.Instance;

        StartCoroutine(Spawner());
    }
    
    // void FixedUpdate()
    // {
    // //    objectPooler.SpawnFromPool("Enemy", transform.position, Quaternion.identity);
    // }

    void Spawn(){
        objectPooler.SpawnFromPool("Coin", transform.position, Quaternion.identity);
    }

    IEnumerator Spawner(){
        WaitForSeconds wait = new WaitForSeconds(spawnRate);
        while(canSpawn){
            yield return wait;
            Spawn();
        }
    }
}
