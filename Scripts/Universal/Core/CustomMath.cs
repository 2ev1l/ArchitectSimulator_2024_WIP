using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Universal.Core
{
    public static class CustomMath
    {
        #region fields & properties
        #endregion fields & properties

        #region methods
        public static IEnumerator WaitAFrame()
        {
            yield return null;
        }

        #region Geometry
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Result with math quarters 1 => (1, 1); 2 => (-1, 1); 3 => (-1, -1); 4 => (1, -1)</returns>
        public static Vector2 GetScreenSquare(Vector3 cameraPosition, Vector3 currentPosition) => (currentPosition - cameraPosition) switch
        {
            Vector3 vec when vec.x >= 0 && vec.y >= 0 => Vector2.right + Vector2.up,
            Vector3 vec when vec.x <= 0 && vec.y >= 0 => -Vector2.right + Vector2.up,
            Vector3 vec when vec.x <= 0 && vec.y <= 0 => -Vector2.right - Vector2.up,
            Vector3 vec when vec.x >= 0 && vec.y <= 0 => Vector2.right - Vector2.up,
            _ => -Vector2.right - Vector2.up
        };
        public static Vector3 Project(Vector3 direction, Vector3 surface) => direction - Vector3.Dot(direction, surface) * surface;
        public static bool AreLinesIntersecting(Vector2 l1_p1, Vector2 l1_p2, Vector2 l2_p1, Vector2 l2_p2, bool shouldIncludeEndPoints)
        {
            //To avoid floating point precision issues we can add a small value
            float epsilon = 0.0001f;

            bool isIntersecting = false;

            float denominator = (l2_p2.y - l2_p1.y) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.y - l1_p1.y);

            //Make sure the denominator is > 0, if not the lines are parallel
            if (denominator != 0f)
            {
                float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.y - l2_p1.y) - (l2_p2.y - l2_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;
                float u_b = ((l1_p2.x - l1_p1.x) * (l1_p1.y - l2_p1.y) - (l1_p2.y - l1_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;

                //Are the line segments intersecting if the end points are the same
                if (shouldIncludeEndPoints)
                {
                    //Is intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                    if (u_a >= 0f + epsilon && u_a <= 1f - epsilon && u_b >= 0f + epsilon && u_b <= 1f - epsilon)
                    {
                        isIntersecting = true;
                    }
                }
                else
                {
                    //Is intersecting if u_a and u_b are between 0 and 1
                    if (u_a > 0f + epsilon && u_a < 1f - epsilon && u_b > 0f + epsilon && u_b < 1f - epsilon)
                    {
                        isIntersecting = true;
                    }
                }
            }

            return isIntersecting;
        }

        /// <summary>
        /// Method to compute the centroid of a polygon. This does NOT work for a complex polygon. <br></br>
        /// https://stackoverflow.com/questions/9815699/how-to-calculate-centroid
        /// </summary>
        /// <param name="points">points that define the polygon</param>
        /// <returns>Centroid point, or Vector2.zero if something wrong</returns>
        public static Vector2 GetCentroid(List<Vector2> points)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;
            int pointsCount = points.Count;
            for (int i = 0, j = pointsCount - 1; i < pointsCount; j = i++)
            {
                Vector2 pointsJ = points[j];
                Vector2 pointsI = points[i];
                float temp = pointsI.x * pointsJ.y - pointsJ.x * pointsI.y;
                accumulatedArea += temp;
                centerX += (pointsI.x + pointsJ.x) * temp;
                centerY += (pointsI.y + pointsJ.y) * temp;
            }

            if (Math.Abs(accumulatedArea) < 1E-7f)
                return Vector3.zero;  // Avoid division by zero

            accumulatedArea *= 3f;
            return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
        }
        public static float CalculateArea(List<Vector2> points)
        {
            float area = 0;
            int pointsCountMinus1 = points.Count - 1;
            if (pointsCountMinus1 < 0) return 0;

            Vector2 currentPointCoord = points[0];
            for (int i = 0; i < pointsCountMinus1; ++i)
            {
                Vector2 nextPointCoord = points[i + 1];
                area += (nextPointCoord.x - currentPointCoord.x) *
                        (nextPointCoord.y + currentPointCoord.y);
                currentPointCoord = nextPointCoord;
            }
            return Mathf.Abs(area / 2f);
        }

        /// <summary>
        /// Determines if the given point is inside the polygon without corners/lines<br></br>
        /// <see cref="https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon"/>
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="point">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public static bool IsPointInsidePolygon(List<Vector2> polygon, Vector2 point, float precision)
        {
            bool result = false;
            int polygonCount = polygon.Count;
            Vector2 lastCoord = polygon[polygonCount - 1];
            for (int i = 0; i < polygonCount; ++i)
            {
                Vector2 curentCoord = polygon[i];
                if (point.PointOnLine2D(lastCoord, curentCoord, precision))
                {
                    return false;
                }
                if (curentCoord.y < point.y && lastCoord.y > point.y || lastCoord.y < point.y && curentCoord.y > point.y)
                {
                    if (curentCoord.x + (point.y - curentCoord.y) / (lastCoord.y - curentCoord.y) * (lastCoord.x - curentCoord.x) < point.x)
                    {
                        result = !result;
                    }
                }
                lastCoord = curentCoord;
            }
            return result;
        }
        /// <summary>
        /// Determines if the given point is inside the polygon without corners with integer values<br></br>
        /// <see cref="https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon"/>
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="point">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public static bool IsPointInsidePolygonInt(List<Vector2> polygon, Vector2 point)
        {
            bool result = false;
            int polygonCount = polygon.Count;
            int j = polygonCount - 1;
            int pointX = Mathf.RoundToInt(point.x);
            int pointY = Mathf.RoundToInt(point.y);
            for (int i = 0; i < polygonCount; ++i)
            {
                Vector2 curentCoord = polygon[i];
                Vector2 lastCoord = polygon[j];
                int curentCoordX = Mathf.RoundToInt(curentCoord.x);
                int curentCoordY = Mathf.RoundToInt(curentCoord.y);
                int lastCoordX = Mathf.RoundToInt(lastCoord.x);
                int lastCoordY = Mathf.RoundToInt(lastCoord.y);
                if (curentCoordY < pointY && lastCoordY > pointY ||
                    lastCoordY < pointY && curentCoordY > pointY)
                {
                    if (curentCoord.x + (pointY - curentCoordY) /
                       (lastCoordY - curentCoordY) *
                       (lastCoordX - curentCoordX) < pointX)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        /// <summary>
        /// Alternative method for determine if the given point is inside the polygon <br></br>
        /// <see cref="https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon"/>
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns>True if the point is inside the polygon</returns>
        public static bool IsPointInsidePolygonAlter(List<Vector2> polygon, Vector2 point, float epsilon = 0f)
        {
            bool result = false;
            int polygonCount = polygon.Count;
            var a = polygon[polygonCount - 1];
            float pointX = point.x;
            float pointY = point.y;
            for (int i = 0; i < polygonCount; ++i)
            {
                Vector2 b = polygon[i];

                if ((b.x.Approximately(pointX, epsilon)) && (b.y.Approximately(pointY, epsilon)))
                    return true;

                if ((b.y.Approximately(a.y, epsilon)) && (pointY.Approximately(a.y, epsilon)))
                {
                    if ((a.x <= pointX) && (pointX <= b.x))
                        return true;

                    if ((b.x <= pointX) && (pointX <= a.x))
                        return true;
                }

                if ((b.y < pointY) && (a.y >= pointY) || (a.y < pointY) && (b.y >= pointY))
                {
                    if (b.x + (pointY - b.y) / (a.y - b.y) * (a.x - b.x) <= pointX)
                        result = !result;
                }
                a = b;
            }
            return result;
        }
        /// <summary>
        /// Alternative method for determine if the given point is inside the polygon <br></br>
        /// <see cref="https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon"/>
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns>True if the point is inside the polygon</returns>
        public static bool IsPointInsidePolygonAlter(Vector2[] polygon, Vector2 point, float epsilon = 0f)
        {
            bool result = false;
            int polygonCount = polygon.Length;
            var a = polygon[polygonCount - 1];
            float pointX = point.x;
            float pointY = point.y;
            for (int i = 0; i < polygonCount; ++i)
            {
                Vector2 b = polygon[i];

                if ((b.x.Approximately(pointX, epsilon)) && (b.y.Approximately(pointY, epsilon)))
                    return true;

                if ((b.y.Approximately(a.y, epsilon)) && (pointY.Approximately(a.y, epsilon)))
                {
                    if ((a.x <= pointX) && (pointX <= b.x))
                        return true;

                    if ((b.x <= pointX) && (pointX <= a.x))
                        return true;
                }

                if ((b.y < pointY) && (a.y >= pointY) || (a.y < pointY) && (b.y >= pointY))
                {
                    if (b.x + (pointY - b.y) / (a.y - b.y) * (a.x - b.x) <= pointX)
                        result = !result;
                }
                a = b;
            }
            return result;
        }
        /// <summary>
        /// Alternative method for determine if the given point is inside the polygon with integer coordinates<br></br>
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns>True if the point is inside the polygon</returns>
        public static bool IsPointInsidePolygonAlterInt(List<Vector2> polygon, Vector2 point, float epsilon = 1f)
        {
            bool result = false;
            int polygonCount = polygon.Count;
            if (polygonCount == 0) return false;
            Vector2 a = polygon[polygonCount - 1];
            int pointX = Mathf.RoundToInt(point.x);
            int pointY = Mathf.RoundToInt(point.y);
            for (int i = 0; i < polygonCount; ++i)
            {
                Vector2 b = polygon[i];
                int bX = Mathf.RoundToInt(b.x);
                int bY = Mathf.RoundToInt(b.y);
                int aX = Mathf.RoundToInt(a.x);
                int aY = Mathf.RoundToInt(a.y);

                if ((bX == pointX) && (bY == pointY))
                    return true;

                if ((bY == aY) && (pointY == aY))
                {
                    if ((aX <= pointX) && (pointX <= bX))
                        return true;

                    if ((bX <= pointX) && (pointX <= aX))
                        return true;
                }
                if (point.PointOnLine2D(a, b, epsilon))
                    return true;

                if ((bY < pointY) && (aY >= pointY) || (aY < pointY) && (bY >= pointY))
                {
                    if (bX + (pointY - bY) / (aY - bY) * (aX - bX) <= pointX)
                        result = !result;
                }
                a = b;
            }
            return result;
        }
        /// <summary>
        /// Alternative method for determine if the given point is inside the polygon with integer coordinates<br></br>
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns>True if the point is inside the polygon</returns>
        public static bool IsPointInsidePolygonAlterInt(Vector2[] polygon, Vector2 point, float epsilon = 1f)
        {
            bool result = false;
            int polygonCount = polygon.Length;
            Vector2 a = polygon[polygonCount - 1];
            int pointX = Mathf.RoundToInt(point.x);
            int pointY = Mathf.RoundToInt(point.y);
            for (int i = 0; i < polygonCount; ++i)
            {
                Vector2 b = polygon[i];
                int bX = Mathf.RoundToInt(b.x);
                int bY = Mathf.RoundToInt(b.y);
                int aX = Mathf.RoundToInt(a.x);
                int aY = Mathf.RoundToInt(a.y);

                if ((bX == pointX) && (bY == pointY))
                    return true;

                if ((bY == aY) && (pointY == aY))
                {
                    if ((aX <= pointX) && (pointX <= bX))
                        return true;

                    if ((bX <= pointX) && (pointX <= aX))
                        return true;
                }
                if (point.PointOnLine2D(a, b, epsilon))
                    return true;

                if ((bY < pointY) && (aY >= pointY) || (aY < pointY) && (bY >= pointY))
                {
                    if (bX + (pointY - bY) / (aY - bY) * (aX - bX) <= pointX)
                        result = !result;
                }
                a = b;
            }
            return result;
        }
        #endregion Geometry

        public static int RoundTo(this int i, int round)
        {
            return ((int)Math.Round(i / (float)round)) * round;
        }
        /// <summary>
        /// True if chance gained.
        /// </summary>
        /// <param name="chancePercent">0..100%</param>
        /// <returns></returns>
        public static bool GetRandomChance(float chancePercent) => UnityEngine.Random.Range(0f, 100f) < chancePercent;
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Value (0..100) </returns>
        public static float GetRandomChance() => Mathf.Clamp(UnityEngine.Random.Range(0f, 100f), 0.001f, 99.999f);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chancePercent">0..100%</param>
        /// <returns>True if chance gained</returns>
        public static bool GetRandomChance(int chancePercent) => UnityEngine.Random.Range(0, 100) < chancePercent;
        public static int GetRandomFromChancesArray(float[] chances)
        {
            int finalIndex = chances.Length - 1;
            float maxChance = 100;
            for (int i = 0; i < chances.Length; i++)
            {
                float rnd = UnityEngine.Random.Range(0, maxChance);
                if (rnd <= chances[i])
                {
                    finalIndex = i;
                    return finalIndex;
                }
                else
                {
                    maxChance -= chances[i];
                }
            }
            return -1;
        }
        /// <summary>
        /// Used to optimize work with different resolutions in <see cref="Canvas"/><br></br>
        /// Actually, you shouldn't use this for a new projects. All you need is <see cref="RectTransform"/>
        /// </summary>
        /// <returns>0.5..1f</returns>
        [System.Obsolete]
        public static float GetOptimalScreenScale()
        {
            int width = Screen.currentResolution.width;
            int height = Screen.currentResolution.height;
            float scale = (width / (float)height) switch
            {
                float i when i < 1.01f => 0.5f,
                float i when i < 1.26f => 0.6f,
                float i when i < 1.46f => 0.7f,
                float i when i < 1.61f => 0.8f,
                float i when i < 1.75f => 0.9f,
                _ => 1f,
            };
            return scale;
        }
        /// <summary>
        /// Calculating rounded value with percent multiplier. e.g: 10 * 50(%) = 5
        /// </summary>
        /// <param name="value"></param>
        /// <param name="multiplier">0..any%</param>
        /// <returns></returns>
        public static int Multiply(int value, float multiplier) => Mathf.RoundToInt(value * multiplier / 100f);
        /// <summary>
        /// Calculating with percent multiplier. e.g: 10 * 50(%) = 5
        /// </summary>
        /// <param name="value"></param>
        /// <param name="multiplier">0..any%</param>
        /// <returns></returns>
        public static float Multiply(float value, float multiplier) => value * multiplier / 100f;
        /// <summary>
        /// E.g. 10 * 5 = 50, returns 40
        /// </summary>
        /// <param name="value"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static int GetMultipliedIncrease(int value, float multiplier) => Multiply(value, multiplier) - value;

        /// <summary>
        /// The same as <see cref="Mathf.Lerp"/> but simplified with vector
        /// </summary>
        /// <param name="length"></param>
        /// <param name="t">0..1f</param>
        /// <returns></returns>
        public static float LerpVector(Vector2 length, float t) => Mathf.Lerp(length.x, length.y, t);
        /// <summary>
        /// The same as <see cref="Mathf.InverseLerp"/> but simplified with vector
        /// </summary>
        /// <param name="length"></param>
        /// <param name="vectorPosition">length.x..length.y</param>
        /// <returns></returns>
        public static float InverseLerpVector(Vector2 length, float vectorPosition) => Mathf.InverseLerp(length.x, length.y, vectorPosition);
        /// <summary>
        /// Transform value from old length to new, e.g. old:Vector(4, 1), old:2, new:Vector(0, 10) = 6.66f,
        /// </summary>
        /// <param name="oldLength"></param>
        /// <param name="oldLengthValue"></param>
        /// <param name="newLength"></param>
        /// <returns></returns>
        public static float ConvertValueFromTo(Vector2 oldLength, float oldLengthValue, Vector2 newLength)
        {
            float lerpedValue = InverseLerpVector(oldLength, oldLengthValue);
            return LerpVector(newLength, lerpedValue);
        }
        public static Vector2 ConvertVectorFromTo(Vector2 oldMinMax, Vector2 scaleable, Vector2 newMinMax)
        {
            return ConvertVectorFromTo(oldMinMax, oldMinMax, scaleable, newMinMax, newMinMax);
        }
        public static Vector2 ConvertVectorFromTo(Vector2 oldMinMaxX, Vector2 oldMinMaxY, Vector2 scaleable, Vector2 newMinMaxX, Vector2 newMinMaxY)
        {
            float x = ConvertValueFromTo(oldMinMaxX, scaleable.x, newMinMaxX);
            float y = ConvertValueFromTo(oldMinMaxY, scaleable.y, newMinMaxY);
            return new(x, y);
        }

        public static bool GetLogicalResult(float value, LogicalOperation @is, float thanValue) => @is switch
        {
            LogicalOperation.Less => value < thanValue,
            LogicalOperation.Greater => value > thanValue,
            _ => throw new System.NotImplementedException($"Logical operation {@is}")
        };

        public static float ConvertToFloat(double value) => System.Convert.ToSingle(value);
        public static float ConvertToFloat(string value) => ConvertToFloat(System.Convert.ToDouble(value.Replace(".", ",")));
        #endregion methods
    }
    public enum LogicalOperation { Greater, Less };
}