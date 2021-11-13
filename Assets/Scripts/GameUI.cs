using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{

    public static GameUI instance;

    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject nextLevelButton;


    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        instance = this;

        restartButton.SetActive(false);
        nextLevelButton.SetActive(false);
    }

    public void DisplayRestart()
    {
        restartButton.SetActive(true);
    }
    
    public void DisplayNextLevel()
    {
        nextLevelButton.SetActive(true);
    }

    public void OnRestartClicked()
    {
        LevelManager.instance.RestartLevel();
    }

    public void OnNextLevelClicked()
    {
        LevelManager.instance.NextLevel();
    }
}
