using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public TMP_Text CounterText;
    public GameObject PlayGameObjectUI;
    public TMP_Text ScorePlayerText;
    public TMP_Text MultiplierPlayerText;
    public TMP_Text CoinsPlayerText;
    public List<RoadScript> Roads;
    private List<RoadScript> _RoadsRemoverFromList;
    public float StepPositionRoad;
    public float StepDistanceToMoveStreetCollider = 286;
    public GameObject StreetBoxCollider;
    private float DistanceToMoveStreetCollider;
    private int _NumberOfCurrentRoad;
    private float _NextStepPositionRoad;
    private float DistanceToDeleteOldRoad;
    private int _CountRoadsKeptInListRoads;
    private int _RandomIndexRoad;
    public UILevelManager UILevelManager;
    public List<ObstackesCoinsInfo> ObstackesCanUse;
    public List<ObstackesCoinsInfo> CoinsCanUse;
    public LoadInstantiatesGameObject LoadInstantiatesGameObjectScript;
    private int _NextDistanceForObstackeAndCoinsCenterLane;
    private int _NextDistanceForObstackeAndCoinsLeftLane;
    private int _NextDistanceForObstackeAndCoinsRightLane;
    public int TotalCountRoads
    {
        get
        {
            if (_RoadsRemoverFromList != null && Roads != null)
                return _RoadsRemoverFromList.Count + Roads.Count;
            else if (_RoadsRemoverFromList != null)
                return _RoadsRemoverFromList.Count;
            else if (Roads != null)
                return Roads.Count;
            else
                return 0;
        }
    }
    private int _CountChainOfCoins;
    private int _TypeObstackesCoinsLength;
    private void Awake()
    {
        Instance = this;
        _NextDistanceForObstackeAndCoinsCenterLane = GameManager.GameSettings.SetAfterDistanceObstacke;
        _NextDistanceForObstackeAndCoinsLeftLane = GameManager.GameSettings.SetAfterDistanceObstacke;
        _NextDistanceForObstackeAndCoinsRightLane = GameManager.GameSettings.SetAfterDistanceObstacke;
        _CountChainOfCoins = GameManager.GameSettings.CountChainOfCoins;
        _TypeObstackesCoinsLength = ((int)TypeObstackesCoins.None);
        StartCoroutine(ChangePositionRoads());
        SetPlayerCoins(0);

    }
    /// <summary>
    /// This function randomizes the roads and calls function n to generate obstacles or Coins.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangePositionRoads()
    {
        yield return new WaitWhile(() => !LoadInstantiatesGameObjectScript.IsFinishInstantiate);
        _RoadsRemoverFromList = new List<RoadScript>(Roads);
        Roads.Clear();
        var CountRoads = _RoadsRemoverFromList.Count;
        for (int i = 0; i < CountRoads; i++)
        {
            _RandomIndexRoad = UnityEngine.Random.RandomRange(0, _RoadsRemoverFromList.Count);
            var item = _RoadsRemoverFromList[_RandomIndexRoad];
            item.transform.localPosition = new Vector3(0, 0, StepPositionRoad * i);
            int stratPositionRoad = Mathf.FloorToInt(StepPositionRoad * i);
            MakeAllLanesEquallyDistance();
            if (i != 0 && stratPositionRoad >= _NextDistanceForObstackeAndCoinsCenterLane)
            {
                _NextDistanceForObstackeAndCoinsCenterLane = stratPositionRoad;
                MakeAllLanesEquallyDistance();
            }
            AddObstackesOrCoinsToRoad(item, StepPositionRoad * (i + 1));
            Roads.Add(item);
            _RoadsRemoverFromList.Remove(item);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGame());
        CalculatePositionOfRoad();
    }
    private IEnumerator StartGame()
    {
        PlayGameObjectUI.SetActive(false);
        int counter = 3;
        CounterText.gameObject.SetActive(true);
        CounterText.text = counter.ToString();

        PlayerController.Instance.StartAnimation();
        yield return new WaitWhile(() => !GameManager.Instance.IsLoadingFinish);
        while (counter > 0)
        {
            CounterText.text = counter.ToString();
            yield return new WaitForSecondsRealtime(1);
            counter--;
        }
        CounterText.gameObject.SetActive(false);
        PlayGameObjectUI.SetActive(true);
        PlayerController.Instance.StartRun();
    }
    /// <summary>
    /// Calculate the distance in the municipality for regeneration of roads
    /// </summary>
    private void CalculatePositionOfRoad()
    {
        _RoadsRemoverFromList = new List<RoadScript>();
        _CountRoadsKeptInListRoads = Roads.Count / 2;
        _NextStepPositionRoad = StepPositionRoad * Roads.Count;
        _NumberOfCurrentRoad = 1;
        DistanceToDeleteOldRoad = (StepPositionRoad * _NumberOfCurrentRoad) + 10;
        DistanceToMoveStreetCollider = StepDistanceToMoveStreetCollider;
    }
    internal void SetPlayerScore(float scoreValue, int multiplier)
    {
        ScorePlayerText.text = string.Format("Score : {0}", Mathf.FloorToInt(scoreValue).ToString());
        MultiplierPlayerText.text = string.Format("X{0}", PlayerController.Instance.Multiplier);
    }
    /// <summary>
    /// Check if the player has gone off the road or if he should add a road.
    /// </summary>
    /// <param name="distance"> It is the player's current distance.</param>
    internal void CheckIfRoadExitAndNeedMore(float distance)
    {
        if (DistanceToDeleteOldRoad <= distance)
        {
            _NumberOfCurrentRoad++;
            DistanceToDeleteOldRoad = (StepPositionRoad * _NumberOfCurrentRoad) + 10;
            var item = Roads[0];
            item.gameObject.SetActive(false);
            _RoadsRemoverFromList.Add(item);
            Roads.Remove(item);
        }
        if (Roads.Count < _CountRoadsKeptInListRoads)
        {
            _RandomIndexRoad = UnityEngine.Random.RandomRange(0, _RoadsRemoverFromList.Count);
            var item = _RoadsRemoverFromList[_RandomIndexRoad];
            item.transform.localPosition = new Vector3(0, 0, _NextStepPositionRoad);
            MakeAllLanesEquallyDistance();
            if (_NextDistanceForObstackeAndCoinsCenterLane <= _NextStepPositionRoad)
            {
                _NextDistanceForObstackeAndCoinsCenterLane = Mathf.FloorToInt(_NextStepPositionRoad);
                MakeAllLanesEquallyDistance();
            }
            _NextStepPositionRoad += StepPositionRoad;
            AddObstackesOrCoinsToRoad(item, _NextStepPositionRoad);
            item.gameObject.SetActive(true);
            Roads.Add(item);
            _RoadsRemoverFromList.Remove(item);
        }
        if (DistanceToMoveStreetCollider <= distance)
        {
            StreetBoxCollider.transform.position = new Vector3(0, 0, DistanceToMoveStreetCollider);
            DistanceToMoveStreetCollider += StepDistanceToMoveStreetCollider;
        }

    }

    internal void FinishGamePlay()
    {
        GameManager.Score = (int)PlayerController.Instance.Score;
        GameManager.Coins = PlayerController.Instance.Coins;
        UILevelManager.Show();
    }

    internal void AddObstackesToList(ObstackesCoinsInfo obstackesInfo)
    {
        if (ObstackesCanUse == null || ObstackesCanUse.Count == 0)
            ObstackesCanUse = new List<ObstackesCoinsInfo>();
        ObstackesCanUse.Add(obstackesInfo);
    }

    internal void AddCoinsToList(ObstackesCoinsInfo coinsInfo)
    {

        if (CoinsCanUse == null || CoinsCanUse.Count == 0)
            CoinsCanUse = new List<ObstackesCoinsInfo>();
        CoinsCanUse.Add(coinsInfo);
    }
    /// <summary>
    /// Add random obstacles or coins and add them to the RoadScript
    ///taking into account the current road distance
    /// </summary>
    /// <param name="road">Current RoadScript</param>
    /// <param name="endDistanse">Final distance of current route</param>
    /// <param name="laneIndex">The lane that will put obstacles or coin in it</param>
    private void AddObstackesOrCoinsToRoad(RoadScript road, float endDistanse, int laneIndex = 0)
    {
        int positionLane = 0;
        if (laneIndex == 1)
            positionLane = -1;
        else if (laneIndex == 2)
            positionLane = 1;
        else
            MakeAllLanesEquallyDistance();
           
        int typeUsed = UnityEngine.Random.Range(0, _TypeObstackesCoinsLength);
        if (endDistanse > GetDisanceForCurrenLaneUsingToAddObstackesOrCoins(positionLane) && (ObstackesCanUse.Count > 0 || CoinsCanUse.Count > 0))
        {

            if (typeUsed == (int)TypeObstackesCoins.Coin)
            {
                for (int i = 0; i < _CountChainOfCoins; i++)
                {
                    if (CoinsCanUse.Count > 0 && endDistanse > GetDisanceForCurrenLaneUsingToAddObstackesOrCoins(positionLane) )
                    {
                        CoinsCanUse[0].ObstackeCoinGameObject.transform.position = CoinsCanUse[0].PoistionInRoad + new Vector3(positionLane, 0, GetDisanceForCurrenLaneUsingToAddObstackesOrCoins(positionLane) );
                        CoinsCanUse[0].ObstackeCoinGameObject.transform.eulerAngles = CoinsCanUse[0].RotationInRoad;
                        IncreaseDistanceForObstackeAndCoins(positionLane, CoinsCanUse[0].StepDistance);
                        CoinsCanUse[0].ObstackeCoinGameObject.SetActive(true);
                        road.AddCoinsToRoadList(CoinsCanUse[0]);
                        CoinsCanUse.RemoveAt(0);
                    }
                    else if(i == _CountChainOfCoins-1)
                    {
                        AddObstackesOrCoinsToRoad(road, endDistanse);
                    }
                }
                if (laneIndex == 0)
                {
                    AddObstackesOrCoinsToRoad(road, endDistanse, 1);
                    AddObstackesOrCoinsToRoad(road, endDistanse, 2);
                }
            }
            else if (typeUsed == (int)TypeObstackesCoins.Obstacke)
            {
                if (ObstackesCanUse.Count > 0)
                {
                    var index = UnityEngine.Random.Range(0, ObstackesCanUse.Count);
                    if ((laneIndex == 0 && ObstackesCanUse[index].IsGameObjectTakeAllLane) || !ObstackesCanUse[index].IsGameObjectTakeAllLane)
                    {
                        var Obstackes = ObstackesCanUse[index];
                        Obstackes.ObstackeCoinGameObject.transform.position = Obstackes.PoistionInRoad + new Vector3(positionLane, 0, GetDisanceForCurrenLaneUsingToAddObstackesOrCoins(positionLane) );
                        Obstackes.ObstackeCoinGameObject.transform.eulerAngles = Obstackes.RotationInRoad;
                        road.AddObstackesToRoadList(Obstackes);
                        IncreaseDistanceForObstackeAndCoins(positionLane, Obstackes.StepDistance, Obstackes.IsGameObjectTakeAllLane);
                        Obstackes.ObstackeCoinGameObject.SetActive(true);
                        ObstackesCanUse.RemoveAt(index);
                        if (laneIndex == 0 && !Obstackes.IsGameObjectTakeAllLane)
                            AddObstackesOrCoinsToRoad(road, endDistanse, UnityEngine.Random.Range(1, 2));
                    }
                    else
                    {
                        AddObstackesOrCoinsToRoad(road, endDistanse, laneIndex);
                    }
                }
                else
                {
                    AddObstackesOrCoinsToRoad(road, endDistanse, laneIndex);
                }
            }
            else
            {
                if (laneIndex == 0)
                {
                    AddObstackesOrCoinsToRoad(road, endDistanse, 2);
                    AddObstackesOrCoinsToRoad(road, endDistanse, 1);
                }
            }
            if (laneIndex == 0)
                AddObstackesOrCoinsToRoad(road, endDistanse);
        }
    }
    private int GetDisanceForCurrenLaneUsingToAddObstackesOrCoins(int lanePosition)
    {
        if (lanePosition == 1)
            return _NextDistanceForObstackeAndCoinsRightLane;
        else if (lanePosition == -1)
            return _NextDistanceForObstackeAndCoinsLeftLane;
        else
            return _NextDistanceForObstackeAndCoinsCenterLane;
    }
    private void IncreaseDistanceForObstackeAndCoins(int lanePosition, int increaseDistanc, bool isTakenAllLanes = false)
    {
        if (!isTakenAllLanes)
        {
            if (lanePosition == 0)
                _NextDistanceForObstackeAndCoinsCenterLane += increaseDistanc;
            else if (lanePosition == 1)
                _NextDistanceForObstackeAndCoinsRightLane += increaseDistanc;
            else if (lanePosition == -1)
                _NextDistanceForObstackeAndCoinsLeftLane += increaseDistanc;
        }
        else
        {
            MakeAllLanesEquallyDistance();
            _NextDistanceForObstackeAndCoinsRightLane += increaseDistanc;
            _NextDistanceForObstackeAndCoinsLeftLane += increaseDistanc;
            _NextDistanceForObstackeAndCoinsCenterLane += increaseDistanc;
        }
    }
    private void MakeAllLanesEquallyDistance()
    {
        int newDistance = 0;
        if (_NextDistanceForObstackeAndCoinsCenterLane >= _NextDistanceForObstackeAndCoinsLeftLane
                 && _NextDistanceForObstackeAndCoinsCenterLane >= _NextDistanceForObstackeAndCoinsRightLane)
            newDistance = _NextDistanceForObstackeAndCoinsCenterLane;
        else if (_NextDistanceForObstackeAndCoinsLeftLane >= _NextDistanceForObstackeAndCoinsCenterLane
             && _NextDistanceForObstackeAndCoinsLeftLane >= _NextDistanceForObstackeAndCoinsRightLane)
            newDistance = _NextDistanceForObstackeAndCoinsLeftLane;
        else
            newDistance = _NextDistanceForObstackeAndCoinsRightLane;
        _NextDistanceForObstackeAndCoinsRightLane = newDistance;
        _NextDistanceForObstackeAndCoinsLeftLane = newDistance;
        _NextDistanceForObstackeAndCoinsCenterLane = newDistance;
    }

    internal void SetPlayerCoins(int coinsValue)
    {
        CoinsPlayerText.text = string.Format("Coins : {0}", coinsValue);
    }
}
