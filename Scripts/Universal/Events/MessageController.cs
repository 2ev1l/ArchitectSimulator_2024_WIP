using System.Collections.Generic;
using UnityEngine;
using Universal.Core;

namespace Universal.Events
{
    /// <summary>
    /// Using HashSet instead of Action/Message etc. for better performance
    /// </summary>
    public class MessageController : MonoBehaviour, IInitializable
    {
        #region fields & properties
        public static MessageController Instance { get; private set; }
        private HashSet<IUpdateSender> updateSenders = new();
        private HashSet<IFixedUpdateSender> fixedUpdateSenders = new();
        private HashSet<ILateUpdateSender> lateUpdateSenders = new();
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
        }

        private void AddObjectByType(IMessageSender obj)
        {
            //don't want to do different (same) implementation in each class
            if (obj is IUpdateSender us) updateSenders.Add(us);
            if (obj is IFixedUpdateSender fs) fixedUpdateSenders.Add(fs);
            if (obj is ILateUpdateSender ls) lateUpdateSenders.Add(ls);
        }
        private void RemoveObjectByType(IMessageSender obj)
        {
            if (obj is IUpdateSender us) updateSenders.Remove(us);
            if (obj is IFixedUpdateSender fs) fixedUpdateSenders.Remove(fs);
            if (obj is ILateUpdateSender ls) lateUpdateSenders.Remove(ls);
        }
        /// <summary>
        /// Don't need to call it more than one time for single object for initializing different interfaces
        /// </summary>
        /// <param name="obj"></param>
        public void AddSender(IMessageSender obj) => AddObjectByType(obj);
        /// <summary>
        /// Don't need to call it more than one time for single object for initializing different interfaces
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveSender(IMessageSender obj) => RemoveObjectByType(obj);
        public void Update()
        {
            foreach (var el in updateSenders)
            {
                el.UpdateMessage();
            }
        }
        public void FixedUpdate()
        {
            foreach (var el in fixedUpdateSenders)
            {
                el.FixedUpdateMessage();
            }
        }
        public void LateUpdate()
        {
            foreach (var el in lateUpdateSenders)
            {
                el.LateUpdateMessage();
            }
        }
        #endregion methods
    }
}