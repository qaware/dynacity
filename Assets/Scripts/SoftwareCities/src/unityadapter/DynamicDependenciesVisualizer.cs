using System;
using System.Collections.Generic;
using System.Linq;
using SoftwareCities.dynamic;
using UnityEngine;
using UnityEngine.UI;
using Material = SoftwareCities.figures.Material;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace SoftwareCities.unityadapter
{
    public class DynamicDependenciesVisualizer
    {
        private readonly DynamicDependenciesImporter dependenciesImporter;

        private readonly List<GameObject> gameObjects;

        private IntensityMinMaxScaler scalerComponentLoad;
        private IntensityMinMaxScaler scalerCallLoad;

        private static readonly int MainColorId = Shader.PropertyToID("_Color");

        private Text text;

        /// <summary>
        /// List holding the drawn dependency lines. Needed to redraw them on increasing / decreasing time frame
        /// </summary>
        private readonly List<GameObject> dependencyLines = new List<GameObject>();

        public static DynamicDependenciesVisualizer VisualizeDependenciesOf(
            DynamicDependenciesImporter dependenciesImporter,
            List<GameObject> gameObjects)
        {
            return new DynamicDependenciesVisualizer(dependenciesImporter, gameObjects);
        }

        /// <summary>
        /// Subscribe to user input to change time frames.
        /// </summary>
        /// <param name="dependenciesImporter">To get the activity of components in order to change the light intensity</param>
        /// <param name="gameObjects">To change the material / light of game objects</param>
        private DynamicDependenciesVisualizer(DynamicDependenciesImporter dependenciesImporter,
            List<GameObject> gameObjects)
        {
            this.dependenciesImporter = dependenciesImporter;
            this.gameObjects = gameObjects;
            InitMinMaxScaler();
            CreateLabel();
            UpdateIntensity();
            EventManager.OnNextTimeFrame += IncreaseTimeFrame;
            EventManager.OnLastTimeFrame += DecreaseTimeFrame;
            MouseClick.OnComponentClick += ShowDepsForComponent;
            MouseClick.OnComponentRelease += ShowAllDeps;
        }

        private void ShowAllDeps()
        {
            foreach (GameObject dependencyLine in dependencyLines)
            {
                if (dependencyLine != null)
                    dependencyLine.SetActive(true);
            }
        }

        private void ShowDepsForComponent(string componentName)
        {
            foreach (GameObject dependencyLine in dependencyLines)
            {
                if (dependencyLine != null && !dependencyLine.name.Contains(componentName + "_"))
                    dependencyLine.SetActive(false);
            }
        }

        /// <summary>
        /// Unsubscribe from events 
        /// </summary>
        ~DynamicDependenciesVisualizer()
        {
            EventManager.OnNextTimeFrame -= IncreaseTimeFrame;
            EventManager.OnLastTimeFrame -= DecreaseTimeFrame;
            MouseClick.OnComponentClick -= ShowDepsForComponent;
            MouseClick.OnComponentRelease -= ShowAllDeps;
        }

        /// <summary>
        /// Initialize MinMaxScaler based on the minimal and maximal load found in the dependencies 
        /// </summary>
        private void InitMinMaxScaler()
        {
            scalerComponentLoad =
                new IntensityMinMaxScaler(dependenciesImporter.GetMinLoad(), dependenciesImporter.GetMaxLoad());
            scalerCallLoad = new IntensityMinMaxScaler(dependenciesImporter.GetMinCallLoad(),
                dependenciesImporter.GetMaxCallLoad());
        }

        /// <summary>
        /// Update the light intensities of the game objects based on current time frame 
        /// </summary>
        private void UpdateIntensity()
        {
            foreach (GameObject t in dependencyLines)
            {
                Object.Destroy(t);
            }

            foreach (GameObject gameObject in gameObjects)
            {
                // Only one child meaning we're on building level 
                Transform parent = gameObject.transform.parent;
                if (parent == null || parent.childCount > 1) continue;
                List<DynamicDependenciesImporter.ComponentLoad> allLoads =
                    dependenciesImporter.GetActivityFor(parent.name.Split('.').Last());
                DrawDependencies(allLoads, gameObject);
            }

            text.text = dependenciesImporter.GetCurrentTimestamp();
        }

        private void DrawDependencies(List<DynamicDependenciesImporter.ComponentLoad> allLoads,
            GameObject gameObject)
        {
            foreach (DynamicDependenciesImporter.ComponentLoad load in allLoads)
            {
                Color componentColorBasedOnLoad = calculateColorBasedOnLoad(load.Load, scalerComponentLoad);
                gameObject.GetComponent<Renderer>().material
                    .SetColor(MainColorId, componentColorBasedOnLoad);
                // float totalComponentLoad = load.Load.Sum(pair => pair.Value); 
                // gameObject.GetComponent<Renderer>().material
                //     .SetColor(MainColorId, purple * scalerComponentLoad.Scale(totalComponentLoad) * 1.5F);
                foreach (DynamicDependenciesImporter.CallLoad callLoad in load.CallLoads)
                {
                    GameObject to = gameObjects.Find(go =>
                        go.transform.parent.gameObject.name.Contains(callLoad.ParentName));
                    Vector3 startPos = gameObject.transform.position;
                    Vector3 endPos = to.transform.position;
                    Vector3 fromBounds = gameObject.GetComponent<Collider>().bounds.size;
                    Vector3 toBounds = to.GetComponent<Collider>().bounds.size;
                    List<Vector3> attractionPoints = new List<Vector3>();
                    float lineHeight = Math.Abs(startPos.y - endPos.y) + 15;
                    Random random = new Random();
                    float offsetX = random.Next(-40, 40) / 100F;
                    float offsetZ = random.Next(-40, 40) / 100F;
                    attractionPoints.Add(new Vector3(fromBounds.x * -offsetX, lineHeight, fromBounds.z * -offsetZ));
                    attractionPoints.Add(new Vector3(endPos.x - startPos.x - toBounds.x * offsetX, lineHeight,
                        endPos.z - startPos.z - toBounds.z * offsetZ));
                    startPos.x += fromBounds.x * offsetX;
                    startPos.z += fromBounds.z * offsetZ;
                    endPos.x += toBounds.x * offsetX;
                    endPos.z += toBounds.z * offsetZ;
                    GameObject line = DependencyDrawer.DrawLine(startPos,
                        endPos, attractionPoints,
                        Material.RedGlass);
                    Color arcColorBasedOnLoad = calculateColorBasedOnLoad(callLoad.Load, scalerCallLoad);
                    line.GetComponent<Renderer>().material
                        .SetColor(MainColorId, arcColorBasedOnLoad);
                    line.name = callLoad.ParentName + "_" + load.ComponentName + "_";
                    dependencyLines.Add(line);
                }
            }
        }

        private Color calculateColorBasedOnLoad(Dictionary<DynamicDependenciesImporter.HttpStatusCodes, float> load,
            IntensityMinMaxScaler scaler)
        {
            load.TryGetValue(DynamicDependenciesImporter.HttpStatusCodes.Success,
                out float successLoad);
            load.TryGetValue(DynamicDependenciesImporter.HttpStatusCodes.ClientError,
                out float clientErrorLoad);
            load.TryGetValue(DynamicDependenciesImporter.HttpStatusCodes.ServerError,
                out float serverErrorLoad);
            float totalLoad = successLoad + clientErrorLoad + serverErrorLoad;
            float hueColor = (successLoad * 120 + clientErrorLoad * 60 + serverErrorLoad * 0) /
                             totalLoad;
            Color rgbColor = Color.HSVToRGB(hueColor / 360, 1, 1);
            return rgbColor * 255 * scaler.Scale(totalLoad);
        }

        /// <summary>
        /// Create a UI label for the clicked class
        /// </summary>
        private void CreateLabel()
        {
            // Canvas
            Canvas infoCanvas = CreateCanvas();

            // Panel
            GameObject panel = CreatePanel(infoCanvas);

            // Text
            CreateText(panel);
        }

        /// <summary>
        /// Add a text to the given panel, center alignment
        /// </summary>
        /// <param name="panel">the panel to which the text will be added</param>
        private void CreateText(GameObject panel)
        {
            GameObject myText = new GameObject();
            myText.transform.SetParent(panel.transform);
            myText.name = "Component name";
            text = myText.AddComponent<Text>();
            text.font = Resources.Load<Font>("Fonts/Montserrat-Medium");
            text.text = dependenciesImporter.GetCurrentTimestamp();
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.resizeTextForBestFit = false;

            // Text position
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.localPosition = new Vector3(0, 0, 0);
            textRect.sizeDelta = new Vector2(200, 100);
            textRect.transform.SetParent(panel.transform);
        }

        /// <summary>
        /// Add a panel to a given canvas
        /// </summary>
        /// <param name="canvas">the canvas to which the panel will be added</param>
        /// <returns>the created panel</returns>
        private static GameObject CreatePanel(Component canvas)
        {
            GameObject panel = new GameObject();
            panel.AddComponent<CanvasRenderer>();
            panel.AddComponent<RectTransform>();
            panel.transform.SetParent(canvas.transform);

            // Panel position 
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(1, 1);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.anchoredPosition = new Vector2(-100, -50);
            panelRect.sizeDelta = new Vector2(200, 100);
            panelRect.transform.SetParent(panel.transform);
            return panel;
        }

        /// <summary>
        /// Creates a new canvas as a container for the class label
        /// </summary>
        /// <returns>the created canvas</returns>
        private Canvas CreateCanvas()
        {
            GameObject timestampLabelGo = new GameObject {name = "Component Label"};
            timestampLabelGo.AddComponent<Canvas>();
            if (Camera.main != null)
            {
                Transform cameraTransform = Camera.main.transform;
                timestampLabelGo.transform.position = cameraTransform.position + cameraTransform.forward * 0.5F;
            }

            Canvas infoCanvas = timestampLabelGo.GetComponent<Canvas>();
            infoCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            timestampLabelGo.AddComponent<CanvasScaler>();
            timestampLabelGo.AddComponent<GraphicRaycaster>();
            return infoCanvas;
        }

        /// <summary>
        /// Increase time frame and update intensities  
        /// </summary>
        private void IncreaseTimeFrame()
        {
            Debug.Log("IncreaseTimeFrame");
            if (dependenciesImporter.IncrementBucketIndex())
                UpdateIntensity();
        }

        /// <summary>
        /// Decrease time frame and update intensities  
        /// </summary>
        private void DecreaseTimeFrame()
        {
            Debug.Log("DecreaseTimeFrame");
            if (dependenciesImporter.DecrementBucketIndex())
                UpdateIntensity();
        }

        /// <summary>
        /// Helper class for MinMaxScaling 
        /// </summary>
        private class IntensityMinMaxScaler
        {
            private readonly float min;
            private readonly float max;
            private const float MINIntensity = 0.0F;
            private const float MAXIntensity = 0.05F;
            private const float OutlierIntensity = 0.05F;

            public IntensityMinMaxScaler(float min, float max)
            {
                this.min = min;
                this.max = max;
            }

            internal float Scale(float value)
            {
                float scaled = MINIntensity + (value - min) / (max - min) * (MAXIntensity - MINIntensity);
                return scaled > MAXIntensity ? OutlierIntensity : scaled;
            }
        }
    }
}