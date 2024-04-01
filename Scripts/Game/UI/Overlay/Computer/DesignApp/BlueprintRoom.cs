using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Core;

namespace Game.UI.Overlay.Computer.DesignApp
{
    internal class BlueprintRoom : PolygonBlueprintGraphic
    {
        #region fields & properties
        public string SizeText => sizeText.text;
        [SerializeField] private TextMeshProUGUI sizeText;
        public string RoomNameText => roomNameText.text;
        [SerializeField] private TextMeshProUGUI roomNameText;
        [SerializeField] private GameObject UICenter;
        public BlueprintRoomInfo Info => info;
        [Title("Read Only")][SerializeField][ReadOnly] private BlueprintRoomInfo info;
        /// <summary>
        /// May be null
        /// </summary>
        public BlueprintRoomMarkerPlacer Marker => marker;
        [SerializeField][ReadOnly] private BlueprintRoomMarkerPlacer marker;
        /// <summary>
        /// May be null
        /// </summary>
        public BlueprintRoomMarkerPlacer AdditionalMarker => additionalMarker;
        [SerializeField][ReadOnly] private BlueprintRoomMarkerPlacer additionalMarker;
        [SerializeField] BuildingRoom rt;
        public float Area => area;
        private float area = 0f;

        #endregion fields & properties

        #region methods
        public void UpdateGraphic(BlueprintRoomInfo info)
        {
            this.info = info;
            base.UpdateGraphic(localTexturePoints, true);
        }
        protected override void UpdateTextureCoordinates()
        {
            localTexturePoints.Clear();
            int pointsCount = info.LoopPoints.Count;
            for (int pi = 0; pi < pointsCount; ++pi)
            {
                localTexturePoints.Add(info.LoopPoints[pi].LocalWorkflowCoordinates);
            }
        }
        protected override void OnCollisionUpdated()
        {
            UpdateMarkers();
        }
        protected override void UpdatePlacement()
        {
            PolygonCollider.GetPath(0, colldierPoints);
            IsGoodPlacement = true;
            foreach (PolygonBlueprintGraphic polygon in lastCatchedPolygons)
            {
                if (polygon is not BlueprintRoom room) continue;
                room.PolygonCollider.GetPath(0, room.colldierPoints);
                int totalPointsCount = colldierPoints.Count;
                int nonCollidedPointsFound = 0;
                int allowedPlacementPointsCount = CustomMath.Multiply(totalPointsCount, 20);
                bool isPlacementAllowed = false;
                for (int j = 0; j < totalPointsCount; ++j)
                {
                    if (CustomMath.IsPointInsidePolygonAlter(room.colldierPoints, colldierPoints[j])) continue;
                    nonCollidedPointsFound++;
                    if (nonCollidedPointsFound >= allowedPlacementPointsCount)
                    {
                        isPlacementAllowed = true;
                        break;
                    }
                }
                if (!isPlacementAllowed)
                {
                    IsGoodPlacement = false;
                    break;
                }
            }
        }
        public override void DisableObject()
        {
            base.DisableObject();
            marker = null;
            additionalMarker = null;
            UpdateMarkersUI();
        }

        public void GetRoomTypes(out BuildingRoom type1, out BuildingRoom type2)
        {
            type1 = BuildingRoom.Unknown;
            type2 = BuildingRoom.Unknown;
            if (marker != null)
            {
                type1 = marker.Marker.RoomType;
            }
            if (additionalMarker != null)
            {
                type2 = additionalMarker.Marker.RoomType;
            }
        }
        /// <summary>
        /// Invokes UI update
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        public bool TryRemoveMarker(BlueprintRoomMarkerPlacer marker)
        {
            if (this.marker == marker)
            {
                RemoveBaseMarker();
                this.marker = this.additionalMarker;
                this.additionalMarker = null;
                UpdateMarkersUI();
                return true;
            }
            if (this.additionalMarker == marker)
            {
                RemoveAdditionalMarker();
                UpdateMarkersUI();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Doesn't invokes UI update
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        private bool TrySetMarker(BlueprintRoomMarkerPlacer marker)
        {
            if (this.marker == null)
            {
                SetBaseMarker(marker);
                if (this.marker == additionalMarker)
                {
                    RemoveAdditionalMarker();
                }
                if (this.additionalMarker != null)
                {
                    if (this.marker.Marker.RoomType == additionalMarker.Marker.RoomType)
                    {
                        RemoveAdditionalMarker();
                    }
                }
                return true;
            }
            if (this.additionalMarker == null)
            {
                SetAdditionalMarker(marker);
                if (this.marker == additionalMarker || this.marker.Marker.RoomType == additionalMarker.Marker.RoomType)
                {
                    RemoveAdditionalMarker();
                }
                return true;
            }
            return false;
        }
        private void SetBaseMarker(BlueprintRoomMarkerPlacer marker)
        {
            RemoveBaseMarker();
            this.marker = marker;
            this.marker.SetAsBaseRoom(this);
        }
        private void SetAdditionalMarker(BlueprintRoomMarkerPlacer marker)
        {
            RemoveAdditionalMarker();
            this.additionalMarker = marker;
            this.additionalMarker.SetAsBaseRoom(this);
        }
        private void RemoveAdditionalMarker()
        {
            if (additionalMarker == null) return;
            additionalMarker.RemoveBaseRoom(this);
            additionalMarker = null;
        }
        private void RemoveBaseMarker()
        {
            if (marker == null) return;
            marker.RemoveBaseRoom(this);
            marker = null;
        }
        public void UpdateMarkers()
        {
            FindMarkers();
            UpdateMarkersUI();
        }

        private void FindMarkers()
        {
            marker = null;
            additionalMarker = null;
            BlueprintEditorValidator.GetCollidedRoomMarkers(PolygonCollider, out List<BlueprintRoomMarkerPlacer> collidedMarkers);
            foreach (BlueprintRoomMarkerPlacer marker in collidedMarkers)
            {
                if (!marker.IsGoodPlacement) continue;
                if (!CustomMath.IsPointInsidePolygonAlter(localTexturePoints, marker.BlueprintGraphic.LocalCenter)) continue;
                if (TrySetMarker(marker)) continue;
                break;
            }
        }
        protected override void GenerateUI()
        {
            base.GenerateUI();
            RectTransform workflow = BlueprintEditor.Instance.Creator.ElementsParent;
            area = BlueprintEditor.Instance.Rooms.CalculateRoomArea(info);
            UICenter.transform.position = workflow.TransformPoint(CustomMath.GetCentroid(localTexturePoints));
            sizeText.text = $"{area:F2} m2";
        }

        private void UpdateMarkersUI()
        {
            if (marker == null)
            {
                roomNameText.text = $"{BuildingRoom.Unknown.ToLanguage()}";
                return;
            }
            string roomName = $"{marker.Marker.RoomType.ToLanguage()}";
            if (additionalMarker != null && additionalMarker.Marker.RoomType != BuildingRoom.Unknown)
                roomName += $"/{additionalMarker.Marker.RoomType.ToLanguage()}";
            roomNameText.text = roomName;
        }

        #endregion methods
    }
}