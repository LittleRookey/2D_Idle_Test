using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class SkillRankScrollView : FancyScrollView<Dictionary<int, List<StatModifier>>>
{
    [SerializeField] Scroller scroller = default;
    [SerializeField] GameObject cellPrefab = default;
    protected override GameObject CellPrefab => cellPrefab;

    protected override void Initialize()
    {
        base.Initialize();
        scroller.OnValueChanged(UpdatePosition);
    }

    public void UpdateData(IList<Dictionary<int, List<StatModifier>>> items)
    {
        UpdateContents(items);
        scroller.SetTotalCount(items.Count);
    }
}
