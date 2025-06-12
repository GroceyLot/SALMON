using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class UIScript : MonoBehaviour
{
    private ProgressBar leftBar;
    private ProgressBar rightBar;
    private VisualElement screenOverlay;

    public float leftValue = 0;
    public float rightValue = 0;

    private bool isOverlayVisible = false;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Find UI elements
        leftBar = root.Q<ProgressBar>("LeftBar");
        rightBar = root.Q<ProgressBar>("RightBar");
        screenOverlay = root.Q<VisualElement>("ScreenOverlay");

        // Attach button events
        root.Q<Button>("ResetSaveButton").clicked += OnResetSave;
        root.Q<Button>("SaveQuitButton").clicked += OnSaveQuit;
        root.Q<Button>("TestLevel").clicked += () => SceneManager.LoadScene("TestScene");
        root.Q<Button>("CityLevel").clicked += () => SceneManager.LoadScene("OutsideScene");

        // Initially hide the overlay
        screenOverlay.style.display = DisplayStyle.None;
    }

    void Update()
    {
        // Update progress bars
        leftBar.value = leftValue;
        rightBar.value = rightValue;

        // Control visibility of progress bars
        leftBar.style.display = leftValue < 0.01f ? DisplayStyle.None : DisplayStyle.Flex;
        rightBar.style.display = rightValue < 0.01f ? DisplayStyle.None : DisplayStyle.Flex;

        // Toggle overlay visibility when pressing Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOverlay();
        }
    }

    // Toggles the overlay's visibility
    private void ToggleOverlay()
    {
        isOverlayVisible = !isOverlayVisible;
        screenOverlay.style.display = isOverlayVisible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    // Button event handlers
    private void OnResetSave()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void OnSaveQuit()
    {
#if UNITY_EDITOR
        // Exit play mode in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Close the application
        Application.Quit();
#endif
    }
}
