using EditorCustom.Attributes;
using Game.Events;
using Game.Serialization.World;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.Environment.Observers
{
    public class StatsObserver : SingleSceneInstance<StatsObserver>
    {
        #region fields & properties
        [SerializeField] private PopupRequest moneyPopup;
        [SerializeField] private PopupRequest ratingPopup;
        [SerializeField] private PopupRequest moodPopup;
        [SerializeField] private PopupRequest timePopup;

        [SerializeField] private PopupRequest newTaskPopup;
        [SerializeField] private PopupRequest completedTaskPopup;

        private static LanguageInfo newTaskInfo = new(0, TextType.Task);
        private static LanguageInfo completedTaskInfo = new(1, TextType.Task);
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            PlayerData playerData = GameData.Data.PlayerData;
            playerData.Wallet.OnValueChanged += SendMoneyRequest;
            playerData.Mood.OnValueChanged += SendMoodRequest;
            playerData.MonthData.FreeTime.OnValueChanged += SendTimeRequest;

            playerData.Tasks.OnTaskCompleted += SendTaskCompletedRequest;
            playerData.Tasks.Data.OnItemAdded += SendTaskStartRequest;

            CompanyData companyData = GameData.Data.CompanyData;
            companyData.Rating.OnValueChanged += SendRatingRequest;

        }
        private void OnDisable()
        {
            PlayerData playerData = GameData.Data.PlayerData;
            playerData.Wallet.OnValueChanged -= SendMoneyRequest;
            playerData.Mood.OnValueChanged -= SendMoodRequest;
            playerData.MonthData.FreeTime.OnValueChanged -= SendTimeRequest;

            playerData.Tasks.OnTaskCompleted -= SendTaskCompletedRequest;
            playerData.Tasks.Data.OnItemAdded -= SendTaskStartRequest;

            CompanyData companyData = GameData.Data.CompanyData;
            companyData.Rating.OnValueChanged -= SendRatingRequest;
        }
        private void SendTaskStartRequest(TaskData task)
        {
            newTaskPopup.TextPostfix = newTaskInfo.Text;
            newTaskPopup.Send();
        }
        private void SendTaskCompletedRequest(TaskData task)
        {
            completedTaskPopup.TextPostfix = completedTaskInfo.Text;
            completedTaskPopup.Send();
        }
        private void SendMoneyRequest(int currentValue, int changedAmount)
        {
            SetPopupValues(moneyPopup, currentValue, changedAmount);
            moneyPopup.Send();
        }
        private void SendRatingRequest(int currentValue, int changedAmount)
        {
            SetPopupValues(ratingPopup, currentValue, changedAmount);
            ratingPopup.Send();
        }
        private void SendMoodRequest(int currentValue, int changedAmount)
        {
            SetPopupValues(moodPopup, currentValue, changedAmount);
            moodPopup.Send();
        }
        private void SendTimeRequest(int currentValue, int changedAmount)
        {
            SetPopupValues(timePopup, currentValue, changedAmount);
            timePopup.Send();
        }
        private void SetPopupValues(PopupRequest popupRequest, int currentValue, int changedAmount)
        {
            popupRequest.ValueGain = changedAmount;
            popupRequest.ValueCurrent = currentValue;
        }
        #endregion methods
    }
}