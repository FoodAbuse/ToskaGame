using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class CoroutineHandler : MonoBehaviour
    {
        // this class is for running coroutines, cause sometimes we wanna defer coroutines to a different class... for reasons..
        public static List<GameObject> CouroutineHandlerRuntimeSet =  new List<GameObject>();

        public static void RegisterCouroutineHandler(GameObject obj)
        {
            CouroutineHandlerRuntimeSet.Add(obj);
        }

        public static void UnRegisterCouroutineHandler(GameObject obj)
        {
            CouroutineHandlerRuntimeSet.Remove(obj);
        }

        public static CoroutineHandler CreateCouroutineHandler(IEnumerator coroutine)
        {
            // here we will create an empty game object and attach the coroutine handler to it. it will enable and run its scripts
            GameObject  obj = new GameObject();
            DontDestroyOnLoad(obj);
            CoroutineHandler newHandler = obj.AddComponent<CoroutineHandler>();
            newHandler.coroutines.Add(coroutine);
            newHandler.RunCoroutines();
            return newHandler;

        }
        public List<IEnumerator> coroutines = new List<IEnumerator>();

        void OnEnable()
        {
            RegisterCouroutineHandler(gameObject);
        }

        void OnDisable()
        {
            UnRegisterCouroutineHandler(gameObject);
        }

        void RunCoroutines()
        {
            StartCoroutine(CoroutineRunner());
        }

        IEnumerator CoroutineRunner()
        {
            foreach (IEnumerator coroutine in coroutines)
                yield return StartCoroutine(coroutine);
            Destroy(gameObject);
        }

    }
}
