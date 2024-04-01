using Game.DataBase;
using Game.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.UI.Overlay.Computer.DesignApp
{
    [System.Serializable]
    internal class BlueprintEditorRooms
    {
        #region fields & properties
        /// <summary>
        /// Don't modify collection. <br></br>
        /// Points are connected by <see cref="ConnectedPoint.PointLocation"/>
        /// </summary>
        public HashSet<BlueprintPointInfo> ElementsPoints => elementsPoints;
        private readonly HashSet<BlueprintPointInfo> elementsPoints = new(100);
        public HashSet<BlueprintRoomInfo> RoomsInfo => roomsInfo;
        private readonly HashSet<BlueprintRoomInfo> roomsInfo = new();

        [System.NonSerialized] private readonly BlueprintEditorCreator creator = null;
        #endregion fields & properties

        #region methods

        #region Rooms
        /// <summary>
        /// Reference to <see cref="RoomsInfo"/>
        /// </summary>
        public void UpdateAllInsideRooms()
        {
            SetAllInsideRooms(RoomsInfo);
        }
        private readonly List<BlueprintPointInfo> _FindAllInsideRooms_tempResultStack = new();
        private readonly HashSet<BlueprintResourcePlacer> _FindAllInsideRooms_roomDoors = new();
        /// <summary>
        /// Rooms will be cleared and filled with new elements
        /// </summary>
        /// <param name="rooms"></param>
        private void SetAllInsideRooms(HashSet<BlueprintRoomInfo> rooms)
        {
            HashSet<BlueprintResourcePlacer> roomDoors = _FindAllInsideRooms_roomDoors;
            List<BlueprintPointInfo> tempResultStack = _FindAllInsideRooms_tempResultStack;
            roomDoors.Clear();
            tempResultStack.Clear();
            rooms.Clear();

            foreach (BlueprintPointInfo point in ElementsPoints)
            {
                ConnectedPoint connected = point.ConnectedCoordinates;
                if (connected == null) continue; //check for is using
                if (connected.PointLocation == ConstructionLocation.Outside) continue;
                BlueprintResourcePlacer choosedDoor = null;
                foreach (BlueprintResourcePlacer element in point.AdjacentElements)
                {
                    if (element.Element.ResourceInfo.ConstructionSubtype != ConstructionSubtype.Door) continue;
                    choosedDoor = element;
                    break;
                }
                if (choosedDoor == null) continue;
                if (roomDoors.Contains(choosedDoor)) continue;
                roomDoors.Add(choosedDoor);
                if (!TryFindRoomForPoint(point, ref tempResultStack)) continue;
                BlueprintRoomInfo room = new(point);
                tempResultStack.SetElementsTo(room.LoopPoints);
                rooms.Add(room);
            }
        }

        private HashSet<List<BlueprintPointInfo>> _TryFindRoomForPoint_tempResult = new();
        private bool TryFindRoomForPoint(BlueprintPointInfo point, ref List<BlueprintPointInfo> foundStack)
        {
            _TryFindRoomForPoint_tempResult.Clear();
            FindAllLoopsByPoints(point, _TryFindRoomForPoint_tempResult, ConstructionLocation.Inside);
            if (_TryFindRoomForPoint_tempResult.Count == 0) return false;

            float minPointsArea = float.MaxValue;
            int minPointsCount = int.MaxValue;
            List<BlueprintPointInfo> found = null;
            foreach (List<BlueprintPointInfo> pointsStack in _TryFindRoomForPoint_tempResult)
            {
                int currentPointsCount = pointsStack.Count;
                //if (currentPointsCount > minPointsCount) continue;
                float currentArea = CalculateRoomArea(pointsStack);
                if (currentArea > minPointsArea) continue;
                minPointsArea = currentArea;
                minPointsCount = currentPointsCount;
                found = pointsStack;
            }
            if (found == null) return false;
            found.SetElementsTo(foundStack);

            return true;
        }
        private readonly HashSet<List<BlueprintPointInfo>> _TryFindMostDistancedLoop_tempResult = new();
        public bool TryFindMostDistancedLoop(BlueprintPointInfo point, List<BlueprintPointInfo> foundStack, ConstructionLocation location)
        {
            _TryFindMostDistancedLoop_tempResult.Clear();
            FindAllLoopsByPoints(point, _TryFindMostDistancedLoop_tempResult, location);
            if (_TryFindMostDistancedLoop_tempResult.Count == 0) return false;

            float maxPointsArea = float.MinValue;
            List<BlueprintPointInfo> found = null;
            foreach (List<BlueprintPointInfo> pointsStack in _TryFindMostDistancedLoop_tempResult)
            {
                int currentPointsCount = pointsStack.Count;
                float currentArea = CalculateRoomArea(pointsStack);
                if (currentArea < maxPointsArea) continue;
                maxPointsArea = currentArea;
                found = pointsStack;
            }
            if (found == null) return false;
            found.SetElementsTo(foundStack);

            return true;
        }
        private readonly List<Vector2> _CalculateRoomArea_tempPoints = new();
        public float CalculateRoomArea(BlueprintRoomInfo roomInfo) => CalculateRoomArea(roomInfo.LoopPoints);
        public float CalculateRoomArea(List<BlueprintPointInfo> loopPoints) => CalculateRoomArea(loopPoints, loopPoints.Count);
        private float CalculateRoomArea(List<BlueprintPointInfo> loopPoints, int loopPointsCount)
        {
            List<Vector2> points = _CalculateRoomArea_tempPoints;
            points.Clear();
            for (int i = 0; i < loopPointsCount; ++i)
            {
                BlueprintPointInfo point = loopPoints[i];
                points.Add(point.LocalWorkflowCoordinates);
            }
            points.Add(loopPoints[0].LocalWorkflowCoordinates);
            float divideScale = BlueprintEditor.CELL_SIZE * 10; //10 cells per meter
            return CustomMath.CalculateArea(points) / divideScale / divideScale;
        }
        #endregion Rooms

        #region Points
        public void FindAllPointsContainsElement(BlueprintPlacerBase element, HashSet<BlueprintPointInfo> resultOutput)
        {
            resultOutput.Clear();
            foreach (BlueprintPointInfo searchPoint in ElementsPoints)
            {
                if (!searchPoint.AdjacentElements.Contains(element)) continue;
                resultOutput.Add(searchPoint);
            }
        }
        public BlueprintPointInfo FindAnyPointContainElement(BlueprintPlacerBase element)
        {
            foreach (BlueprintPointInfo searchPoint in ElementsPoints)
            {
                if (!searchPoint.AdjacentElements.Contains(element)) continue;
                return searchPoint;
            }
            return null;
        }

        #region Points Connection
        private readonly HashSet<BlueprintPlacerBase> tempAdjacentBlueprintElements = new();
        private readonly HashSet<ConnectedPoint> tempAdjacentBlueprintPoints = new();
        private readonly HashSet<ConnectedPoint> lastAdjacentBlueprintPoints = new();

        /// <summary>
        /// Reference to <see cref="ElementsPoints"/> <br></br>
        /// Creates no garbage but works slow
        /// </summary>
        public void UpdateAllBlueprintPoints(ConstructionLocation updatePointsLocation)
        {
            SetBlueprintPointsTo(ElementsPoints, updatePointsLocation);
        }
        private void AddBlueprintPointWithAdjacentElementsInInfo(HashSet<BlueprintPointInfo> info, BlueprintResourcePlacer element, ConstructionLocation pointsLocation)
        {
            float precision = BlueprintEditor.VECTOR_WORKFLOW_PRECISION;

            if (pointsLocation == ConstructionLocation.Inside)
            {
                element.SetAdjacentInsideBlueprintsTo(tempAdjacentBlueprintElements);
                element.Element.SetInsideLinesLocalCornersTo(tempAdjacentBlueprintPoints);
            }
            else
            {
                element.SetAdjacentOutsideBlueprintsTo(tempAdjacentBlueprintElements);
                element.Element.SetOutsideLinesLocalCornersTo(tempAdjacentBlueprintPoints);
            }

            foreach (ConnectedPoint linePoint in tempAdjacentBlueprintPoints)
            {
                Vector2 localCoord = linePoint.LocalWorfklowCoordinates;
                Vector2 localCoordEnd = linePoint.Connected.LocalWorfklowCoordinates;
                BlueprintPointInfo currentPoint = null;
                foreach (BlueprintPointInfo searchPoint in info)
                {
                    if (!searchPoint.LocalWorkflowCoordinates.Approximately(localCoord, precision)) continue;
                    currentPoint = searchPoint;
                    break;
                }
                currentPoint.TryRecreate(linePoint, element);
                HashSet<BlueprintResourcePlacer> currentPointAdjacentElements = currentPoint.AdjacentElements;
                currentPointAdjacentElements.Add(element);

                foreach (BlueprintPlacerBase adjacentElement in tempAdjacentBlueprintElements)
                {
                    if (pointsLocation == ConstructionLocation.Inside)
                    {
                        adjacentElement.BlueprintGraphic.SetInsideLinesLocalCornersTo(lastAdjacentBlueprintPoints);
                    }
                    else
                    {
                        adjacentElement.BlueprintGraphic.SetOutsideLinesLocalCornersTo(lastAdjacentBlueprintPoints);
                    }

                    foreach (ConnectedPoint adjLinePoint in lastAdjacentBlueprintPoints)
                    {
                        Vector2 adjLocalCoord = adjLinePoint.LocalWorfklowCoordinates;
                        Vector2 adjLocalCoordEnd = adjLinePoint.Connected.LocalWorfklowCoordinates;

                        //HERE'S THE CHECK FOR REQUIRED POINTS UNITON
                        if (!adjLocalCoord.Approximately(localCoord, precision))
                        {
                            if (!localCoord.PointOnLine2D(adjLocalCoord, adjLocalCoordEnd, precision))
                            {
                                continue;
                            }
                            //nothing
                        }
                        currentPointAdjacentElements.Add((BlueprintResourcePlacer)adjacentElement);
                        break;
                    }
                }
            }
        }
        private void ConnectBlueprintAdjacentPoints(BlueprintPointInfo point, HashSet<BlueprintPointInfo> info, ConstructionLocation pointsLocation)
        {
            ConnectedPoint pointConnected = point.ConnectedCoordinates;
            if (pointConnected == null) return;
            float precision = BlueprintEditor.VECTOR_WORKFLOW_PRECISION;
            ConstructionLocation pointLocation = pointConnected.PointLocation;
            HashSet<BlueprintPointInfo> pointAdjacentPoints = point.AdjacentPoints;
            Vector2 pointCoordinates = point.LocalWorkflowCoordinates;
            foreach (BlueprintResourcePlacer adjacentElement in point.AdjacentElements)
            {
                if (pointsLocation == ConstructionLocation.Inside)
                {
                    adjacentElement.Element.SetInsideLinesLocalCornersTo(tempAdjacentBlueprintPoints);
                }
                else
                {
                    adjacentElement.Element.SetOutsideLinesLocalCornersTo(tempAdjacentBlueprintPoints);
                }

                foreach (ConnectedPoint adjLinePoint in tempAdjacentBlueprintPoints)
                {
                    Vector2 adjLocalCoord = adjLinePoint.LocalWorfklowCoordinates;
                    if (adjLocalCoord.Approximately(pointCoordinates, precision)) continue;
                    Vector2 adjLocalCoordEnd = adjLinePoint.Connected.LocalWorfklowCoordinates;

                    //add adjacent points for own element in connection order to reduce cyclic counts
                    if (adjacentElement == point.CreatedElement)
                    {
                        if (!adjLocalCoordEnd.Approximately(pointCoordinates, precision)) continue;
                    }
                    if (!adjLocalCoord.x.Approximately(pointCoordinates.x, precision) && !adjLocalCoord.y.Approximately(pointCoordinates.y, precision)) continue;
                    if (adjLinePoint.PointLocation != pointLocation) continue;
                    BlueprintPointInfo found = null;
                    foreach (BlueprintPointInfo searchPoint in info)
                    {
                        ConnectedPoint searchPointConnected = searchPoint.ConnectedCoordinates;
                        if (searchPointConnected == null) continue;
                        if (searchPointConnected.PointLocation != pointLocation) continue;
                        if (!searchPoint.LocalWorkflowCoordinates.Approximately(adjLocalCoord, precision)) continue;
                        found = searchPoint;
                        break;
                    }
                    if (found == null) continue;
                    pointAdjacentPoints.Add(found);
                    found.AdjacentPoints.Add(point);
                }
            }
        }
        private void UniteAllPointsCoordinates(BlueprintResourcePlacer element, ConstructionLocation pointsLocation)
        {
            if (pointsLocation == ConstructionLocation.Inside)
            {
                element.Element.SetInsideLinesLocalCornersTo(tempAdjacentBlueprintPoints);
            }
            else
            {
                element.Element.SetOutsideLinesLocalCornersTo(tempAdjacentBlueprintPoints);
            }
            int i = 0;
            foreach (ConnectedPoint linePoint in tempAdjacentBlueprintPoints)
            {
                _FixBlueprintPointInfoSet_coordinates.Add(linePoint.LocalWorfklowCoordinates);
                i++;
            }
        }

        private readonly HashSet<Vector2> _FixBlueprintPointInfoSet_coordinates = new();
        private readonly List<Vector2> _FixBlueprintPointInfoSet_coordinatesList = new();
        private readonly HashSet<BlueprintPlacerBase> _FixBlueprintPointInfoSet_tempElements = new();
        private void FixBlueprintPointInfoSet(HashSet<BlueprintPointInfo> fullInfo, ConstructionLocation pointsLocation)
        {
            int infoCount = fullInfo.Count;
            _FixBlueprintPointInfoSet_coordinates.Clear();
            foreach (ObjectPool<BlueprintPlacerBase> objectPool in creator.CurrentFloor.ResourcesPool.Values)
            {
                objectPool.SetObjectsTo(_FixBlueprintPointInfoSet_tempElements);
                foreach (BlueprintPlacerBase placer in _FixBlueprintPointInfoSet_tempElements)
                {
                    if (!placer.IsUsing) continue;
                    BlueprintResourcePlacer element = (BlueprintResourcePlacer)placer;
                    UniteAllPointsCoordinates(element, pointsLocation);
                }
            }

            int coordinatesCount = _FixBlueprintPointInfoSet_coordinates.Count;
            if (infoCount > coordinatesCount)
            {
                while (fullInfo.Count != coordinatesCount)
                {
                    BlueprintPointInfo tempPoint = null;
                    foreach (BlueprintPointInfo p in fullInfo)
                    {
                        tempPoint = p;
                        break;
                    }
                    fullInfo.Remove(tempPoint);
                }
            }
            else
            {
                while (fullInfo.Count != coordinatesCount)
                {
                    fullInfo.Add(new(null, Vector2.zero, null));
                }
            }

            _FixBlueprintPointInfoSet_coordinates.SetElementsTo(_FixBlueprintPointInfoSet_coordinatesList);
            int taken = 0;
            foreach (BlueprintPointInfo point in fullInfo)
            {
                point.Recreate(null, _FixBlueprintPointInfoSet_coordinatesList[taken], null);
                taken++;
            }
        }

        private readonly HashSet<BlueprintPlacerBase> _SetBlueprintPointsTo_tempElements = new();
        private void SetBlueprintPointsTo(HashSet<BlueprintPointInfo> info, ConstructionLocation updatePointsLocation)
        {
            FixBlueprintPointInfoSet(info, updatePointsLocation);
            foreach (ObjectPool<BlueprintPlacerBase> objectPool in creator.CurrentFloor.ResourcesPool.Values)
            {
                objectPool.SetObjectsTo(_SetBlueprintPointsTo_tempElements);
                foreach (BlueprintPlacerBase placer in _SetBlueprintPointsTo_tempElements)
                {
                    if (!placer.IsUsing) continue;
                    BlueprintResourcePlacer element = (BlueprintResourcePlacer)placer;
                    AddBlueprintPointWithAdjacentElementsInInfo(info, element, updatePointsLocation);
                }
            }
            foreach (BlueprintPointInfo point in info)
            {
                ConnectBlueprintAdjacentPoints(point, info, updatePointsLocation);
            }
        }
        #endregion Points Connection

        #region Search Algorithm    
        private readonly List<BlueprintPointInfo> _FindAllLoopsByPoints_loopPoints = new(10);
        private readonly Stack<BlueprintPointInfo> _FindAllLoopsByPoints_subLoopPoints = new(10);
        private readonly HashSet<BlueprintPointInfo> _FindAllLoopsByPoints_lockedConstructionAdjacentPoints = new(100);
        private readonly HashSet<BlueprintPointInfo> _FindAllLoopsByPoints_unlockedConstructionAdjacentPoints = new(100);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="allLoopPoints"></param>
        /// <param name="pointsLocation"></param>
        /// <returns>Normalized stack results (start point is only one - in '0' position)</returns>
        public void FindAllLoopsByPoints(BlueprintPointInfo startPoint, HashSet<List<BlueprintPointInfo>> allLoopPoints, ConstructionLocation pointsLocation)
        {
            int safety = 500000;
            int currentLoop = 0;
            float vectorPrecision = BlueprintEditor.VECTOR_WORKFLOW_PRECISION;
            List<BlueprintPointInfo> loopPoints = _FindAllLoopsByPoints_loopPoints;
            Stack<BlueprintPointInfo> subLoopPoints = _FindAllLoopsByPoints_subLoopPoints;
            List<BlueprintPointInfo> tempResultPoints = null;
            HashSet<BlueprintPointInfo> lockedConstructionAdjacentPoints = _FindAllLoopsByPoints_lockedConstructionAdjacentPoints;
            HashSet<BlueprintPointInfo> unlockedConstructionAdjacentPoints = _FindAllLoopsByPoints_unlockedConstructionAdjacentPoints;

            allLoopPoints.Clear();
            loopPoints.Clear();
            subLoopPoints.Clear();
            lockedConstructionAdjacentPoints.Clear();
            unlockedConstructionAdjacentPoints.Clear();
            loopPoints.Add(startPoint);

            while (loopPoints.Count > 0)
            {
                int allLoopPointsCount = allLoopPoints.Count;
                if (allLoopPointsCount > 200)
                {
                    return;
                }
                currentLoop++;
                if (currentLoop > safety)
                {
                    InfoRequest.GetErrorRequest(300).Send();
                    return;
                }
                bool doDebug = false;
                if (currentLoop < 200)
                {
                    //doDebug = true;
                }
                else
                {

                }
                int loopPointsCount = loopPoints.Count;
                BlueprintPointInfo tempPoint = loopPoints[loopPointsCount - 1];
                int freeAjacentsCount = 0;
                BlueprintPointInfo freeAdjacentPoint = null;
                subLoopPoints.TryPeek(out BlueprintPointInfo subLoopTop);

                foreach (BlueprintPointInfo adjacentPoint in tempPoint.AdjacentPoints)
                {
                    if (tempPoint.UsedAdjacentPoints.Contains(adjacentPoint)) continue;
                    if (!IsAdjacentPointAllowedByConstruction(tempPoint, adjacentPoint, startPoint, loopPoints, pointsLocation, vectorPrecision))
                    {
                        lockedConstructionAdjacentPoints.Add(adjacentPoint);
                        tempPoint.UsedAdjacentPoints.Add(adjacentPoint);
                        tempPoint.SubUsedAdjacentPoints.Push(adjacentPoint);
                        adjacentPoint.UsedAdjacentPoints.Add(tempPoint);
                        adjacentPoint.SubUsedAdjacentPoints.Push(tempPoint);
                        continue;
                    }

                    if (adjacentPoint == startPoint)
                    {
                        if (!CanDoFinalPointLoop(loopPoints, loopPointsCount))
                        {
                            continue;
                        }
                        bool stackExists = false;
                        foreach (List<BlueprintPointInfo> stack in allLoopPoints)
                        {
                            if (!stack.ElementsSameReversed(loopPoints))
                            {
                                continue;
                            }
                            stackExists = true;
                            break;
                        }
                        if (stackExists) continue;
                        tempResultPoints = new(loopPointsCount);
                        loopPoints.SetElementsTo(tempResultPoints);

                        allLoopPoints.Add(tempResultPoints);
                        startPoint.UsedAdjacentPoints.Add(tempPoint);
                        continue;
                    }

                    if (adjacentPoint == subLoopTop) continue; //guarantees that it's not the same as tempPoint
                    if (adjacentPoint.UsedAdjacentPoints.Contains(tempPoint)) continue;
                    if (currentLoop > 2000) //try prevent infinity loops with cost of some minor bugs
                    {
                        if (CheckPreTempPoint(loopPointsCount, loopPoints, adjacentPoint, tempPoint)) continue;
                    }
                    if (loopPoints.ContainsAlt(adjacentPoint)) continue;

                    freeAdjacentPoint ??= adjacentPoint;
                    freeAjacentsCount++;
                }
                if (freeAjacentsCount == 1 && allLoopPointsCount < 100)
                {
                    if (TryAddExistLoop(allLoopPoints, loopPoints, subLoopPoints, loopPointsCount, tempResultPoints, tempPoint, freeAdjacentPoint, startPoint))
                        freeAjacentsCount = 0;
                }

                switch (freeAjacentsCount)
                {
                    case 0:
                        if (freeAdjacentPoint == startPoint)
                        {
                            if (doDebug) Debug.Log($"{currentLoop}:F=S :<color=#FF88FF> {tempPoint.LocalWorkflowCoordinates}</color>");
                            break;
                        }
                        if (tempPoint == startPoint)
                        {
                            //Debug.Log($"currentLoop : {currentLoop}; loops count : {allLoopPoints.Count}");
                            return;
                        }
                        if (subLoopPoints.Contains(tempPoint))
                        {
                            while (subLoopPoints.TryPeek(out subLoopTop) && subLoopTop == tempPoint)
                            {
                                if (doDebug) Debug.Log($"{currentLoop}:<color=#8888FF>Pop : {tempPoint.LocalWorkflowCoordinates}</color>");
                                subLoopPoints.Pop();
                                tempPoint.SubUsedAdjacentPoints.Pop();
                            }
                        }
                        tempPoint.SubUsedAdjacentPoints.SetElementsTo(tempPoint.UsedAdjacentPoints);
                        var p = loopPoints[loopPointsCount - 1];
                        loopPoints.RemoveAt(loopPointsCount - 1);
                        if (doDebug) Debug.Log($"{currentLoop}:<color=#FF8888>Remove : {tempPoint.LocalWorkflowCoordinates}</color> => - {p.LocalWorkflowCoordinates}");
                        continue;

                    case int i when i > 0:
                        tempPoint.SubUsedAdjacentPoints.Push(freeAdjacentPoint);
                        if (tempPoint == startPoint || freeAdjacentPoint == startPoint)
                        {
                            break;
                        }
                        subLoopPoints.Push(tempPoint);
                        if (doDebug) Debug.Log($"{currentLoop}: <color=#88FF88>Push : {tempPoint.LocalWorkflowCoordinates}</color> => + {freeAdjacentPoint.LocalWorkflowCoordinates}");
                        break;
                }

                tempPoint.UsedAdjacentPoints.Add(freeAdjacentPoint);
                freeAdjacentPoint.UsedAdjacentPoints.Add(tempPoint);
                if (freeAdjacentPoint == startPoint) continue;

                loopPoints.Add(freeAdjacentPoint);
            }
        }
        private bool CheckPreTempPoint(int loopPointsCount, List<BlueprintPointInfo> loopPoints, BlueprintPointInfo adjacentPoint, BlueprintPointInfo tempPoint)
        {
            if (loopPointsCount - 2 < 0) return false; //try prevent infinity loops with cost of some minor bugs
            BlueprintPointInfo preTempPoint = loopPoints[loopPointsCount - 2];
            if (!adjacentPoint.AdjacentPoints.Contains(preTempPoint)) return false;
            tempPoint.UsedAdjacentPoints.Add(adjacentPoint);
            tempPoint.SubUsedAdjacentPoints.Push(adjacentPoint);
            adjacentPoint.UsedAdjacentPoints.Add(tempPoint);
            adjacentPoint.SubUsedAdjacentPoints.Push(tempPoint);
            return true;
        }
        private readonly HashSet<List<BlueprintPointInfo>> _TryAddExistLoop_tempAllLoopPoints = new(10);
        private bool TryAddExistLoop(HashSet<List<BlueprintPointInfo>> allLoopPoints, List<BlueprintPointInfo> loopPoints, Stack<BlueprintPointInfo> subLoopPoints, int loopPointsCount, List<BlueprintPointInfo> tempResultPoints, BlueprintPointInfo tempPoint, BlueprintPointInfo freeAdjacentPoint, BlueprintPointInfo startPoint)
        {
            _TryAddExistLoop_tempAllLoopPoints.Clear();
            bool anyResultCreated = false;
            foreach (List<BlueprintPointInfo> result in allLoopPoints)
            {
                int resultCount = result.Count;
                if (resultCount <= loopPointsCount + 1) continue;
                if (result.ElementsSameOfPart(loopPoints)) continue;

                bool createTempResult = false;
                bool checkNextPointRequirements = false;
                int c = 0;
                for (int i = 0; i < resultCount; ++i)
                {
                    if (createTempResult)
                    {
                        tempResultPoints.Add(result[i]);
                        continue;
                    }
                    BlueprintPointInfo resultPoint = result[i];

                    c++;
                    if (!checkNextPointRequirements && resultPoint == tempPoint)
                    {
                        checkNextPointRequirements = true;
                        continue;
                    }
                    if (!checkNextPointRequirements) continue;
                    if (resultPoint != freeAdjacentPoint)
                    {
                        checkNextPointRequirements = false;
                        continue;
                    }

                    createTempResult = true;
                    tempResultPoints = new(loopPointsCount + resultCount - c + 1);
                    for (int j = 0; j < loopPointsCount; ++j)
                    {
                        tempResultPoints.Add(loopPoints[j]);
                    }
                    tempResultPoints.Add(freeAdjacentPoint);

                    if (anyResultCreated) continue; //push only one time
                    tempPoint.SubUsedAdjacentPoints.Push(freeAdjacentPoint);
                    if (tempPoint != startPoint && freeAdjacentPoint != startPoint)
                    {
                        subLoopPoints.Push(tempPoint);
                    }
                    tempPoint.UsedAdjacentPoints.Add(freeAdjacentPoint);
                    anyResultCreated = true;
                }
                if (createTempResult)
                {
                    if (!CanDoFinalPointLoop(tempResultPoints, tempResultPoints.Count)) continue;
                    _TryAddExistLoop_tempAllLoopPoints.Add(tempResultPoints);
                }
            }
            if (!anyResultCreated) return false;
            foreach (List<BlueprintPointInfo> resultLoop in _TryAddExistLoop_tempAllLoopPoints)
            {
                bool stackExists = false;
                foreach (List<BlueprintPointInfo> stack in allLoopPoints)
                {
                    if (!stack.ElementsSame(resultLoop))
                    {
                        continue;
                    }
                    stackExists = true;
                    break;
                }
                if (stackExists) continue;
                allLoopPoints.Add(resultLoop);
            }
            return true;
        }
        /// <summary>
        /// Simplified algorithm compared to <see cref="FindAllLoopsByPoints"/> <br></br>
        /// Creates no garbage
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="loopPoints"></param>
        /// <param name="pointsLocation"></param>
        /// <returns></returns>
        public bool TryFindAnyLoopByPoints(BlueprintPointInfo startPoint, ref List<BlueprintPointInfo> loopPoints, ConstructionLocation pointsLocation)
        {
            int safety = 1000000;
            int currentLoop = 0;
            float vectorPrecision = BlueprintEditor.VECTOR_WORKFLOW_PRECISION;
            HashSet<BlueprintPointInfo> lockedConstructionAdjacentPoints = _FindAllLoopsByPoints_lockedConstructionAdjacentPoints;

            loopPoints.Clear();
            lockedConstructionAdjacentPoints.Clear();
            loopPoints.Add(startPoint);

            while (loopPoints.Count > 0)
            {
                currentLoop++;
                if (currentLoop > safety)
                {
                    InfoRequest.GetErrorRequest(301).Send();
                    return false;
                }

                int loopPointsCount = loopPoints.Count;
                BlueprintPointInfo tempPoint = loopPoints[loopPointsCount - 1];

                bool isAnyPushed = false;
                foreach (BlueprintPointInfo adjacentPoint in tempPoint.AdjacentPoints)
                {
                    if (tempPoint.UsedAdjacentPoints.Contains(adjacentPoint))
                    {
                        continue;
                    }
                    if (!IsAdjacentPointAllowedByConstruction(tempPoint, adjacentPoint, startPoint, loopPoints, pointsLocation, vectorPrecision))
                    {
                        lockedConstructionAdjacentPoints.Add(adjacentPoint);
                        tempPoint.UsedAdjacentPoints.Add(adjacentPoint);
                        adjacentPoint.UsedAdjacentPoints.Add(tempPoint);
                        continue;
                    }
                    if (adjacentPoint == startPoint)
                    {
                        if (!CanDoFinalPointLoop(loopPoints, loopPointsCount))
                        {
                            continue;
                        }
                        loopPoints.Add(adjacentPoint);
                        return true;
                    }
                    //push only one
                    tempPoint.UsedAdjacentPoints.Add(adjacentPoint);
                    adjacentPoint.UsedAdjacentPoints.Add(tempPoint);

                    loopPoints.Add(adjacentPoint);
                    isAnyPushed = true;
                    break;
                }
                if (isAnyPushed) continue;

                loopPoints.RemoveAt(loopPointsCount - 1);
                continue;
            }
            return false;
        }
        private readonly Vector2[] _IsAdjacentPointAllowedByConstruction_pointsScale = new Vector2[] { new(-5, -5), new(-5, 5), new(5, 5), new(5, -5) };
        private readonly List<Vector2> _IsAdjacentPointAllowedByConstruction_tempCollision = new(5);
        private bool IsAdjacentPointAllowedByConstruction(BlueprintPointInfo currentPoint, BlueprintPointInfo adjacentPoint, BlueprintPointInfo startPoint, List<BlueprintPointInfo> loopPoints, ConstructionLocation pointsLocation, float vectorPrecision)
        {
            if (_FindAllLoopsByPoints_lockedConstructionAdjacentPoints.Contains(adjacentPoint))
            {
                return false;
            }
            //temp point may be same as adjacent point

            ConstructionResourceInfo adjacentPointElementInfo = adjacentPoint.CreatedElement.ResourceInfoUnsafe;
            if (adjacentPointElementInfo.ConstructionType != ConstructionType.Wall)
            {
                if (adjacentPoint.AdjacentElements.Count > 2)
                {
                    if (_FindAllLoopsByPoints_unlockedConstructionAdjacentPoints.Contains(adjacentPoint)) return true;
                    int occupiedPoints = 0;
                    int scalesLength = _IsAdjacentPointAllowedByConstruction_pointsScale.Length;
                    for (int i = 0; i < scalesLength; ++i)
                    {
                        Vector2 point = adjacentPoint.LocalWorkflowCoordinates + _IsAdjacentPointAllowedByConstruction_pointsScale[i];
                        bool containedInsideAny = false;
                        foreach (BlueprintResourcePlacer adjacentPoint_AdjacentResource in adjacentPoint.AdjacentElements)
                        {
                            adjacentPoint_AdjacentResource.BlueprintGraphic.SetEntrieLocalCollisionCorners(_IsAdjacentPointAllowedByConstruction_tempCollision);
                            _IsAdjacentPointAllowedByConstruction_tempCollision.RemoveAt(4);
                            if (!CustomMath.IsPointInsidePolygonAlter(_IsAdjacentPointAllowedByConstruction_tempCollision, point)) continue;
                            containedInsideAny = true;
                            break;
                        }
                        if (containedInsideAny)
                            occupiedPoints++;
                        else
                        {
                            break;
                        }
                    }
                    if (occupiedPoints == scalesLength)
                    {
                        return false;
                    }
                    _FindAllLoopsByPoints_unlockedConstructionAdjacentPoints.Add(adjacentPoint);
                }

                return true;
            }

            ConstructionLocation adjacentResourceLocation = adjacentPointElementInfo.ConstructionLocation;
            if (pointsLocation == ConstructionLocation.Outside)
            {
                //lock for outside search only within outside objects
                if (adjacentResourceLocation != startPoint.CreatedElement.Element.ResourceInfo.ConstructionLocation)
                {
                    return false;
                }
            }

            BlueprintResource currentPointElement = currentPoint.CreatedElement.Element;
            BlueprintResource adjacentPointElement = adjacentPoint.CreatedElement.Element;
            Vector2 currentResourceCenter = currentPointElement.LocalCenter;
            Vector2 adjacentResourceCenter = adjacentPointElement.LocalCenter;

            if (adjacentPoint.AdjacentPoints.Count < 2)
            {
                return false;
            }

            if (adjacentPoint.ConnectedCoordinates.PointLocation != pointsLocation)
            {
                return false;
            }

            ConstructionResourceInfo currentPointElementInfo = currentPoint.CreatedElement.ResourceInfoUnsafe;
            ConstructionSubtype currentResourceSubtype = currentPointElementInfo.ConstructionSubtype;
            ConstructionSubtype adjacentResourceSubtype = adjacentPointElementInfo.ConstructionSubtype;
            ConstructionLocation currentResourceLocation = currentPointElementInfo.ConstructionLocation;

            //if you encounter strange bug, try add resource subtype for here
            if (currentResourceSubtype != ConstructionSubtype.Base && currentResourceSubtype != ConstructionSubtype.CornerIn && currentResourceSubtype != ConstructionSubtype.CornerOut)
            {
                //lock for door, window etc. that aren't same locations (bugs)
                if (adjacentResourceLocation != currentResourceLocation)
                {
                    return false;
                }
            }


            bool hasSameX = false;
            float sameX = 0f;
            bool hasSameY = false;
            float sameY = 0f;
            if (adjacentResourceCenter.x.Approximately(currentResourceCenter.x, vectorPrecision))
            {
                hasSameX = true;
                sameX = adjacentResourceCenter.x;
            }
            if (adjacentResourceCenter.y.Approximately(currentResourceCenter.y, vectorPrecision))
            {
                hasSameY = true;
                sameY = adjacentResourceCenter.y;
            }

            if (!hasSameX)
            {
                if (!hasSameY)
                {
                    //lock all non corner/door-transitions
                    if (adjacentResourceSubtype != ConstructionSubtype.CornerIn && adjacentResourceSubtype != ConstructionSubtype.CornerOut &&
                        currentResourceSubtype != ConstructionSubtype.CornerIn && currentResourceSubtype != ConstructionSubtype.CornerOut &&
                        currentResourceSubtype != ConstructionSubtype.Door && adjacentResourceSubtype != ConstructionSubtype.Door)
                    {
                        //lock only between same walls
                        if (adjacentResourceLocation == currentResourceLocation)
                        {
                            return false;
                        }
                    }
                }
            }

            if (hasSameX || hasSameY)
            {
                //lock for one-line wrong subtype (in addition to check at ~676 line) (bugs)
                if (adjacentResourceLocation != currentResourceLocation)
                {
                    return false;
                }
                foreach (BlueprintResourcePlacer adjacentElPlacer in currentPoint.AdjacentElements)
                {
                    BlueprintResource adjacentEl = adjacentElPlacer.Element;
                    Vector2 adjacentElCenter = adjacentEl.LocalCenter;
                    ConstructionSubtype adjacentElSubtype = adjacentEl.ResourceInfo.ConstructionSubtype;
                    if (adjacentEl.ResourceInfo.ConstructionLocation == currentResourceLocation)
                    {
                        continue;
                    }
                    if (adjacentElSubtype != ConstructionSubtype.Base)
                    {
                        return false;
                    }
                    if (adjacentResourceSubtype == ConstructionSubtype.CornerIn || adjacentResourceSubtype == ConstructionSubtype.CornerOut)
                    {
                        return false;
                    }
                    if (adjacentElCenter.x.Approximately(currentResourceCenter.x, vectorPrecision) || adjacentElCenter.y.Approximately(currentResourceCenter.y, vectorPrecision))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        private bool CanDoFinalPointLoop(List<BlueprintPointInfo> loopPoints, int loopPointsCount)
        {
            //check that we didn't get result for instant in adjacent element
            if (loopPointsCount < 4)
            {
                return false;
            }
            if (CalculateRoomArea(loopPoints, loopPointsCount) < 1)
            {
                return false;
            }
            //don't need because has check in main loop
            /*if (FinalLoopHasReversedCombination(loopPoints, loopPointsCount))
            {
                return false;
            }*/
            return true;
        }
        private bool FinalLoopHasReversedCombination(List<BlueprintPointInfo> loopPoints, int loopPointsCount)
        {
            for (int i = 0; i < loopPointsCount - 1; ++i)
            {
                BlueprintPointInfo currentPoint = loopPoints[i];
                BlueprintPointInfo adjacentPoint = loopPoints[i + 1];

                BlueprintPointInfo lastListPoint = adjacentPoint;
                for (int j = i + 1; j < loopPointsCount - 1; ++j)
                {
                    BlueprintPointInfo currentListPoint = loopPoints[j + 1];
                    if (lastListPoint == adjacentPoint && currentListPoint == currentPoint)
                    {
                        return true;
                    }
                    lastListPoint = currentListPoint;
                }
            }
            return false;
        }

        private readonly HashSet<BlueprintResourcePlacer> _GetAdjacentObjectsToLoopPointsCount_unitedObjects = new();
        private int GetAdjacentObjectsToLoopPointsCount(List<BlueprintPointInfo> loopPoints)
        {
            _GetAdjacentObjectsToLoopPointsCount_unitedObjects.Clear();
            int pointsCount = loopPoints.Count;
            for (int i = 0; i < pointsCount; ++i)
            {
                BlueprintPointInfo loopPoint = loopPoints[i];
                if (loopPoint.CreatedElement == null) continue;
                _GetAdjacentObjectsToLoopPointsCount_unitedObjects.Add(loopPoint.CreatedElement);
                //actually this will GREATLY reduces performance
                /*foreach (BlueprintPointInfo insideLoopPoint in loopPoints)
                {
                    if (loopPoint == insideLoopPoint) continue;
                    foreach (BlueprintPlacer loopPlacer in loopPoint.AdjacentElements)
                    {
                        foreach (BlueprintPlacer insideLoopPlacer in insideLoopPoint.AdjacentElements)
                        {
                            if (loopPlacer != insideLoopPlacer) continue;
                            _GetAdjacentObjectsToLoopPointsCount_unitedObjects.Add(loopPlacer);
                        }
                    }
                }*/
            }
            return _GetAdjacentObjectsToLoopPointsCount_unitedObjects.Count;
        }
        #endregion Search Algorithm

        #endregion Points

        #endregion methods
        public BlueprintEditorRooms(BlueprintEditorCreator creator)
        {
            this.creator = creator;
        }
    }
}