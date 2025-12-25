using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime.UI.Shared
{
    /// <summary>
    /// Base class for menu panels. All menu UIs inherit from this to provide
    /// consistent back button behavior and panel management.
    /// </summary>
    public abstract class UIMenuPanel : MonoBehaviour
{
    [SerializeField] protected MenuManager menuManager;

    protected virtual void Start()
    {
        // Can be overridden by derived classes
    }

    /// <summary>
    /// Called when the back button is pressed or navigation returns to previous menu.
    /// </summary>
    public abstract void GoBack();

    /// <summary>
    /// Initializes this panel with a reference to the MenuManager.
    /// Called after instantiation to set up the reference.
    /// </summary>
    public virtual void Initialize(MenuManager manager)
    {
        menuManager = manager;
    }
}
}
