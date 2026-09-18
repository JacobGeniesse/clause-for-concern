using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] TaskManager taskManager;
    [SerializeField] RectTransform mapRect;
    [SerializeField] RectTransform playerMarker;
    [SerializeField] RectTransform taskMarker;
    [SerializeField] Transform worldMin;
    [SerializeField] Transform worldMax;

    void LateUpdate()
    {
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

    void PlaceMarker(RectTransform marker, Vector3 worldPos)
    {
        Vector2 local = WorldToMap(worldPos);
        marker.localPosition = new Vector3(local.x, local.y, 0f);
    }

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
