using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private LevelManager levelManager;

    private Tween slowMoTween = null;

    private float chickenPercent = 0;
    private float maxChickenParts = 5f;


    public Color[] gameColors = new Color[3];
    public Material[] gameMaterials = new Material[3];


    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);    
        instance = this;

        levelManager = GetComponent<LevelManager>();
    }

    public void OnGameOver()
    {
        // display button
        GameUI.instance.DisplayRestart();
    }
    
    public void OnSuccess(int parts)
    {
        slowMoTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.15f, 0.75f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(() =>
        {
            //GameUI.instance.DisplayNextLevel();
            GameUI.instance.ShowChickenCompletion((float)parts / maxChickenParts);
        });
        int actualLevel = PlayerPrefs.GetInt("Level");
        PlayerPrefs.SetInt("Level", actualLevel + 1);

        
    }

}
