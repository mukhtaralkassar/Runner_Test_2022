using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadScript : MonoBehaviour
{
    public List<ObstackesCoinsInfo> ObstackesInfo;
    public List<ObstackesCoinsInfo> CoinsInfo;
    internal void AddCoinsToRoadList(ObstackesCoinsInfo CoinInfo)
    {
        if (CoinsInfo == null || CoinsInfo.Count <= 0)
            CoinsInfo = new List<ObstackesCoinsInfo>();
        CoinsInfo.Add(CoinInfo);
    }

    internal void AddObstackesToRoadList(ObstackesCoinsInfo obstackeInfo)
    {
        if (ObstackesInfo == null || ObstackesInfo.Count <= 0)
            ObstackesInfo = new List<ObstackesCoinsInfo>();
        ObstackesInfo.Add(obstackeInfo);
    }

    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        ClearLists();
    }
    private void ClearLists()
    {
        for (int i = 0; i < CoinsInfo.Count; i++)
        {
            if (CoinsInfo[i].ObstackeCoinGameObject)
            {
                CoinsInfo[i].ObstackeCoinGameObject.SetActive(false);
                LevelManager.Instance.AddCoinsToList(CoinsInfo[i]);
            }
        }
        for (int i = 0; i < ObstackesInfo.Count; i++)
        {
            if (ObstackesInfo[i].ObstackeCoinGameObject)
            {
                ObstackesInfo[i].ObstackeCoinGameObject.SetActive(false);
                LevelManager.Instance.AddObstackesToList(ObstackesInfo[i]);
            }
        }
        CoinsInfo.Clear();
        ObstackesInfo.Clear();
    }
}
