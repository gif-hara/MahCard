using System;
using System.Collections.Generic;
using UnityEngine;

namespace HK
{
    /// <summary>
    /// Represents a UI document that contains a collection of UI elements.
    /// </summary>
    public class HKUIDocument : MonoBehaviour
    {
        [SerializeField]
        private Element[] elements;

        private readonly Dictionary<string, GameObject> elementMap = new();

        private readonly Dictionary<GameObject, Dictionary<Type, Component>> componentMap = new();

        /// <summary>
        /// Retrieves a component of type T from the UI element with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the component to retrieve.</typeparam>
        /// <param name="name">The name of the UI element.</param>
        /// <returns>The component of type T if found; otherwise, null.</returns>
        public T Q<T>(string name) where T : Component
        {
            var e = TryQ<T>(name);
            if (e == null)
            {
                Debug.LogError($"Component not found: {typeof(T)} on object {name}");
            }
            return e;
        }

        public T TryQ<T>(string name) where T : Component
        {
            var e = TryQ(name);
            if (e == null)
            {
                return null;
            }

            if (!componentMap.TryGetValue(e, out var c))
            {
                c = new Dictionary<Type, Component>();
                componentMap[e] = c;
            }

            if (c.TryGetValue(typeof(T), out var component))
            {
                return (T)component;
            }

            if (e.TryGetComponent<T>(out var newComponent))
            {
                c[typeof(T)] = newComponent;
                return newComponent;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the UI element with the specified name.
        /// </summary>
        /// <param name="name">The name of the UI element.</param>
        /// <returns>The UI element if found; otherwise, null.</returns>
        public GameObject Q(string name)
        {
            var result = TryQ(name);
            if (result == null)
            {
                Debug.LogError($"Element not found: {name}");
            }
            return result;
        }

        public GameObject TryQ(string name)
        {
            if (elementMap.Count == 0)
            {
                foreach (var element in elements)
                {
                    elementMap[element.Name] = element.Document;
                }
            }

            if (elementMap.TryGetValue(name, out var e))
            {
                return e;
            }

            return null;
        }

        [Serializable]
        public class Element
        {
            [SerializeField]
            private GameObject document;

            [SerializeField]
            private string overrideName;

            public GameObject Document => document;

            public string Name => string.IsNullOrEmpty(overrideName) ? document.name : overrideName;
        }
    }
}