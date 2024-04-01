using Game.DataBase;
using Game.Serialization.World;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Behaviour;
using Universal.Collections.Generic;

namespace Game.UI.Overlay.Computer.DesignApp
{
    internal partial class BlueprintEditorCreator
    {
        [System.Serializable]
        public class Floor
        {
            #region fields & properties
            private BlueprintEditorCreator Context { get; }
            public BuildingFloor FloorIndex { get; }
            public bool HasGreenZones => hasGreenZones;
            private bool hasGreenZones = false;
            /// <summary>
            /// int - <see cref="BlueprintResource.ConstructionReferenceId"/> <br></br> 
            /// Object Pool - <see cref="BlueprintResourcePlacer"/>
            /// </summary>
            public Dictionary<int, ObjectPool<BlueprintPlacerBase>> ResourcesPool => resourcesPool;
            private readonly Dictionary<int, ObjectPool<BlueprintPlacerBase>> resourcesPool = new();
            /// <summary>
            /// <see cref="BlueprintZone"/>
            /// </summary>
            public ObjectPool<StaticPoolableObject> ZonesPool
            {
                get
                {
                    zonesPool ??= new(Context.blueprintZoneTemplate, Context.elementsParent);
                    return zonesPool;
                }
            }
            private ObjectPool<StaticPoolableObject> zonesPool = null;
            /// <summary>
            /// <see cref="BlueprintRoom"/>
            /// </summary>
            public ObjectPool<StaticPoolableObject> RoomsPool
            {
                get
                {
                    roomsPool ??= new(Context.blueprintRoomTemplate, Context.elementsParent);
                    return roomsPool;
                }
            }
            private ObjectPool<StaticPoolableObject> roomsPool = null;
            /// <summary>
            /// <see cref="BlueprintRoomMarkerPlacer"/>
            /// </summary>
            public ObjectPool<BlueprintPlacerBase> RoomMarkersPool
            {
                get
                {
                    roomMarkersPool ??= new(Context.blueprintRoomMarkerTemplate, Context.elementsParent);
                    return roomMarkersPool;
                }
            }
            private ObjectPool<BlueprintPlacerBase> roomMarkersPool = null;
            #endregion fields & properties

            #region methods
            public void SpawnRoomMarker(BlueprintRoomMarkerData roomData) => SpawnRoomMarker(roomData.RoomType, roomData.UnitData.LocalPosition);
            public BlueprintRoomMarkerPlacer SpawnRoomMarker(BuildingRoom roomType, Vector3 localPosition)
            {
                BlueprintRoomMarkerPlacer marker = RoomMarkersPool.GetObject() as BlueprintRoomMarkerPlacer;
                marker.Marker.ChangeLocalPosition(localPosition);
                marker.Marker.ChangeRoomType(roomType);
                return marker;
            }
            private void SpawnNewRoomMarkers(IReadOnlyList<BlueprintRoomMarkerData> roomMarkersData)
            {
                RoomMarkersPool.FakeDisableObjects();
                int markersCount = roomMarkersData.Count;
                for (int i = 0; i < markersCount; ++i)
                {
                    if (roomMarkersData[i].UnitData.FloorPlaced != FloorIndex) continue;
                    SpawnRoomMarker(roomMarkersData[i]);
                }
                RoomMarkersPool.FixFakeDisabledObjects();
            }

            private BlueprintRoom SpawnRoom(BlueprintRoomInfo roomInfo)
            {
                BlueprintRoom room = RoomsPool.GetObject() as BlueprintRoom;
                room.UpdateGraphic(roomInfo);
                return room;
            }
            public void SpawnNewRooms(HashSet<BlueprintRoomInfo> rooms)
            {
                RoomsPool.FakeDisableObjects();
                foreach (BlueprintRoomInfo room in rooms)
                {
                    if (room.LoopPoints.Count == 0) continue;
                    SpawnRoom(room);
                }
                RoomsPool.FixFakeDisabledObjects();
            }

