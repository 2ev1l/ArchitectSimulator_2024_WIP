using System.Collections;
using System.Collections.Generic;
using Game.DataBase;
using Game.Serialization.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode
{
    public class WorldDataTests
    {
        [Test]
        public void T1WalletTest()
        {
            Wallet wallet = new(1000);
            Assert.IsTrue(wallet.TryDecreaseValue(1000));
            Assert.IsFalse(wallet.TryDecreaseValue(0));
            Assert.IsFalse(wallet.TryDecreaseValue(100));
            Assert.AreEqual(wallet.Value, 0);
            Assert.IsTrue(wallet.TryIncreaseValue(100));
            Assert.IsFalse(wallet.TryDecreaseValue(101));
            Assert.AreEqual(wallet.Value, 100);
            Assert.IsTrue(wallet.TryDecreaseValue(50));
            Assert.AreEqual(wallet.Value, 50);
            Assert.IsTrue(wallet.CanDecreaseValue(50));
            Assert.IsTrue(wallet.CanDecreaseValue(49));
            Assert.IsFalse(wallet.CanDecreaseValue(51));
            Assert.IsFalse(wallet.CanDecreaseValue(0));
            Assert.IsFalse(wallet.TryIncreaseValue(0));
            Assert.AreEqual(wallet.Value, 50);
        }

        [Test]
        public void T1RangedValueTest()
        {
            RangedValue rangedValue = new(100, new(-50, 50));
            Assert.AreEqual(rangedValue.Value, 50);
            Assert.IsFalse(rangedValue.TryIncreaseValue(0));
            Assert.IsFalse(rangedValue.TryDecreaseValue(0));
            Assert.IsTrue(rangedValue.CanDecreaseValue(10));
            Assert.IsTrue(rangedValue.TryDecreaseValue(10));
            Assert.AreEqual(rangedValue.Value, 40);
            Assert.IsFalse(rangedValue.TryDecreaseValue(100));
            Assert.AreEqual(rangedValue.Value, 40);
            Assert.IsTrue(rangedValue.TryDecreaseValue(90));
            Assert.AreEqual(rangedValue.Value, -50);
            Assert.IsFalse(rangedValue.TryIncreaseValue(101));
            Assert.IsTrue(rangedValue.TryIncreaseValue(99));
            Assert.AreEqual(rangedValue.Value, 49);

            Assert.AreEqual(rangedValue.MaxChangesLimit, 1);
            Assert.AreEqual(rangedValue.MinChangesLimit, 99);
        }
        [Test]
        public void T2RangedValueTest()
        {
            RangedValue rangedValue = new(49, new(-50, 50));
            rangedValue.SetMaxRange(45);
            Assert.AreEqual(rangedValue.Value, 45);
            Assert.AreEqual(rangedValue.Range.y, 45);

            rangedValue.SetMinRange(50);
            Assert.AreEqual(rangedValue.Range.y, 50);
            Assert.AreEqual(rangedValue.Range.x, 50);
            Assert.AreEqual(rangedValue.Value, 50);

            rangedValue.SetRange(new(24, -15));
            Assert.AreEqual(rangedValue.Range.x, -15);
            Assert.AreEqual(rangedValue.Range.y, 24);
            Assert.AreEqual(rangedValue.Value, 24);
        }
    }
}