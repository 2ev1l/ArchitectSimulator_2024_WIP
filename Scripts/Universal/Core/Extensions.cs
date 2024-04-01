using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Universal.Core
{
    [System.Serializable]
    public static class Extensions
    {
        #region fields & properties
        public const float EPSILON = 0.001f;
        #endregion fields & properties

        #region methods
        #region IEnumerable
        /// <summary>
        /// >O(n) <br></br>
        /// Out first found element <br></br>
        /// Creates many garbage
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <param name="match"></param>
        /// <param name="exist">Found element or null</param>
        /// <returns>True if any element has match</returns>
        public static bool Exists<TSource>(this IEnumerable<TSource> collection, System.Predicate<TSource> match, out TSource exist)
        {
            exist = default;
            foreach (TSource el in collection)
            {
                if (match.Invoke(el))
                {
                    exist = el;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// >O(n) <br></br>
        /// Out first found element <br></br>
        /// Creates less garbage than default Exists method
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <param name="match"></param>
        /// <param name="exist">Found element or null</param>
        /// <returns>True if any element has match</returns>
        public static bool Exists<TSource>(this HashSet<TSource> collection, System.Predicate<TSource> match, out TSource exist)
        {
            exist = default;
            foreach (TSource el in collection)
            {
                if (match.Invoke(el))
                {
                    exist = el;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Slightly faster than original <see cref="List{T}.Contains(T)"/> (+20%)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool ContainsAlt<TSource>(this List<TSource> collection, TSource element) where TSource : class
        {
            int collectionCount = collection.Count;
            for (int i = 0; i < collectionCount; ++i)
            {
                if (collection[i] == element) return true;
            }
            return false;
        }

        /// <summary>
        /// O(n) <br></br>
        /// If no one in range, returns default
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <param name="getValue"></param>
        /// <param name="nearestValue"></param>
        /// <returns></returns>
        public static TSource NearestBottom<TSource>(this IEnumerable<TSource> collection, System.Func<TSource, float> getValue, float nearestValue)
        {
            TSource minSource = default;
            float minDifference = float.MaxValue;
            foreach (var el in collection)
            {
                float elValue = getValue.Invoke(el);
                if (elValue > nearestValue) continue;
                float difference = Mathf.Abs(nearestValue - elValue);
                if (difference > minDifference) continue;
                minDifference = difference;
                minSource = el;
            }
            return minSource;
        }
        /// <summary>
        /// O(n) <br></br>
        /// If no one in range, returns default
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <param name="getValue"></param>
        /// <param name="nearestValue"></param>
        /// <returns></returns>
        public static TSource NearestTop<TSource>(this IEnumerable<TSource> collection, System.Func<TSource, float> getValue, float nearestValue)
        {
            TSource minSource = default;
            float minDifference = float.MaxValue;
            foreach (var el in collection)
            {
                float elValue = getValue.Invoke(el);
                if (elValue < nearestValue) continue;
                float difference = Mathf.Abs(nearestValue - elValue);
                if (difference > minDifference) continue;
                minDifference = difference;
                minSource = el;
            }
            return minSource;
        }
        /// <summary>
        /// O(n^2) <br></br>
        /// Actually there's no difference between <see cref="FindEquals{TSource}(IEnumerable{TSource}, Func{TSource, TSource, bool})"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static HashSet<TSource> FindSame<TSource>(this IEnumerable<TSource> collection, System.Func<TSource, TSource, bool> match) where TSource : class
        {
            HashSet<TSource> result = new();
            foreach (TSource el in collection)
            {
                foreach (TSource el2 in collection)
                {
                    if (match.Invoke(el, el2) && el != el2)
                    {
                        result.Add(el2);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// O(n^2)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static HashSet<TSource> FindEquals<TSource>(this IEnumerable<TSource> collection, System.Func<TSource, TSource, bool> match)
        {
            HashSet<TSource> result = new();
            int baseIndex = -1;
            int inheritIndex = -1;
            foreach (TSource el in collection)
            {
                inheritIndex = -1;
                baseIndex++;
                foreach (TSource el2 in collection)
                {
                    inheritIndex++;
                    if (baseIndex <= inheritIndex) continue;
                    if (match.Invoke(el, el2))
                    {
                        result.Add(el);
                        result.Add(el2);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// O(n^2) <br></br>
        /// Actually there's no difference between <see cref="ExistsEquals{TSource}(IEnumerable{TSource}, Func{TSource, TSource, bool})"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool ExistsSame<TSource>(this IEnumerable<TSource> collection, System.Func<TSource, TSource, bool> match) where TSource : class
        {
            foreach (TSource el in collection)
            {
                foreach (TSource el2 in collection)
                {
                    if (match.Invoke(el, el2) && el != el2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// O(n^2)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool ExistsEquals<TSource>(this IEnumerable<TSource> collection, System.Func<TSource, TSource, bool> match)
        {
            int baseIndex = -1;
            int inheritIndex = -1;
            foreach (TSource el in collection)
            {
                inheritIndex = -1;
                baseIndex++;
                foreach (TSource el2 in collection)
                {
                    inheritIndex++;
                    if (baseIndex <= inheritIndex) continue;
                    if (match.Invoke(el, el2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// O(n) <br></br>
        /// New hashset will be cleared and replaced entire with original elements. <br></br>
        /// Creates no garbage. <br></br>
        /// Probably that may be used when you are trying to save insularity for object that may be used many times.
        /// </summary>
        public static void SetElementsTo<TSource>(this HashSet<TSource> originalHashSet, HashSet<TSource> newHashSet)
        {
            newHashSet.Clear();
            int originalCount = originalHashSet.Count;
            if (newHashSet.Count < originalCount)
                newHashSet.EnsureCapacity(originalCount);
            foreach (TSource el in originalHashSet)
            {
                newHashSet.Add(el);
            }
        }
        /// <summary>
        /// O(n) <br></br>
        /// New list will be cleared and replaced entire with original elements. <br></br>
        /// Creates no garbage. <br></br>
        /// </summary>
        public static void SetElementsTo<TSource>(this HashSet<TSource> originalHashSet, List<TSource> newList)
        {
            newList.Clear();
            newList.Capacity = originalHashSet.Count;
            foreach (TSource el in originalHashSet)
            {
                newList.Add(el);
            }
        }
        /// <summary>
        /// O(n) <br></br>
        /// New list will be cleared and replaced entire with original elements. <br></br>
        /// Creates no garbage. <br></br>
        /// </summary>
        public static void SetElementsTo<TSource>(this TSource[] originalList, List<TSource> newList)
        {
            newList.Clear();
            int listCount = originalList.Length;
            if (newList.Capacity < listCount)
                newList.Capacity = listCount;
            for (int i = 0; i < listCount; ++i)
            {
                newList.Add(originalList[i]);
            }
        }

        /// <summary>
        /// O(n) <br></br>
        /// New list will be cleared and replaced entire with original elements. <br></br>
        /// Creates no garbage. <br></br>
        /// </summary>
        public static void SetElementsTo<TSource>(this List<TSource> originalList, List<TSource> newList)
        {
            newList.Clear();
            int listCount = originalList.Count;
            if (newList.Capacity < listCount)
                newList.Capacity = listCount;
            for (int i = 0; i < listCount; ++i)
            {
                newList.Add(originalList[i]);
            }
        }

        /// <summary>
        /// O(n) <br></br>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="originalData"></param>
        /// <param name="comparable"></param>
        /// <returns>True if all elements are same</returns>
        public static bool ElementsSameReversed<TSource>(this List<TSource> originalData, List<TSource> comparable) where TSource : class
        {
            if (originalData.Count != comparable.Count) return false;
            return ElementsSameOfPartReversed(originalData, comparable);
        }
        /// <summary>
        /// O(n) <br></br>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="originalData"></param>
        /// <param name="comparable"></param>
        /// <returns>True if all elements are same</returns>
        public static bool ElementsSameOfPartReversed<TSource>(this List<TSource> originalData, List<TSource> comparablePart) where TSource : class
        {
            int totalCount = comparablePart.Count;
            for (int i = 0; i < totalCount; ++i)
            {
                if (comparablePart[totalCount - i - 1] != originalData[i]) return false;
            }
            return true;
        }
        /// <summary>
        /// O(n) <br></br>
        /// </summary>
        /// <returns>True if all elements are same</returns>
        public static bool ElementsSame<TSource>(this List<TSource> originalData, List<TSource> comparable) where TSource : class
        {
            if (originalData.Count != comparable.Count) return false;
            return ElementsSameOfPart(originalData, comparable);
        }
        /// <summary>
        /// O(n) <br></br>
        /// </summary>
        /// <returns>True if all elements are same</returns>
        public static bool ElementsSameOfPart<TSource>(this List<TSource> originalData, List<TSource> comparablePart) where TSource : class
        {
            int totalCount = comparablePart.Count;
            for (int i = 0; i < totalCount; ++i)
            {
                if (comparablePart[i] != originalData[i]) return false;
            }
            return true;
        }
        /// <summary>
        /// O(n) <br></br>
        /// Inverses original list <br></br>
        /// Temp list will be cleared and replaced entire with original elements. <br></br>
        /// </summary>
        public static void Reverse<TSource>(this List<TSource> originalData)
        {
            int listCount = originalData.Count;
            int listCountHalved = listCount / 2;
            for (int i = 0; i < listCountHalved; ++i)
            {
                (originalData[i], originalData[listCount - i - 1]) = (originalData[listCount - i - 1], originalData[i]);
            }
        }
        /// <summary>
        /// O(n) <br></br>
        /// New hashset will be cleared and replaced entire with original elements. <br></br>
        /// Creates no garbage. <br></br>
        /// Probably that may be used when you are trying to save insularity for object that may be used many times.
        /// </summary>
        public static void SetElementsTo<TSource>(this Stack<TSource> originalData, HashSet<TSource> newHashSet)
        {
            newHashSet.Clear();
            foreach (TSource el in originalData)
            {
                newHashSet.Add(el);
            }
        }
        /// <summary>
        /// O(n) <br></br>
        /// Inverses original stack <br></br>
        /// Temp stack will be cleared and replaced entire with original elements. <br></br>
        /// </summary>
        public static void Reverse<TSource>(this Stack<TSource> originalData, List<TSource> tempList)
        {
            tempList.Clear();
            foreach (TSource el in originalData)
            {
                tempList.Add(el);
            }
            originalData.Clear();
            int listCount = tempList.Count;
            for (int i = 0; i < tempList.Count; ++i)
            {
                originalData.Push(tempList[i]);
            }
        }
        /// <summary>
        /// O(2*n) <br></br>
        /// New stack will be cleared and replaced entire with original elements. <br></br>
        /// Preserve order
        /// </summary>
        public static void SetElementsToOrdered<TSource>(this Stack<TSource> originalData, Stack<TSource> newData, List<TSource> tempList)
        {
            newData.Clear();
            originalData.SetElementsTo(newData);
            newData.Reverse(tempList);
        }
        /// <summary>
        /// O(n) <br></br>
        /// New stack will be cleared and replaced entire with original elements. <br></br>
        /// There's simple foreach loop, so order will be reversed
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="originalData"></param>
        /// <param name="newData"></param>
        public static void SetElementsTo<TSource>(this Stack<TSource> originalData, Stack<TSource> newData)
        {
            newData.Clear();
            foreach (TSource el in originalData)
            {
                newData.Push(el);
            }
        }
        /// <summary>
        /// O(n) <br></br>
        /// New list will be cleared and replaced entire with original elements. <br></br>
        /// There's simple foreach loop, so order will be reversed
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="originalData"></param>
        /// <param name="newData"></param>
        public static void SetElementsTo<TSource>(this Stack<TSource> originalData, List<TSource> newData)
        {
            newData.Clear();
            foreach (TSource el in originalData)
            {
                newData.Add(el);
            }
        }
        #endregion IEnumerable

        #region Math
        public static bool Approximately(this float a, float b, float epsilon = EPSILON) => Mathf.Abs(a - b) < epsilon;
        /// <summary>
        /// Optimized
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool Approximately(this Vector2 a, Vector2 b, float epsilon = EPSILON)
        {
            float ax = a.x - b.x;
            float ay = a.y - b.y;
            return ax * ax + ay * ay < epsilon;
        }
        /// <summary>
        /// Non optimized (see <see cref="Approximately(Vector2, Vector2, float)"/>)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool Approximately(this Vector3 a, Vector3 b, float epsilon = EPSILON) => Vector3.SqrMagnitude(a - b) < epsilon;
        /// <summary>
        /// Non optimized (see <see cref="Approximately(Vector2, Vector2, float)"/>)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool Approximately(this Vector4 a, Vector4 b, float epsilon = EPSILON) => Vector4.SqrMagnitude(a - b) < epsilon;

        public static float Max(this Vector2 vector2) => Mathf.Max(vector2.x, vector2.y);
        public static float Min(this Vector2 vector2) => Mathf.Min(vector2.x, vector2.y);
        public static int Max(this Vector2Int vector2) => Mathf.Max(vector2.x, vector2.y);
        public static int Min(this Vector2Int vector2) => Mathf.Min(vector2.x, vector2.y);
        public static float Max(this Vector3 vector3) => Mathf.Max(vector3.x, vector3.y, vector3.z);
        public static float Min(this Vector3 vector3) => Mathf.Min(vector3.x, vector3.y, vector3.z);

        public static Vector3 Abs(this Vector3 vector3) => new(Mathf.Abs(vector3.x), Mathf.Abs(vector3.y), Mathf.Abs(vector3.z));
        public static Vector2 Abs(this Vector2 vector2) => new(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y));
        public static float ClampPosition(this Vector2 vector2, float position) => Mathf.Clamp(position, vector2.Min(), vector2.Max());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="max"></param>
        /// <returns>Vector with heighest components</returns>
        public static Vector3 MaxClamp(this Vector3 vector3, Vector3 max) => new(Mathf.Max(vector3.x, max.x), Mathf.Max(vector3.y, max.y), Mathf.Max(vector3.z, max.z));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="min"></param>
        /// <returns>Vector with smallest components</returns>
        public static Vector3 MinClamp(this Vector3 vector3, Vector3 min) => new(Mathf.Min(vector3.x, min.x), Mathf.Min(vector3.y, min.y), Mathf.Min(vector3.z, min.z));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>Vector with components clamped in range</returns>
        public static Vector3 Clamp(this Vector3 vector3, Vector3 min, Vector3 max)
        {
            return new(Mathf.Clamp(vector3.x, min.x, max.x), Mathf.Clamp(vector3.y, min.y, max.y), Mathf.Clamp(vector3.z, min.z, max.z));
        }
        #endregion Math

        #region Enum
        private static readonly Dictionary<Type, System.Array> enumsValues = new();
        /// <summary>
        /// Optimized with <see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="num"></param>
        /// <returns></returns>
        public static System.Array GetEnumValues<T>(this T num) where T : Enum
        {
            Type type = num.GetType();
            if (enumsValues.TryGetValue(type, out Array value)) return value;
            value = Enum.GetValues(type);
            enumsValues.Add(type, value);
            return value;
        }

        /// <summary>
        /// Iterates over each flag value <br></br>
        /// Creates many garbage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="num"></param>
        /// <param name="action"></param>
        public static void ForEachFlag<T>(this T num, System.Action<T> action) where T : Enum
        {
            foreach (T value in GetEnumValues(num))
            {
                if (num.HasFlag(value))
                {
                    action?.Invoke(value);
                }
            }
        }
        /// <summary>
        /// Creates new list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="num"></param>
        /// <returns></returns>
        public static List<T> ToFlagList<T>(this T num) where T : Enum
        {
            List<T> result = new();
            foreach (T value in GetEnumValues(num))
            {
                if (num.HasFlag(value))
                {
                    result.Add(value);
                }
            }
            return result;
        }
        /// <summary>
        /// Sets result to enumValues
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="num"></param>
        /// <param name="enumValues"></param>
        /// <returns></returns>
        public static void ToFlagList<T>(this T num, List<T> enumValues) where T : Enum
        {
            enumValues.Clear();
            foreach (T value in GetEnumValues(num))
            {
                if (num.HasFlag(value))
                {
                    enumValues.Add(value);
                }
            }
        }
        public static bool ContainLayer(this LayerMask layerMask, int layer)
        {
            return (layerMask & 1 << layer) != 0;
        }
        public static LayerMask AddLayer(this LayerMask layerMask, int layer)
        {
            return layerMask |= (1 << layer);
        }
        public static LayerMask RemoveLayer(this LayerMask layerMask, int layer)
        {
            return layerMask &= ~(1 << layer);
        }
        #endregion Enum

        #region Geometry
        public static bool PointOnLine2D(this Vector2 point, Vector2 start, Vector2 end, float epsilon = EPSILON)
        {
            // ensure points are collinear
            var zero = (end.x - start.x) * (point.y - start.y) - (point.x - start.x) * (end.y - start.y);
            if (zero > epsilon || zero < -epsilon) return false;

            // check if x-coordinates are not equal
            if (start.x - end.x > epsilon || end.x - start.x > epsilon)
                // ensure x is between a.x & b.x (use tolerance)
                return start.x > end.x
                    ? point.x + epsilon > end.x && point.x - epsilon < start.x
                    : point.x + epsilon > start.x && point.x - epsilon < end.x;

            // ensure y is between a.y & b.y (use tolerance)
            return start.y > end.y
                ? point.y + epsilon > end.y && point.y - epsilon < start.y
                : point.y + epsilon > start.y && point.y - epsilon < end.y;
        }

        public static bool ContainsAny(this RectTransform rt, Vector2[] localPoints)
        {
            for (int i = 0; i < localPoints.Length; ++i)
            {
                if (rt.Contains(localPoints[i]))
                    return true;
            }
            return false;
        }
        public static bool ContainsAll(this RectTransform rt, Vector2[] localPoints)
        {
            for (int i = 0; i < localPoints.Length; ++i)
            {
                if (!rt.Contains(localPoints[i]))
                    return false;
            }
            return true;
        }
        public static bool ContainsAll(this RectTransform rt, IEnumerable<Vector2> localPoints)
        {
            foreach (Vector2 el in localPoints)
            {
                if (!rt.Contains(el))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="localPoint"></param>
        /// <returns>True if point point inside the rect transform</returns>
        public static bool Contains(this RectTransform rt, Vector2 localPoint)
        {
            Rect rect = rt.rect;
            Vector2 anchoredPosition = rt.anchoredPosition;

            return (localPoint.x >= anchoredPosition.x - rect.width / 2 &&
                    localPoint.x <= anchoredPosition.x + rect.width / 2 &&
                    localPoint.y >= anchoredPosition.y - rect.height / 2 &&
                    localPoint.y <= anchoredPosition.y + rect.height / 2);
        }
        #endregion Geometry

        #endregion methods
    }
}