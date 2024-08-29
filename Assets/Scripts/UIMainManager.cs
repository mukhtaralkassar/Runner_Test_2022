using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainManager : MonoBehaviour
{
    public Button PlayButton;
    public TMP_Text ScoreText;
    public TMP_Text CoinText;
    // Start is called before the first frame update
    void Start()
    {
        PlayButton.onClick.AddListener(PlayButton_Onclick);
        ScoreText.text = string.Format("Score : {0}", GameManager.Score);
        CoinText.text = string.Format("Coins : {0}", GameManager.Coins);
    }

    private void PlayButton_Onclick()
    {
        GameManager.Instance.ShowLoading("GamePlay");
    }
}
