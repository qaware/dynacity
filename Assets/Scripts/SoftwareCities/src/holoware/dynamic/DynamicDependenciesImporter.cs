using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareCities.dynamic
{
    public abstract class DynamicDependenciesImporter
    {
        private int currentBucketIndex;

        /**
         * The start timestamp of the bucket as key and the list of all component loads within the bucket as value. 
         */
        protected readonly SortedDictionary<long, List<ComponentLoad>> Buckets =
            new SortedDictionary<long, List<ComponentLoad>>();

        private float minComponentLoad = float.MaxValue;
        private float maxComponentLoad = float.MinValue;

        private float minCallLoad = float.MaxValue;
        private float maxCallLoad = float.MinValue;

        public List<ComponentLoad> GetActivityFor(string componentName)
        {
            List<ComponentLoad> componentLoad = Buckets.ElementAt(currentBucketIndex).Value
                .FindAll(load => load.ComponentName.Equals(componentName));
            return componentLoad.Count == 0
                ? new List<ComponentLoad> {new ComponentLoad(componentName, minComponentLoad, HttpStatusCodes.Success)}
                : componentLoad;
        }

        public float GetMinLoad()
        {
            return minComponentLoad;
        }

        public float GetMaxLoad()
        {
            return maxComponentLoad;
        }


        public float GetMinCallLoad()
        {
            return minCallLoad;
        }

        public float GetMaxCallLoad()
        {
            return maxCallLoad;
        }

        /// <summary>
        /// Normalize loads to generate light intensities that are somehow balanced. 
        /// </summary>
        protected void NormalizeLoads()
        {
            // Flatten
            List<Dictionary<HttpStatusCodes, float>> flatDictionary = Buckets.Values
                .SelectMany(x => x)
                .ToList().Select(load => load.Load).ToList();
            List<float> flatList = flatDictionary.Select(floats => floats.Sum(pair => pair.Value)).ToList();
            List<CallLoad> flatCallLoadDictionary = Buckets.Values
                .SelectMany(x => x)
                .ToList().SelectMany(load => load.CallLoads).ToList();
            List<float> flatCallLoadList =
                flatCallLoadDictionary.Select(loads => loads.Load.Sum(pair => pair.Value)).ToList();

            AddLoads(flatList, flatCallLoadList);

            flatList = flatDictionary.Select(floats => floats.Sum(pair => pair.Value)).ToList();
            double thirdQuartile1 = CalculatePercentile(flatList.ToArray(), 0.75);
            // Set maxLoad for MinMaxScaling to third quartile to make also shorter calls visible 
            maxComponentLoad = (float) thirdQuartile1;
            minComponentLoad = flatList.Min();

            flatCallLoadList = flatCallLoadDictionary.Select(loads => loads.Load.Sum(pair => pair.Value)).ToList();
            thirdQuartile1 = CalculatePercentile(flatCallLoadList.ToArray(), 0.75);
            // Set maxLoad for MinMaxScaling to third quartile to make also shorter calls visible 
            maxCallLoad = (float) thirdQuartile1;
            minCallLoad = flatCallLoadList.Min();
        }

        private void AddLoads(List<float> flatList, List<float> flatCallLoadList)
        {
            double firstQuartile = CalculatePercentile(flatList.ToArray(), 0.25);
            double thirdQuartile = CalculatePercentile(flatList.ToArray(), 0.75);
            double median = CalculatePercentile(flatList.ToArray(), 0.5);
            double firstQuartileCalls = CalculatePercentile(flatCallLoadList.ToArray(), 0.25);
            double thirdQuartileCalls = CalculatePercentile(flatCallLoadList.ToArray(), 0.75);
            double medianCalls = CalculatePercentile(flatCallLoadList.ToArray(), 0.5);
            if (Math.Abs(firstQuartile - thirdQuartile) < double.Epsilon)
            {
                maxComponentLoad = (float) thirdQuartile;
                minComponentLoad = 0F;
            }

            foreach (ComponentLoad componentLoad in Buckets.SelectMany(keyValuePair => keyValuePair.Value))
            {
                FillComponentLoad(componentLoad, median, thirdQuartile, firstQuartile);

                foreach (CallLoad callLoad in componentLoad.CallLoads)
                {
                    FillCallLoad(callLoad, medianCalls, thirdQuartileCalls, firstQuartileCalls);
                }
            }
        }

        private static void FillCallLoad(CallLoad callLoad, double medianCalls, double thirdQuartileCalls,
            double firstQuartileCalls)
        {
            if (callLoad.Load.ContainsKey(HttpStatusCodes.Success))
            {
                callLoad.Load[HttpStatusCodes.Success] =
                    (float) Math.Abs((callLoad.Load[HttpStatusCodes.Success] - medianCalls) /
                                     (thirdQuartileCalls - firstQuartileCalls));
            }

            if (callLoad.Load.ContainsKey(HttpStatusCodes.ClientError))
            {
                callLoad.Load[HttpStatusCodes.ClientError] =
                    (float) Math.Abs((callLoad.Load[HttpStatusCodes.ClientError] - medianCalls) /
                                     (thirdQuartileCalls - firstQuartileCalls));
            }

            if (callLoad.Load.ContainsKey(HttpStatusCodes.ServerError))
            {
                callLoad.Load[HttpStatusCodes.ServerError] =
                    (float) Math.Abs((callLoad.Load[HttpStatusCodes.ServerError] - medianCalls) /
                                     (thirdQuartileCalls - firstQuartileCalls));
            }
        }

        private static void FillComponentLoad(ComponentLoad componentLoad, double median, double thirdQuartile,
            double firstQuartile)
        {
            if (componentLoad.Load.ContainsKey(HttpStatusCodes.Success))
            {
                componentLoad.Load[HttpStatusCodes.Success] =
                    (float) Math.Abs((componentLoad.Load[HttpStatusCodes.Success] - median) /
                                     (thirdQuartile - firstQuartile));
            }

            if (componentLoad.Load.ContainsKey(HttpStatusCodes.ClientError))
            {
                componentLoad.Load[HttpStatusCodes.ClientError] =
                    (float) Math.Abs((componentLoad.Load[HttpStatusCodes.ClientError] - median) /
                                     (thirdQuartile - firstQuartile));
            }

            if (componentLoad.Load.ContainsKey(HttpStatusCodes.ServerError))
            {
                componentLoad.Load[HttpStatusCodes.ServerError] =
                    (float) Math.Abs((componentLoad.Load[HttpStatusCodes.ServerError] - median) /
                                     (thirdQuartile - firstQuartile));
            }
        }

        /// <summary>
        /// Calculates the given percentile of a given sequence. 
        /// </summary>
        /// <param name="sequence">for which the percentile should be calculated</param>
        /// <param name="percentile">the percentile that should be calculated, e.g., 0.25</param>
        /// <returns>the percentile</returns>
        private static double CalculatePercentile(float[] sequence, double percentile)
        {
            Array.Sort(sequence);
            int length = sequence.Length;
            double n = (length - 1) * percentile + 1;
            if (Math.Abs(n - 1d) < float.Epsilon) return sequence[0];
            if (Math.Abs(n - length) < float.Epsilon) return sequence[length - 1];
            int k = (int) n;
            double d = n - k;
            return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
        }

        /// <summary>
        /// Called when the user requests the next time frame. Nothing happens when we reach the end.
        /// <returns>whether or not the index reached the bounds</returns>
        /// </summary>
        public bool IncrementBucketIndex()
        {
            if (currentBucketIndex >= Buckets.Count - 1) return false;
            currentBucketIndex += 1;
            return true;
        }

        /// <summary>
        /// Called when the user requests the last time frame. Nothing happens when we reach the beginning.
        /// <returns>whether or not the index reached the bounds</returns>
        /// </summary>
        public bool DecrementBucketIndex()
        {
            if (currentBucketIndex <= 0) return false;
            currentBucketIndex -= 1;
            return true;
        }


        /// <summary>
        /// Helper class to sort components and their load within buckets
        /// </summary>
        public class ComponentLoad
        {
            public readonly string ComponentName;
            public Dictionary<HttpStatusCodes, float> Load = new Dictionary<HttpStatusCodes, float>();
            public List<CallLoad> CallLoads;

            public ComponentLoad(string componentName, float load, HttpStatusCodes statusCode)
            {
                ComponentName = componentName;
                Load.Add(statusCode, load);
                CallLoads = new List<CallLoad>();
            }

            /// <summary>
            /// Adds the given load to the call load if call load with given parent already exists or create it. 
            /// </summary>
            /// <param name="parentName">parent component</param>
            /// <param name="load">of the parent child dependency</param>
            public void AddCallLoad(string parentName, float load, HttpStatusCodes statusCode)
            {
                if (parentName == null)
                {
                    return;
                }

                CallLoad existingLoad = CallLoads.Find(callLoad => callLoad.ParentName.Equals(parentName));
                if (existingLoad == null)
                {
                    CallLoads.Add(new CallLoad(parentName, load, statusCode));
                }
                else
                {
                    if (existingLoad.Load.ContainsKey(statusCode))
                    {
                        existingLoad.Load[statusCode] += load;
                    }
                    else
                    {
                        existingLoad.Load.Add(statusCode, load);
                    }
                }
            }
        }

        /// <summary>
        /// Helper class to sort components and their load within buckets
        /// </summary>
        public class CallLoad
        {
            public readonly string ParentName;
            public Dictionary<HttpStatusCodes, float> Load = new Dictionary<HttpStatusCodes, float>();

            public CallLoad(string parentName, float load, HttpStatusCodes statusCode)
            {
                ParentName = parentName;
                Load.Add(statusCode, load);
            }
        }

        public enum HttpStatusCodes
        {
            Success,
            ClientError,
            ServerError
        }

        public string GetCurrentTimestamp()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Buckets.ElementAt(currentBucketIndex).Key / 1000)
                .ToString("G");
        }
    }
}