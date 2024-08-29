using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public Animator AnimatorPlayer;
    public Rigidbody RigidbodyPlayer;
    public BoxCollider PlayerNormalBoxCollider, PlayerSkiingBoxCollider;
    public float MinSpeedForward = 10;
    public float MaxSpeedForward = 40;
    private float SpeedUpStep = 0.2f;
    public float StepSpeedForward = 4;
    private Vector3 TargetMove = Vector3.zero;
    public PlayerSound PlayerSound;
    public float SpeedRatio
    {
        get
        {
            return (CurrentSpeed - MinSpeedForward) / (MaxSpeedForward - MinSpeedForward);
        }
    }
    public float _CurrentSpeed;
    public float CurrentSpeed
    {
        get
        {
            return _CurrentSpeed;
        }
        set
        {
            if (value > MaxSpeedForward)
                _CurrentSpeed = MaxSpeedForward;
            else
                _CurrentSpeed = value;
        }
    }
    public float HorizontalMoveSpeed = 4;
    public float Score
    {
        get
        {
            return _Score;
        }
        set
        {
            _Score = value;
            LevelManager.Instance.SetPlayerScore(value, Multiplier);
        }
    }
    private float _Score = 0;
    private int _Coins = 0;
    public int Coins
    {
        get
        {
            return _Coins;
        }
        set
        {
            _Coins = value;
            LevelManager.Instance.SetPlayerCoins(value);
            AddScore(ValueTakenAfterGettingCoins);
        }

    }
    public int CurrentLane
    {
        get
        {
            return _CurrentLane;
        }
    }
    private int _CurrentLane;
    private float _ScaledSpeed;
    private float _WorldDistance;
    public float WorldDistance
    {
        get
        {
            return _WorldDistance;
        }
        set
        {
            _WorldDistance = value;
            LevelManager.Instance.CheckIfRoadExitAndNeedMore(value);
        }
    }
    #region Keybord Settings
    public bool LeftInput
    {
        get
        {
            return GameSettings.LeftInput;
        }
    }
    public bool DownInput
    {
        get
        {
            return GameSettings.DownInput;
        }
    }
    public static bool JumpInput
    {
        get
        {
            return GameSettings.JumpInput;
        }
    }
    public static bool RigthInput
    {
        get
        {
            return GameSettings.RigthInput;
        }
    }
    private bool IsInputUsingKeybord
    {
        get
        {
            return GameManager.GameSettings.IsInputUsingKeybord;
        }
    }

    #endregion
    public bool IsStartRun
    {
        get
        {
            return _IsStartRun;
        }
    }

    private float ValueTakenAfterGettingCoins;
    public Vector3 SlidingColliderCenter;
    public Vector3 SlidingColliderSize;
    public Vector3 NormalColliderCenter;
    public Vector3 NormalColliderSize;
    #region Animation Hash
    private int _DeadHash = Animator.StringToHash("Dead");
    private int _HitHash = Animator.StringToHash("Hit");
    private int _RunStartHash = Animator.StringToHash("runStart");
    private int _MovingHash = Animator.StringToHash("Moving");
    private int _JumpingHash = Animator.StringToHash("Jumping");
    private int _JumpingSpeedHash = Animator.StringToHash("JumpSpeed");
    private int _SkiingHash = Animator.StringToHash("Sliding");
    private int _StartHash = Animator.StringToHash("Start");
    #endregion
    #region Touch Parameters
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;
    #endregion
    #region Jump Parameters
    private bool _IsJumping = false;
    private bool _EnableIncreaceJump = false;
    public float JumpLength = 10;
    public float _TrackSpeedToJumpAnimSpeedRatio = 0.2f;
    private float _JumpTime = 1;
    public float BaseJumpTime = 1;
    #endregion
    private bool _IsSkiing = false;
    private float _SkiingTime = 1;
    public float BaseSkiingTime = 1;
    private bool _IsStartRun = false;
    private float _SkiingStartDistance;
    public float SkiingLength = 2.0f;
    private float StartPlayerPosition;
    public int SpeedStep = 4;
    public int Multiplier
    {
        get
        {
            return 1 + Mathf.FloorToInt(SpeedRatio * SpeedStep);
        }
    }
    private void Awake()
    {
        Instance = this;
        CurrentSpeed = MinSpeedForward;
        ValueTakenAfterGettingCoins = GameManager.GameSettings.ValueTakenAfterGettingCoins;
    }
    private void Start()
    {
        StartPlayerPosition = transform.position.z;
    }
    public void StartRun()
    {
        AnimatorPlayer.Play(_RunStartHash);
        AnimatorPlayer.SetBool(_MovingHash, true);
        _IsStartRun = true;
        PlayerSound.MoveEffect();
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsStartRun)
        {
            TargetMove = (transform.forward * CurrentSpeed);
            InputManager();
            CheckIfPlayerIsFly();
            CheckIfSkiingFinish();
            CheckIfPlayerInCurretLane();
            RigidbodyPlayer.velocity = TargetMove;
            CalculateScoreAndDistance();
            IncreaseSpeed();
        }
    }
    /// <summary>
    /// Check if the player is on the correct lane or add value to put him on the correct lane.
    /// </summary>
    private void CheckIfPlayerInCurretLane()
    {
        if (_CurrentLane == 0 && transform.position.x != 0)
        {
            TargetMove += (transform.right * -transform.position.x * HorizontalMoveSpeed);
        }
        else if (_CurrentLane == 1 && transform.position.x != 1)
        {
            TargetMove += (transform.right * (1 - transform.position.x) * HorizontalMoveSpeed);
        }
        else if (_CurrentLane == -1 && transform.position.x != -1)
        {
            TargetMove += (transform.right * (-1 - transform.position.x) * HorizontalMoveSpeed);
        }
    }
    /// <summary>
    /// Make sure the player has finished skiing
    /// </summary>
    private void CheckIfSkiingFinish()
    {
        if (_IsSkiing)
        {
            if (_SkiingTime > 0)
            {
                _SkiingTime -= Time.deltaTime;
            }
            else
            {
                StopSkiingPlayer();
            }
        }
    }
    /// <summary>
    /// Make sure the player has finished jump.
    /// </summary>
    private void CheckIfPlayerIsFly()
    {
        if (_IsJumping)
        {
            if (_JumpTime > 0)
            {
                _JumpTime -= Time.deltaTime;
                if (_EnableIncreaceJump)
                {
                    if (transform.position.y <= 1)
                    {
                        TargetMove += transform.up * JumpLength;
                    }
                    else if (transform.position.y >= 0)
                    {
                        _EnableIncreaceJump = false;
                    }
                }
                else if (transform.position.y <= 0)
                    StopJumping();
            }

            else
            {
                StopJumping();
            }

        }
    }

    private void CalculateScoreAndDistance()
    {
        _ScaledSpeed = CurrentSpeed * Time.deltaTime;
        WorldDistance = Math.Abs(transform.position.z - StartPlayerPosition);
        AddScore(_ScaledSpeed * Multiplier);
    }

    private void AddScore(float _Score)
    {
        Score += _Score;
    }

    private void IncreaseSpeed()
    {
        CurrentSpeed += SpeedUpStep * Time.deltaTime;
    }

    public void StartAnimation()
    {
        AnimatorPlayer.Play(_StartHash);

    }
    /// <summary>
    /// Check if the player is using keyboard or touch for in-game movement.
    /// </summary>
    private void InputManager()
    {

        if (IsInputUsingKeybord)
        {
            InputKeybord();
        }
        else
        {
            TouchInput();
        }
    }
    /// <summary>
    /// Checks if Swipe is horizontal (if left or right it calls function ChangeLanePlayer) or
    /// vertical (if down it calls function SkiingPlayer, if up it calls function JumpPlayer)
    /// </summary>
    private void CheckSwipe()
    {
        if (verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
        {
            if (fingerDown.y - fingerUp.y > 0)
            {
                JumpPlayer();
            }
            else if (fingerDown.y - fingerUp.y < 0)
            {
                SkiingPlayer();
            }
            fingerUp = fingerDown;
        }
        else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
        {

            if (fingerDown.x - fingerUp.x > 0)
            {
                ChangeLanePlayer(1);
            }
            else if (fingerDown.x - fingerUp.x < 0)
            {
                ChangeLanePlayer(-1);
            }
            fingerUp = fingerDown;
        }
    }

    private float verticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    private float horizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }
    /// <summary>
    /// Knowing the start and end position of the swipe and putting it in parameters
    /// </summary>
    private void TouchInput()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                CheckSwipe();
            }
        }
    }

    private void InputKeybord()
    {
        if (LeftInput)
        {
            ChangeLanePlayer(-1);
        }
        else if (RigthInput)
        {
            ChangeLanePlayer(1);
        }
        else if (JumpInput)
        {
            JumpPlayer();
        }
        else if (DownInput)
        {
            SkiingPlayer();
        }
    }
    /// <summary>
    /// Make sure the player is not jumping and is not skating (if he is skating, the StopSkiingPlayer function is called),
    /// then add the animation time and set BaseJumpTime to _JumpTime and play the jump sound, to start jumping
    /// </summary>
    private void JumpPlayer()
    {
        if (!_IsJumping)
        {
            if (_IsSkiing)
                StopSkiingPlayer();
            _IsJumping = true;
            TargetMove += transform.up;
            _EnableIncreaceJump = true;
            _JumpTime = BaseJumpTime + (BaseJumpTime * SpeedRatio);
            PlayerSound.JumpEffect();
            AnimatorPlayer.SetFloat(_JumpingSpeedHash, _JumpTime);
            AnimatorPlayer.SetBool(_JumpingHash, true);
        }
    }

    /// <summary>
    /// Change the lane to one of the three lanes
    /// </summary>
    /// <param name="direction">The movement value of the player's lane to the right or left.</param>
    private void ChangeLanePlayer(int direction)
    {
        fingerUp = fingerDown = Vector2.zero;
        int targetLane = _CurrentLane + direction;

        if (targetLane >= -1 && targetLane <= 1)
        {
            _CurrentLane = targetLane;
            TargetMove += (transform.right * direction) * HorizontalMoveSpeed;
        }
    }
    /// <summary>
    /// Make sure the player is neither skiing nor jumping (if he is jumping, the StopJumping function is called),
    /// then add the animation time and set BaseSkiingTime to _SkiingTime and play the skiing  sound, to start skiing .
    /// </summary>
    private void SkiingPlayer()
    {
        if (!_IsSkiing)
        {

            if (_IsJumping)
                StopJumping();
            float correctSkiingLength = SkiingLength * (1.0f + SpeedRatio);
            _SkiingStartDistance = WorldDistance;
            _SkiingTime = BaseSkiingTime + (BaseSkiingTime * SpeedRatio);
            AnimatorPlayer.SetFloat(_JumpingSpeedHash, _SkiingTime);
            AnimatorPlayer.SetBool(_SkiingHash, true);
            _IsSkiing = true;
            ChangeBoxColliderForSkiing();
            TargetMove -= transform.up * JumpLength;
            PlayerSound.SKiingEffect();
        }
    }

    private void ChangeBoxColliderForSkiing()
    {
        PlayerNormalBoxCollider.gameObject.SetActive(!_IsSkiing);
        PlayerSkiingBoxCollider.gameObject.SetActive(_IsSkiing);
    }

    public void StopSkiingPlayer()
    {
        if (_IsSkiing)
        {
            AnimatorPlayer.SetBool(_SkiingHash, false);
            _IsSkiing = false;
            ChangeBoxColliderForSkiing();
            PlayerSound.MoveEffect();
        }
    }
    private void StopJumping()
    {
        if (_IsJumping)
        {
            if (transform.position.y > 0)
                TargetMove -= transform.up * JumpLength;
            AnimatorPlayer.SetBool(_JumpingHash, false);
            _IsJumping = false;
            PlayerSound.MoveEffect();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Obstacle"))
        {
            _IsStartRun = false;
            other.GetComponent<ObstacleEffect>().HitEffect();
            PlayerSound.HitSound();
            AnimatorPlayer.SetBool(_MovingHash, false);
            AnimatorPlayer.SetBool(_HitHash, true);
            AnimatorPlayer.SetBool(_DeadHash, true);
            LevelManager.Instance.FinishGamePlay();
        }
        else if (other.tag.Equals("Coins"))
        {
            other.GetComponent<EffectSound>().HitSound();
            other.gameObject.SetActive(false);
            Coins += 1;
        }
    }
}
