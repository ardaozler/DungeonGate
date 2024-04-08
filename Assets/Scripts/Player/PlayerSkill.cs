using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int _maxAttackSteps;
    private List<SkillStep> _skillSteps = new List<SkillStep>();

    [SerializeField] private float _attackButtonHoldTime = .5f;
    [SerializeField] private List<AttackSkillBlueprint> _attackBlueprints;

    [FormerlySerializedAs("_mobilitySkills")] [SerializeField]
    private List<MobilitySkillBlueprint> _mobilitySkillBlueprints = new List<MobilitySkillBlueprint>();


    [SerializeField] private Transform _firePoint;


    private float _attackButtonHoldTimer = 0f;

    public static UnityEvent<Color> AddedNewColor = new UnityEvent<Color>();
    public static UnityEvent ClearAttack = new UnityEvent();

    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleAttack();

        if (_skillSteps.Count < _maxAttackSteps)
        {
            if (Input.GetMouseButtonDown(1))
            {
                AddSkillStep(Color.red);
            }

            if (Input.GetMouseButtonDown(0))
            {
                AddSkillStep(Color.blue);
            }
        }
    }

    private void HandleAttack()
    {
        //if one of the mouse buttons is held down for more than _attackButtonHoldTime 
        if ((Input.GetMouseButton(1) || Input.GetMouseButton(0)) && _skillSteps.Count > 0)
        {
            _attackButtonHoldTimer += Time.deltaTime;
            if (_attackButtonHoldTimer >= _attackButtonHoldTime)
            {
                //check if any of the attack blueprints matches the magic steps
                foreach (var attackBlueprint in _attackBlueprints)
                {
                    if (attackBlueprint.CompareTo(_skillSteps))
                    {
                        //if they match, instantiate the attack, and clear the steps, return
                        UseAttackSkill(attackBlueprint);
                        break;
                    }
                }

                foreach (var mobilitySkillBlueprint in _mobilitySkillBlueprints)
                {
                    if (mobilitySkillBlueprint.CompareTo(_skillSteps))
                    {
                        UseMobilitySkill(mobilitySkillBlueprint);
                        break;
                    }
                }

                //if they don't match, just clear the steps
                _skillSteps.Clear();
                ClearAttack.Invoke();
            }
        }
        else
        {
            _attackButtonHoldTimer = 0f;
        }
    }

    private void AddSkillStep(Color color)
    {
        _skillSteps.Add(new SkillStep(color));
        AddedNewColor.Invoke(color);
    }

    private void UseAttackSkill(AttackSkillBlueprint attackSkillBlueprint)
    {
        Instantiate(attackSkillBlueprint.AttackPrefab, _firePoint.position, _firePoint.rotation);
    }

    private void UseMobilitySkill(MobilitySkillBlueprint mobilitySkillBlueprint)
    {
        transform.DOMove(transform.position + Vector3.zero, mobilitySkillBlueprint.channelTime).OnComplete(() =>
        {
            Vector3 towards = Vector3.zero;
            switch (mobilitySkillBlueprint.goTowards)
            {
                case MobilitySkillBlueprint.GoOrientation.Mouse:
                    towards = Camera.main.transform.forward;
                    break;
                case MobilitySkillBlueprint.GoOrientation.Keyboard:
                    towards = _controller.velocity.normalized;
                    break;
            }

            switch (mobilitySkillBlueprint.movementType)
            {
                case MobilitySkillBlueprint.MovementType.Dash:
                    StartCoroutine(Dash(Camera.main.transform.forward, mobilitySkillBlueprint.completionTime,
                        mobilitySkillBlueprint.dashSpeed));
                    break;
                case MobilitySkillBlueprint.MovementType.Blink:
                    
                    break;
            }
        });
    }

    private IEnumerator Dash(Vector3 towards, float dashTime, float dashSpeed)
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            _controller.Move(towards * (dashSpeed * Time.deltaTime));
            yield return null;
        }
    }
    
    private void Blink()
}