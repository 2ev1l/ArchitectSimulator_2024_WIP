using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Universal.Collections.Generic;
using Universal.Events;

namespace Tests.PlayMode
{
    public class ItemListTests
    {
        #region fields & properties
        #endregion fields & properties

        #region methods
        public InfinityItemList<TestItem, int> CreateInfinityList(out TestItem prefab, out GameObject content)
        {
            GameObject obj = new GameObject("p");
            content = new GameObject("c");
            prefab = obj.AddComponent<TestItem>();
            return new(prefab, content.transform);
        }
        [Test]
        public void T1InfinityList()
        {
            InfinityItemList<TestItem, int> infil = CreateInfinityList(out TestItem prefab, out GameObject content);
            List<int> data = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            infil.UpdateListDefault(data, x => x);
            Assert.AreEqual(infil.Items.Count, data.Count);
            Assert.AreEqual(infil.Items[0].value, 0);

            data = new() { 4, 3, 2, 1, 6 };
            infil.UpdateListDefault(data, x => x);
            Assert.AreEqual(infil.Items.Count, data.Count);
            Assert.AreEqual(infil.Items[2].value, 2);
            Assert.AreEqual(infil.Items[4].value, 6);
            Assert.Catch<System.ArgumentOutOfRangeException>(delegate { _ = infil.Items[5]; });
        }
        [Test]
        public void T2InfinityList()
        {
            InfinityItemList<TestItem, int> infil = CreateInfinityList(out TestItem prefab, out GameObject content);
            List<int> data = new() { };
            infil.UpdateListDefault(data, x => x);
            Assert.AreEqual(infil.Items.Count, data.Count);

            data = new() { 0, 1, 2, 3, 4 };
            infil.UpdateListDefault(data, x => x);
            Assert.AreEqual(infil.Items.Count, data.Count);

            data = new() { };
            infil.UpdateListDefault(data, x => x);
            Assert.AreEqual(infil.Items.Count, data.Count);

            data = new() { -45 };
            infil.UpdateListDefault(data, x => x);
            Assert.AreEqual(infil.Items.Count, data.Count);
            Assert.AreEqual(infil.Items[0].value, -45);
        }
        #endregion methods
    }
    public class TestItem : MonoBehaviour, IListUpdater<int>
    {
        public int value;
        public void OnListUpdate(int param)
        {
            value = param;
        }
    }
}