using UnityEngine;

namespace SoftwareCities.unityadapter
{
    public class EventManager : MonoBehaviour
    {
        public delegate void NextTimeFrame();

        public static event NextTimeFrame OnNextTimeFrame;

        public delegate void LastTimeFrame();

        public static event LastTimeFrame OnLastTimeFrame;

        private void OnEnable()
        {
            // Default settings? 
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnNextTimeFrame?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnLastTimeFrame?.Invoke();
            }
        }
    }
}