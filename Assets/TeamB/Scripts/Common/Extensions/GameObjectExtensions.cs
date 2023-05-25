using UnityEngine;

namespace Extensions
{
    public static class GameObjectExtensions
    {
        public static T CreateGameObjectWithComponent<T>(Transform parent = null) where T : Component
        {
            return CreateGameObjectWithComponent<T>(typeof(T).Name, parent);
        }

        public static T CreateGameObjectWithComponent<T>(string componentName, Transform parent) where T : Component
        {
            var gameObjectWithComponent = new GameObject(componentName, typeof(T)).GetComponent<T>();
            gameObjectWithComponent.transform.SetParent(parent);
            gameObjectWithComponent.transform.localPosition = Vector3.zero;
            gameObjectWithComponent.transform.localRotation = Quaternion.identity;
            return gameObjectWithComponent;
        }

        public static T AddComponentIfNotPresent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component != null) return component;
            return gameObject.AddComponent<T>();
        }

        public static void RemoveCloneFromName(this GameObject gameObject)
        {
            gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
        }
    }
}