            /// <summary>
            /// Spawns zone with fully exported parameters (non placeable)
            /// </summary>
            public BlueprintZone SpawnZone(Vector3 localPosition, List<Vector2> textureCoordinates, bool isGoodPlacement, bool isSerializable, bool isTestCreated)
            {
                BlueprintZone zone = ZonesPool.GetObject() as BlueprintZone;
                zone.IsSerializable = isSerializable;
                zone.IsTestCreated = isTestCreated;
                zone.Transform.localPosition = localPosition;
                zone.IsGoodPlacement = isGoodPlacement;
                if (isGoodPlacement)
                {
                    if (!hasGreenZones)
                    {
                        BlueprintEditorValidator.CheckAllObjectsPlacementSmoothly(this);
                    }
                    hasGreenZones = true;
                }
                zone.UpdateGraphic(textureCoordinates, true);
                return zone;
            }
            /// <summary>
            /// Spawns zone with fully exported parameters (non placeable)
            /// </summary>
            public BlueprintZone SpawnZone(BlueprintZoneData data, bool isSerializable, bool isTestCreated) => SpawnZone(data.LocalPosition, data.TexturePoints.ToList(), data.IsPlacementAllowed, isSerializable, isTestCreated);

            private void SpawnNewZones(IReadOnlyList<BlueprintZoneData> zones, bool isSerializable, bool isTestCreated)
            {
                ZonesPool.FakeDisableObjects();
                int zonesCount = zones.Count;
                for (int i = 0; i < zonesCount; ++i)
                {
                    if (zones[i].FloorPlaced != FloorIndex) continue;
                    SpawnZone(zones[i], isSerializable, isTestCreated);
                }
                ZonesPool.FixFakeDisabledObjects();
            }
            private readonly HashSet<BlueprintPointInfo> _GenerateZonesFor_distantPoints = new();
            private readonly List<BlueprintPointInfo> _GenerateZonesFor_foundLoop = new();
            private readonly List<Vector2> _GenerateZonesFor_texutreCoords = new(10);
            private readonly List<Vector2> _GenerateZonesFor_corners = new(5);
            /// <summary>
            /// Removes all test (or non-serialized) zones from 'toFloor' and creates a new ones based on own objects
            /// </summary>
            /// <param name="toFloor">May be own floor</param>
            public void GenerateZonesFor(Floor toFloor, out BlueprintZone oneGreenZone)
            {
                bool isOwnFloor = toFloor == this;
                if (isOwnFloor)
                    toFloor.RemoveTestCreatedZones();
                else
                    toFloor.RemoveNonSerializedZones();

                if (FloorIndex == BuildingFloor.F1_Flooring || FloorIndex == BuildingFloor.F2_FlooringRoof || FloorIndex == BuildingFloor.F3_Roof)
                {
                    oneGreenZone = SpawnZoneForFlooring(toFloor);
                    if (isOwnFloor) return;
                    //copy red zones if floor
                    foreach (StaticPoolableObject obj in ZonesPool.Objects)
                    {
                        BlueprintZone zone = (BlueprintZone)obj;
                        if (!zone.IsUsing) continue;
                        if (zone.IsGoodPlacement) continue;
                        if (zone.IsTestCreated) continue;
                        zone.SetTextureCoordinatesTo(_GenerateZonesFor_texutreCoords);
                        toFloor.SpawnZone(zone.Transform.localPosition, _GenerateZonesFor_texutreCoords, false, false, false);
                    }
                    return;
                }
                oneGreenZone = SpawnZoneForWalls(toFloor);
                if (isOwnFloor) return;
                //spawn for stairs
                foreach (ObjectPool<BlueprintPlacerBase> pool in ResourcesPool.Values)
                {
                    BlueprintResourcePlacer resourceOriginal = (BlueprintResourcePlacer)pool.OriginalPrefab;
                    if (resourceOriginal.Element.ResourceInfo.ConstructionSubtype != ConstructionSubtype.Staircase) continue;
                    foreach (BlueprintPlacerBase placer in pool.Objects)
                    {
                        if (!placer.IsUsing) continue;
                        placer.BlueprintGraphic.SetEntrieLocalCollisionCorners(_GenerateZonesFor_corners);
                        _GenerateZonesFor_corners.RemoveAt(4);
                        toFloor.SpawnZone(Vector3.zero, _GenerateZonesFor_corners, false, false, false);
                    }
                }
            }

