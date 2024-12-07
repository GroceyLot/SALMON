using UnityEngine;
using UnityEngine.UIElements;
public class UIScript : MonoBehaviour
{
    private ProgressBar leftBar;
    private ProgressBar rightBar;
    private ProgressBar middleBar;
    public float leftValue = 0;
    public float rightValue = 0;
    public float middleValue = 0;
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        leftBar = root.Q<ProgressBar>("LeftBar");
        rightBar = root.Q<ProgressBar>("RightBar");
        middleBar = root.Q<ProgressBar>("MiddleBar");
    }

    void Update()
    {
        leftBar.value = leftValue;
        rightBar.value = rightValue;
        middleBar.value = middleValue;
        leftBar.style.display = leftValue == 0 ? DisplayStyle.None : DisplayStyle.Flex;
        rightBar.style.display = rightValue == 0 ? DisplayStyle.None : DisplayStyle.Flex;
        middleBar.style.display = middleValue < 0.01f ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
