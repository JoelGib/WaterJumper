using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public struct Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton
    public static ObjectPooler Instance;

    private void Awake() {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    // Start is called before the first frame update
    void Start()
    {
        InitializePool();
        // getPooledObjectsTagList();
    }

    public void ReturnToPool(string tag, GameObject obj){
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }

    private void InitializePool(){
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                // Debug.Log(obj.transform.position);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public List<string> getPooledObjectsTagList(){
        List<string> tagsList = new List<string>();
        foreach (Pool pool in pools)
        {
            tagsList.Add(pool.tag);
        }
        return tagsList;
        // for (int i = 0; i < tagsList.Count; i++)
        // {
        //     Debug.Log(tagsList[i] + "::" + i);
        // }
        
    }

    public GameObject SpawnFromPool(string tag, Vector2 position, Quaternion rotation)
    {
        if(!poolDictionary.ContainsKey(tag)){
            Debug.LogWarning("Poolwith tag " + tag + " doesn't exist!");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        if(pooledObj != null){
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

}
