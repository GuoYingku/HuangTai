using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Utility
{
    public static class Utility
    {
        #region Extension Functions
        #region transform position
        public static void ChangeLocalPosition(this Transform transform, float dx, float dy, float dz)
        {
            transform.localPosition += new Vector3(dx, dy, dz);
        }
        public static void ChangeWorldPosition(this Transform transform, float dx, float dy, float dz)
        {
            transform.position += new Vector3(dx, dy, dz);
        }
        public static void ChangeLocalPosition(this Transform transform, Vector3 offset)
        {
            transform.localPosition += offset;
        }
        public static void ChangeWorldPosition(this Transform transform, Vector3 offset)
        {
            transform.position += offset;
        }
        #endregion

        public static T LoadAssetAtAddress<T>(this string addressablePath) where T : class
        {
            try
            {
                T res = Addressables.LoadAssetAsync<T>(addressablePath).WaitForCompletion();
                return res;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return null;
        }

        public static Ray MouseToRay(this Camera cam)
        {
            return cam.ScreenPointToRay(Input.mousePosition);
        }

        public static Vector3 CorrectDir(this Vector3 dir)
        {
            if (dir.x > 0.99f)
                return Vector3.right;
            if (dir.x < -0.99f)
                return Vector3.left;
            if (dir.y > 0.99f)
                return Vector3.up;
            if (dir.y < -0.99f)
                return Vector3.down;
            if (dir.z > 0.99f)
                return Vector3.forward;
            if (dir.z < -0.99f)
                return Vector3.back;
            return Vector3.zero;
        }

        public static void RandomShuffle<T>(this List<T> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                int r = UnityEngine.Random.Range(i, count);
                T temp = list[i];
                list[i] = list[r];
                list[r] = temp;
            }
        }

        public static T FindComponent<T>(this Transform transform, string name) where T : MonoBehaviour
        {
            return transform.Find(name)?.GetComponent<T>();
        }
        #endregion
        #region Reflection
        public static List<Type> GetAllSubclasses(Type t, bool includeSelf = false)
        {
            if (t.IsGenericType)
                return GetAllSubclassesGeneric(t, includeSelf);
            List<Type> types = new List<Type>();
            types.AddRange(AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(
                p =>
                t.IsAssignableFrom(p)
                ));
            if (!includeSelf)
                types.Remove(t);
            return types;
        }
        public static List<Type> GetAllConcreteSubclasses(Type t, bool includeSelf = false)
        {
            if (t.IsGenericType)
                return GetAllConcreteSubclassesGeneric(t, includeSelf);
            List<Type> types = new List<Type>();
            types.AddRange(AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(
                p =>
                t.IsAssignableFrom(p) &&
                !p.IsAbstract &&
                !p.IsInterface
                ));
            if (!includeSelf)
                types.Remove(t);
            return types;
        }
        public static List<Type> GetAllSubclassesGeneric(Type t, bool includeSelf = false)
        {
            List<Type> types = new List<Type>();
            types.AddRange(AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(
                p =>
                p.IsGenericType &&
                p.GetGenericTypeDefinition() == t
                ));
            if (!includeSelf)
                types.Remove(t);
            return types;
        }
        public static List<Type> GetAllConcreteSubclassesGeneric(Type t, bool includeSelf = false)
        {
            List<Type> types = new List<Type>();
            types.AddRange(AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(
                p =>
                p.IsGenericType &&
                p.GetGenericTypeDefinition() == t &&
                !p.IsAbstract &&
                !p.IsInterface
                ));
            if (!includeSelf)
                types.Remove(t);
            return types;
        }
        public static List<UnityEngine.Object> GetAllOfType(Type t)
        {
            List<UnityEngine.Object> res = new List<UnityEngine.Object>();
            var allObjects = Resources.FindObjectsOfTypeAll(t);
            foreach (var obj in allObjects)
#if UNITY_EDITOR
                if (!UnityEditor.EditorUtility.IsPersistent(obj))
#endif
                {
                    res.Add(obj);
                }
            return res;
        }
        #endregion
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}