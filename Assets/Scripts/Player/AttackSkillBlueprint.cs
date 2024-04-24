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
    [FormerlySerializedAs("SkillSteps")] public List<SkillStep> skillSteps;

    public string skillId;

    public GameObject AttackPrefab;

    public AttackSkillBlueprint(List<SkillStep> skillSteps, GameObject attackPrefab)
    {
        this.skillSteps = skillSteps;
        AttackPrefab = attackPrefab;
        skillId = PlayerSkill.GetSkillId(skillSteps);
    }

    public bool CompareTo(AttackSkillBlueprint other)
    {
        return CompareTo(other.skillSteps) && other.AttackPrefab == AttackPrefab;
    }

    public bool CompareTo(List<SkillStep> other)
    {
        bool result = false;

        if (this.skillSteps.Count == other.Count)
        {
            result = true;
            for (int i = 0; i < this.skillSteps.Count; i++)
            {
                if (this.skillSteps[i].color != other[i].color)
                {
                    result = false;
                    break;
                }
            }
        }

        return result;
    }
}