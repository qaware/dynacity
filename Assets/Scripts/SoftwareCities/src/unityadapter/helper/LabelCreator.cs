using UnityEngine;
using UnityEngine.UI;

namespace EasyCurvedLine
{
    public class LabelCreator
    {
        private static Text text;

        /// <summary>
        /// Create a UI label for the clicked class
        /// </summary>
        public static void CreateLabel(string labelText)
        {
            // Canvas
            Canvas infoCanvas = CreateCanvas();

            // Panel
            GameObject panel = CreatePanel(infoCanvas);

            // Text
            CreateText(panel, labelText);
        }

        public static void UpdateLabel(string labelText)
        {
            if (text != null)
            {
                text.text = labelText; 
            }
        }

        /// <summary>
        /// Add a text to the given panel, center alignment
        /// </summary>
        /// <param name="panel">the panel to which the text will be added</param>
        /// <param name="labelText">that should be shown in the canvas</param>
        private static void CreateText(GameObject panel, string labelText)
        {
            GameObject myText = new GameObject();
            myText.transform.SetParent(panel.transform);
            myText.name = "Component name";
            text = myText.AddComponent<Text>();
            text.font = Resources.Load<Font>("Fonts/Montserrat-Medium");
            text.text = labelText;
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
        private static Canvas CreateCanvas()
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
    }
}