            private BlueprintZone SpawnZoneForWalls(Floor toFloor)
            {
                Vector2 distantCoordinates = new(10000, -10000);
                float maxDistance = float.MinValue;
                BlueprintResourcePlacer mostDistancedPlacer = null;
                foreach (ObjectPool<BlueprintPlacerBase> pool in resourcesPool.Values)
                {
                    foreach (BlueprintPlacerBase placer in pool.Objects)
                    {
                        if (!placer.IsUsing) continue;
                        BlueprintResourcePlacer resource = (BlueprintResourcePlacer)placer;
                        if (resource.Element.ResourceInfo.ConstructionLocation != ConstructionLocation.Outside) continue;
                        float newDistance = Vector2.Distance(resource.Element.LocalCenter, distantCoordinates);
                        if (newDistance <= maxDistance) continue;
                        maxDistance = newDistance;
                        mostDistancedPlacer = resource;
                    }
                }
                if (mostDistancedPlacer == null) return null;
                return TryCreateZoneByMostDistancedPoint(toFloor, distantCoordinates, mostDistancedPlacer, ConstructionLocation.Outside);
            }
            private BlueprintZone SpawnZoneForFlooring(Floor toFloor)
            {
                Vector2 distantCoordinates = new(10000, -10000);
                float maxDistance = float.MinValue;
                BlueprintResourcePlacer mostDistancedPlacer = null;
                foreach (ObjectPool<BlueprintPlacerBase> pool in resourcesPool.Values)
                {
                    foreach (BlueprintPlacerBase placer in pool.Objects)
                    {
                        if (!placer.IsUsing) continue;
                        BlueprintResourcePlacer resource = (BlueprintResourcePlacer)placer;
                        float newDistance = Vector2.Distance(resource.Element.LocalCenter, distantCoordinates);
                        if (newDistance <= maxDistance) continue;
                        maxDistance = newDistance;
                        mostDistancedPlacer = resource;
                    }
                }
                if (mostDistancedPlacer == null) return null;
                return TryCreateZoneByMostDistancedPoint(toFloor, distantCoordinates, mostDistancedPlacer, ConstructionLocation.Inside);
            }
            private BlueprintPointInfo FindMostDistancedPointForZone(Vector2 distantCoordinates)
            {
                float maxDistance = float.MinValue;
                BlueprintPointInfo mostDistantPoint = null;
                foreach (BlueprintPointInfo point in _GenerateZonesFor_distantPoints)
                {
                    float newDistance = Vector2.Distance(point.LocalWorkflowCoordinates, distantCoordinates);
                    if (point.AdjacentElements.Count > 2) continue;
                    if (newDistance <= maxDistance) continue;
                    maxDistance = newDistance;
                    mostDistantPoint = point;
                }
                return mostDistantPoint;
            }
            private readonly List<Vector2> _TryCreateZoneByMostDistancedPoint_textureCoords = new(10);
            private BlueprintZone TryCreateZoneByMostDistancedPoint(Floor toFloor, Vector2 distantCoordinates, BlueprintResourcePlacer mostDistancedPlacer, ConstructionLocation mostDistancedLoopLocation)
            {
                BlueprintEditorRooms rooms = BlueprintEditor.Instance.Rooms;
                rooms.UpdateAllBlueprintPoints(mostDistancedLoopLocation);
                rooms.FindAllPointsContainsElement(mostDistancedPlacer, _GenerateZonesFor_distantPoints);
                BlueprintPointInfo mostDistantPoint = FindMostDistancedPointForZone(distantCoordinates);
                if (mostDistantPoint == null) return null;
                if (!rooms.TryFindMostDistancedLoop(mostDistantPoint, _GenerateZonesFor_foundLoop, mostDistancedLoopLocation)) return null;
                bool isTestCreated = toFloor == this;
                _TryCreateZoneByMostDistancedPoint_textureCoords.Clear();
                int count = _GenerateZonesFor_foundLoop.Count;
                for (int i = 0; i < count; ++i)
                {
                    _TryCreateZoneByMostDistancedPoint_textureCoords.Add(_GenerateZonesFor_foundLoop[i].LocalWorkflowCoordinates);
                }
                return toFloor.SpawnZone(Vector3.zero, _TryCreateZoneByMostDistancedPoint_textureCoords, true, false, isTestCreated);
            }
            /// <summary>
            /// Spawns copy of this element allowed for placement
            /// </summary>
            /// <param name="element"></param>
            /// <returns></returns>
            public BlueprintResourcePlacer SpawnResource(BlueprintResource element)
            {
                TryAddNewResourcesDictionaryEntry(element, out ObjectPool<BlueprintPlacerBase> objectPool);
                BlueprintResourcePlacer placer = objectPool.GetObject() as BlueprintResourcePlacer;
                placer.ReplaceUI(Context.blueprintPlacerTemplate);
                Context.OnAnyPlacerAdded?.Invoke();
                return placer;
            }
            private bool TryAddNewResourcesDictionaryEntry(BlueprintResource originalElement, out ObjectPool<BlueprintPlacerBase> existOrNewPool)
            {
                if (ResourcesPool.TryGetValue(originalElement.ConstructionReferenceId, out existOrNewPool))
                {
                    return false;
                }
                BlueprintResource instantiated = GameObject.Instantiate(originalElement, Context.ElementsParent);
                BlueprintResourcePlacer placer = instantiated.GameObject.AddComponent<BlueprintResourcePlacer>();
                ObjectPool<BlueprintPlacerBase> newPool = new(placer, Context.elementsParent);
                existOrNewPool = newPool;
                instantiated.GameObject.SetActive(false);
                ResourcesPool.Add(originalElement.ConstructionReferenceId, newPool);
                return true;
            }
            /// <summary>
            /// Spawns element with exported reference parameters
            /// </summary>
            /// <param name="resourceId"></param>
            /// <returns></returns>
            public BlueprintResourcePlacer SpawnResource(int resourceId)
            {
                BlueprintResourcePlacer placer = SpawnResource(DB.Instance.ConstructionResourceInfo[resourceId].Data.Blueprint);
                placer.Element.ChangeReference(resourceId);
                return placer;
            }
            /// <summary>
            /// Spawns element with fully exported parameters
            /// </summary>
            public BlueprintResourcePlacer SpawnResource(int resourceId, Vector3 localPosition, int rotation, int choosedColorId, bool isMultipleInstantiating)
            {
                BlueprintResourcePlacer placer = SpawnResource(resourceId);
                BlueprintResource resource = placer.Element;
                resource.Transform.localPosition = localPosition;
                if (isMultipleInstantiating)
                    resource.RotateToWithMultipleInstantiating(rotation);
                else
                    resource.RotateTo(rotation);
                resource.ChangeResourceColor(choosedColorId);
                return placer;
            }
            /// <summary>
            /// Spawns element with fully exported parameters
            /// </summary>
            public BlueprintResourcePlacer SpawnResource(BlueprintResourceData data, bool isMultipleInstantiating) => SpawnResource(data.Id, data.UnitData.LocalPosition, data.UnitData.Rotation, data.ChoosedColorId, isMultipleInstantiating);
            private void SpawnNewResources(IReadOnlyList<BlueprintResourceData> elements)
            {
                foreach (ObjectPool<BlueprintPlacerBase> objectPool in ResourcesPool.Values)
                {
                    objectPool.FakeDisableObjects();
                }
                int elementsCount = elements.Count;
                for (int i = 0; i < elementsCount; ++i)
                {
                    if (elements[i].UnitData.FloorPlaced != FloorIndex) continue;
                    SpawnResource(elements[i], true);
                }
                foreach (ObjectPool<BlueprintPlacerBase> objectPool in ResourcesPool.Values)
                {
                    objectPool.FixFakeDisabledObjects();
                }
            }


