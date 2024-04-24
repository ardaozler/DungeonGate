using System;
using UnityEngine;

public class LeftHandAnimationEventRelayer : MonoBehaviour
{
    public static event Action SkillShootAnimEventRelay;

    public void SkillShootAnimEvent()
    {
        SkillShootAnimEventRelay?.Invoke();
    }
}