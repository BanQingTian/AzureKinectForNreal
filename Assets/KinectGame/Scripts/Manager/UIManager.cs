using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    #region UI Label

    public Text ScoreLabel;

    public Text CountdownLabel;

    #endregion



    private void Awake()
    {
        Instance = this;
        ScoreLabel.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (run)
        {
            UpdateCountdown();
        }
    }

    public void UpdateScore(int score)
    {
        if (!run) return;

        ScoreLabel.gameObject.SetActive(true);
        ScoreLabel.text = score.ToString();
    }

    bool run = false;
    float time;
    Action finishEvent;
    public void StartCountdown(float t, Action finish = null)
    {
        run = true;
        time = t;
        finishEvent = finish;
        CountdownLabel.gameObject.SetActive(true);
    }

    private void UpdateCountdown()
    {
        CountdownLabel.text = time.ToString("00.00");
        time -= Time.deltaTime;
        if (time <= 0)
        {
            finishEvent?.Invoke();
            run = false;
        }
    }
}
