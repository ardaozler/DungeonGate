using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackIndicatorPanel : MonoBehaviour
{
    [SerializeField] private GameObject _indicatorPrefab;
    private List<GameObject> _indicators = new List<GameObject>();

    private void OnEnable()
    {
        PlayerSkill.AddedNewColor.AddListener(OnAddedNewColor);
        PlayerSkill.ClearAttack.AddListener(OnClearAttack);
    }

    private void OnAddedNewColor(Color color)
    {
        var indicator = Instantiate(_indicatorPrefab, transform);
        _indicators.Add(indicator);
        indicator.GetComponent<Image>().color = color;
    }

    private void OnClearAttack()
    {
        foreach (var indicator in _indicators)
        {
            Destroy(indicator);
        }
        _indicators.Clear();
    }
}