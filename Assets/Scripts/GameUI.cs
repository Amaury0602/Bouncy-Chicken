using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUI : MonoBehaviour
{

    public static GameUI instance;

    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private GameObject tapStartTuto;
    

    [SerializeField] private Image panel;
    [SerializeField] private GameObject directionTapTuto;


    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        instance = this;

        restartButton.SetActive(false);
        nextLevelButton.SetActive(false);

        if (PlayerPrefs.GetInt("Level") > 1)
        {
            tapStartTuto.SetActive(false);
        } else
        {
            tapStartTuto.SetActive(true);
        }
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

    public void HideTapStartTuto()
    {
        tapStartTuto.SetActive(false);
    }


    public void ShowPanel(bool show, bool tuto = false)
    {
        float amount = show ? 0.5f : 0;
        panel.DOFade(amount, 0.25f).SetEase(Ease.Linear).SetUpdate(true);

        directionTapTuto.SetActive(tuto);
    }
}
