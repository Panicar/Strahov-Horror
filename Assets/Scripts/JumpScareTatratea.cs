using UnityEngine;

public class JumpScareController : MonoBehaviour
{
    [Header("Player Setup")]
    [Tooltip("Drag the Main Camera (child of your player) here")]
    public Transform playerCamera;
    
    [Tooltip("Drag your Player GameObject here to disable the PlayerController")]
    public PlayerController playerScript; 

    [Header("Enemy Setup")]
    public Transform enemy;
    public Animator enemyAnimator;
    [Tooltip("The exact name of the Trigger parameter in your Animator")]
    public string runAnimationTrigger = "Run";

    [Tooltip("The exact name of the Idle trigger in your Animator")]
    public string idleAnimationTrigger = "Idle";

    public float rushSpeed = 12f;
    [Tooltip("How close the enemy gets before stopping")]
    public float stopDistance = 1.5f;

    private bool isTriggered = false;
    private bool hasReachedPlayer = false;


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object entered trigger: " + other.gameObject.name);

        // Check if the object entering the trigger is the Player
        if (other.CompareTag("Player") && !isTriggered)
        {
            ExecuteJumpScare();
        }
    }

    void ExecuteJumpScare()
    {
        isTriggered = true;

        // 1. Completely freeze the player by disabling your custom controller
        if (playerScript != null) 
        {
            playerScript.enabled = false;
        }

        // 2. Snap the Camera to look at the enemy's face
        // We add Vector3.up * 1.5f so the camera looks at the chest/face, not the feet.
        playerCamera.LookAt(enemy.position + Vector3.up * 1.5f);

        // 3. Trigger the Running Animation
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger(runAnimationTrigger);
        }

    }

    // Add this new variable right below your "private bool isTriggered = false;"

    void Update()
    {
        if (isTriggered && !hasReachedPlayer)
        {
            Vector3 flatEnemyPos = new Vector3(enemy.position.x, 0f, enemy.position.z);
            Vector3 flatPlayerPos = new Vector3(playerCamera.position.x, 0f, playerCamera.position.z);
            float distance = Vector3.Distance(flatEnemyPos, flatPlayerPos);

            if (distance > stopDistance)
            {
                Vector3 targetPosition = new Vector3(playerCamera.position.x, enemy.position.y, playerCamera.position.z);
                enemy.position = Vector3.MoveTowards(enemy.position, targetPosition, rushSpeed * Time.deltaTime);
                enemy.LookAt(targetPosition);
            }
            else
            {
                // The enemy has arrived!
                hasReachedPlayer = true;
                
                if (enemyAnimator != null)
                {
                    // THE NUCLEAR OPTION
                    // We bypass triggers and force the exact name of the Animation State.
                    // Replace "Idle" with the EXACT name of the box in your Animator window.
                    enemyAnimator.CrossFade("Idle", 0.1f); 
                }
            }


            if(hasReachedPlayer)
            {
                if (enemyAnimator != null)
                {
                    // THE NUCLEAR OPTION
                    // We bypass triggers and force the exact name of the Animation State.
                    // Replace "Idle" with the EXACT name of the box in your Animator window.
                    enemyAnimator.CrossFade("IdleAnim", 0.1f); 
                }
            }

        }
    }
}