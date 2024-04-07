using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int _maxAttackSteps;
    private List<MagicStep> _attackSteps = new List<MagicStep>();

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
        _attackSteps.Add(new MagicStep(color));
        AddedNewColor.Invoke(color);
    }
}

[System.Serializable]
public class MagicStep
{
    public Color color;

    public MagicStep(Color color)
    {
        this.color = color;
    }
}

[System.Serializable]
public class AttackBlueprint
{
    public List<MagicStep> MagicSteps;

    public GameObject AttackPrefab;

    public AttackBlueprint(List<MagicStep> magicSteps, GameObject attackPrefab)
    {
        MagicSteps = magicSteps;
        AttackPrefab = attackPrefab;
    }

    public bool CompareTo(AttackBlueprint other)
    {
        return CompareTo(other.MagicSteps) && other.AttackPrefab == AttackPrefab;
    }

    public bool CompareTo(List<MagicStep> other)
    {
        bool result = false;

        if (this.MagicSteps.Count == other.Count)
        {
            result = true;
            for (int i = 0; i < this.MagicSteps.Count; i++)
            {
                if (this.MagicSteps[i].color != other[i].color)
                {
                    result = false;
                    break;
                }
            }
        }

        return result;
    }
}