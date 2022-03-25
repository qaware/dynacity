using System;
using System.Collections.Generic;

namespace SoftwareCities.dynamic
{
    /// <summary>
    /// Helper class for JSON deserialization of ElasticSearch spans.
    /// Representing the nested JSON of ES trace data. 
    /// </summary>
    [Serializable]
    public class ElasticSearchSpans
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
            public Identifier parent;
            public Identifier transaction;
            public Microseconds timestamp;
            public Span span;
        }

        [Serializable]
        public class Identifier
        {
            public string id;
        }

        [Serializable]
        public class Microseconds
        {
            public long us;
        }

        [Serializable]
        public class Span
        {
            public string id;
            public string name;
            public Microseconds duration;
        }
    }
}