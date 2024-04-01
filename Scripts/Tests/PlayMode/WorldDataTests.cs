using System.Collections;
using System.Collections.Generic;
using DebugStuff;
using Game.DataBase;
using Game.Serialization.World;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Universal.Behaviour;
using Universal.Collections.Generic;
using Universal.Core;
using Universal.Events;
using static UnityEditor.Progress;

namespace Tests.PlayMode
{
    public class WorldDataTests
    {
        #region fields & properties
        #endregion fields & properties

        #region methods

        [Test]
        public void T1Warehouse()
        {
            AssetLoader.InitSingleGameInstance();

            WarehouseData warehouseData = new(0);
            ResourceData resourceData = new(0, ResourceType.Construction);
            resourceData.Add(1);
            float rVolume = resourceData.GetTotalVolumeM3();
            float newSpace = warehouseData.OccupiedSpace + rVolume;
            Assert.IsTrue(warehouseData.TryAddResource(resourceData));
            Assert.AreSame(warehouseData.Resources[0], resourceData);
            Assert.IsTrue(warehouseData.OccupiedSpace == newSpace);

            resourceData = new(0, ResourceType.Construction);
            resourceData.Add(2);
            rVolume = resourceData.GetTotalVolumeM3();
            newSpace = warehouseData.OccupiedSpace + rVolume;
            Assert.IsTrue(warehouseData.TryAddResource(resourceData));
            Assert.IsTrue(warehouseData.OccupiedSpace == newSpace);
            Assert.IsTrue(warehouseData.Resources.Count == 1);

            resourceData = new(1, ResourceType.Construction);
            resourceData.Add(2);
            resourceData.Add(1);
            rVolume = resourceData.GetTotalVolumeM3();
            newSpace = warehouseData.OccupiedSpace + rVolume;
            Assert.IsTrue(warehouseData.TryAddResource(resourceData));
            Assert.AreSame(resourceData, warehouseData.Resources[1]);
            Assert.IsTrue(warehouseData.OccupiedSpace == newSpace);
            Assert.IsTrue(warehouseData.Resources.Count == 2);

            rVolume = 3 * resourceData.Info.Prefab.VolumeM3;
            newSpace = warehouseData.OccupiedSpace - rVolume;
            warehouseData.RemoveResource(resourceData, 3);

            Assert.AreEqual(newSpace, warehouseData.OccupiedSpace);
            Assert.IsTrue(warehouseData.Resources.Count == 2);

            warehouseData.RemoveResource(resourceData, 0);
            Assert.IsTrue(warehouseData.Resources.Count == 2);

            warehouseData.RemoveResource(resourceData, 1);
            Assert.IsTrue(warehouseData.Resources.Count == 1);

        }
        [Test]
        public void T2Warehouse()
        {
            AssetLoader.InitSingleGameInstance();

            WarehouseData warehouseData = new(0);
            List<ResourceData> resources = new();
            ResourceData resourceData = new(0, ResourceType.Construction);
            resourceData.Add(1);
            resources.Add(resourceData);

            resourceData = new(1, ResourceType.Construction);
            resourceData.Add(2);
            resourceData.Add(1);
            resources.Add(resourceData);

            float rVolume = 0;
            foreach (var el in resources)
                rVolume += el.GetTotalVolumeM3();

            float newSpace = warehouseData.OccupiedSpace + rVolume;

            Assert.IsTrue(warehouseData.CanAddResources(resources, out float floatVolume));
            Assert.IsTrue(floatVolume == rVolume);
            Assert.IsTrue(warehouseData.TryAddResources(resources));
            Assert.IsTrue(warehouseData.OccupiedSpace == newSpace);
            Assert.AreSame(warehouseData.Resources[1], resourceData);
            Assert.IsTrue(warehouseData.Resources.Count == 2);
        }
        [Test]
        public void T3Warehouse()
        {
            AssetLoader.InitSingleGameInstance();

            WarehouseData warehouseData = new(0);
            List<ResourceData> resources = new();
            ResourceData resourceData = new(0, ResourceType.Construction);
            resourceData.Add(1);
            resources.Add(resourceData);

            resourceData = new(1, ResourceType.Construction);
            resourceData.Add(2);
            resourceData.Add(100000);
            resources.Add(resourceData);

            float rVolume = 0;
            foreach (var el in resources)
                rVolume += el.GetTotalVolumeM3();

            float newSpace = warehouseData.OccupiedSpace + rVolume;
            Assert.IsFalse(warehouseData.CanAddResources(resources, out float floatVolume));
            Assert.IsTrue(floatVolume == rVolume);
            Assert.IsFalse(warehouseData.TryAddResources(resources));
            Assert.IsFalse(warehouseData.OccupiedSpace == newSpace);
            Assert.IsTrue(warehouseData.Resources.Count == 0);
        }
        [Test]
        public void T1ResourceShop()
        {
            AssetLoader.InitSingleGameInstance();

            ResourceShopData shop = new();
            shop.GenerateNewData();
            shop.Items.Exists(x => x.Id == 0, out ResourceShopItemData item);
            Assert.IsTrue(shop.Cart.Items.Count == 0);

            shop.AddToCart(item, 1);
            Assert.IsTrue(shop.Cart.Items.Count == 1);
            Assert.IsTrue(shop.Cart.Items[0].Count == 1);

            shop.AddToCart(item, 2);
            Assert.IsTrue(shop.Cart.Items.Count == 1);
            Assert.IsTrue(shop.Cart.Items[0].Count == 3);

            Assert.IsTrue(shop.Items.Exists(x => x.Id == 1, out item));
            shop.AddToCart(item, 0);
            Assert.IsTrue(shop.Cart.Items.Count == 1);
            shop.AddToCart(item, 1);
            Assert.IsTrue(shop.Cart.Items.Count == 2);
            shop.AddToCart(item, 3);
            Assert.IsTrue(shop.Cart.Items.Count == 2);
            Assert.IsTrue(shop.Cart.Items[0].Count == 3);
            Assert.IsTrue(shop.Cart.Items[1].Count == 4);

            shop.RemoveFromCart(item, 2);
            Assert.IsTrue(shop.Cart.Items.Count == 2);
            Assert.IsTrue(shop.Cart.Items[1].Count == 2);
            shop.RemoveFromCart(item, 1);
            Assert.IsTrue(shop.Cart.Items.Count == 2);
            Assert.IsTrue(shop.Cart.Items[1].Count == 1);
            shop.RemoveFromCart(item, 22);
            Assert.IsTrue(shop.Cart.Items.Count == 1);
            Assert.IsTrue(shop.Cart.Items[0].Count == 3);
        }
        #endregion methods
    }
}