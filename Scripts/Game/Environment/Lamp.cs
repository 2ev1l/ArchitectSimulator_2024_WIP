using Game.Audio;
using Game.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
    public class Lamp : InteractableObject
    {
        #region fields & properties
        [SerializeField] private Material enabledMaterial;
        [SerializeField] private Material disabledMaterial;
        [SerializeField] private GameObject pointLight;
        [SerializeField] private AudioClipData switchAudio;
        [SerializeField] private bool isActivated = false;
        #endregion fields & properties

        #region methods

        protected override void OnInteract()
        {
            base.OnInteract();
            if (isActivated)
                TurnOff();
            else
                TurnOn();
        }
        private void TurnOn()
        {
            isActivated = true;
            pointLight.SetActive(true);
            Render.material = enabledMaterial;
            switchAudio.Play();
        }
        private void TurnOff()
        {
            isActivated = false;
            pointLight.SetActive(false);
            Render.material = disabledMaterial;
            switchAudio.Play();
        }
        private void OnValidate()
        {
            try
            {
                if (isActivated)
                    TurnOn();
                else
                    TurnOff();
            }
            catch { }
        }
        #endregion methods
    }
}