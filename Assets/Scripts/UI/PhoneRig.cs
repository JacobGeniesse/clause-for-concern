using UnityEngine;

// Raises and lowers the held phone between two camera-local poses
public class PhoneRig : MonoBehaviour
{
    [SerializeField] float moveDuration = 0.45f; // seconds for the raise/lower move
    [SerializeField] Vector3 hiddenPosition; // below the FOV
    [SerializeField] Vector3 shownPosition; // in front of the camera
    [SerializeField] Vector3 hiddenEuler;
    [SerializeField] Vector3 shownEuler;

    bool initialized;
    bool wantRaised; // player asked for the phone up
    float pose; // 0 = hidden, 1 = shown

    public bool IsRaised => wantRaised;
    public bool IsVisible => wantRaised || pose > 0.001f; // still on screen, including while lowering

    public void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        ApplyPose(0f);
        gameObject.SetActive(false);
    }

    public void SetRaised(bool raised)
    {
        wantRaised = raised;
        if (raised)
        {
            gameObject.SetActive(true);
        }
    }

    // Instant hide, used by pause/startup so we skip the animatiom
    public void SnapHidden()
    {
        wantRaised = false;
        pose = 0f;
        ApplyPose(0f);
        gameObject.SetActive(false);
    }

    // Driven by Minimap because this object is inactive when put away
    public void Tick(float deltaTime)
    {
        if (!initialized || !gameObject.activeSelf)
        {
            return;
        }

        float target = wantRaised ? 1f : 0f;
        float speed = moveDuration > 0.01f ? 1f / moveDuration : 100f;
        pose = Mathf.MoveTowards(pose, target, deltaTime * speed);
        ApplyPose(pose);

        if (!wantRaised && pose <= 0.001f)
        {
            pose = 0f;
            gameObject.SetActive(false);
        }
    }

    void ApplyPose(float t)
    {
        float eased = t * t * (3f - 2f * t);
        transform.localPosition = Vector3.Lerp(hiddenPosition, shownPosition, eased);
        transform.localRotation = Quaternion.Slerp(
            Quaternion.Euler(hiddenEuler),
            Quaternion.Euler(shownEuler),
            eased);
    }
}
