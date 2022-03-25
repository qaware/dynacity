using System.Collections.Generic;

namespace SoftwareCities.dynamic
{
    public static class SpanMapper
    {
        /// <summary>
        /// Converts the nested ES trace data to a list of spans, filtering out any spans without a name. 
        /// </summary>
        /// <param name="spansData">the extracted ES trace data</param>
        /// <returns>a list of spans</returns>
        public static List<Span> ToSpans(ElasticSearchSpans spans)
        {
            List<Span> results = new List<Span>();
            spans.hits.hits.RemoveAll(source => source._source.span.name.Split('#').Length == 1);
            foreach (ElasticSearchSpans.Source hit in spans.hits.hits)
            {
                ElasticSearchSpans.Source parent =
                    spans.hits.hits.Find(source => source._source.span.id.Equals(hit._source.parent.id));
                Span resultingSpan = new Span(hit._source.span.id, hit._source.parent.id, hit._source.transaction.id,
                    SplitClassName(hit._source.span.name), SplitClassName(parent?._source.span.name),
                    hit._source.timestamp.us,
                    hit._source.span.duration.us, DynamicDependenciesImporter.HttpStatusCodes.Success);
                results.Add(resultingSpan);
            }

            return results;
        }

        private static string SplitClassName(string name)
        {
            // name is class#method we only have classes 
            return name?.Split('#')[0];
        }
    }
}