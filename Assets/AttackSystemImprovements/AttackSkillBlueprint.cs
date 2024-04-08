using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SkillStep
{
    public Color color;

    public SkillStep(Color color)
    {
        this.color = color;
    }
}

[System.Serializable]
public class AttackSkillBlueprint
{
    [FormerlySerializedAs("AttackSteps")] public List<SkillStep> SkillSteps;

    public GameObject AttackPrefab;

    public AttackSkillBlueprint(List<SkillStep> skillSteps, GameObject attackPrefab)
    {
        SkillSteps = skillSteps;
        AttackPrefab = attackPrefab;
    }

    public bool CompareTo(AttackSkillBlueprint other)
    {
        return CompareTo(other.SkillSteps) && other.AttackPrefab == AttackPrefab;
    }

    public bool CompareTo(List<SkillStep> other)
    {
        bool result = false;

        if (this.SkillSteps.Count == other.Count)
        {
            result = true;
            for (int i = 0; i < this.SkillSteps.Count; i++)
            {
                if (this.SkillSteps[i].color != other[i].color)
                {
                    result = false;
                    break;
                }
            }
        }

        return result;
    }
}