            /// <summary>
            /// Disables all objects but they are still using
            /// </summary>
            public void HideBlueprintObjects()
            {
                foreach (ObjectPool<BlueprintPlacerBase> objectPool in ResourcesPool.Values)
                {
                    objectPool.HideUsingObjects();
                }
                ZonesPool.HideUsingObjects();
                RoomMarkersPool.HideUsingObjects();
                RoomsPool.HideUsingObjects();
            }
            /// <summary>
            /// Enables all using objects
            /// </summary>
            public void ShowBlueprintObjects()
            {
                foreach (ObjectPool<BlueprintPlacerBase> objectPool in ResourcesPool.Values)
                {
                    objectPool.ShowUsingObjects();
                }
                ZonesPool.ShowUsingObjects();
                RoomMarkersPool.ShowUsingObjects();
                RoomsPool.ShowUsingObjects();
            }
            public void DeserializeBlueprintObjects(BlueprintData currentData)
            {
                RemoveBlueprintRooms(); //should be persistent
                SpawnNewZones(currentData.BlueprintZones, true, false);
                SpawnNewResources(currentData.BlueprintResources);
                SpawnNewRoomMarkers(currentData.BlueprintRoomMarkers);
                BlueprintEditorValidator.CheckAllObjectsPlacementSmoothly(this);
            }

