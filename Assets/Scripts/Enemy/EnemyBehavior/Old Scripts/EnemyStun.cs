using UnityEngine;
using System.Collections;

public class EnemyStun : MonoBehaviour
{

    [SerializeField]
    private Behaviour[] disableWhileStunned;
    [SerializeField]
    private Animator anim; // the enemys animator 

    public bool isStunned {get; private set;}  // any script can check if the enemy is stunned only this one can set it

    private Coroutine stunRoutine; // the running stun timer so we can restart it
   
   public void Stun(float duration){
    if(stunRoutine != null){
        StopCoroutine(stunRoutine); // already stunned so stop the current stun timer
    }
    stunRoutine = StartCoroutine(StunTimer(duration)); // start a new stun timer
   }

   private IEnumerator StunTimer(float duration){
    isStunned = true;
    foreach (Behaviour script in disableWhileStunned){
        script.enabled = false; // disable the script
    }
    if (anim != null){
        anim.SetFloat("Movement", 0); // stop the animation from playing
        anim.SetBool("Pissy", false); // stop the animation from playing
    }
    yield return new WaitForSeconds(duration); //wait here for x seconds
    foreach (Behaviour script in disableWhileStunned){
        script.enabled = true; // enable the script
    }
    isStunned = false;
    stunRoutine = null;
   }
}
