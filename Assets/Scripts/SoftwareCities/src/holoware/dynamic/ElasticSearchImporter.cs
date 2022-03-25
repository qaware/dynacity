using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SoftwareCities.holoware.lsm;
using UnityEngine;

namespace SoftwareCities.dynamic
{
    public class ElasticSearchImporter : DynamicDependenciesImporter
    {
        private long aggregationIntervalInUs;

        private long minTimestamp = long.MaxValue;
        private long maxTimestamp = long.MinValue;

        /// <summary>
        /// Load the JSON span file extracted from elasticsearch 
        /// </summary>
        /// <param name="spanDirectoryPath">path to the span files</param>
        /// <param name="timeSpan">The time to aggregate over</param>
        /// <returns>The resulting <see cref="ElasticSearchImporter"/></returns>
        public static ElasticSearchImporter LoadDynamicDependencies(string spanDirectoryPath, TimeSpan timeSpan)
        {
            // Ticks are 0.1us
            ElasticSearchImporter result = new ElasticSearchImporter
            {
                aggregationIntervalInUs = timeSpan.Ticks / 10
            };

            ElasticSearchTransactions elasticSearchTransactions = ProcessTransactionFiles(spanDirectoryPath);

            string[] spanFiles =
                Directory.GetFiles(spanDirectoryPath, "spans*.json", SearchOption.TopDirectoryOnly);

            foreach (string spanFile in spanFiles)
            {
                Debug.Log("Processing span file " + spanFile + "...");
                ProcessSpanFile(spanFile, elasticSearchTransactions, result);
                Debug.Log("Done!");
            }

            result.NormalizeLoads();
            return result;
        }

        private static ElasticSearchTransactions ProcessTransactionFiles(string spanDirectoryPath)
        {
            string[] transactionFiles =
                Directory.GetFiles(spanDirectoryPath, "transaction*.json", SearchOption.TopDirectoryOnly);
            List<ElasticSearchTransactions> transactionsList = new List<ElasticSearchTransactions>();
            foreach (string transactionFile in transactionFiles)
            {
                Debug.Log("Processing transaction file " + transactionFile + "...");
                using (StreamReader reader = new StreamReader(File.OpenRead(transactionFile)))
                {
                    string jsonString = reader.ReadToEnd();
                    ElasticSearchTransactions elasticSearchTransactions =
                        JsonUtility.FromJson<ElasticSearchTransactions>(jsonString);
                    transactionsList.Add(elasticSearchTransactions);
                }

                Debug.Log("Done!");
            }

            ElasticSearchTransactions flattenedTransactions = transactionsList[0];
            for (int i = 1; i < transactionsList.Count; i++)
            {
                flattenedTransactions.hits.hits.AddRange(transactionsList[i].hits.hits);
            }

            return flattenedTransactions;
        }

        private static void ProcessSpanFile(string spanFileName, ElasticSearchTransactions transactions,
            ElasticSearchImporter result)
        {
            using (StreamReader reader = new StreamReader(File.OpenRead(spanFileName)))
            {
                string jsonString = reader.ReadToEnd();
                ElasticSearchSpans elasticSearchSpans = JsonUtility.FromJson<ElasticSearchSpans>(jsonString);
                List<Span> spans = SpanMapper.ToSpans(elasticSearchSpans);
                Debug.Log("Number of spans in file: " + spans.Count);
                long startTimestamp = spans.Min(span => span.Timestamp);
                long endTimestamp = spans.Max(span => span.Timestamp + span.Duration);

                ExtendTimeRange(result, startTimestamp, endTimestamp);

                foreach (Span span in spans)
                {
                    Span spanExceededBucket = ProcessSpan(span, transactions, result);
                    while (spanExceededBucket != null)
                    {
                        spanExceededBucket = ProcessSpan(spanExceededBucket, transactions, result);
                    }
                }
            }
        }

        private static Span ProcessSpan(Span span, ElasticSearchTransactions transactions,
            ElasticSearchImporter importer)
        {
            (long bucketTimestamp, List<ComponentLoad> componentLoads) = importer.Buckets.FirstOrDefault(pair =>
                pair.Key >= span.Timestamp && pair.Key < span.Timestamp + importer.aggregationIntervalInUs);
            Span splitSpan = GenerateSplitSpan(span, bucketTimestamp);
            ComponentLoad existingLoad =
                componentLoads.Find(load => load.ComponentName.Contains(span.ComponentName));
            ElasticSearchTransactions.Source transaction =
                transactions.hits.hits.Find(source => source._source.transaction.id.Equals(span.TransactionId));
            if (transaction != null)
            {
                span.StatusCode = StringToHttpStatusCode(transaction._source.http.response.status_code);
            }

            if (existingLoad == null)
            {
                ComponentLoad load = new ComponentLoad(span.ComponentName, span.Duration, span.StatusCode);
                load.AddCallLoad(span.ParentName, span.Duration, span.StatusCode);
                componentLoads.Add(load);
            }
            else
            {
                if (existingLoad.Load.ContainsKey(span.StatusCode))
                    existingLoad.Load[span.StatusCode] += span.Duration;
                else
                    existingLoad.Load.Add(span.StatusCode, span.Duration);
                existingLoad.AddCallLoad(span.ParentName, span.Duration, span.StatusCode);
            }

            return splitSpan;
        }

        private static HttpStatusCodes StringToHttpStatusCode(string stringCode)
        {
            switch (stringCode)
            {
                case string a when a.StartsWith("4"): return HttpStatusCodes.ClientError;
                case string a when a.StartsWith("5"): return HttpStatusCodes.ServerError;
                default: return HttpStatusCodes.Success;
            }
        }

        private static Span GenerateSplitSpan(Span span, long bucketTimestamp)
        {
            Span splitSpan = null;
            if (span.Timestamp + span.Duration > bucketTimestamp)
            {
                splitSpan = new Span(span.SpanId, span.ParentId, span.TransactionId,
                    span.ComponentName, span.ParentName,
                    bucketTimestamp + 1,
                    span.Timestamp + span.Duration - bucketTimestamp, HttpStatusCodes.Success);
                span.Duration = bucketTimestamp - span.Timestamp;
            }

            return splitSpan;
        }

        private static void ExtendTimeRange(ElasticSearchImporter result, long startTimestamp, long endTimestamp)
        {
            // Fill first time range if none is in the component load list yet 
            if (result.Buckets.Count == 0)
            {
                result.minTimestamp = startTimestamp;
                result.maxTimestamp = endTimestamp;
                while (startTimestamp < endTimestamp)
                {
                    result.Buckets.Add(startTimestamp, new List<ComponentLoad>());
                    startTimestamp += result.aggregationIntervalInUs;
                }

                result.Buckets.Add(startTimestamp, new List<ComponentLoad>());
            }

            while (startTimestamp < result.minTimestamp)
            {
                result.minTimestamp -= result.aggregationIntervalInUs;
                result.Buckets.Add(result.minTimestamp, new List<ComponentLoad>());
            }

            while (endTimestamp > result.maxTimestamp)
            {
                result.maxTimestamp += result.aggregationIntervalInUs;
                result.Buckets.Add(result.maxTimestamp, new List<ComponentLoad>());
            }
        }
    }
}