            /// <summary>
            /// Entire clear floor with ZONES, ROOMS, RESOURCES and MARKERS <br></br>
            /// Invokes UI update (creates garbage)
            /// </summary>
            public void RemoveAllBlueprintObjects()
            {
                RemoveBlueprintResources();
                RemoveBlueprintZones();
                RemoveBlueprintRooms();
                RemoveBlueprintRoomMarkers();
            }

            /// <summary>
            /// Invokes UI update (creates garbage)
            /// </summary>
            public void RemoveBlueprintRoomMarkers()
            {
                RoomMarkersPool.DisableObjects();
            }
            /// <summary>
            /// Invokes UI update
            /// </summary>
            /// <param name="roomMarker"></param>
            public void RemoveBlueprintRoomMarker(BlueprintRoomMarkerPlacer roomMarker)
            {
                RemovePlacer(roomMarker);
                if (roomMarker.RoomPlaced == null) return;
                roomMarker.RoomPlaced.UpdateMarkers();
            }
            /// <summary>
            /// Invokes UI update (creates garbage)
            /// </summary>
            public void RemoveBlueprintRooms()
            {
                RoomsPool.DisableObjects();
            }
            /// <summary>
            /// Invokes UI update (creates garbage)
            /// </summary>
            private void RemoveBlueprintZones()
            {
                ZonesPool.DisableObjects();
                hasGreenZones = false;
            }
            /// <summary>
            /// Invokes UI update (creates garbage)
            /// </summary>
            private void RemoveNonSerializedZones()
            {
                foreach (StaticPoolableObject placer in ZonesPool.Objects)
                {
                    BlueprintZone zone = (BlueprintZone)placer;
                    if (!zone.IsUsing) continue;
                    if (zone.IsSerializable) continue;
                    zone.DisableObject();
                }
                UpdateHasGreenZones();
            }
            public void RemoveTestCreatedZones()
            {
                foreach (StaticPoolableObject placer in ZonesPool.Objects)
                {
                    BlueprintZone zone = (BlueprintZone)placer;
                    if (!zone.IsUsing) continue;
                    if (!zone.IsTestCreated) continue;
                    zone.DisableObject();
                }
                UpdateHasGreenZones();
            }
            private void UpdateHasGreenZones()
            {
                foreach (StaticPoolableObject placer in ZonesPool.Objects)
                {
                    BlueprintZone zone = (BlueprintZone)placer;
                    if (!zone.IsUsing) continue;
                    if (zone.IsGoodPlacement)
                    {
                        hasGreenZones = true;
                        return;
                    }
                }
                hasGreenZones = false;
            }
            /// <summary>
            /// Invokes UI update (creates garbage)
            /// </summary>
            public void RemoveBlueprintResources()
            {
                foreach (ObjectPool<BlueprintPlacerBase> objectPool in ResourcesPool.Values)
                {
                    objectPool.DisableObjects();
                }
            }
            private readonly HashSet<BlueprintPlacerBase> _RemoveElement_tempAdjacentElements = new();
            /// <summary>
            /// Invokes UI update. If you need to delete list of objects, you should use <see cref="RemovePlacerFast(BlueprintResourcePlacer)"/>
            /// </summary>
            /// <param name="element"></param>
            public void RemoveResource(BlueprintResourcePlacer element)
            {
                RemovePlacer(element);
            }
            private void RemovePlacer(BlueprintPlacerBase placer)
            {
                RemovePlacerFast(placer);
                placer.SetLastCollidedBlueprintsTo(_RemoveElement_tempAdjacentElements);
                foreach (BlueprintPlacerBase el in _RemoveElement_tempAdjacentElements)
                {
                    el.CheckDeepPlacement();
                }
            }
            private void RemovePlacerFast(BlueprintPlacerBase placer)
            {
                placer.DisableObject();
                Context.OnAnyPlacerRemoved?.Invoke();
            }
            public Floor(BlueprintEditorCreator context, BuildingFloor floorIndex)
            {
                this.Context = context;
                this.FloorIndex = floorIndex;
            }
            #endregion methods
        }
    }
}