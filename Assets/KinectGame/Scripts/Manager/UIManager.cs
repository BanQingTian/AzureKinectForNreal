using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    #region UI Label

    public TextMeshPro ScoreLabel;

    public TextMeshPro CountdownLabel;

    public TextMeshPro JumpV;

    #endregion



    private void Awake()
    {
        Instance = this;
        //ScoreLabel.gameObject.SetActive(true);
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
        //if (!run) return;

        ScoreLabel.gameObject.SetActive(true);
        //ScoreLabel.text = score.ToString();
    }

    public void UpdateScore()
    {
        int temp = int.Parse(ScoreLabel.text);
        ScoreLabel.text = (temp + 1).ToString();
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

    string s = "00";
    string m = "00";
    private void UpdateCountdown()
    {
        if (time >= 60f)
        {
            s = (time / 60f).ToString();
            if (s.Contains("."))
            {
                string[] temp1 = s.Split('.');
                s = temp1[0].ToString();
            }

            if ((time - int.Parse(s) * 60f) > 0)
            {
                m = (time - int.Parse(s) * 60f).ToString();
            }
        }
        else
        {
            s = "00";
            m = time.ToString();
        }

        if (m.Contains("."))
        {
            m = m.Split('.')[0];
        }

        if (int.Parse(s) < 10 && int.Parse(s) != 0)
        {
            s = "0" + s;
        }

        if (int.Parse(m) < 10)
        {
            m = "0" + m;
        }

        CountdownLabel.text = "00" + ":" + s + ":" + m;
        time -= Time.deltaTime;
        if (time <= 0)
        {
            ScoreLabel.text = "0";
            CountdownLabel.text = "00:00:00";
            finishEvent?.Invoke();
            ScoreLabel.gameObject.SetActive(false);
            CountdownLabel.gameObject.SetActive(false);
            run = false;
        }
    }
}
