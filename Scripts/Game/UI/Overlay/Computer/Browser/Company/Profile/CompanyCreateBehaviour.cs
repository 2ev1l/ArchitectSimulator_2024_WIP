using Game.Serialization.World;
using Game.UI.Elements;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Browser.Company
{
    public class CompanyCreateBehaviour : MonoBehaviour
    {
        #region fields & properties
        private CompanyData Context => GameData.Data.CompanyData;
        public UnityEvent OnFirstCreated;
        public UnityEvent OnAlreadyCreated;

        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI infoText;

        [SerializeField] private Color errorColor;
        [SerializeField] private Color successColor;

        private static readonly LanguageInfo WrongInputInfo = new(37, TextType.Game);
        private static readonly LanguageInfo ServerErrorInfo = new(38, TextType.Game);
        private static readonly LanguageInfo SuccessInfo = new(51, TextType.Game);
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (Context.IsCreated)
                OnAlreadyCreated?.Invoke();
        }
        public void TryCreateCompany()
        {
            if (Context.IsCreated)
            {
                SetServerError();
                return;
            }

            string companyName = inputField.text;
            if (companyName.Length < 3)
            {
                SetWrongInput();
                return;
            }

            if (CustomMath.GetRandomChance(40))
            {
                SetServerError();
                return;
            }
            
            if (!Context.TryCreateCompany(companyName))
            {
                SetServerError();
                return;
            }

            SetSuccess();
            OnFirstCreated?.Invoke();
        }
        private void SetWrongInput()
        {
            infoText.text = WrongInputInfo.Text;
            infoText.color = errorColor;
        }

        private void SetServerError()
        {
            infoText.text = ServerErrorInfo.Text;
            infoText.color = errorColor;
        }

        private void SetSuccess()
        {
            infoText.text = SuccessInfo.Text;
            infoText.color = successColor;
        }
        #endregion methods
    }
}