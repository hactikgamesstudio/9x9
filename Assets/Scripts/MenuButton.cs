using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides visual feedback for menu buttons on mouse interaction.
/// Handles hover effects and click animations.
/// </summary>
public class MenuButton : MonoBehaviour
{
    private Button button;
    private CanvasGroup canvasGroup;
    private Color originalColor;

    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    private void Start()
    {
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        var image = GetComponent<Image>();
        if (image != null)
        {
            originalColor = image.color;
        }

        // Set up button transitions
        var colors = button.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = hoverColor;
        colors.pressedColor = pressedColor;
        button.colors = colors;
    }
}
