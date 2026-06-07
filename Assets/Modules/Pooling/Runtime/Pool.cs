using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pooling
{
    public class Pool
    {
        public const int DefaultPoolSize = 10;
        /// <summary>
        /// Invoked whenever <see cref="CleanToPreferredSize"/> is called.
        /// The bool argument is true when called via a scheduled Cleaning and false when called via user
        /// </summary>
        public event Action<Pool, bool /*wasScheduled*/> PoolCleaned;
        
        private readonly GameObject prefab;
        
        private List<GameObject> objects;
        private CancellationTokenSource cleanTaskCancellationTokenSource;

        public Pool(GameObject prefab, int size)
        {
            this.prefab = prefab;
            InitializePool(size);
        }

        /// <summary>
        /// Destroys avery object in the pool and reinitializes the pool with a size of <paramref name="size"/>
        /// </summary>
        /// <param name="size">The size of the pool to initialize</param>
        public void ResetPool(int size)
        {
            // Since destruction is scheduled it is safe  to iterate via foreach
            foreach (GameObject go in objects)
            {
                Object.Destroy(go);
            }
            InitializePool(size);    
        }
        
        private void InitializePool(int size)
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
        /// Returns the first inactive GameObject in the pool, or creates a new one from prefab if needed
        /// </summary>
        /// <remarks>
        /// You shouldn't keep a permanent reference to this object as it may be destroyed on reset
        /// </remarks>
        /// <param name="autoActivate">Whether to automatically set active the gameobject</param>
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
        
        /// <inheritdoc cref="Get"/>
        /// <summary>
        /// Returns the first inactive GameObject's component of type <typeparamref name="TObject"/> in the pool. If no inactive gameobject, creates a new one from prefab.
        /// </summary>
        public TObject Get<TObject>(bool autoActivate = true) where TObject : Object
        {
            return Get(autoActivate: autoActivate).GetComponent<TObject>();
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

        // /// <summary>
        // /// Tries to trim the list to a size of <paramref name="preferredSize"/>
        // /// </summary>
        // /// <param name="preferredSize">Size min of the list</param>
        // public void CleanToPreferredSize(int preferredSize = 0)
        // {
        //     int i = 0;
        //     while (i < objects.Count && objects.Count > preferredSize)
        //     {
        //         GameObject gameObject = objects[i];
        //         if (gameObject.activeSelf)
        //         {
        //             i++;
        //         } else
        //         {
        //             objects.RemoveAt(i);
        //         }
        //     }
        //     PoolCleaned?.Invoke(this, false);
        // }

        /// <summary>
        /// Tries to trim the list to a size of <paramref name="preferredSize"/>
        /// </summary>
        /// <param name="preferredSize">Size min of the list</param>
        /// <param name="callerMemberName">The method that called this method, used internally to know if it was called via schedule</param>
        public void CleanToPreferredSize(int preferredSize = 0, [CallerMemberName] string callerMemberName = "") // tbh could have gone with some internal shenanigans but I do love me some [CallerMemberName] sorry ^^ 
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
            PoolCleaned?.Invoke(this, callerMemberName == nameof(Pooling_CleanTask));
        }

        /// <summary>
        /// Stops any <see cref="CleanToPreferredSize"/> schedulled via <see cref="ScheduleCleanEverySeconds"/> if any
        /// </summary>
        public void StopCleanEverySeconds()
        {
            cleanTaskCancellationTokenSource?.Cancel();
            cleanTaskCancellationTokenSource?.Dispose();
            cleanTaskCancellationTokenSource = null;
        }
        
        /// <summary>
        /// Calls <see cref="CleanToPreferredSize"/> every <paramref name="delay"/> seconds
        /// </summary>
        /// <param name="preferredSize">Size min of the list</param>
        /// <param name="delay">The delay between each call</param>
        public void ScheduleCleanEverySeconds(int preferredSize, int delay)
        {
            cleanTaskCancellationTokenSource?.Cancel();
            cleanTaskCancellationTokenSource?.Dispose();
            cleanTaskCancellationTokenSource = new CancellationTokenSource();
            
            _ = Pooling_CleanTask(preferredSize, delay, cleanTaskCancellationTokenSource.Token);
        }

        private async Awaitable Pooling_CleanTask(int preferredSize, int seconds, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Awaitable.WaitForSecondsAsync(seconds, cancellationToken);
                CleanToPreferredSize(preferredSize);
            }
        }
    }
}