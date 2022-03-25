using System;
using System.IO;
using System.Threading.Tasks;
using SoftwareCities.dynamic;
using SoftwareCities.figures;
using SoftwareCities.holoware.city;
using SoftwareCities.holoware.lsm;
using UnityEngine;

namespace SoftwareCities.unityadapter
{
    public class UnitySoftwareCity : MonoBehaviour
    {
        public string dotFilePath = "example.dot";

        public string spansFilePath = "spans.json";

        public int aggregationSpanInSeconds = 360;

        private Figure current;

        private Figure rootElement;

        private DependencyGraph graph;
        private DynamicDependenciesImporter importer;

        public void Start()
        {
            // Avoid blocking. Load file async.

            Task.Factory.StartNew(() =>
            {
                try
                {
                    Stream dotFile = File.OpenRead(dotFilePath);
                    Debug.Log("Loading .dot file: " + dotFile + " ...");
                    graph = DependencyGraph.FromDot(dotFile);
                    Debug.Log("Constructing LSM ...");
                    LsmBuilder builder = LsmBuilder.LsmFromGraph(graph);
                    importer = ElasticSearchImporter.LoadDynamicDependencies(spansFilePath,
                        TimeSpan.FromSeconds(aggregationSpanInSeconds));
                    Lsm2CityVisitor visitor = new Lsm2CityVisitor(graph);
                    Debug.Log("Constructing UI elements ...");
                    builder.RootNode.Accept(visitor);
                    rootElement = visitor.GetResult();
                    current = rootElement;
                    Debug.Log("Success.");
                }
                catch (IOException e)
                {
                    Debug.LogError(e);
                }
            });
        }

        public void Update()
        {
            // Todo: Only update specific portions for huge models.
            if (current == null) return;
            UnityVisitor unity = new UnityVisitor(rootElement);
            unity.Run();
            current = null; // done
            DynamicDependenciesVisualizer.VisualizeDependenciesOf(importer, unity.GameObjects);
        }
    }
}