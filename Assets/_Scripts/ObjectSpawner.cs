using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    ObjectPooler objectPooler;

    private List<string> ObjectTags;
    public Vector2 spawnDistance;
    [SerializeField]private float spawnRate = 1f;
    private bool canSpawn = true;
    private string randomTag;

    private void Start() {
        objectPooler = ObjectPooler.Instance;
        ObjectTags = objectPooler.getPooledObjectsTagList();
        StartCoroutine(Spawner());
    }
    
    void FixedUpdate()
    {
    //    objectPooler.SpawnFromPool("Enemy", transform.position, Quaternion.identity);
    }

    void Spawn(){
        int r = Random.Range(0, ObjectTags.Count);
        // Debug.Log(r);
        
        objectPooler.SpawnFromPool(ObjectTags[r], transform.position + DistanceRandomModifier(), Quaternion.identity);
    }

    private Vector3 DistanceRandomModifier(){
        float randomValueX;
        float randomValueY;
        Vector3 randomVec3;

        if(spawnDistance.x > 0){
            randomValueX = Random.Range(0, spawnDistance.x);
        }
        else{
            randomValueX = 0;
        }

        if(spawnDistance.y > 0){
            randomValueY = Random.Range(-spawnDistance.y, spawnDistance.y);
        }
        else{
            randomValueY = 0;
        }

        randomVec3 = new Vector3(randomValueX, randomValueY, 0);
        // Debug.Log(randomVec3);

        return randomVec3;
    }

    IEnumerator Spawner(){
        WaitForSeconds wait = new WaitForSeconds(spawnRate);
        while(canSpawn){
            yield return wait;
            Spawn();
        }
    }
}
