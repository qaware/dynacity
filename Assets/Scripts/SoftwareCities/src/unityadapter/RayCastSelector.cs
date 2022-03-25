using UnityEngine;
using UnityEngine.Serialization;

namespace SoftwareCities.unityadapter
{
    /// <summary>
    /// Selector to highlight the selected object and show the name in a UI overlay.
    /// Bind this selector to the main camera or to the players VR camera.
    /// </summary>
    public class RayCastSelector : MonoBehaviour
    {
        [FormerlySerializedAs("SelectedObject")] public string selectedObject = "";

        void Update()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
            {
                GameObject selectedObject = hit.transform.gameObject;
                string name = selectedObject.name; // the class / package name
                this.selectedObject = name;
            }
            else
            {
                selectedObject = "";
            }
        }
    }
}