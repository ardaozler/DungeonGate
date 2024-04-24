using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class MobilitySkillBlueprint
{
    public List<SkillStep> skillSteps;


    public string skillId;

    [Tooltip("The time in seconds it takes to channel the skill")]
    public float channelTime;

    [Header("Dash Properties")]
    [Tooltip(
        "The time it takes to complete the movement after the skill is channeled(Only applicable if it is not a blink)")]
    public float completionTime;

    [Tooltip(
        "The movement vector that will be applied to the player after the skill is channeled(Only applicable if it is not a blink)")]
    public float dashSpeed;

    [Header("Blink Properties")] public float maxBlinkDistance;

    [Tooltip("Which way the  \"dash\" should move, mouse means it will move towards the forward of the mouse, " +
             "keyboard means it will move towards the last pressed key on the keyboard")]
    public GoOrientation goTowards;

    public MovementType movementType;

    public enum MovementType
    {
        Dash,
        Blink
    }

    public enum GoOrientation
    {
        Mouse,
        Keyboard
    }

    public MobilitySkillBlueprint(List<SkillStep> skillSteps)
    {
        this.skillSteps = skillSteps;
        skillId = PlayerSkill.GetSkillId(skillSteps);
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