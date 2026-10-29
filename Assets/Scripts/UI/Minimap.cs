using UnityEngine;
using UnityEngine.InputSystem;

// Tab toggle for the phone
public class Minimap : MonoBehaviour
{
    public static bool IsOpen { get; private set; } // true while the phone is on screen

    private Transform player;
    private TaskManager taskManager;
    [SerializeField] RectTransform mapRect; // floorplan image the markers sit on
    [SerializeField] RectTransform playerMarker;
    [SerializeField] RectTransform taskMarker;
    [SerializeField] Transform worldMin; // world corner that maps to the image's min
    [SerializeField] Transform worldMax;
    [SerializeField] GameObject mapRoot; // HeldPhone object
    [SerializeField] InputActionAsset masterList;

    InputAction mapToggle;
    PhoneRig phoneRig;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }

        if (taskManager == null)
        {
            taskManager = GameObject.Find("GameManager").GetComponent<TaskManager>();
        }

        if (mapRoot != null)
        {
            phoneRig = mapRoot.GetComponent<PhoneRig>();
            if (phoneRig == null)
            {
                phoneRig = mapRoot.GetComponentInParent<PhoneRig>(true);
            }

            if (phoneRig != null)
            {
                phoneRig.Initialize();
            }
        }

        if (masterList != null)
        {
            try
            {
                mapToggle = masterList["Map"];
                mapToggle.Enable();
            }
            catch
            {
                mapToggle = null;
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
        if (WasMapPressed())
        {
            bool raised = phoneRig != null ? phoneRig.IsRaised : IsOpen;
            SetOpen(!raised);
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
        if (mapToggle != null)
        {
            return mapToggle.WasPressedThisFrame();
        }

        return Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame;
    }

    public void SetOpen(bool open)
    {
        SetOpen(open, false);
    }

    // immediate = true skips the raise/lower animation.
    public void SetOpen(bool open, bool immediate)
    {
        if (phoneRig != null)
        {
            if (immediate && !open)
            {
                phoneRig.SnapHidden();
            }
            else
            {
                phoneRig.SetRaised(open);
            }

            IsOpen = phoneRig.IsVisible;
        }
        else
        {
            IsOpen = open;
            if (mapRoot != null)
            {
                mapRoot.SetActive(open);
            }
        }

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
