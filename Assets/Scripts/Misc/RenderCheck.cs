using UnityEngine;

public class RenderCheck : MonoBehaviour
{
    private Renderer cameraRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraRenderer = GetComponentInChildren<Renderer>();
    }

    public bool SightCheck()
    {
        if (cameraRenderer.isVisible)
        {
            //Debug.Log(true);
            return(true);
        }
        else
        {
            //Debug.Log(false);
            return(false);
        }
    }
}
