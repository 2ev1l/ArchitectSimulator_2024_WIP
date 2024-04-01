using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Universal.Events;

namespace Tests.EditMode
{
    public class EventTests
    {
        [Test]
        public void T1ActionRequest()
        {
            bool value = false;
            ActionRequest ar = new(delegate { TestAction(ref value); });
            Assert.AreEqual(ar.CanExecute(0), true);
            Assert.AreEqual(ar.CanExecute(int.MinValue), true);
            Assert.AreEqual(ar.CanExecute(int.MaxValue), true);
            ar.TryExecute(0);
            Assert.AreEqual(value, true);
        }

        [Test]
        public void T2ActionRequest()
        {
            bool value = false;
            ActionRequest ar = new(delegate { TestAction(ref value); });
            ar.AddBlockLevel(1);
            ar.AddBlockLevel(1);
            ar.AddBlockLevel(1);

            ar.TryExecute(0);
            Assert.AreEqual(value, false);

            ar.TryExecute(1);
            Assert.AreEqual(value, false);

            ar.TryExecute(2);
            Assert.AreEqual(value, true);
            value = false;

            ar.RemoveBlockLevel(1);
            ar.TryExecute(1);
            Assert.AreEqual(value, false);

            ar.RemoveBlockLevel(1);
            ar.TryExecute(1);
            ar.TryExecute(0);
            Assert.AreEqual(value, false);

            ar.RemoveBlockLevel(1);
            ar.TryExecute(-1);
            Assert.AreEqual(value, true);
            value = false;

            ar.AddBlockLevel(1);
            ar.RemoveBlockLevel(2);
            ar.RemoveBlockLevel(0);
            ar.RemoveBlockLevel(-1);
            ar.TryExecute(1);
            Assert.AreEqual(value, false);
        }

        private void TestAction(ref bool value)
        {
            value = true;
        }

    }
}