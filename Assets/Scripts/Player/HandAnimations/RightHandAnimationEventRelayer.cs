using System;
using UnityEngine;

public class RightHandAnimationEventRelayer : MonoBehaviour
{
    public static event Action SkillShootAnimEventRelay;

    public void SkillShootAnimEvent()
    {
        SkillShootAnimEventRelay?.Invoke();
    }
}