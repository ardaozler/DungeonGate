using System;
using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    [SerializeField] private Animator _leftHandAnimator, _rightHandAnimator;
    public int _currentLeftAnimation, _currentRightAnimation;

    private static readonly int Idle = Animator.StringToHash("HandIdle");
    private static readonly int Running = Animator.StringToHash("HandRunning");
    private static readonly int Shoot = Animator.StringToHash("HandShoot");
    private static readonly int ShootStart = Animator.StringToHash("HandShootStart");
    private static readonly int ShootEnd = Animator.StringToHash("-HandShootStart");
    private static readonly int RedSkill1 = Animator.StringToHash("HandRedSkill1");
    private static readonly int RedSkill1End = Animator.StringToHash("-HandRedSkill1");

    public float _shootAnimDuration;
    public float _redSkill1AnimDuration;

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerSkill _playerSkill;

    private bool _isLeftShooting;
    private bool _isRightShooting;
    private bool _isUsingRightSkill;
    private bool _isUsingLeftSkill;
    private bool _isMoving;

    private float _leftLockedUntil;
    private float _rightLockedUntil;


    private void Update()
    {
        _isMoving = _playerMovement.IsMoving();
        _isLeftShooting = _playerSkill.IsLeftShooting();
        _isRightShooting = _playerSkill.IsRightShooting();
        _isUsingLeftSkill = _playerSkill.IsUsingLeftSkill();
        _isUsingRightSkill = _playerSkill.IsUsingRightSkill();


        //left hand------------------------------------
        var leftAnim = GetLeftAnim();
        if (leftAnim != _currentLeftAnimation)
        {
            _leftHandAnimator.CrossFade(leftAnim, 0, 0);
            _currentLeftAnimation = leftAnim;
        }


        //right hand------------------------------------
        var rightAnim = GetRightAnim();
        if (rightAnim != _currentRightAnimation)
        {
            _rightHandAnimator.CrossFade(rightAnim, 0, 0);
            _currentRightAnimation = rightAnim;
        }
    }

    private int GetLeftAnim()
    {
        //can override other anims with shooting anim
        // shooting
        if (_isLeftShooting) return LeftLockAnimState(ShootStart, _shootAnimDuration);

        //the rest needs to wait until available
        if (Time.time < _leftLockedUntil) return _currentLeftAnimation;
        // using skills
        if (_isUsingLeftSkill) return LeftLockAnimState(RedSkill1, _redSkill1AnimDuration);
        // moving
        if (_isMoving) return Running;
        // idle
        return Idle;
    }

    private int GetRightAnim()
    {
        //can override other anims with shooting anim
        // shooting
        if (_isRightShooting) return RightLockAnimState(ShootStart, _shootAnimDuration);

        //the rest needs to wait until available
        if (Time.time < _rightLockedUntil) return _currentRightAnimation;
        // using skills
        if (_isUsingRightSkill) return RightLockAnimState(RedSkill1, _redSkill1AnimDuration);
        // moving
        if (_isMoving) return Running;
        // idle
        return Idle;
    }

    private int LeftLockAnimState(int state, float time)
    {
        _leftLockedUntil = Time.time + time;
        return state;
    }

    private int RightLockAnimState(int state, float time)
    {
        _rightLockedUntil = Time.time + time;
        return state;
    }
}