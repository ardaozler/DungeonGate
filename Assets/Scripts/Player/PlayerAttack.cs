using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int _maxAttackSteps;
    private List<AttackStep> _attackSteps = new List<AttackStep>();

    [SerializeField] private float _attackButtonHoldTime = .5f;
    [SerializeField] private List<AttackBlueprint> _attackBlueprints;

    [SerializeField] private Transform _firePoint;


    private float _attackButtonHoldTimer = 0f;

    public static UnityEvent<Color> AddedNewColor = new UnityEvent<Color>();
    public static UnityEvent ClearAttack = new UnityEvent();


    private void Update()
    {
        HandleAttack();

        if (_attackSteps.Count < _maxAttackSteps)
        {
            if (Input.GetMouseButtonDown(1))
            {
                AddAttackStep(Color.red);
            }

            if (Input.GetMouseButtonDown(0))
            {
                AddAttackStep(Color.blue);
            }
        }
    }

    private void HandleAttack()
    {
        //if one of the mouse buttons is held down for more than _attackButtonHoldTime 
        if ((Input.GetMouseButton(1) || Input.GetMouseButton(0)) && _attackSteps.Count > 0)
        {
            _attackButtonHoldTimer += Time.deltaTime;
            if (_attackButtonHoldTimer >= _attackButtonHoldTime)
            {
                //check if any of the attack blueprints matches the magic steps
                foreach (var attackBlueprint in _attackBlueprints)
                {
                    if (attackBlueprint.CompareTo(_attackSteps))
                    {
                        //if they match, instantiate the attack, and clear the steps, return
                        Instantiate(attackBlueprint.AttackPrefab, _firePoint.position, _firePoint.rotation);
                        _attackSteps.Clear();
                        ClearAttack.Invoke();
                        return;
                    }
                }

                //if they don't match, just clear the steps
                _attackSteps.Clear();
                ClearAttack.Invoke();
            }
        }
        else
        {
            _attackButtonHoldTimer = 0f;
        }
    }

    private void AddAttackStep(Color color)
    {
        _attackSteps.Add(new AttackStep(color));
        AddedNewColor.Invoke(color);
    }
}

[System.Serializable]
public class AttackStep
{
    public Color color;

    public AttackStep(Color color)
    {
        this.color = color;
    }
}

[System.Serializable]
public class AttackBlueprint
{
    [FormerlySerializedAs("MagicSteps")] public List<AttackStep> AttackSteps;

    public GameObject AttackPrefab;

    public AttackBlueprint(List<AttackStep> attackSteps, GameObject attackPrefab)
    {
        AttackSteps = attackSteps;
        AttackPrefab = attackPrefab;
    }

    public bool CompareTo(AttackBlueprint other)
    {
        return CompareTo(other.AttackSteps) && other.AttackPrefab == AttackPrefab;
    }

    public bool CompareTo(List<AttackStep> other)
    {
        bool result = false;

        if (this.AttackSteps.Count == other.Count)
        {
            result = true;
            for (int i = 0; i < this.AttackSteps.Count; i++)
            {
                if (this.AttackSteps[i].color != other[i].color)
                {
                    result = false;
                    break;
                }
            }
        }

        return result;
    }
}