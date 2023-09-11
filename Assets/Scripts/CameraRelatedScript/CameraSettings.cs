using UnityEngine;

namespace CameraRelatedScript
{
    public class CameraSettings : MonoBehaviour
    {
        private bool isSettingsOpen = false;
        private bool showPerformanceData = false;

        private int qualityIndex = 0;
        private float fieldOfView = 60f;

        private void Start()
        {
            qualityIndex = QualitySettings.GetQualityLevel();
            fieldOfView = Camera.main.fieldOfView;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isSettingsOpen = !isSettingsOpen;

                if (isSettingsOpen)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    Time.timeScale = 0;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    Time.timeScale = 1;
                }
            }
        }

        private void OnGUI()
        {
            if (isSettingsOpen)
            {
                GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 300));

                GUILayout.Label("Settings");

                GUILayout.Label("Graphics Settings");
                qualityIndex = GUILayout.SelectionGrid(qualityIndex, QualitySettings.names, 1);

                GUILayout.Label("Camera Settings");
                fieldOfView = GUILayout.HorizontalSlider(fieldOfView, 30f, 100f);

                showPerformanceData = GUILayout.Toggle(showPerformanceData, "Show Performance Data");

                if (showPerformanceData)
                {
                    GUILayout.Label("FPS: " + (1 / Time.unscaledDeltaTime).ToString("F1"));
                    GUILayout.Label("Quality: " + QualitySettings.names[qualityIndex]);
                }

                GUILayout.EndArea();
            }
        }
    }
}
