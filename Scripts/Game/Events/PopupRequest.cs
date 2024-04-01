using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Events;

namespace Game.Events
{
    [System.Serializable]
    public class PopupRequest : ExecutableRequest
    {
        #region fields & properties
        public int ValueGain
        {
            get => valueGain;
            set => valueGain = value;
        }
        [SerializeField] private int valueGain;
        public int ValueCurrent
        {
            get => valueCurrent;
            set => valueCurrent = value;
        }
        [SerializeField] private int valueCurrent;
        public bool ShowOnlyText
        {
            get => showOnlyText;
            set => showOnlyText = value;
        }
        [SerializeField] private bool showOnlyText = false;
        public Sprite IndicatorSprite => indicatorSprite;
        [SerializeField] private Sprite indicatorSprite;
        public string TextPostfix
        {
            get => textPostfix;
            set => textPostfix = value;
        }
        [SerializeField] private string textPostfix;
        #endregion fields & properties

        #region methods
        public override void Close()
        {

        }
        #endregion methods
    }
}