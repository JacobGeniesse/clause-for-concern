using UnityEngine;
using UnityEngine.AI;

public class BreakableWall : MonoBehaviour
{
    [Tooltip("Game Object that holders the unbroken wall model.")]
    [SerializeField] private GameObject fixedWall;

    [Tooltip("Game Object that holders the broken wall model.")]
    [SerializeField] private GameObject brokenWall;

    [Tooltip("Particle effect that shows wall chunks breaking off of the wall.")]
    [SerializeField] private GameObject chunkParticles;

    private bool broken = false;

    private void Start()
    {
        FixWall(); //Fix the wall when the scene loads in
    }

    //Trigger check for breaking the wall
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Enemy") //If it is an enemy which is colliding with the wall
        {
            if (broken == false) //If the wall isn't already broken
            {
                BreakWall(); //Break the wall
            }
        }
    }

    //Func for breaking the wall
    public void BreakWall()
    {
        fixedWall.SetActive(false); //Set the fixed wall model to be inactive
        brokenWall.SetActive(true); //Set the broken wall model to be active
        chunkParticles.SetActive(true); //Activate the chunk particles
        broken = true; //Set broken to true
    }

    //Func for fixing the wall
    public void FixWall()
    {
        fixedWall.SetActive(true); //Set the fixed wall model to be active
        brokenWall.SetActive(false); //Set the broken wall model to be inactive
        broken = false; //Set broken to true
    }
}