using Game.DataBase;
using Game.Serialization.World;
using Game.UI.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Universal.Behaviour;
using Universal.Collections.Generic;
using Universal.Core;
using static Game.UI.Overlay.Computer.DesignApp.BlueprintEditorCreator;

namespace Game.UI.Overlay.Computer.DesignApp
{
    internal static class BlueprintEditorValidator
    {
        #region fields & properties
        #endregion fields & properties

        #region methods
        private static readonly Dictionary<ResourceData, int> insufficientResourcesIdCount = new();
        private static readonly Dictionary<ResourceData, int> tempResourcesIdCount = new();
        public static bool IsAllResourcesSufficient(BlueprintEditorCreator creator, out Dictionary<ResourceData, int> insufficientResourcesCount, out Dictionary<ResourceData, int> allResourcesCount)
        {
            allResourcesCount = tempResourcesIdCount;
            insufficientResourcesCount = BlueprintEditorValidator.insufficientResourcesIdCount;
            insufficientResourcesCount.Clear();
            creator.GetAllResourcesRequirements(allResourcesCount);
            foreach (var kv in allResourcesCount)
            {
                int difference = kv.Key.Count - kv.Value;
                if (difference < 0)
                {
                    insufficientResourcesCount.Add(kv.Key, difference);
                }
            }
            return insufficientResourcesCount.Count == 0;
        }
        public static void CheckAllObjectsPlacementSmoothly(Floor floor)
        {
            foreach (ObjectPool<BlueprintPlacerBase> objectPool in floor.ResourcesPool.Values)
            {
                foreach (BlueprintPlacerBase element in objectPool.Objects)
                {
                    if (!element.IsUsing) continue;
                    element.CheckFastPlacementSmoothly();
                }
            }
            foreach (BlueprintPlacerBase marker in floor.RoomMarkersPool.Objects)
            {
                if (!marker.IsUsing) continue;
                marker.CheckFastPlacementSmoothly();
            }
        }
        public static bool IsAllObjectsPlacedCorrectly(Floor floor)
        {
            foreach (ObjectPool<BlueprintPlacerBase> objectPool in floor.ResourcesPool.Values)
            {
                foreach (BlueprintPlacerBase element in objectPool.Objects)
                {
                    if (!element.IsUsing) continue;
                    if (!element.IsGoodPlacement) return false;
                }
            }
            foreach (BlueprintPlacerBase marker in floor.RoomMarkersPool.Objects)
            {
                if (!marker.IsUsing) continue;
                if (!marker.IsGoodPlacement) return false;
            }
            return true;
        }
        public static bool IsRoomSizeCorrect(BlueprintRoom room) //todo
        {
            float size = room.Area;
            return true;
        }
        private static System.Collections.IEnumerator CheckFloor_Coroutine = null;
        /// <summary>
        /// When call again, old check will close without actions
        /// </summary>
        /// <param name="OnCorrectCheck"></param>
        public static void CheckFloor(Floor floor, System.Action OnCorrectCheck, System.Action<string> OnBadCheck, System.Action<string> OnCheckStage)
        {
            if (CheckFloor_Coroutine != null)
                SingleGameInstance.Instance.StopCoroutine(CheckFloor_Coroutine);
            CheckFloor_Coroutine = CheckFloor_IEnumerator(floor, OnCorrectCheck, OnBadCheck, OnCheckStage);
            SingleGameInstance.Instance.StartCoroutine(CheckFloor_Coroutine);
        }
        private static System.Collections.IEnumerator CheckFloor_IEnumerator(Floor floor, System.Action OnCorrectCheck, System.Action<string> OnBadCheck, System.Action<string> OnCheckStage)
        {
            OnCheckStage?.Invoke(LanguageLoader.GetTextByType(TextType.Game, 129));
            BlueprintEditor editor = BlueprintEditor.Instance;
            GameObject editorObj = editor.gameObject;
            if (!CheckEditorEnabled(editorObj, OnBadCheck)) yield break;
            float physicsCheckTime = 0.6f;
            editor.Selector.DeselectCurrentElement();
            BlueprintEditorRooms rooms = editor.Rooms;
            BlueprintEditorCreator creator = editor.Creator;
            int height = 0;
            int counter = 0;
            bool containStair = false;

            foreach (ObjectPool<BlueprintPlacerBase> objectPool in floor.ResourcesPool.Values)
            {
                foreach (BlueprintPlacerBase element in objectPool.Objects)
                {
                    if (!element.IsUsing) continue;
                    BlueprintResourcePlacer resource = (BlueprintResourcePlacer)element;
                    ConstructionResourceInfo resInfo = resource.Element.ResourceInfo;
                    int currentHeight = resInfo.Prefab.SizeCentimeters.y;
                    if (counter == 0)
                    {
                        height = currentHeight;
                    }
                    if (currentHeight != height)
                    {
                        OnBadCheck?.Invoke($"{LanguageLoader.GetTextByType(TextType.Game, 122)}\n[{resource.Transform.localPosition.x}, {resource.Transform.localPosition.y}]"); //122
                        yield break;
                    }
                    counter++;
                    if (containStair) continue;
                    if (resInfo.ConstructionSubtype == ConstructionSubtype.Staircase)
                    {
                        containStair = true;
                    }
                }
            }

            BuildingFloor maxFloor = editor.CurrentData.BuildingData.MaxFloor;
            BuildingFloor currentFloor = floor.FloorIndex;
            if ((maxFloor == BuildingFloor.F3_Roof || maxFloor == BuildingFloor.F2) && currentFloor == BuildingFloor.F1)
            {
                if (!containStair)
                {
                    OnBadCheck?.Invoke(LanguageLoader.GetTextByType(TextType.Game, 123));
                    yield break;
                }
            }
            else
            {
                if (containStair)
                {
                    OnBadCheck?.Invoke(LanguageLoader.GetTextByType(TextType.Game, 124));
                    yield break;
                }
            }

            OnCheckStage?.Invoke(LanguageLoader.GetTextByType(TextType.Game, 130));

            floor.RemoveTestCreatedZones();
            rooms.UpdateAllBlueprintPoints(ConstructionLocation.Inside);

            if (currentFloor == BuildingFloor.F1 || currentFloor == BuildingFloor.F2)
            {
                rooms.UpdateAllInsideRooms();
                floor.SpawnNewRooms(rooms.RoomsInfo);
                yield return new WaitForSeconds(physicsCheckTime);
                if (!CheckEditorEnabled(editorObj, OnBadCheck)) yield break;

                List<BuildingRoom> allowedRooms = editor.CurrentData.BuildingData.BuildingType.GetAllowedRooms();
                foreach (StaticPoolableObject obj in floor.RoomsPool.Objects)
                {
                    if (!obj.IsUsing) continue;
                    BlueprintRoom room = (BlueprintRoom)obj;
                    if (!room.IsGoodPlacement)
                    {
                        OnBadCheck?.Invoke(BlueprintEditor.IncorrectRoomsPlacementTextInfo.Text);
                        yield break;
                    }
                    if (!IsRoomSizeCorrect(room))
                    {
                        OnBadCheck?.Invoke($"{LanguageLoader.GetTextByType(TextType.Game, 125)}\n[{room.RoomNameText} x {room.SizeText}]");
                        yield break;
                    }
                    if (!CheckRoomMarkers(allowedRooms, room, OnBadCheck))
                        yield break;
                }
            }
            else
            {
                yield return new WaitForSeconds(physicsCheckTime);
            }
            if (!CheckEditorEnabled(editorObj, OnBadCheck)) yield break;
            if (!CheckAllObjectsPlacedCorrectly(floor, OnBadCheck)) yield break;

            OnCheckStage?.Invoke(LanguageLoader.GetTextByType(TextType.Game, 131));

            floor.GenerateZonesFor(floor, out BlueprintZone greenZone);
            if (greenZone == null)
            {
                OnBadCheck?.Invoke(LanguageLoader.GetTextByType(TextType.Game, 126));
                yield break;
            }
            yield return new WaitForSeconds(physicsCheckTime);
            if (!CheckEditorEnabled(editorObj, OnBadCheck)) yield break;
            if (!CheckAllObjectsPlacedCorrectly(floor, OnBadCheck)) yield break;
            if (currentFloor == BuildingFloor.F1_Flooring || currentFloor == BuildingFloor.F2_FlooringRoof || currentFloor == BuildingFloor.F3_Roof)
            {
                if (FloorHasEmptyGreenZones(floor))
                {
                    OnBadCheck?.Invoke(LanguageLoader.GetTextByType(TextType.Game, 127));
                    yield break;
                }
            }
            if (currentFloor == BuildingFloor.F1 || currentFloor == BuildingFloor.F2)
            {
                if (FloorHasOutsideObjectsNotInGreenZones(floor))
                {
                    OnBadCheck?.Invoke(LanguageLoader.GetTextByType(TextType.Game, 128));
                    yield break;
                }
            }

            OnCorrectCheck?.Invoke();
        }
        private static bool CheckRoomMarkers(List<BuildingRoom> allowedRooms, BlueprintRoom room, System.Action<string> OnBadCheck)
        {
            if (room.Marker == null)
            {
                OnBadCheck?.Invoke($"{LanguageLoader.GetTextByType(TextType.Game, 138)}\n[{room.RoomNameText}]");
                return false;
            }
            if (!allowedRooms.Contains(room.Marker.Marker.RoomType))
            {
                OnBadCheck?.Invoke($"{LanguageLoader.GetTextByType(TextType.Game, 138)}\n[{room.RoomNameText}]");
                return false;
            }
            if (room.AdditionalMarker == null)
            {
                return true;
            }
            if (!allowedRooms.Contains(room.AdditionalMarker.Marker.RoomType))
            {
                OnBadCheck?.Invoke($"{LanguageLoader.GetTextByType(TextType.Game, 138)}\n[{room.RoomNameText}]");
                return false;
            }
            return true;
        }
        private static bool CheckAllObjectsPlacedCorrectly(Floor floor, System.Action<string> OnBadCheck)
        {
            if (IsAllObjectsPlacedCorrectly(floor)) return true;
            OnBadCheck?.Invoke(BlueprintEditor.IncorrectObjectPlacementTextInfo.Text);
            return false;
        }
        private static bool CheckEditorEnabled(GameObject editor, System.Action<string> OnBadCheck)
        {
            if (editor.activeInHierarchy) return true;
            OnBadCheck?.Invoke("");
            return false;
        }
        private static readonly List<Vector2> _FloorHasEmptyGreenZones_resourcePoints = new(5);
        private static readonly List<Vector2> _FloorHasEmptyGreenZones_zonePoints = new(10);
        private static readonly List<Collider2D> _FloorHasEmptyGreenZones_collidedObjects = new(10);
        private static readonly Vector2[] _FloorHasEmptyGreenZones_pointsScale = new Vector2[] { new(-5, -5), new(-5, 5), new(5, 5), new(5, -5) };
        /// <summary>
        /// Works only with 'flooring' / 'roof' type floors
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public static bool FloorHasEmptyGreenZones(Floor floor)
        {
            int pointsScaleLength = _FloorHasEmptyGreenZones_pointsScale.Length;
            ContactFilter2D contactFilter = new();
            contactFilter.NoFilter();
            RectTransform workflow = BlueprintEditor.Instance.Creator.ElementsParent;
            foreach (StaticPoolableObject obj in floor.ZonesPool.Objects)
            {
                if (!obj.IsUsing) continue;
                BlueprintZone zone = (BlueprintZone)obj;
                if (!zone.IsGoodPlacement) continue;
                zone.SetTextureCoordinatesTo(_FloorHasEmptyGreenZones_zonePoints);
                foreach (ObjectPool<BlueprintPlacerBase> objectPool in floor.ResourcesPool.Values)
                {
                    foreach (BlueprintPlacerBase element in objectPool.Objects)
                    {
                        if (!element.IsUsing) continue;
                        element.BlueprintGraphic.SetEntrieLocalCollisionCorners(_FloorHasEmptyGreenZones_resourcePoints);
                        _FloorHasEmptyGreenZones_resourcePoints.RemoveAt(4);
                        for (int i = 0; i < 4; ++i) //don't get center
                        {
                            Vector2 startPoint = _FloorHasEmptyGreenZones_resourcePoints[i];
                            for (int j = 0; j < pointsScaleLength; ++j)
                            {
                                Vector2 scaledPoint = startPoint + _FloorHasEmptyGreenZones_pointsScale[j];
                                if (CustomMath.IsPointInsidePolygonAlterInt(_FloorHasEmptyGreenZones_resourcePoints, scaledPoint)) continue;
                                if (!CustomMath.IsPointInsidePolygonAlterInt(_FloorHasEmptyGreenZones_zonePoints, scaledPoint)) continue;

                                Physics2D.OverlapPoint(workflow.TransformPoint(scaledPoint), contactFilter, _FloorHasEmptyGreenZones_collidedObjects);
                                int collidedObjectsCount = _FloorHasEmptyGreenZones_collidedObjects.Count;
                                if (collidedObjectsCount == 0)
                                {
                                    //Debug.Log($"Col = 0 : {scaledPoint}");
                                    return true;
                                }
                                bool collidedRedZone = false;
                                bool collidedResource = false;
                                for (int k = 0; k < collidedObjectsCount; ++k)
                                {
                                    GameObject collidedGameObject = _FloorHasEmptyGreenZones_collidedObjects[k].gameObject;
                                    if (collidedGameObject.TryGetComponent(out BlueprintZone colZone))
                                    {
                                        if (!colZone.IsGoodPlacement)
                                        {
                                            collidedRedZone = true;
                                            break;
                                        }
                                    }

                                    Transform collidedParent = collidedGameObject.transform.parent;
                                    if (collidedParent == null) continue;
                                    GameObject collidedParentGameObject = collidedParent.gameObject;
                                    if (collidedParentGameObject.TryGetComponent(out BlueprintResourcePlacer _))
                                    {
                                        collidedResource = true;
                                        break;
                                    }
                                }
                                if (!collidedRedZone && !collidedResource)
                                {
                                    //Debug.Log($"{!collidedRedZone} && {!collidedResource} {scaledPoint}");
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;

        }
        private static readonly List<Vector2> _FloorHasOutsideObjectsNotInGreenZones_zonePoints = new(10);
        private static readonly List<Vector2> _FloorHasOutsideObjectsNotInGreenZones_resourcePoints = new(5);
        /// <summary>
        /// Works only with 'wall' type floors
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public static bool FloorHasOutsideObjectsNotInGreenZones(Floor floor)
        {
            foreach (StaticPoolableObject obj in floor.ZonesPool.Objects)
            {
                if (!obj.IsUsing) continue;
                BlueprintZone zone = (BlueprintZone)obj;
                if (!zone.IsGoodPlacement) continue;
                if (FloorHasOutsideObjectsNotInGreenZone(floor, zone))
                    return true;
            }
            return false;
        }
        public static bool FloorHasOutsideObjectsNotInGreenZone(Floor floor, BlueprintZone zone)
        {
            ContactFilter2D contactFilter = new();
            contactFilter.NoFilter();
            RectTransform workflow = BlueprintEditor.Instance.Creator.ElementsParent;
            float precision = BlueprintEditor.VECTOR_WORKFLOW_PRECISION;
            zone.SetTextureCoordinatesTo(_FloorHasOutsideObjectsNotInGreenZones_zonePoints);
            int zonePointsCount = _FloorHasOutsideObjectsNotInGreenZones_zonePoints.Count;
            foreach (ObjectPool<BlueprintPlacerBase> objectPool in floor.ResourcesPool.Values)
            {
                foreach (BlueprintPlacerBase element in objectPool.Objects)
                {
                    if (!element.IsUsing) continue;
                    BlueprintResourcePlacer resource = (BlueprintResourcePlacer)element;
                    ConstructionResourceInfo resInfo = resource.Element.ResourceInfo;
                    if (resInfo.ConstructionLocation != ConstructionLocation.Outside) continue;
                    ConstructionSubtype resSubtype = resInfo.ConstructionSubtype;
                    if (resSubtype == ConstructionSubtype.CornerOut) continue;

                    element.BlueprintGraphic.SetEntrieLocalCollisionCorners(_FloorHasOutsideObjectsNotInGreenZones_resourcePoints);
                    bool hasAnyLine = false;
                    for (int i = 0; i < 4; ++i) //don't get center
                    {
                        Vector2 corner = _FloorHasOutsideObjectsNotInGreenZones_resourcePoints[i];
                        Vector2 lineStart = _FloorHasOutsideObjectsNotInGreenZones_zonePoints[zonePointsCount - 1];
                        for (int j = 0; j < zonePointsCount; ++j)
                        {
                            Vector2 lineEnd = _FloorHasOutsideObjectsNotInGreenZones_zonePoints[j];
                            if (corner.PointOnLine2D(lineStart, lineEnd, precision))
                            {
                                hasAnyLine = true;
                                break;
                            }
                            lineStart = lineEnd;
                        }
                        if (hasAnyLine) break;
                    }
                    if (!hasAnyLine)
                    {
                        //Debug.Log($"{element.Transform.localPosition} x {element.BlueprintGraphic.LocalCenter}");
                        return true;
                    }
                }
            }
            return false;

        }

        private static readonly List<BlueprintRoomMarkerPlacer> _GetCollidedRoomMarkers_result = new();
        private static readonly List<Collider2D> _GetCollidedRoomMarkers_colliders = new();
        /// <summary>
        /// Collided Markers will be cleared after next method call
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="collidedMarkers"></param>
        public static void GetCollidedRoomMarkers(PolygonCollider2D collider, out List<BlueprintRoomMarkerPlacer> collidedMarkers)
        {
            _GetCollidedRoomMarkers_result.Clear();
            Physics2D.OverlapCollider(collider, EmptyContactFilter2D, _GetCollidedRoomMarkers_colliders);
            int collidersCount = _GetCollidedRoomMarkers_colliders.Count;
            for (int i = 0; i < collidersCount; ++i)
            {
                Collider2D el = _GetCollidedRoomMarkers_colliders[i];
                if (el == null) continue;
                Transform elParent = el.transform.parent;
                if (elParent == null) continue;
                GameObject elGameObjectParent = elParent.gameObject;
                if (!elGameObjectParent.TryGetComponent(out BlueprintRoomMarkerPlacer marker)) continue;
                _GetCollidedRoomMarkers_result.Add(marker);
            }
            collidedMarkers = _GetCollidedRoomMarkers_result;
        }

        private static readonly HashSet<BlueprintPlacerBase> _IsCollidedWithAny_placers = new();
        private static readonly HashSet<BlueprintZone> _IsCollidedWithAny_zones = new();
        private static readonly HashSet<BlueprintRoom> _IsCollidedWithAny_rooms = new();
        private static readonly List<Collider2D> _IsCollidedWithAny_colliders = new();
        public static readonly ContactFilter2D EmptyContactFilter2D = new();
        /// <summary>
        /// This method is highly inaccurate. You should use it just only to get collided list and use it with <see cref="IsElementCollidedWithOther(BlueprintResource, BlueprintResource)"/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool IsCollidedWithAny(BlueprintGraphic element, out HashSet<BlueprintPlacerBase> collidedBlueprints, out HashSet<BlueprintZone> collidedZones, out HashSet<BlueprintRoom> collidedRooms)
        {
            _IsCollidedWithAny_colliders.Clear();
            EmptyContactFilter2D.NoFilter();
            Physics2D.OverlapCollider(element.BaseCollider, EmptyContactFilter2D, _IsCollidedWithAny_colliders);
            _IsCollidedWithAny_placers.Clear();
            _IsCollidedWithAny_zones.Clear();
            _IsCollidedWithAny_rooms.Clear();
            bool isCollided = false;
            int collidersCount = _IsCollidedWithAny_colliders.Count;
            for (int i = 0; i < collidersCount; ++i)
            {
                Collider2D el = _IsCollidedWithAny_colliders[i];
                if (el == null) continue;
                GameObject elGameObject = el.gameObject;
                if (elGameObject.TryGetComponent(out BlueprintRoom room))
                {
                    _IsCollidedWithAny_rooms.Add(room);
                    continue;
                    //don't set is collided while it doesn't matter on placement
                }
                if (elGameObject.TryGetComponent(out BlueprintZone blueprintZone))
                {
                    _IsCollidedWithAny_zones.Add(blueprintZone);
                    isCollided = true;
                    continue;
                }
                Transform elTransformParent = elGameObject.transform.parent;
                if (elTransformParent == null) continue;
                GameObject elGameObjectParent = elTransformParent.gameObject;
                if (elGameObjectParent.TryGetComponent(out BlueprintPlacerBase blueprintElement))
                {
                    if (blueprintElement.BlueprintGraphic == element) continue;
                    _IsCollidedWithAny_placers.Add(blueprintElement);
                    isCollided = true;
                    continue;
                }

            }
            collidedBlueprints = _IsCollidedWithAny_placers;
            collidedZones = _IsCollidedWithAny_zones;
            collidedRooms = _IsCollidedWithAny_rooms;
            return isCollided;
        }

        private static readonly Vector2[] _IsElementCollidedWithAny_elementCorners = new Vector2[5];
        private static readonly List<Vector2> _IsElementCollidedWithAny_elementCornersUnchecked = new();
        /// <summary>
        /// Creates no garbage but HashSets will be modified each time method calls. <br></br>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nearCollidedPlacers"></param>
        /// <returns></returns>
        public static bool IsElementCollidedWithAny(BlueprintGraphic element, out HashSet<BlueprintPlacerBase> nearCollidedPlacers, out HashSet<BlueprintRoom> collidedRooms)
        {
            if (!IsCollidedWithAny(element, out nearCollidedPlacers, out HashSet<BlueprintZone> collidedZones, out collidedRooms)) return false;
            element.SetEntrieLocalCollisionCorners(_IsElementCollidedWithAny_elementCorners);
            foreach (BlueprintPlacerBase blueprintPlacer in nearCollidedPlacers)
            {
                if (IsElementCollidedWithOther(_IsElementCollidedWithAny_elementCorners, blueprintPlacer.BlueprintGraphic)) return true;
            }
            bool hasGreenZones = BlueprintEditor.Instance.Creator.CurrentFloor.HasGreenZones;
            bool collidedAnyGreenZone = false;
            bool greenCornersCompleted = false;
            if (hasGreenZones)
            {
                _IsElementCollidedWithAny_elementCorners.SetElementsTo(_IsElementCollidedWithAny_elementCornersUnchecked);
                _IsElementCollidedWithAny_elementCornersUnchecked.RemoveAt(4);
            }
            foreach (BlueprintZone blueprintZone in collidedZones)
            {
                if (blueprintZone.IsGoodPlacement)
                {
                    if (!hasGreenZones || greenCornersCompleted) continue;
                    collidedAnyGreenZone = true;
                    int uncheckedCount = 0;
                    while (true)
                    {
                        int currentCount = _IsElementCollidedWithAny_elementCornersUnchecked.Count;
                        if (currentCount == 0) break;
                        Vector2 uncheckedCorner = _IsElementCollidedWithAny_elementCornersUnchecked[0];
                        if (!IsPointInsidePolygon(uncheckedCorner, blueprintZone))
                        {
                            if (element is not BlueprintResource resource)
                                return true;
                            uncheckedCount++;
                            ConstructionSubtype subtype = resource.ResourceInfo.ConstructionSubtype;
                            if (subtype == ConstructionSubtype.CornerIn || subtype == ConstructionSubtype.CornerOut)
                            {
                                if (uncheckedCount > 1) return true;
                            }
                            else break;
                        }
                        _IsElementCollidedWithAny_elementCornersUnchecked.Remove(uncheckedCorner);
                        if (currentCount - 1 == 0)
                        {
                            greenCornersCompleted = true;
                        }
                    }
                    continue;
                }
                if (IsElementIntersectWithPolygon(element, blueprintZone))
                {
                    return true;
                }
            }
            if (hasGreenZones)
            {
                if (!collidedAnyGreenZone) return true;
                if (!greenCornersCompleted) return true;
            }
            return false;
        }

        private static readonly Vector2[] elementCornersBuffer1 = new Vector2[5];
        private static readonly Vector2[] elementCornersBuffer2 = new Vector2[5];
        private static readonly List<Vector2> polygonCornersBuffer1 = new();
        private static readonly List<Vector2> elementCornersBufferList1 = new(5);
        public static bool CanPlaceEntireElementInsideParent(BlueprintGraphic element, RectTransform parent)
        {
            element.SetEntrieLocalCollisionCorners(elementCornersBuffer1);
            return parent.ContainsAll(elementCornersBuffer1);
        }
        public static bool IsPointInsideGraphic(Vector2 point, BlueprintGraphic graphic)
        {
            graphic.SetEntrieLocalCollisionCorners(elementCornersBuffer1);
            return IsPointInsideContainerCompletely(elementCornersBuffer1, point);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygon"></param>
        /// <returns>True if point somehow collided with polygon</returns>
        public static bool IsPointInsidePolygon(Vector2 point, PolygonBlueprintGraphic polygon)
        {
            polygon.SetTextureCoordinatesTo(polygonCornersBuffer1);
            return IsPointInsidePolygon(point, polygonCornersBuffer1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygon"></param>
        /// <returns>True if point somehow collided with polygon</returns>
        public static bool IsPointInsidePolygon(Vector2 point, List<Vector2> polygon)
        {
            return CustomMath.IsPointInsidePolygonAlterInt(polygon, point, BlueprintEditor.VECTOR_WORKFLOW_PRECISION);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="polygon"></param>
        /// <returns>True if has intersect with 2D area > 0</returns>
        public static bool IsElementIntersectWithPolygon(BlueprintGraphic element, PolygonBlueprintGraphic polygon)
        {
            element.SetEntrieLocalCollisionCorners(elementCornersBufferList1);
            polygon.SetTextureCoordinatesTo(polygonCornersBuffer1);
            float precision = BlueprintEditor.VECTOR_WORKFLOW_PRECISION;
            int count = elementCornersBufferList1.Count;

            Vector2 lastCorner = elementCornersBufferList1[3];
            for (int i = 0; i < count; ++i)
            {
                Vector2 currentCorner = elementCornersBufferList1[i];
                if (CustomMath.IsPointInsidePolygon(polygonCornersBuffer1, currentCorner, precision)) return true;
                Vector2 midPoint = new((currentCorner.x + lastCorner.x) / 2f, (currentCorner.y + lastCorner.y) / 2f);
                if (i == 4) continue;
                if (CustomMath.IsPointInsidePolygon(polygonCornersBuffer1, midPoint, precision)) return true;
                lastCorner = currentCorner;
            }

            elementCornersBufferList1.RemoveAt(4);
            count = polygonCornersBuffer1.Count;
            lastCorner = polygonCornersBuffer1[count - 1];
            for (int i = 0; i < count; ++i)
            {
                Vector2 currentCorner = polygonCornersBuffer1[i];
                if (CustomMath.IsPointInsidePolygon(elementCornersBufferList1, currentCorner, precision)) return true;
                Vector2 midPoint = new((currentCorner.x + lastCorner.x) / 2f, (currentCorner.y + lastCorner.y) / 2f);
                if (CustomMath.IsPointInsidePolygon(elementCornersBufferList1, midPoint, precision)) return true;
                lastCorner = currentCorner;
            }

            return false;
        }
        public static bool IsElementCollidedWithOther(Vector2[] fourCornersFirstElement, BlueprintGraphic element2)
        {
            element2.SetEntrieLocalCollisionCorners(elementCornersBuffer2);
            return fourCornersFirstElement[4].Approximately(elementCornersBuffer2[4], BlueprintEditor.VECTOR_WORKFLOW_PRECISION) ||
                IsAnyPointInsideContainer(fourCornersFirstElement, elementCornersBuffer2) ||
                IsAnyPointInsideContainer(elementCornersBuffer2, fourCornersFirstElement);
        }
        public static bool IsElementCollidedWithOther(BlueprintGraphic element1, BlueprintGraphic element2)
        {
            element1.SetEntrieLocalCollisionCorners(elementCornersBuffer1);
            element2.SetEntrieLocalCollisionCorners(elementCornersBuffer2);
            return elementCornersBuffer1[4].Approximately(elementCornersBuffer2[4], BlueprintEditor.VECTOR_WORKFLOW_PRECISION) ||
                    IsAnyPointInsideContainer(elementCornersBuffer1, elementCornersBuffer2) ||
                    IsAnyPointInsideContainer(elementCornersBuffer2, elementCornersBuffer1);
        }

        /// <summary>
        /// Container: <br></br>
        /// 0 => Top Left; <br></br>
        /// 1 => Top Right; <br></br>
        /// 2 => Botoom Right; <br></br>
        /// 3 => Bottom Left; 
        /// </summary>
        private static bool IsContainedInside(Vector2[] container, Vector2[] containable)
        {
            for (int i = 0; i < containable.Length; ++i)
            {
                if (!IsPointInsideContainerCompletely(container, containable[i])) return false;
            }
            return true;
        }
        /// <summary>
        /// Container and Points: <br></br>
        /// 0 => Top Left; <br></br>
        /// 1 => Top Right; <br></br>
        /// 2 => Botoom Right; <br></br>
        /// 3 => Bottom Left; <br></br>
        /// 4 => Center;
        /// Points[4] => Center
        /// </summary>
        /// <returns>True if any collision performed</returns>
        private static bool IsAnyPointInsideContainer(Vector2[] container, Vector2[] points)
        {
            return (IsPointInsideContainerCompletely(container, points[4]) ||
                    IsPointInsideContainerWithoutCorner(container, points[0]) ||
                    IsPointInsideContainerWithoutCorner(container, points[1]) ||
                    IsPointInsideContainerWithoutCorner(container, points[2]) ||
                    IsPointInsideContainerWithoutCorner(container, points[3]) ||
                    AnyLineIntersected(container, points[0], points[2]) ||
                    AnyLineIntersected(container, points[1], points[3]) ||
                    AnyLineIntersected(container, points[0], points[1]) ||
                    AnyLineIntersected(container, points[2], points[3]));
        }
        private static bool AnyLineIntersected(Vector2[] container, Vector2 lineStart, Vector2 lineEnd)
        {
            return (CustomMath.AreLinesIntersecting(lineStart, lineEnd, container[0], container[1], true) ||
                    CustomMath.AreLinesIntersecting(lineStart, lineEnd, container[1], container[2], true) ||
                    CustomMath.AreLinesIntersecting(lineStart, lineEnd, container[2], container[3], true) ||
                    CustomMath.AreLinesIntersecting(lineStart, lineEnd, container[3], container[0], true));
        }
        /// <summary>
        /// Includes end point <br></br>
        /// 0 => Top Left; <br></br>
        /// 1 => Top Right; <br></br>
        /// 2 => Botoom Right; <br></br>
        /// 3 => Bottom Left; 
        /// </summary>
        /// <returns>True if point inside the fourPointsArray</returns>
        private static bool IsPointInsideContainerCompletely(Vector2[] container, Vector2 point)
        {
            return ((container[1].x >= point.x && container[1].y >= point.y) &&
                    (container[3].x <= point.x && container[3].y <= point.y));
        }
        /// <summary>
        /// Not includes end point <br></br>
        /// 0 => Top Left; <br></br>
        /// 1 => Top Right; <br></br>
        /// 2 => Botoom Right; <br></br>
        /// 3 => Bottom Left; 
        /// </summary>
        /// <returns>True if point inside the fourPointsArray</returns>
        private static bool IsPointInsideContainerWithoutCorner(Vector2[] container, Vector2 point)
        {
            return ((container[1].x > point.x && container[1].y > point.y) &&
                    (container[3].x < point.x && container[3].y < point.y));
        }

        #endregion methods
    }
}