using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Universal.Events;
using Universal.Core;
using System;
using Game.DataBase;
using System.Linq;

namespace Tests.EditMode
{
    public class ExtensionTests
    {
        [Test]
        public void T1Dictionary()
        {
            Dictionary<int, int> d = new();
            d.Add(1, 2);
            d.Add(2, 3);

            d[1] += 5;
            Assert.AreEqual(7, d[1]);
        }
        [Test]
        public void T1LoopPoints()
        {
            List<Vector2> loopPoints = new()
            {
                new(280, -420),
                new(340, -360),
                new(340, -220),
                new(480, -220),
                new(680, -220),
                new(880, -220),
                new(940, -220),
                new(940, -160),
                new(940, 40),
                new(940, 240),
                new(940, 440),
                new(940, 500),
                new(880, 500),
                new(700, 500),
                new(700, 300),
                new(700, 100),
                new(700, 20),
                new(620, 20),

                new(20, 20),
                new(20, 40),
                new(20, 240),
                new(20, 500),
                new(80, 500),
                new(280, 500),
                new(480, 500),
                new(480, 300),
                new(480, 220),
                new(400, 220),
                new(200, 220),
                new(120, 220),
                new(120, 300),
                new(120, 500),
                new(280, 500),
                new(80, 500),
                new(20, 500),
                new(20, 440),
                new(20, 240),
                new(20, 40),
                new(20, 20),

                new(20, -160),
                new(20, -360),
                new(20, -360),
                new(20, -420),
                new(80, -420)
            };
            Assert.IsTrue(FinalLoopHasReversedCombination(loopPoints));
        }
        private bool FinalLoopHasReversedCombination(List<Vector2> loopPoints)
        {
            int loopCount = loopPoints.Count;
            float precision = 1f;
            for (int i = 0; i < loopCount - 1; ++i)
            {
                Vector2 currentPoint = loopPoints[i];
                Vector2 adjacentPoint = loopPoints[i + 1];
                for (int j = i + 1; j < loopCount - 1; ++j)
                {
                    if (loopPoints[j].Approximately(adjacentPoint, precision) && loopPoints[j + 1].Approximately(currentPoint, precision))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        [Test]
        public void T1ListReverse()
        {
            List<TestItem> temp = new();
            List<TestItem> original = new();
            TestItem bottom = new(0, "5");
            TestItem top = new(10, "10");
            original.Add(bottom);
            original.Add(new(1, "4"));
            original.Add(new(2, "3"));
            original.Add(new(7, "5"));
            original.Add(new(3, "2"));
            original.Add(new(4, "1"));
            original.Add(top);
            Assert.AreSame(top, original[original.Count - 1]);
            original.SetElementsTo(temp);
            temp.Reverse();
            Assert.AreSame(bottom, temp[temp.Count - 1]);
            Assert.IsTrue(original.ElementsSameReversed(temp));

        }
        [Test]
        public void T1StackElementsSamePart()
        {
            List<TestItem> comparable = new();
            List<TestItem> original = new();
            TestItem bottom = new(0, "5");
            TestItem top = new(10, "10");
            original.Add(bottom);
            original.Add(new(1, "4"));
            original.Add(new(2, "3"));
            original.Add(new(3, "2"));
            original.Add(new(4, "1"));
            original.Add(top);
            original.SetElementsTo(comparable);
            Assert.IsTrue(original.ElementsSame(comparable));
            comparable.RemoveAt(comparable.Count - 1);
            Assert.IsTrue(original.ElementsSameOfPart(comparable));
        }
        [Test]
        public void T1StackElementsSame()
        {
            List<TestItem> comparable = new();
            List<TestItem> original = new();
            TestItem bottom = new(0, "5");
            TestItem top = new(10, "10");
            original.Add(bottom);
            original.Add(new(1, "4"));
            original.Add(new(2, "3"));
            original.Add(new(3, "2"));
            original.Add(new(4, "1"));
            original.Add(top);
            original.SetElementsTo(comparable);
            Assert.IsTrue(original.ElementsSame(comparable));
            comparable.RemoveAt(comparable.Count - 1);
            comparable.Add(new(4, "1"));
            Assert.IsFalse(original.ElementsSame(comparable));
        }
        [Test]
        public void T1StackReverse()
        {
            List<TestItem> temp = new();
            Stack<TestItem> original = new();
            TestItem bottom = new(0, "5");
            TestItem top = new(10, "10");
            original.Push(bottom);
            original.Push(new(1, "4"));
            original.Push(new(2, "3"));
            original.Push(new(3, "2"));
            original.Push(new(4, "1"));
            original.Push(top);
            Assert.AreSame(top, original.Peek());
            original.Reverse(temp);
            Assert.AreSame(bottom, original.Peek());
        }
        [Test]
        public void T1HashsetSetElements()
        {
            HashSet<TestItem> original = new()
            {
                new(0, "5"),
                new(1, "4"),
                new(2, "3"),
                new(3, "2"),
                new(4, "1"),
                new(10, "10")
            };
            HashSet<TestItem> clone = new();
            original.SetElementsTo(clone);
            Assert.AreNotSame(original, clone);
            foreach (TestItem el in original)
            {
                TestItem found = original.Where(x => x.Id == el.Id).First();
                Assert.AreSame(el, found);
            }
        }
        [Test]
        public void T1NearestBottom()
        {
            List<int> list = new() { -1, 5, 10, 15, 20, 25, 30, 35 };
            Assert.AreEqual(list.NearestBottom(x => x, -5), default(int));
            Assert.AreEqual(list.NearestBottom(x => x, 14), 10);
            Assert.AreEqual(list.NearestBottom(x => x, 15), 15);
            Assert.AreEqual(list.NearestBottom(x => x, 16), 15);
            Assert.AreEqual(list.NearestBottom(x => x, 444), 35);
        }
        [Test]
        public void T1NearestTop()
        {
            List<int> list = new() { -1, 5, 10, 15, 20, 25, 30, 35 };
            Assert.AreEqual(list.NearestTop(x => x, -5), -1);
            Assert.AreEqual(list.NearestTop(x => x, 14), 15);
            Assert.AreEqual(list.NearestTop(x => x, 15), 15);
            Assert.AreEqual(list.NearestTop(x => x, 16), 20);
            Assert.AreEqual(list.NearestTop(x => x, 36), default(int));
        }
        [Test]
        public void T1FindEquals()
        {
            List<TestItem> items = new() { new(2, "1"), new(1, "2"), new(2, "1"), new(2, "3"), new(3, "3"), new(4, "5"), new(2, "1") };
            HashSet<TestItem> found = new();
            found = items.FindEquals((x, y) => x.Text == y.Text);
            Func<TestItem, TestItem, bool> match = new((x, y) => x.Text == y.Text);
            Assert.IsTrue(found.TryGetValue(items[0], out _));
            Assert.IsFalse(found.TryGetValue(items[1], out _));
            Assert.IsTrue(found.TryGetValue(items[2], out _));
            Assert.IsTrue(found.TryGetValue(items[3], out _));
            Assert.IsTrue(found.TryGetValue(items[4], out _));
            Assert.IsFalse(found.TryGetValue(items[5], out _));
            Assert.IsTrue(found.TryGetValue(items[6], out _));

            Assert.IsTrue(items.ExistsEquals((x, y) => x.Id == y.Id));
            Assert.IsFalse(items.ExistsEquals((x, y) => x.Id == System.Convert.ToInt32(x.Text) && y.Id == System.Convert.ToInt32(y.Text)));
        }
        [Test]
        public void T2FindEquals()
        {
            List<int> items = new() { 0, 0, 1, 0 };
            HashSet<int> found = new();
            found = items.FindEquals((x, y) => x == y);
            Assert.IsTrue(found.TryGetValue(items[0], out _));
            Assert.IsTrue(found.TryGetValue(items[1], out _));
            Assert.IsFalse(found.TryGetValue(items[2], out _));
            Assert.IsTrue(found.TryGetValue(items[3], out _));
        }
        [Test]
        public void T1FindSame()
        {
            List<TestItem> items = new() { new(2, "1"), new(1, "2"), new(2, "1"), new(2, "3"), new(3, "3"), new(4, "5"), new(2, "1") };
            HashSet<TestItem> found = new();
            found = items.FindSame((x, y) => x.Text == y.Text);
            Func<TestItem, TestItem, bool> match = new((x, y) => x.Text == y.Text);
            Assert.IsTrue(found.TryGetValue(items[0], out _));
            Assert.IsFalse(found.TryGetValue(items[1], out _));
            Assert.IsTrue(found.TryGetValue(items[2], out _));
            Assert.IsTrue(found.TryGetValue(items[3], out _));
            Assert.IsTrue(found.TryGetValue(items[4], out _));
            Assert.IsFalse(found.TryGetValue(items[5], out _));
            Assert.IsTrue(found.TryGetValue(items[6], out _));

            Assert.IsTrue(items.ExistsSame((x, y) => x.Id == y.Id));
            Assert.IsFalse(items.ExistsSame((x, y) => x.Id == System.Convert.ToInt32(x.Text) && y.Id == System.Convert.ToInt32(y.Text)));
        }
        [Test]
        public void T1ForEachEnum()
        {
            BuildingType type = BuildingType.Doghouse | BuildingType.Apartments;
            string value = "";
            string expectedValue = "13";
            type.ForEachFlag(x => value += $"{(int)x - 1}");
            Assert.AreEqual(expectedValue, value);

            type = 0;
            type.ForEachFlag(x => value += $"{(int)x - 1}");
            Assert.AreEqual(expectedValue, value);
        }
        private class TestItem
        {
            public int Id { get; set; }
            public string Text { get; set; }

            public override string ToString()
            {
                return $"#{Id} : {Text}";
            }
            public TestItem() { }
            public TestItem(int id, string text)
            {
                Id = id;
                Text = text;
            }
        }
    }
}