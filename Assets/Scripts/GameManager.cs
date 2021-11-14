using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private LevelManager levelManager;

    private Tween slowMoTween = null;


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
    
    public void OnSuccess()
    {
        slowMoTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.15f, 0.75f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(() =>
        {
            GameUI.instance.DisplayNextLevel();
        });
        int actualLevel = PlayerPrefs.GetInt("Level");
        PlayerPrefs.SetInt("Level", actualLevel + 1);
    }

}
