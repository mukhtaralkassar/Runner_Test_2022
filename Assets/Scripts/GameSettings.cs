using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GameSettings", menuName = "Resources/GameSettings", order = 0)]
public class GameSettings : ScriptableObject
{
    #region Keybord Settings
    public static bool LeftInput
    {
        get
        {
            return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        }
    }
    public static bool DownInput
    {
        get
        {
            return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        }
    }
    public static bool JumpInput
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        }
    }
    public static bool RigthInput
    {
        get
        {
            return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        }
    }
    public bool IsInputUsingKeybord
    {
        get
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return false;
#else
            return true;
#endif
        }
    }
    #endregion
    public int CountChainOfCoins = 5;
    public int SetAfterDistanceObstacke = 30;
    public int CountObstackesInstantiate = 3;
    public int CountCoinInstantiate = 10;
    public int ValueTakenAfterGettingCoins = 50;
    public List<ObstackesCoinsInfo> ObstackesCoinsInfos;
}
