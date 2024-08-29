using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelManager : MonoBehaviour
{
    public TMP_Text ScoreText;
    public Button RetryButton;

    public void Show()
    {
        RetryButton.onClick.RemoveAllListeners();
        RetryButton.onClick.AddListener(RetryButton_OnClick);
        ScoreText.text = string.Format("Score : {0}",(int) PlayerController.Instance.Score);
        gameObject.SetActive(true);
    }
    
    private void RetryButton_OnClick()
    {         
        GameManager.Instance.ShowLoading("GamePlay");
    }

}
