using System;
using System.Collections.Generic;

namespace SoftwareCities.dynamic
{
    /// <summary>
    /// Helper class for JSON deserialization of ElasticSearch transactions.
    /// Representing the nested JSON of ES trace data. 
    /// </summary>
    [Serializable]
    public class ElasticSearchTransactions
    {
        public Hits hits;

        [Serializable]
        public class Hits
        {
            public List<Source> hits;
        }

        [Serializable]
        public class Source
        {
            public Hit _source;
        }

        [Serializable]
        public class Hit
        {
            public Identifier transaction;
            public Http http;
        }

        [Serializable]
        public class Identifier
        {
            public string id;
        }

        [Serializable]
        public class Http
        {
            public Response response;
        }

        [Serializable]
        public class Response
        {
            public string status_code;
        }
    }
}