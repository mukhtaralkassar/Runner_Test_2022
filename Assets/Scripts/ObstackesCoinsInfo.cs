using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class ObstackesCoinsInfo
{
    public TypeObstackesCoins Type;
    public Vector3 RotationInRoad;
    public Vector3 PoistionInRoad;
    public bool IsGameObjectTakeAllLane;
    public int StepDistance;
    public GameObject ObstackeCoinGameObject;
    public ObstackesCoinsInfo(TypeObstackesCoins type, Vector3 rotationInRoad, Vector3 poistionInRoad, bool isGameObjectTakeAllLane, int stepDistance, GameObject obstackeCoinGameObject)
    {
        Type = type;
        RotationInRoad = rotationInRoad;
        PoistionInRoad = poistionInRoad;
        IsGameObjectTakeAllLane = isGameObjectTakeAllLane;
        StepDistance = stepDistance;
        ObstackeCoinGameObject = obstackeCoinGameObject;
    }
    public ObstackesCoinsInfo(ObstackesCoinsInfo obstackesCoinsInfo)
    {
        Type = obstackesCoinsInfo.Type;
        RotationInRoad = obstackesCoinsInfo.RotationInRoad;
        PoistionInRoad = obstackesCoinsInfo.PoistionInRoad;
        IsGameObjectTakeAllLane = obstackesCoinsInfo.IsGameObjectTakeAllLane;
        StepDistance = obstackesCoinsInfo.StepDistance;
        ObstackeCoinGameObject = obstackesCoinsInfo.ObstackeCoinGameObject;
    }
}
public enum TypeObstackesCoins
{
    Obstacke,
    Coin,
    None
}