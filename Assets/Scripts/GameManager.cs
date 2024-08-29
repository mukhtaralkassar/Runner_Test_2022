using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Slider LoadingSlider;
    public GameObject LoadingGameObject;
    public GameSettings _GameSettings;
    public static GameSettings GameSettings;
    public static float Score
    {
        get
        {
            return PlayerPrefs.GetFloat("ScorePoint", 0);
        }
        set
        {
            if (PlayerPrefs.GetFloat("ScorePoint", 0) < value)
                PlayerPrefs.SetFloat("ScorePoint", value);
        }
    }

    public static int Coins
    {
        get
        {
            return PlayerPrefs.GetInt("Coins", 0);
        }
        set
        {
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + value);
        }
    }
    public bool IsLoadingFinish = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameSettings = _GameSettings;
        }
        else
        {
            Destroy(this);
        }
    }

    public void ShowLoading(string sceneNumber)
    {
        LoadingSlider.value = 0;
        SetActiveLoadingPanel(true);
        StartCoroutine(AsynchronousLoad(sceneNumber));
    }
    IEnumerator AsynchronousLoad(string sceneNumber)
    {
        IsLoadingFinish = false;
        yield return null;
        AsyncOperation AsyncOperationScene = SceneManager.LoadSceneAsync(sceneNumber);
        AsyncOperationScene.allowSceneActivation = false;
        float progress = 0;

        while (LoadingSlider.value != 1 || !AsyncOperationScene.isDone)
        {
            if (LoadingSlider.value != 1)
                progress += 0.01f;
            LoadingSlider.value = progress;
            if (LoadingSlider.value >= 0.5f && !AsyncOperationScene.isDone)
                AsyncOperationScene.allowSceneActivation = true;
            if (progress < 0.75f)
                yield return new WaitForSecondsRealtime(0.01f);
            else
            {
                yield return new WaitForSecondsRealtime(0.5f);
                progress = 1;
            }
            LoadingSlider.value = progress;
        }
        // Loading completed
        AsyncOperationScene.allowSceneActivation = true;
        IsLoadingFinish = true;
        SetActiveLoadingPanel(false);
    }
    void SetActiveLoadingPanel(bool state)
    {
        LoadingGameObject.SetActive(state);
    }
}
