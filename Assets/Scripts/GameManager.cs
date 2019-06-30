using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    bool gameStarted = false;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }else if(Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void GameOver()
    {
        ScenesManager.GoTo("GameOverScreen");
    }
    private void Update()
    {
        if(ScenesManager.GetCurrentScene().name == "SplashScreen")
        {
            print("Holi");
            if (Input.GetButtonDown("Start"))
            {
                print("Olovorgo");
                StartGame();
            }
        }
    }
    public void StartGame()
    {
        ScenesManager.GoTo("Game");
    }
}
