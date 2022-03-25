using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SoftwareCities.unityadapter
{
    public class MouseClick : MonoBehaviour
    {
        private GameObject labelGo;
        
        public delegate void ComponentClick(string componentName);

        public static event ComponentClick OnComponentClick;
        
        public delegate void ComponentRelease();

        public static event ComponentRelease OnComponentRelease;

        /// <summary>
        /// Show the class label when the mouse is pressed on a gameObject (class or package)
        /// </summary>
        private void OnMouseDown()
        {
            CreateLabel();
            OnComponentClick?.Invoke(gameObject.transform.parent.gameObject.name.Split('.').Last());
        }

        /// <summary>
        /// Create a UI label for the clicked class
        /// </summary>
        private void CreateLabel()
        {
            // Canvas
            var infoCanvas = CreateCanvas();

            // Panel
            var panel = CreatePanel(infoCanvas);

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
            Text text = myText.AddComponent<Text>();
            text.font = Resources.Load<Font>("Fonts/Montserrat-Medium");
            text.text = gameObject.transform.parent.gameObject.name.Split('.').Last();
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;
            text.resizeTextForBestFit = true;

            // Text position
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.localPosition = new Vector3(0, 0, 0);
            textRect.sizeDelta = new Vector2(300, 150);
            textRect.transform.SetParent(panel.transform);
        }

        /// <summary>
        /// Add a panel to a given canvas
        /// </summary>
        /// <param name="canvas">the canvas to which the panel will be added</param>
        /// <returns>the created panel</returns>
        private static GameObject CreatePanel(Canvas canvas)
        {
            GameObject panel = new GameObject();
            panel.AddComponent<CanvasRenderer>();
            panel.AddComponent<RectTransform>();
            panel.AddComponent<Image>();
            panel.transform.SetParent(canvas.transform);
            Image image = panel.GetComponent<Image>();
            image.color = new Color(255, 255, 255, 0.6F);
            image.sprite = Resources.Load<Sprite>("class-label");
            image.type = Image.Type.Sliced;
            image.fillCenter = true;

            // Panel position 
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5F, 0);
            panelRect.anchorMax = new Vector2(0.5F, 0);
            panelRect.anchoredPosition = new Vector3(0, 150, 0);
            panelRect.sizeDelta = new Vector2(300, 150);
            panelRect.transform.SetParent(panel.transform);
            return panel;
        }

        /// <summary>
        /// Creates a new canvas as a container for the class label
        /// </summary>
        /// <returns>the created canvas</returns>
        private Canvas CreateCanvas()
        {
            labelGo = new GameObject {name = "Component Label"};
            labelGo.AddComponent<Canvas>();
            if (Camera.main != null)
            {
                Transform cameraTransform = Camera.main.transform;
                labelGo.transform.position = cameraTransform.position + cameraTransform.forward * 0.5F;
            }

            Canvas infoCanvas = labelGo.GetComponent<Canvas>();
            infoCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            labelGo.AddComponent<CanvasScaler>();
            labelGo.AddComponent<GraphicRaycaster>();
            return infoCanvas;
        }

        /// <summary>
        /// Destroy the class label when the mouse is raised 
        /// </summary>
        private void OnMouseUp()
        {
            Destroy(labelGo);
            OnComponentRelease?.Invoke();
        }
    }
}