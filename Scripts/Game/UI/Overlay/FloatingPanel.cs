using Game.Animation;
using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Time;

namespace Game.UI.Overlay
{
    public class FloatingPanel : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Canvas panelRenderCanvas;
        [SerializeField] private CustomButton activateButton;
        [SerializeField] private ObjectMove objectMover;
        [SerializeField][Min(0)] private int closePositionId = 0;
        [SerializeField][Min(0)] private int openPositionId = 1;
        private bool isOpened = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            activateButton.OnClicked += ChangeState;
        }
        private void OnDisable()
        {
            activateButton.OnClicked -= ChangeState;
        }
        private void ChangeState()
        {
            isOpened = !isOpened;
            if (isOpened)
            {
                objectMover.MoveTo(openPositionId);
                CheckCanvasRender();
            }
            else
            {
                objectMover.MoveTo(closePositionId);
                Invoke(nameof(CheckCanvasRender), objectMover.MoveTime);
            }
        }
        private void CheckCanvasRender()
        {
            panelRenderCanvas.enabled = isOpened;
        }
        #endregion methods
    }
}