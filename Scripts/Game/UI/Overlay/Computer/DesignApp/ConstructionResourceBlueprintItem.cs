using EditorCustom.Attributes;
using Game.DataBase;
using Game.Serialization.World;
using Game.UI.Collections;
using Game.UI.Elements;
using Game.UI.Overlay.Computer.Collections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Overlay.Computer.DesignApp
{
    /// <summary>
    /// No other resources will be used for blueprint than 'construction', so there's enough simple inheritance
    /// </summary>
    public class ConstructionResourceBlueprintItem : ContextActionsItem<ResourceData>
    {
        #region fields & properties
        private ConstructionResourceInfo Info => (ConstructionResourceInfo)Context.Info;
        [SerializeField] private ConstructionResourceItem constructionResourceItem;
        [SerializeField] private BlueprintResource blueprintElement;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private CustomButton buttonAdd;
        [SerializeField] private bool clampBlueprintWidth = true;
        [SerializeField][DrawIf(nameof(clampBlueprintWidth), true)] private float clampWidthMax = 270;
        [SerializeField] private bool clampBlueprintHeight = false;
        [SerializeField][DrawIf(nameof(clampBlueprintHeight), true)] private float clampHeightMax = 60;
        [SerializeField] private List<LayoutGroup> staticLayoutGroups;
        [SerializeField] private List<ContentSizeFitter> staticContentSizeFilters;
        #endregion fields & properties

        #region methods
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            Context.OnCountChanged += UpdateCountText;
            if (buttonAdd != null)
                buttonAdd.OnClicked += SpawnBlueprint;
        }
        protected override void OnUnSubscribe()
        {
            base.OnUnSubscribe();
            Context.OnCountChanged -= UpdateCountText;
            if (buttonAdd != null)
                buttonAdd.OnClicked -= SpawnBlueprint;
        }
        private void UpdateCountText()
        {
            if (countText != null)
                countText.text = $"x{Context.Count}";
        }
        private void SpawnBlueprint()
        {
            BlueprintResourcePlacer placer = BlueprintEditor.Instance.Creator.CurrentFloor.SpawnResource(Info.Blueprint.ConstructionReferenceId, BlueprintEditor.Instance.ViewCenter, 0, constructionResourceItem.CurrentColorId, false);
            BlueprintEditor.Instance.Selector.TrySelectElement(placer);
            placer.CheckDeepPlacementSmoothly();
        }
        protected override void UpdateUI()
        {
            base.UpdateUI();
            UpdateCountText();
            if (blueprintElement.ConstructionReferenceId == Info.Id) return;

            blueprintElement.ReplaceWithMultipleInstantiating(Info.Blueprint);
            if (clampBlueprintHeight && clampBlueprintWidth)
            {
                blueprintElement.ClampScaleWidthAndHeight(clampWidthMax, clampHeightMax);
            }
            else
            {
                if (clampBlueprintWidth)
                    blueprintElement.ClampScaleWidth(clampWidthMax);
                if (clampBlueprintHeight)
                    blueprintElement.ClampScaleHeight(clampHeightMax);
            }

            Invoke(nameof(DisableLayoutGroups), Mathf.Max(1, Time.deltaTime * 2));

        }
        private void DisableLayoutGroups()
        {
            int scsfc = staticContentSizeFilters.Count;
            for (int i = 0; i < scsfc; ++i)
            {
                staticContentSizeFilters[i].enabled = false;
            }
            int slgc = staticLayoutGroups.Count;
            for (int i = 0; i < slgc; ++i)
            {
                staticLayoutGroups[i].enabled = false;
            }
        }
        public override void OnListUpdate(ResourceData param)
        {
            if (Context != param)
            {
                base.OnListUpdate(param);
            }
            else
            {
                UpdateUI();
            }
            constructionResourceItem.OnListUpdate(Context.Info);
        }
        #endregion methods
    }
}