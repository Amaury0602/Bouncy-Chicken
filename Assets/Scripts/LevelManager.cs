using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<GameObject> levelPrefabs = new List<GameObject>();
    public static LevelManager instance;

    private int actualLevel = -1;

    private GameObject spawnedLevel;
    private GameObject spawnedPlayer;


    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        instance = this;


        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 0);
        }

        BuildLevel();
    }

    
    public void RestartLevel()
    {
        BuildLevel();
    }

    public void NextLevel()
    {
        BuildLevel();
    }

    private void BuildLevel()
    {

        if (PlayerPrefs.GetInt("Level") >= levelPrefabs.Count)
        {
            PlayerPrefs.SetInt("Level", 0);
        }
        GameObject nextLevel = levelPrefabs[PlayerPrefs.GetInt("Level")];

        if (spawnedLevel != null)
        {
            Destroy(spawnedLevel);
        }

        if (spawnedPlayer != null)
        {
            Destroy(spawnedPlayer);
        }

        spawnedLevel = Instantiate(nextLevel, transform.position, Quaternion.identity);
        spawnedPlayer = Instantiate(playerPrefab);
        spawnedPlayer.transform.position = spawnedLevel.GetComponent<Level>().playerSpawn.position;
        Time.timeScale = 1;
    }
}
