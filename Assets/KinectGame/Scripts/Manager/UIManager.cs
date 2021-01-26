using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    #region UI Data

    public Text ScoreLabel;

    #endregion



    private void Awake()
    {
        Instance = this;
        ScoreLabel.gameObject.SetActive(true);
    }


    public void UpdateScore(int score)
    {
        ScoreLabel.gameObject.SetActive(true);
        ScoreLabel.text = score.ToString();
    }
}
