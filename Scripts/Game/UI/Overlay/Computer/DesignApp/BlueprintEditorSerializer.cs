using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Universal.Behaviour;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.UI.Overlay.Computer.DesignApp
{
    internal static class BlueprintEditorSerializer
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public static bool TryDeSerialize(string blueprintName, ref BlueprintData currentData)
        {
            if (!GameData.Data.BlueprintsData.Blueprints.Exists(x => x.Name.Equals(blueprintName), out BlueprintData exist)) return false;
            LoadData(exist, ref currentData);
            return true;
        }
        private static void LoadData(BlueprintData newData, ref BlueprintData currentData)
        {
            currentData = newData;
        }
        public static void Serialize(BlueprintEditorCreator creator, ref BlueprintData currentData)
        {
            currentData.ClearData();
            foreach (KeyValuePair<BuildingFloor, BlueprintEditorCreator.Floor> floor in creator.Floors)
            {
                SerializeFloor(floor.Value, floor.Key, ref currentData);
            }
        }
        public static List<BlueprintRoomData> SerializeRooms(BlueprintEditorCreator creator)
        {
            List<BlueprintRoomData> rooms = new();
            foreach (KeyValuePair<BuildingFloor, BlueprintEditorCreator.Floor> floor in creator.Floors)
            {
                BuildingFloor currentFloor = floor.Key;
                foreach (StaticPoolableObject poolable in floor.Value.RoomsPool.Objects)
                {
                    if (!poolable.IsUsing) continue;
                    BlueprintRoom room = (BlueprintRoom)poolable;
                    room.GetRoomTypes(out BuildingRoom rt1, out BuildingRoom rt2);
                    rooms.Add(new(rt1, rt2, room.Area, currentFloor));
                }
            }
            return rooms;
        }
        private static void SerializeFloor(BlueprintEditorCreator.Floor floor, BuildingFloor floorIndex, ref BlueprintData currentData)
        {
            foreach (ObjectPool<BlueprintPlacerBase> objectPool in floor.ResourcesPool.Values)
            {
                foreach (BlueprintPlacerBase element in objectPool.Objects)
                {
                    if (!element.IsUsing) continue;
                    BlueprintResourcePlacer placer = (BlueprintResourcePlacer)element;
                    currentData.AddResource(placer.Element, floorIndex);
                }
            }
            foreach (StaticPoolableObject el in floor.ZonesPool.Objects)
            {
                BlueprintZone zone = (BlueprintZone)el;
                if (!zone.IsUsing) continue;
                if (!zone.IsSerializable) continue;
                currentData.AddZone(zone, floorIndex);
            }
            foreach (BlueprintPlacerBase placer in floor.RoomMarkersPool.Objects)
            {
                if (!placer.IsUsing) continue;
                BlueprintRoomMarkerPlacer marker = (BlueprintRoomMarkerPlacer)placer;
                currentData.AddRoomMarker(marker.Marker, floorIndex);
            }
        }
        #endregion methods
    }
}