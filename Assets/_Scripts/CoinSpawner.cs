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
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    void Spawn(){
        objectPooler.SpawnFromPool("Coin", transform.position, Quaternion.identity);
    }
}
