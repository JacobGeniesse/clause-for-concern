using UnityEngine;
using UnityEngine.InputSystem;

// Tab toggle for the phone
public class Minimap : MonoBehaviour
{
    public static bool IsOpen { get; private set; } // true while the phone is on screen

    [SerializeField] RectTransform mapRect; // floorplan image the markers sit on
    [SerializeField] RectTransform playerMarker;
    [SerializeField] RectTransform taskMarker;
    [SerializeField] Transform worldMin; // world corner that maps to the image's min
    [SerializeField] Transform worldMax;
    [SerializeField] GameObject mapRoot; // HeldPhone object
    [SerializeField] InputActionAsset masterList;
    [SerializeField] Transform player;
    [SerializeField] TaskManager taskManager;

    InputAction mapToggle;
    PhoneRig phoneRig;

    void Start()
    {
        if (mapRoot != null)
        {
            phoneRig = mapRoot.GetComponent<PhoneRig>();
            if (phoneRig != null)
            {
                phoneRig.Initialize();
            }
        }

        if (masterList != null)
        {
            mapToggle = masterList.FindAction("Map");
            if (mapToggle != null)
            {
                mapToggle.Enable();
            }
        }

        SetOpen(false, true);
    }

    void OnDisable()
    {
        if (IsOpen)
        {
            SetOpen(false, true);
        }
    }

    void Update()
    {
        if (WasMapPressed() && phoneRig != null)
        {
            SetOpen(!phoneRig.IsRaised);
        }
    }

    void LateUpdate()
    {
        if (phoneRig != null)
        {
            phoneRig.Tick(Time.deltaTime);
            IsOpen = phoneRig.IsVisible;
        }

        if (!IsOpen)
        {
            return;
        }

        if (player != null && playerMarker != null)
        {
            PlaceMarker(playerMarker, player.position);
            playerMarker.localEulerAngles = new Vector3(0f, 0f, -player.eulerAngles.y);
        }

        if (taskMarker == null)
        {
            return;
        }

        Transform location = taskManager != null ? taskManager.Location : null;
        bool hasLocation = location != null;
        if (taskMarker.gameObject.activeSelf != hasLocation)
        {
            taskMarker.gameObject.SetActive(hasLocation);
        }

        if (hasLocation)
        {
            PlaceMarker(taskMarker, location.position);
        }
    }

    bool WasMapPressed()
    {
        return mapToggle != null && mapToggle.WasPressedThisFrame();
    }

    public void SetOpen(bool open)
    {
        SetOpen(open, false);
    }

    // immediate = true skips the raise/lower animation.
    public void SetOpen(bool open, bool immediate)
    {
        if (phoneRig == null)
        {
            return;
        }

        if (immediate && !open)
        {
            phoneRig.SnapHidden();
        }
        else
        {
            phoneRig.SetRaised(open);
        }

        IsOpen = phoneRig.IsVisible;

        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;
    }

    void PlaceMarker(RectTransform marker, Vector3 worldPos)
    {
        Vector2 local = WorldToMap(worldPos);
        marker.localPosition = new Vector3(local.x, local.y, 0f);
    }

    // Maps world XZ into the floorplan rect. worldMin/Max should match the image corners.
    Vector2 WorldToMap(Vector3 worldPos)
    {
        if (mapRect == null || worldMin == null || worldMax == null)
        {
            return Vector2.zero;
        }

        float nx = Mathf.InverseLerp(worldMin.position.x, worldMax.position.x, worldPos.x);
        float ny = Mathf.InverseLerp(worldMin.position.z, worldMax.position.z, worldPos.z);
        Rect rect = mapRect.rect;
        return new Vector2(
            Mathf.Lerp(rect.xMin, rect.xMax, nx),
            Mathf.Lerp(rect.yMin, rect.yMax, ny));
    }

    void OnDrawGizmosSelected()
    {
        if (worldMin == null || worldMax == null)
        {
            return;
        }

        Vector3 min = worldMin.position;
        Vector3 max = worldMax.position;
        float y = min.y;
        Vector3 sw = new Vector3(min.x, y, min.z);
        Vector3 se = new Vector3(max.x, y, min.z);
        Vector3 ne = new Vector3(max.x, y, max.z);
        Vector3 nw = new Vector3(min.x, y, max.z);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(sw, se);
        Gizmos.DrawLine(se, ne);
        Gizmos.DrawLine(ne, nw);
        Gizmos.DrawLine(nw, sw);
    }
}
