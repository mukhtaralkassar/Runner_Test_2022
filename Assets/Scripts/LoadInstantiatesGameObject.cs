using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadInstantiatesGameObject : MonoBehaviour
{
    public LevelManager LevelManagerScript;
    public GameObject ParentObstackesCoins;
    private int TotalCountRoads;
    [NonSerialized]
    public bool IsFinishInstantiate = false;
    private void Awake()
    {
        IsFinishInstantiate = false;
        InstantiatePlayer();
        TotalCountRoads = LevelManagerScript.TotalCountRoads;
        InstantiateObstackesCoins();
    }
    /// <summary>
    /// Equip obstacles and coins to use in the game.
    /// </summary>
    private void InstantiateObstackesCoins()
    {
        List<ObstackesCoinsInfo> ObstackesCoinsInfos = new List<ObstackesCoinsInfo>(GameManager.GameSettings.ObstackesCoinsInfos);


        for (int i = 0; i < ObstackesCoinsInfos.Count; i++)
        {
            if (ObstackesCoinsInfos[i].Type == TypeObstackesCoins.Obstacke)
            {
                var CountObstackesInstantiate = GameManager.GameSettings.CountObstackesInstantiate * TotalCountRoads;
                for (int j = 0; j < CountObstackesInstantiate; j++)
                {
                    ObstackesCoinsInfo ObstackesCoinsInfo = new ObstackesCoinsInfo(ObstackesCoinsInfos[i]);
                    var ObstackesGameObject = Instantiate(ObstackesCoinsInfo.ObstackeCoinGameObject);
                    ObstackesGameObject.transform.SetParent(ParentObstackesCoins.transform);
                    ObstackesCoinsInfo.ObstackeCoinGameObject = ObstackesGameObject;
                    ObstackesGameObject.SetActive(false);
                    LevelManagerScript.AddObstackesToList(ObstackesCoinsInfo);
                }
            }
            else if (ObstackesCoinsInfos[i].Type == TypeObstackesCoins.Coin)
            {
                var CountCoinsInstantiate = GameManager.GameSettings.CountCoinInstantiate * TotalCountRoads;
                for (int j = 0; j < CountCoinsInstantiate; j++)
                {
                    var ObstackesCoinsInfo = new ObstackesCoinsInfo(ObstackesCoinsInfos[i]);
                    var CoinsGameObject = Instantiate(ObstackesCoinsInfo.ObstackeCoinGameObject);
                    CoinsGameObject.transform.SetParent(ParentObstackesCoins.transform);
                    ObstackesCoinsInfo.ObstackeCoinGameObject = CoinsGameObject;
                    CoinsGameObject.SetActive(false);
                    LevelManagerScript.AddCoinsToList(ObstackesCoinsInfo);
                }
            }
        }
        IsFinishInstantiate = true;
    }
    /// <summary>
    /// Generate the player character and place it in the correct position.
    /// </summary>
    private void InstantiatePlayer()
    {
        var PlayerGameObject = Instantiate(Resources.Load("Cat")) as GameObject;
        PlayerGameObject.name = "Player";
        PlayerGameObject.transform.position = Vector3.zero;
    }
}
