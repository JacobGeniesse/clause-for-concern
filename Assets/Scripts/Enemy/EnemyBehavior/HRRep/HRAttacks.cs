using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using UnityEngine.AI;

public class HRAttacks : MonoBehaviour
{
    private bool attacking; //bool for checking if the enemy is attacking

    [Tooltip("Damage dealt to the player by the grab.")]
    [SerializeField] private float damage = 20;

    [Tooltip("Object that represents the grab attack hitbox.")]
    [SerializeField] private Collider attackCol;

    [Header("Player Throw Variables")]
    [Tooltip("Force the player is thrown with.")]
    [SerializeField] private float pushForce = 50;

    [Tooltip("Duration the player is thrown for.")]
    [SerializeField] private float pushDuration = 0.5f;

    [Header("Attack Timing Variables")]
    [Tooltip("Period of time before the player is thrown.")]
    [SerializeField] private float windupLength = 1.5f;

    [Tooltip("Period of time after the player is thrown before the enemy can start moving again.")]
    [SerializeField] private float cooldownLength = 2f;
    private Transform playerTrans; //Player's transform position

    [Header("Sound Variables")]
    [Tooltip("Storage for enemy Voice Lines")]
    [SerializeField] private Voicelines hrVoiceLines;

    //Variable for holding the grabAttack coroutine
    private IEnumerator grabAttack;


    //Helper Scripts and other helper components
    private PlayerHealth playerHealth;
    private PlayerMovement playerMove;
    private NavMeshAgent agent;
    private Minimap map;

    private void Start()
    {
        //Assign necessary script vars with try catch checks

        //Get the player body component
        playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();

        //Assign playerHealth var
        try
        {
            playerHealth = playerTrans.GetComponent<PlayerHealth>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign player health var", nameof(HRAttacks));
        }

        //Assign playermovement variable
        try
        {
            playerMove = playerTrans.GetComponent<PlayerMovement>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign player movement var", nameof(HRAttacks));
        }

        //Assign reference to the enemy's nav mesh agent
        try
        {
            agent = GetComponent<NavMeshAgent>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign Nav Mesh Agent var", nameof(HRAttacks));
        }

        //Assign reference to the player's Minimap for deactivating when they get grabbed
        try
        {
            map = FindAnyObjectByType<Minimap>();
        }
        catch
        {
            throw new ArgumentException("Unable to find Minimap reference.", nameof(HRAttacks));
        }
    }


    //Function for the enemy trying to grab the player
    public Node.Status GrabAttempt()
    {
        Debug.Log("Trying to attack");
        agent.destination = transform.position; //Stop the agent from moving
        //agent.velocity = Vector3.zero;//Extra layer of stoppage

        //AudioManager.Instance.Play3DSFX(grabAttemptGrunt, transform.position, 0.5f); //Play a sound using the audio manager
        //If the player is within the grab hitbox
        if (attackCol.bounds.Contains(playerTrans.position))
        {
            playerMove.StopPlayer(); //Stop the player
            return Node.Status.SUCCESS; //Pass success
        }
        else
        {
            return Node.Status.FAILURE; //Otherwise pass failure and move on to the next func
        }

    }

    //Node for grabbing and throwing the player
    public Node.Status GrabAndThrow()
    {
        if (grabAttack == null)
        {
            Debug.Log("Get a load of this!"); //Debug message showing that the attack worked
            grabAttack = GrabAttack(); //Set the active grab attack
            StartCoroutine(GrabAttack()); //Start the grab attack coroutine
        }

        //Check for if the attack sequence is still happening
        if (attacking == false)
        {
            grabAttack = null;
            return Node.Status.SUCCESS; //After the attack is done, end the sequence
        }
        else
        {
            return Node.Status.RUNNING; //If the attack is still underway, keep running this node
        }

    }

    //Func for ending a grab sequence
    public Node.Status EndGrab()
    {
        //Debug.Log("I hit you!"); //Debug Message
        return Node.Status.SUCCESS;
    }

    //Func for failing the grab if the player isn't in range once the actual grab happens
    public Node.Status FailGrab()
    {
        // Debug.Log("I missed >:("); //Debug Message
        //AudioManager.Instance.Play3DSFX(missedDisappointment, transform.position, 0.5f); //Play a sound using the audio manager
        return Node.Status.SUCCESS;
    }

    //Coroutine for executing a grab attack against the player
    private IEnumerator GrabAttack()
    {
        attacking = true; //The enemy is attacking

        map.SetOpen(false);

        //Attach the player to the enemy's arm hitbox for the purposes of the grab
        playerMove.TetherObject(attackCol.transform);

        playerMove.AlterMoveState(false); //prevent the player from moving

        if(windupLength > 0)
        {
            yield return new WaitForSeconds(windupLength); //Windup Toss Animation Time
        }
        else
        {
            Debug.LogWarning("windupLength has an invalid time! Using backup value!");
            yield return new WaitForSeconds(1.5f); //Backup in case an invalid time is entered
        }

        playerTrans.parent = null; //release the player from the arm hitbox

        //AudioManager.Instance.Play3DSFX(chuckGrunt, transform.position, 0.5f); //Play a sound using the audio manager

        playerMove.Push(transform.forward * pushForce, pushDuration); //Push the player in the direction they were hit, adjust vars as needed for proper push
        
        playerHealth.TakeDamage(damage); //Have the player take damage as a result of the throw
        
        if(cooldownLength > 0)
        {
            yield return new WaitForSeconds(cooldownLength); //Cooldown Animation Time
        }
        else
        {
            Debug.LogWarning("cooldownLength has an invalid time! Using backup value!");
            yield return new WaitForSeconds(2); //Backup in case an invalid time is entered
        }

        attacking = false; //The enemy is no longer attacking
    }
}
