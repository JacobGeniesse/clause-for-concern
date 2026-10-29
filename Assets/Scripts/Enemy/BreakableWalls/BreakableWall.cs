using UnityEngine;
using UnityEngine.AI;

public class BreakableWall : MonoBehaviour
{
    private NavMeshObstacle wallHitBox;


    //private MeshRenderer mat;


    //Vars for model swap
    private MeshFilter wallModel;
    private Mesh fixedWall;
    private Mesh brokenWall;


    public void BreakWall()
    {
        //Testing implementation
        this.gameObject.SetActive(false);
        
        
        //Actual Implementation
        //wallHitBox.enabled = false;
        //wallModel.mesh = brokenWall;
    }

    public void FixWall()
    {
        //Testing implementation
        this.gameObject.SetActive(true);
    }
}
