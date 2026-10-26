using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class OGAttacks : MonoBehaviour
{
    #region Attacking Vars
    private bool attacking = false; //Is the enemy attacking?
    private IEnumerator attackingCoroutine = null; //slot for the active attacking coroutine
    [SerializeField] private Collider attackCol; //Reference to the enemy's grab range
    [SerializeField] private float damageAmount = 10; //Amount of damage dealt by the enemy
    #endregion

    #region Lunging Vars
    private IEnumerator lungeTimer = null; //slot for the active lunging routine
    private bool lunging = false; //Is the enemy lunging?
    [SerializeField] private float lungeDuration = 2; //Length of the enemy's lunge
    #endregion

    #region Lunge Cooldown Vars
    private bool lungeCooldown = false; //Is the enemy stuck in lunge cooldown?
    private IEnumerator cooldownCoroutine = null; //slot for the active lunging cooldown

    [SerializeField] private float waitingPeriod = 2; //Time the ghoul needs to wait before moving after a lunge
    #endregion

    #region HelperScripts
    private Sight sight; //Sight script
    private MoveToPoint move; //movement script
    private PlayerHealth playerHealth; //player health script
    private PlayerMovement playerMove; //player movementscript
    private Transform playerTrans; //player transform
    private NavMeshAgent agent; //This object's nav mesh agent
    private AgentSpeedChanger speedControl; //speed adjustment script
    private OGGrab grabAttackScript; //Grab range script
    #endregion

    private void Start()
    {
        //Assign the player's transform
        try
        {
            playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }
        catch
        {
            throw new ArgumentException("Unable to obtain player body component", nameof(OfficeGhoulBehavior));
        }

        //Assign the player movement script
        try
        {
            playerMove = playerTrans.gameObject.GetComponent<PlayerMovement>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign PlayerMovement component", nameof(OfficeGhoulBehavior));
        }

        //Assign the player's health script
        try
        {
            playerHealth = playerTrans.gameObject.GetComponent<PlayerHealth>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign PlayerHealth component", nameof(OfficeGhoulBehavior));
        }

        //Assign the sight script
        try
        {
            sight = GetComponentInChildren<Sight>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign Sight component", nameof(OfficeGhoulBehavior));
        }

        //Assign the grab attack script
        try
        {
            grabAttackScript = GetComponentInParent<OGGrab>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign OGGrab script.", nameof(OfficeGhoulBehavior));
        }

        //Assign the movement script
        try
        {
            move = GetComponentInParent<MoveToPoint>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign MoveToPoint component", nameof(OfficeGhoulBehavior));
        }

        //Assign the speed control script
        try
        {
            speedControl = GetComponentInParent<AgentSpeedChanger>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign speed control script.", nameof(OfficeGhoulBehavior));
        }

        //Assign the nav mesh agent script
        try
        {
            agent = GetComponentInParent<NavMeshAgent>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign NavMeshAgent script.", nameof(OfficeGhoulBehavior));
        }
    }


    //Node for the sight check
    public Node.Status Look()
    {
        //Check if the player is visible and if they can be grabbed currently
        if (sight.SightCheck(playerTrans) && playerMove.canGrab == true)
        {
            //If so
            move.MoveToPOI(agent, playerTrans.position); //Start moving towards the player's position

            //Check if the player is within lunge range
            if (grabAttackScript.GrabCheck(playerTrans))
            {
                return Node.Status.SUCCESS; // success if true
            }
            else
            {
                return Node.Status.RUNNING; //Keep running this node if false
            }
        }
        else
        {
            //If not
            return Node.Status.FAILURE; //Fail the check
        }
    }

    //Script for lunging
    public Node.Status Lunge()
    {
        //start the lunging coroutine if it hasn't already begun
        if (lungeTimer == null)
        {
            lungeTimer = LungeTimer(); //set the coroutine holder equal to lunge timer
            StartCoroutine(lungeTimer); //Start coroutine holder
        }

        move.MoveToPOI(agent, playerTrans.position); //Move towards the player

        //If the attack collider overlaps with the player, declare success
        if (attackCol.bounds.Contains(playerTrans.position))
        {
            StopCoroutine(lungeTimer); //Stop the active lunging coroutine
            lungeTimer = null; //Clear the lunge timer coroutine
            lunging = false; //Make lunging false
            return Node.Status.SUCCESS; //Declare success
        }
        else
        {
            //If not check if the lunging coroutine has finished
            if (lunging == false)
            {
                //If the coroutine is finish continue to the next func
                //Debug.Log("Stop");
                lungeTimer = null; //Clear the func
                return Node.Status.SUCCESS; //Declare success
            }
            else
            {
                return Node.Status.RUNNING; //If not, continue moving towards the player
            }
        }
    }

    //Coroutine for the lunge's length
    public IEnumerator LungeTimer()
    {
        lunging = true; //Declare the enemy lunging
        //Debug.Log("Go");
        speedControl.ChangeAgentSpeed(1); //Set the agent's speed to its fast speed
        yield return new WaitForSecondsRealtime(lungeDuration); //Wait lunge duration seconds
        lunging = false; //Declare the enemy to not be lunging any more
    }

    //Func for checking it the player is within the grab range
    public Node.Status GrabCheck()
    {
        if (attackCol.bounds.Contains(playerTrans.position) && playerMove.canGrab == true)
        {
            //Debug.Log("true");
            return Node.Status.SUCCESS; //Pass the check if the player is within that range
        }
        else
        {
            return Node.Status.FAILURE; //Fail the check if the player is outside that range
        }
    }

    public Node.Status Attack()
    {
        if (attackingCoroutine == null)
        {
            attackingCoroutine = AttackCoroutine(); //Set the active grab attack
            StartCoroutine(attackingCoroutine); //Start the grab attack coroutine
        }

        //Check for if the attack sequence is still happening
        if (attacking == false)
        {
            speedControl.ChangeAgentSpeed(0);
            attackingCoroutine = null;
            return Node.Status.SUCCESS; //After the attack is done, end the sequence
        }
        else
        {
            return Node.Status.RUNNING; //If the attack is still underway, keep running this node
        }

    }

    //Attacking coroutine
    public IEnumerator AttackCoroutine()
    {
        //Debug.Log("Violence"); //Debug Message
        attacking = true; //Set attacking to true
        agent.destination = transform.position; //Stop the agent's movement

        playerMove.AlterMoveState(false); //Stop the player from moving
        playerMove.StopPlayer(); //Stop the player's movement
        playerTrans.parent = attackCol.transform; //Tether the player to the enemy
        playerTrans.position = attackCol.transform.position; //Lock the player to the animation hitbox

        playerHealth.TakeDamage(damageAmount); //Deal damage to the player

        yield return new WaitForSeconds(1.5f); //Wait for the breakout anim to finish
        playerTrans.parent = null; //Let the player go
        playerMove.AlterMoveState(true); //Let the player move again

        yield return new WaitForSeconds(1.5f); //Wait for the grace period anim to finish
        attacking = false; //Enemy is no longer attacking
    }

    //Fallback node if the grab didn't work
    public Node.Status FailureConfirmation()
    {
        //Debug.Log("Shucks I missed.");
        return Node.Status.SUCCESS;
    }

    //Node for failing a grab
    public Node.Status GrabFail()
    {
        speedControl.ChangeAgentSpeed(0); //Reset agent speed

        //Declare the lunge cooldown coroutine
        if (cooldownCoroutine == null)
        {
            cooldownCoroutine = LungeCooldown();
            StartCoroutine(cooldownCoroutine);
        }

        //If the cooldown coroutine has finished, end the node
        if (lungeCooldown == false)
        {
            cooldownCoroutine = null; //Clear the cooldown coroutine
            //Debug.Log("Here I come!");
            return Node.Status.SUCCESS; //End the node
        }
        else
        {
            return Node.Status.RUNNING; //If the coroutine is still running continue this func
        }
    }

    //Coroutine for waiting out the lunge cooldown
    private IEnumerator LungeCooldown()
    {
        lungeCooldown = true; //In the cooldown anim
        agent.destination = transform.position; //Stop the agent from moving
        yield return new WaitForSeconds(waitingPeriod); //Wait the length of the cooldown
        lungeCooldown = false; //Cleared the cooldown anim
    }
}
