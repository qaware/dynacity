namespace SoftwareCities.dynamic
{
    /// <summary>
    /// Class holding all information of a span needed to visualize the components load. 
    /// </summary>
    public class Span
    {
        public readonly string SpanId;
        public readonly string ParentId;
        public readonly string TransactionId;
        public readonly string ComponentName;
        public readonly string ParentName;
        public readonly long Timestamp;
        public long Duration;
        public DynamicDependenciesImporter.HttpStatusCodes StatusCode; 

        public Span(string spanId, string parentId, string transactionId, string componentName, string parentName,
            long timestamp, long duration, DynamicDependenciesImporter.HttpStatusCodes statusCode)
        {
            SpanId = spanId;
            ParentId = parentId;
            TransactionId = transactionId;
            ComponentName = componentName;
            ParentName = parentName;
            Timestamp = timestamp;
            Duration = duration;
            StatusCode = statusCode;
        }
    }
}