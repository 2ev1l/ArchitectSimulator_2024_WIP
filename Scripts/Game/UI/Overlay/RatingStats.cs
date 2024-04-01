using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Overlay
{
    public class RatingStats : StatsContent
    {
        #region fields & properties
        [SerializeField] private Slider progressSlider;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            CompanyData.Rating.OnValueChanged += UpdateUI;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            CompanyData.Rating.OnValueChanged -= UpdateUI;
            base.OnDisable();
        }
        private void UpdateUI(int _1, int _2) => UpdateUI();
        public override void UpdateUI()
        {
            progressSlider.value = CompanyData.Rating.Value;
        }
        #endregion methods
    }
}