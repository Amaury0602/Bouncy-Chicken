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


    [SerializeField] private GameObject chickenCompletion;
    [SerializeField] private Image progressImage;
    

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

        ResetChickenCompletion();
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
        nextLevelButton.SetActive(false);
        restartButton.SetActive(false);
        LevelManager.instance.RestartLevel();
        ResetChickenCompletion();
    }

    public void OnNextLevelClicked()
    {
        LevelManager.instance.NextLevel();
        ResetChickenCompletion();
    }

    public void HideTapStartTuto()
    {
        tapStartTuto.SetActive(false);
    }


    public void ShowPanel(bool show, bool tuto = false)
    {
        float amount = show ? 0.15f : 0;
        panel.DOFade(amount, 0.25f).SetEase(Ease.Linear).SetUpdate(true);

        directionTapTuto.SetActive(tuto);
    }

    public void ShowChickenCompletion(float percent)
    {
        chickenCompletion.SetActive(true);
        progressImage.DOFillAmount(percent, 0.1f).SetEase(Ease.Linear).OnComplete(() => { DisplayNextLevel();  });
    }

    private void ResetChickenCompletion()
    {
        chickenCompletion.SetActive(false);
        progressImage.fillAmount = 0;
    }
}
