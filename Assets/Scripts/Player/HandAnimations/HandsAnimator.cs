using System;
using UnityEngine;

[RequireComponent(typeof(PlayerSkill), typeof(PlayerMovement))]
public class HandsAnimator : MonoBehaviour
{
    private PlayerSkill _playerSkill;
    private PlayerMovement _playerMovement;


    [SerializeField] private Animator _leftHandAnimator;
    [SerializeField] private Animator _rightHandAnimator;

    private void Awake()
    {
        _playerSkill = GetComponent<PlayerSkill>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        SetBothHandsBool("IsMoving", _playerMovement.IsMoving());
        SetLeftHandBool("HoldingDown", _playerSkill.IsLeftHoldingDown());
        SetRightHandBool("HoldingDown", _playerSkill.IsRightHoldingDown());
    }

    public void SetBothHandsTrigger(string trigger)
    {
        _leftHandAnimator.SetTrigger(trigger);
        _rightHandAnimator.SetTrigger(trigger);
    }

    public void SetBothHandsBool(string boolName, bool value)
    {
        _leftHandAnimator.SetBool(boolName, value);
        _rightHandAnimator.SetBool(boolName, value);
    }

    public void SetLeftHandTrigger(string trigger)
    {
        _leftHandAnimator.SetTrigger(trigger);
    }

    public void SetRightHandTrigger(string trigger)
    {
        _rightHandAnimator.SetTrigger(trigger);
    }

    public void SetLeftHandBool(string boolName, bool value)
    {
        _leftHandAnimator.SetBool(boolName, value);
    }

    public void SetRightHandBool(string boolName, bool value)
    {
        _rightHandAnimator.SetBool(boolName, value);
    }
}