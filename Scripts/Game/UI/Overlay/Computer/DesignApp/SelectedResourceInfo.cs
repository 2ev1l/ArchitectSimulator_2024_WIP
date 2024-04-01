using EditorCustom.Attributes;
using Game.Events;
using Game.UI.Overlay.Computer.Collections;
using Game.UI.Overlay.Computer.DesignApp;
using TMPro;
using UnityEngine;
using Universal.Collections.Generic;
using Universal.Time;

namespace Game.UI.Overlay.Computer
{
    internal class SelectedResourceInfo : SelectedBlueprintInfo<BlueprintResourcePlacer>
    {
        #region fields & properties
        [Title("Resource")]
        [SerializeField] private ConstructionResourceBlueprintItem blueprintItem;

        [SerializeField] private TextMeshProUGUI requiredAndCountText;
        [SerializeField] private TextMeshProUGUI currentPlacementText;
        [SerializeField] private ResourceColorList colorList;
        [SerializeField] private Color badRequiredColor = Color.red;
        [SerializeField] private Color goodRequiredColor = Color.white;
        #endregion fields & properties

        #region methods
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            colorList.OnStateChanged += ChangeColor;
            Context.OnMoveEnd += UpdateCurrentPlacementText;
        }
        protected override void OnUnsubscribe()
        {
            base.OnUnsubscribe();
            colorList.OnStateChanged -= ChangeColor;
            Context.OnMoveEnd -= UpdateCurrentPlacementText;
        }
        protected override void OnUpdateVisibleUI()
        {
            base.OnUpdateVisibleUI();
            if (Context.ResourceData == null)
            {
                InfoRequest.GetErrorRequest(200).Send();
                return;
            }
            blueprintItem.OnListUpdate(Context.ResourceData);
            colorList.TryApplyState(Context.Element.ChoosedColor);
            int requiredCount = 0;
            if (BlueprintEditor.Instance.Creator.CurrentFloor.ResourcesPool.TryGetValue(Context.Element.ConstructionReferenceId, out ObjectPool<BlueprintPlacerBase> objectPool))
            {
                requiredCount = objectPool.ActiveObjectsCount;
            }
            int existsCount = Context.ResourceData.Count;
            requiredAndCountText.text = $"x{requiredCount} / x{existsCount}";
            requiredAndCountText.color = existsCount < requiredCount ? badRequiredColor : goodRequiredColor;
            UpdateCurrentPlacementText();
        }
        private void UpdateCurrentPlacementText()
        {
            currentPlacementText.text = $"[{Context.Transform.localPosition.x:F0}, {Context.Transform.localPosition.y:F0}]";
        }
        private void ChangeColor()
        {
            Context.Element.ChangeResourceColor(colorList.CurrentColorId);
        }
        #endregion methods
    }
}