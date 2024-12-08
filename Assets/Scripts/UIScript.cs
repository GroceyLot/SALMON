using UnityEngine;
using UnityEngine.UIElements;
public class UIScript : MonoBehaviour
{
    private ProgressBar leftBar;
    private ProgressBar rightBar;
    public float leftValue = 0;
    public float rightValue = 0;
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        leftBar = root.Q<ProgressBar>("LeftBar");
        rightBar = root.Q<ProgressBar>("RightBar");
    }

    void Update()
    {
        leftBar.value = leftValue;
        rightBar.value = rightValue;
        leftBar.style.display = leftValue < 0.01f ? DisplayStyle.None : DisplayStyle.Flex;
        rightBar.style.display = rightValue < 0.01f ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
