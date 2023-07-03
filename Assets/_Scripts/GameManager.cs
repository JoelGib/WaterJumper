using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPaused = false;

    public float GameSpeed = 1f;

    public bool isDead = false;

    public static GameManager Instance { get; private set; }
    

    private void Awake() {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // public void PlayerHit(){

    // }

    public void RestartGame(bool checkAlive){
        if(checkAlive){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name,  LoadSceneMode.Single);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
