using Game.Events;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Events;

namespace Game.Environment
{
    public class Bed : InteractableObject
    {
        #region fields & properties
        public static readonly LanguageInfo SleepInfo = new(12, TextType.Game);
        public static readonly LanguageInfo NextMonthInfo = new(13, TextType.Game);
        #endregion fields & properties

        #region methods
        protected override void OnInteract()
        {
            base.OnInteract();
            ConfirmRequest request = new(OnSleepConfirmed, null, SleepInfo, NextMonthInfo);
            request.Send();
        }
        private void OnSleepConfirmed()
        {
            //todo
        }
        #endregion methods
    }
}