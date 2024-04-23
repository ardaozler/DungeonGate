using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController), typeof(HandsAnimator))]
public class PlayerSkill : MonoBehaviour
{
    [SerializeField] private int _maxAttackSteps;
    private List<SkillStep> _skillSteps = new List<SkillStep>();

    [SerializeField] private float _attackButtonHoldTime = .5f;
    [SerializeField] private List<AttackSkillBlueprint> _attackBlueprints;

    [FormerlySerializedAs("_mobilitySkills")] [SerializeField]
    private List<MobilitySkillBlueprint> _mobilitySkillBlueprints = new List<MobilitySkillBlueprint>();


    [SerializeField] private Transform _firePoint;


    [SerializeField] private LayerMask _blinkCheckLayerMask;


    private float _attackButtonHoldTimer = 0f;

    public static UnityEvent<Color> AddedNewColor = new UnityEvent<Color>();
    public static UnityEvent ClearAttack = new UnityEvent();

    private CharacterController _controller;
    private HandsAnimator _handsAnimator;
    
    //attacking
    private string _currentSkillId = "";
    private bool _isLeftShooting;
    private bool _isRightShooting;

    private void OnEnable()
    {
        RightHandAnimationEventRelayer.SkillShootAnimEventRelay += Shoot;
        LeftHandAnimationEventRelayer.SkillShootAnimEventRelay += Shoot;
    }

    private void OnDisable()
    {
        RightHandAnimationEventRelayer.SkillShootAnimEventRelay -= Shoot;
        LeftHandAnimationEventRelayer.SkillShootAnimEventRelay -= Shoot;
    }

    private void Shoot()
    {
        // shoot trigger has been triggered
        UseSkill(_currentSkillId);
        //find a way to differentiate between one shot skills and hold skills so we know when to start the reverse anim 

        //_isShootAnimEventTriggered = true;
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _handsAnimator = GetComponent<HandsAnimator>();
    }

    private void Update()
    {
        HandleAttack();

        if (_skillSteps.Count < _maxAttackSteps)
        {
            if (Input.GetMouseButtonDown(0))
            {
                AddSkillStep(Color.blue);
            }

            if (Input.GetMouseButtonUp(0))
            {
                SetIsLeftShooting(false);
            }

            if (Input.GetMouseButtonDown(1))
            {
                AddSkillStep(Color.red);
            }

            if (Input.GetMouseButtonUp(1))
            {
                SetIsRightShooting(false);
            }
        }
        else
        {
            SetIsLeftShooting(false);
            SetIsRightShooting(false);
        }
    }

    private void HandleAttack()
    {
        //if one of the mouse buttons is held down for more than _attackButtonHoldTime 
        if ((Input.GetMouseButton(1) || Input.GetMouseButton(0)) && _skillSteps.Count > 0)
        {
            _currentSkillId = GetSkillId(_skillSteps);

            _attackButtonHoldTimer += Time.deltaTime;
            if (_attackButtonHoldTimer >= _attackButtonHoldTime)
            {
                if (_skillSteps[^1].color == Color.blue)
                {
                    SetIsLeftShooting(true);
                }
                else
                {
                    SetIsRightShooting(true);
                }

                //check if any of the attack blueprints matches the magic steps
                foreach (var attackBlueprint in _attackBlueprints)
                {
                    if (attackBlueprint.CompareTo(_skillSteps))
                    {
                        break;
                    }
                }

                foreach (var mobilitySkillBlueprint in _mobilitySkillBlueprints)
                {
                    if (mobilitySkillBlueprint.CompareTo(_skillSteps))
                    {
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

    private void UseSkill(string skillId)
    {
        //TODO: OPTIMIZE this poo poo
        //find the blueprint
        var attackBlueprint = _attackBlueprints.Find(x => x.skillId == skillId);
        var mobilitySkillBlueprint = _mobilitySkillBlueprints.Find(x => x.skillId == skillId);
        if (attackBlueprint != null)
        {
            UseAttackSkill(attackBlueprint);
        }

        if (mobilitySkillBlueprint != null)
        {
            UseMobilitySkill(mobilitySkillBlueprint);
        }
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
                    StartCoroutine(Dash(towards, mobilitySkillBlueprint.completionTime,
                        mobilitySkillBlueprint.dashSpeed));
                    break;
                case MobilitySkillBlueprint.MovementType.Blink:
                    Blink(towards, mobilitySkillBlueprint.maxBlinkDistance);
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

    private void Blink(Vector3 towards, float maxDistance)
    {
        //disable the controller so it doesnt override the transform position set 
        _controller.enabled = false;
        if (!Physics.Raycast(transform.position, towards, out RaycastHit hit, maxDistance, _blinkCheckLayerMask))
        {
            //raycast returned false so player is free to blink max distance
            transform.position += towards * maxDistance;
        }
        else
        {
            //raycast returned true so player is not free to blink max distance
            transform.position += towards * (hit.distance - 1);
        }

        _controller.enabled = true;
    }

    private void SetIsRightShooting(bool value)
    {
        //only if the value is changing from false to true set the skillStart
        if (value != _isRightShooting && value)
        {
            _handsAnimator.SetRightHandTrigger("SkillStart");
        }

        _isRightShooting = value;
        _handsAnimator.SetRightHandBool(_currentSkillId, value);
    }

    public bool IsRightHoldingDown()
    {
        return _isRightShooting;
    }

    private void SetIsLeftShooting(bool value)
    {
        if (value != _isLeftShooting && value)
        {
            _handsAnimator.SetLeftHandTrigger("SkillStart");
            _handsAnimator.SetLeftHandTrigger(_currentSkillId);
        }

        _isLeftShooting = value;
    }

    public bool IsLeftHoldingDown()
    {
        return _isLeftShooting;
    }


    public int CurrentSkillStepCount()
    {
        return _skillSteps.Count;
    }


    public static string GetSkillId(List<SkillStep> skillSteps)
    {
        string skillId = "";
        //TODO: Change after demo to be red r blue b green g
        for (int i = 0; i < skillSteps.Count; i++)
        {
            if (skillSteps[i].color == Color.red)
            {
                skillId += "R";
            }
            else
            {
                skillId += "B";
            }
        }

        return skillId;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, Camera.main.transform.forward * 10);
    }
}