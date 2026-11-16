using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class HRAttacks : MonoBehaviour
{
    [SerializeField] private bool attacking; //bool for checking if the enemy is attacking

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

    //Variable for holding the grabAttack coroutine
    private IEnumerator grabAttack;
    private IEnumerator animWait;
    private bool activeAnim = false;

    private bool triedAttacking = true;

    private float throwRadius = 3f;

    private float rotationTarget = 0;

    [SerializeField] private Collider physicalBody;

    private Quaternion storedRotation;
    private Quaternion startRotation;

    private Quaternion finalRotation;
    private float rotTime = 0;

    #region HelperScripts
    //Helper Scripts and other helper components
    private NavMeshAgent agent;
    private EnemyVLManager voiceLines;

    private PlayerHealth playerHealth;
    private PlayerMovement playerMove;
    private Minimap map;

    private HRRepBehavior behaviorTree;
    #endregion

    private void Start()
    {
        storedRotation = transform.rotation;
        finalRotation = transform.rotation;

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

        try
        {
            voiceLines = GetComponent<EnemyVLManager>();
        }
        catch
        {
            throw new ArgumentException("Unable to find EnemyVLManager component!", nameof(HRAttacks));
        }

        try
        {
            behaviorTree = GetComponent<HRRepBehavior>();
        }
        catch
        {
            throw new ArgumentException("Unable to find HRRepBehavior component", nameof(HRAttacks));
        }
    }

    //Function for the enemy trying to grab the player
    public Node.Status GrabAttempt()
    {

        //if (rotationTarget != 0)
        //{
        //    CalcRotation();
        //    swingRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + rotationTarget, transform.rotation.eulerAngles.z);
        //}

        if (animWait == null)
        {
            voiceLines.PlayVoiceLine("attackAttempt");
            animWait = AnimConclusion(2f);
            StartCoroutine(animWait);
        }

        Debug.Log("Trying to Grab");
        if (activeAnim == true)
        {
            //transform.LookAt(playerTrans);

            if (attackCol.bounds.Contains(playerTrans.position))
            {
                rotationTarget = 0;
                agent.isStopped = true; //Stop the agent from moving
                behaviorTree.canAttack = false;
                playerMove.StopPlayer(); //Stop the player
                return Node.Status.SUCCESS; //Pass success
            }
            else
            {
                return Node.Status.RUNNING;
            }
        }
        else
        {
            return Node.Status.FAILURE;
        }
    }

    private void CalcRotation()
    {
        Vector3 playerPos = playerTrans.position - transform.position;

        float currentDif = (Vector3.Dot(transform.forward.normalized, playerPos.normalized));

        currentDif = currentDif * Mathf.Rad2Deg;

        rotationTarget = currentDif;
    }

    public Node.Status SafetyRotation()
    {
        attacking = true; //The enemy is attacking

        map.SetOpen(false);

        //Attach the player to the enemy's arm hitbox for the purposes of the grab
        playerMove.TetherObject(attackCol.transform);

        playerMove.AlterMoveState(false); //prevent the player from moving

        agent.updateRotation = false;
        //Debug.Log(ThrowCheck());

        if (finalRotation == storedRotation)
        {
            float offset = ThrowCheck();
            //Debug.Log(offset);
            finalRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + offset, transform.rotation.eulerAngles.z);
            startRotation = transform.rotation;
            //Debug.Log(transform.rotation.eulerAngles);
            //Debug.Log(finalRotation.eulerAngles);
        }

        transform.rotation = Quaternion.Slerp(startRotation, finalRotation, rotTime);

        rotTime += 1 * Time.deltaTime;

        //transform.rotation = finalRotation;

        //Debug.Log(rotTime);
        //Debug.Log(transform.rotation.eulerAngles);

        if (Quaternion.Angle(transform.rotation, finalRotation) < 0.01f)
        {
            Debug.Log("Gotcha!");
            rotTime = 0;
            agent.updateRotation = true;
            storedRotation = finalRotation;
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.RUNNING;
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
            //Debug.Log("Finished grab attack");
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
        agent.isStopped = false;
        Debug.Log("I hit you!"); //Debug Message
        return Node.Status.SUCCESS;
    }

    //Func for failing the grab if the player isn't in range once the actual grab happens
    public Node.Status FailGrab()
    {
        //if(behaviorTree.chasing == true)
        //{
            return Node.Status.SUCCESS;
        //}
        //else
        //{
        //    return Node.Status.FAILURE;
        //}
    }

    public Node.Status FailAnim()
    {
        if (animWait == null)
        {
            animWait = AnimConclusion(2f);
            StartCoroutine(animWait);
        }


        if (activeAnim == false)
        {
            Debug.Log("I missed >:("); //Debug Message
            agent.isStopped = false; //Stop the agent from moving
            animWait = null;
            voiceLines.PlayVoiceLine("attackFailure");
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.RUNNING;
        }
    }

    private IEnumerator AnimConclusion(float animLength)
    {
        activeAnim = true;
        yield return new WaitForSeconds(animLength);
        activeAnim = false;
        animWait = null;
    }


    //Coroutine for executing a grab attack against the player
    private IEnumerator GrabAttack()
    {

        //SafeThrow();

        if (windupLength > 0)
        {
            yield return new WaitForSeconds(windupLength); //Windup Toss Animation Time
        }
        else
        {
            Debug.LogWarning("windupLength has an invalid time! Using backup value!");
            yield return new WaitForSeconds(1.5f); //Backup in case an invalid time is entered
        }
        voiceLines.PlayVoiceLine("attackSuccess");

        playerMove.ReturnToOriginalParent(); //release the player from the arm hitbox

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

        triedAttacking = false;
        attacking = false; //The enemy is no longer attacking
    }

    //Function for finding a safe angle to throw the player
    private float ThrowCheck()
    {

        float directionsToCheck = 24;
        float angle = 0;
        for(int i = 0; i < directionsToCheck; i++)
        {
            angle += 2 * Mathf.PI / directionsToCheck;
            //RayCastSweep(throwRadius, angle);
            //RayCastSweep(throwRadius, -angle);
            if (RayCastSweep(throwRadius, angle) != Vector3.zero)
            {
                Debug.Log(Vector3.SignedAngle(transform.forward, RayCastSweep(throwRadius, angle), Vector3.up));
                return (Vector3.SignedAngle(transform.forward, RayCastSweep(throwRadius, angle), Vector3.up));
            }
            else if (RayCastSweep(throwRadius, -angle) != Vector3.zero)
            {
                return -angle * directionsToCheck;
            }
        }
        return 0;
    }

    private Vector3 RayCastSweep(float scanRadius, float currentAngle)
    {
        float targetOffsetX = (scanRadius * Mathf.Cos(currentAngle)) * transform.position.x;
        float targetOffsetZ = (scanRadius * Mathf.Sin(currentAngle)) * transform.position.z;


        Vector3 newTarget = new Vector3(targetOffsetX , transform.position.y, targetOffsetZ);


        Debug.Log(currentAngle);

        //Debug.Log(newTarget);

        if (Physics.Raycast(transform.position, newTarget, out RaycastHit hit, scanRadius, ~(1 << 8)))
        {
            Debug.DrawRay(transform.position, newTarget, Color.red, 1f);
            return Vector3.zero;
        }
        else
        {
            Debug.DrawRay(transform.position, newTarget, Color.green, 1f);
            return newTarget;
        }
    }
}
