using UnityEngine;

// Hides the screen HUD (health/ammo) while the phone is on screen.
public class HudCanvas : MonoBehaviour
{
    CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void LateUpdate()
    {
        bool show = !Minimap.IsOpen;
        float alpha = show ? 1f : 0f;
        if (Mathf.Approximately(group.alpha, alpha))
        {
            return;
        }

        group.alpha = alpha;
        group.interactable = show;
        group.blocksRaycasts = show;
    }
}
