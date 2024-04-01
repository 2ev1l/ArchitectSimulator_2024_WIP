using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.DataBase
{
    public class BlueprintGraphicUnitState : StateChange
    {
        #region fields & properties
        /// <summary>
        /// Unmarshalled gameObject
        /// </summary>
        public GameObject GameObject => _gameObject;
        [SerializeField] private GameObject _gameObject = null;
        [SerializeField] private bool applyableAlways = false;
        [SerializeField][DrawIf(nameof(applyableAlways), false)] private bool applyableNever = false;
        [SerializeField][DrawIf(nameof(applyableAlways), false)][DrawIf(nameof(applyableNever), false)] private ConstructionType applyableType;
        [SerializeField][DrawIf(nameof(applyableAlways), false)][DrawIf(nameof(applyableNever), false)] private ConstructionSubtype applyableSubtype;
        [SerializeField][DrawIf(nameof(applyableAlways), false)][DrawIf(nameof(applyableNever), false)] private ConstructionLocation applyableLocation;
        #endregion fields & properties

        #region methods
        public bool TrySetActive(ConstructionType type, ConstructionSubtype subtype, ConstructionLocation location)
        {
            if (applyableAlways)
            {
                SetActive(true);
                return true;
            }
            if (applyableNever)
            {
                SetActive(false);
                return false;
            }
            bool result = type == applyableType && subtype == applyableSubtype && applyableLocation == location;

            SetActive(result);
            return result;
        }
        public override void SetActive(bool active)
        {
            if (GameObject.activeSelf != active)
            {
                GameObject.SetActive(active);
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_gameObject == null)
                _gameObject = gameObject;
        }
#endif //UNIT_EDITOR
        #endregion methods
    }
}