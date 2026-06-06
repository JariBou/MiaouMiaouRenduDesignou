using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Modules.Pooling.Runtime
{
    public class Pool
    {
        public const int DefaultPoolSize = 10;
        public event Action<Pool> PoolCleaned;
        
        private readonly GameObject prefab;
        
        private List<GameObject> objects;
        private CancellationTokenSource cleanTaskCancellationTokenSource;

        public Pool(GameObject prefab, int size)
        {
            this.prefab = prefab;
            InitialisePool(size);
        }

        public void ResetPool(int size)
        {
            // Since destruction is scheduled it is safe  to iterate via foreach
            foreach (GameObject go in objects)
            {
                Object.Destroy(go);
            }
            InitialisePool(size);    
        }
        
        private void InitialisePool(int size)
        {
            objects = new List<GameObject>(size);
            for (int i = 0; i < size; i++)
            {
                GameObject gameObject = Object.Instantiate(prefab);
                gameObject.SetActive(false);
                objects.Add(gameObject);
            }
        }
        
        /// <summary>
        /// </summary>
        /// <remarks>
        /// You shouldn't keep a permanent reference to this object as it may be destroyed on reset
        /// </remarks>
        /// <returns>The first inactive GameObject in the pool, or a new one if needed</returns>
        public GameObject Get(bool autoActivate = true)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator  Justification: not necessary and tbh I prefer a no overhead method 
            foreach (GameObject go in objects)
            {
                if (!go.activeSelf)
                {
                    go.SetActive(autoActivate);
                    return go;
                }
            }

            GameObject gameObject = Object.Instantiate(prefab);
            gameObject.SetActive(autoActivate);
            objects.Add(gameObject);
            return gameObject;
        }
        
        public TObject Get<TObject>(bool autoActivate = true) where TObject : Object
        {
            return Get(autoActivate).GetComponent<TObject>();
        }

        public void DeactivateAll()
        {
            foreach (GameObject go in objects)
            {
                go.SetActive(false);
            }
        }

        public void DeactivateAt(int index, [CallerMemberName] string callerMemberName = null)
        {
            if (index < 0 || index >= objects.Count)
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range ([0, {objects.Count})]) when calling {nameof(DeactivateAt)} in function '{callerMemberName}'");
            }
            
            objects[index].SetActive(false);
        }

        public List<GameObject> GetObjects()
        {
            return objects;
        }

        public void CleanToPreferredSize(int preferredSize = 0)
        {
            int i = 0;
            while (i < objects.Count && objects.Count > preferredSize)
            {
                GameObject gameObject = objects[i];
                if (gameObject.activeSelf)
                {
                    i++;
                } else
                {
                    objects.RemoveAt(i);
                }
            }
            PoolCleaned?.Invoke(this);
        }

        public void StopCleanEverySeconds()
        {
            cleanTaskCancellationTokenSource.Cancel();
            cleanTaskCancellationTokenSource.Dispose();
            cleanTaskCancellationTokenSource = null;
        }
        
        public void ScheduleCleanEverySeconds(int preferredSize, int seconds)
        {
            cleanTaskCancellationTokenSource?.Cancel();
            cleanTaskCancellationTokenSource?.Dispose();
            cleanTaskCancellationTokenSource = new CancellationTokenSource();
            
            _ = CleanTask(preferredSize, seconds, cleanTaskCancellationTokenSource.Token);
        }

        private async Awaitable CleanTask(int preferredSize, int seconds, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Awaitable.WaitForSecondsAsync(seconds, cancellationToken);
                CleanToPreferredSize(preferredSize);
            }
        }
    }
}