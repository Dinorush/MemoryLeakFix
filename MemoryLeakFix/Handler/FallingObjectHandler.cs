using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryLeakFix.Handler
{
    internal sealed class FallingObjectHandler : MonoBehaviour
    {
        static FallingObjectHandler()
        {
            ClassInjector.RegisterTypeInIl2Cpp<FallingObjectHandler>();

            GameObject go = new(EntryPoint.MODNAME + "_FallingObjectHandler");
            GameObject.DontDestroyOnLoad(go);
            Current = go.AddComponent<FallingObjectHandler>();
        }

        public void EnsureInit() { }

        public static FallingObjectHandler Current { get; private set; }
        private readonly static Action<GameObject> BasicDestroy = new((go) => GameObject.Destroy(go));

        private const int MaxSteps = 20;
        private const float UpdateInterval = 1f;

        private readonly LinkedList<(GameObject go, Action<GameObject> destroyFunc)> _objects = new();
        private float _nextUpdateTime;
        private LinkedListNode<(GameObject go, Action<GameObject> destroyFunc)>? _currentNode;

        public void Awake()
        {
            Current = this;
        }

        public static void AddObject(GameObject go, Action<GameObject>? destroyFunc = null) => Current._objects.AddLast((go, destroyFunc ?? BasicDestroy));

        private void Update()
        {
            if (Clock.Time < _nextUpdateTime) return;

            _currentNode ??= _objects.First;
            for (int steps = 0; steps < MaxSteps && _currentNode != null; steps++)
            {
                GameObject? go = _currentNode.Value.go;
                if (go != null && go.transform.position.y < -10000f)
                {
                    _currentNode.Value.destroyFunc.Invoke(go);
                    _objects.Remove(_currentNode);
                }
                else if (go == null || !go.active)
                    _objects.Remove(_currentNode);
                _currentNode = _currentNode.Next;
            }

            if (_currentNode == null)
                _nextUpdateTime = Clock.Time + UpdateInterval;
        }

        private void OnClear()
        {
            foreach ((var go, var destroyFunc) in _objects)
            {
                if (go != null)
                {
                    destroyFunc.Invoke(go);
                }
            }
            _objects.Clear();
            _currentNode = null;
        }

        public static void Clear() => Current.OnClear();
    }
}
