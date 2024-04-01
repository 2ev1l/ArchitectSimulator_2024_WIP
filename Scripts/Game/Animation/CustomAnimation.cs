using System.Collections;
using UnityEngine;
using Universal.Core;
using Universal.Time;

namespace Game.Animation
{
    public class CustomAnimation : MonoBehaviour, IInitializable
    {
        #region fields & properties
        public static CustomAnimation Instance { get; private set; }
        private static ValueTimeChanger ForwardRotationTimeChanger => Instance.forwardRotationTimeChanger;
        private ValueTimeChanger forwardRotationTimeChanger = new();
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
        }
        public static void LookAt2D(Transform lookingObject, Vector3 offsetPosition, Vector3 targetPosition)
        {
            Vector3 targetDirection = targetPosition - offsetPosition;
            targetDirection.z = 0;
            lookingObject.up = targetDirection;
        }

        /// <summary>
        /// Not optimized for multiple queries.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="layer"></param>
        /// <returns>0..1f = 0..100%</returns>
        public static float GetNormalizedAnimatorTime(Animator animator, int layer) => Mathf.Clamp(animator.GetCurrentAnimatorStateInfo(layer).normalizedTime, 0, 1);
        /// <summary>
        /// May cause results for previous state when you call it immediately after <see cref="Animator.Play(int)"/> because of transition time
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="layer"></param>
        /// <returns>Unscaled time until current playing clip ends</returns>
        public static float GetTimeLasts(Animator animator, int layer)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            float clipTime = stateInfo.length;
            float currentNormalizedTime = 0;

            if (stateInfo.loop)
            {
                currentNormalizedTime = stateInfo.normalizedTime % 2;
                if (currentNormalizedTime > 1) currentNormalizedTime -= 1;
            }
            else
            {
                currentNormalizedTime = Mathf.Min(stateInfo.normalizedTime, 1);
            }
            float timeLasts = (1 - currentNormalizedTime) * clipTime / animator.speed;
            return timeLasts;
        }

        public static void RotateToDirectionForward(Transform rotated, Vector3 direction, float secondsToRotate) => RotateToDirection(rotated, new(direction.x, 0, direction.z), secondsToRotate);
        public static void RotateToDirection(Transform rotated, Vector3 direction, float secondsToRotate)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            rotated.transform.rotation = Quaternion.RotateTowards(rotated.transform.rotation, toRotation, 180 * UnityEngine.Time.deltaTime / secondsToRotate);
        }
        /// <summary>
        /// This method applies over time. If you want to use it in something like <see cref="{Update}"/> methods,
        /// you probably want <see cref="RotateToDirectionForward(Transform, Vector3, float)"/>
        /// </summary>
        /// <param name="rotated"></param>
        /// <param name="direction"></param>
        /// <param name="secondsToRotate"></param>
        public static void RotateToDirectionForwardSmooth(Transform rotated, Vector3 direction, float secondsToRotate)
        {
            Instance.StartCoroutine(RotateToDirectionForwardSmoothly(rotated, direction, secondsToRotate));
        }
        public static IEnumerator RotateToDirectionForwardSmoothly(Transform rotated, Vector3 direction, float secondsToRotate)
        {
            ForwardRotationTimeChanger.SetValues(0, 1);
            ForwardRotationTimeChanger.SetActions(x => RotateToDirectionForward(rotated, direction, secondsToRotate));
            ForwardRotationTimeChanger.Restart(secondsToRotate);
            yield return ForwardRotationTimeChanger.WaitUntilEnd();
        }
        #endregion methods
    }
}