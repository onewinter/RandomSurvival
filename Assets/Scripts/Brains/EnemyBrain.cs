using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBrain_", menuName = "Enemy Brains/Standard", order = 51)]
public class EnemyBrain : ScriptableObject
{
    
    [SerializeField] protected float chaseRange = 3.0f;
    [SerializeField] protected float patrollingDirectionSwitchTime = 2.5f;
    [SerializeField] protected float patrolRange = 2f;
    [SerializeField] protected float chaseSpeed = 2.5f;
    [SerializeField] protected float patrolSpeed = 3.5f;

    public virtual void OnStart(Enemy enemy)
    {
        enemy.startingPosition = enemy.transform.position;
        enemy.startingForward = enemy.transform.forward;
        enemy.transform.forward = Vector3.right;
        enemy.moveDirection = enemy.transform.forward;
        enemy.originalColor = enemy.material.color;
        
        enemy.rig.useGravity = true;
    }

    protected virtual Vector3 GetPlayerDirection(Enemy enemy)
    {
        // move towards the player on the x and z axes only
        return new Vector3(enemy.player.transform.position.x, enemy.transform.position.y, enemy.player.transform.position.z) -
               enemy.transform.position;
    }

    
    protected virtual void CheckShouldSleep(Enemy enemy)
    {
        // keep chasing if player is still in range
        if (enemy.distanceToPlayer <= chaseRange) return;
        
        //enemy.transform.forward = Vector3.right;
        //enemy.moveDirection = enemy.transform.forward * patrolSpeed;
        SetRandomPatrolTarget(enemy);
        enemy.currentState = EnemyState.Sleeping;
    }

    protected virtual void FacePlayerWhileAttacking(Enemy enemy) => FacePlayer(enemy);
    
    protected void FacePlayer(Enemy enemy) => enemy.transform.forward = GetPlayerDirection(enemy);

    public virtual void DoAttackAction(Enemy enemy)
    {
        FacePlayerWhileAttacking(enemy);

        enemy.moveDirection = enemy.transform.forward * chaseSpeed;
        enemy.moveDirection.y = enemy.rig.velocity.y;
        enemy.charController.Move(enemy.moveDirection);
        
        CheckShouldSleep(enemy);
    }

    protected virtual void CheckShouldAttack(Enemy enemy)
    {
        // don't chase the player if too far away
        if (enemy.distanceToPlayer >= chaseRange) return;
        enemy.moveDirection = enemy.transform.forward * chaseSpeed;
        enemy.currentState = EnemyState.Attacking;
    }

    private void SetRandomPatrolTarget(Enemy enemy)
    {
        // patrol in a new direction within a circle radius from starting position
        var randomOffset = Random.insideUnitCircle * patrolRange;
        var targetPosition = enemy.startingPosition;
        targetPosition.x += randomOffset.x;
        targetPosition.z += randomOffset.y;

        enemy.transform.forward = targetPosition - enemy.transform.position;
        enemy.moveDirection = enemy.transform.forward * patrolSpeed;
    }

    protected virtual void CheckPatrolReverse(Enemy enemy)
    {
        if (enemy.timer <= patrollingDirectionSwitchTime) return;
        enemy.timer = 0;

        SetRandomPatrolTarget(enemy);
        // patrol in a new direction within a circle radius from starting position
        //var randomOffset = Random.insideUnitCircle * patrolRange;
        //var targetPosition = enemy.startingPosition;
        //targetPosition.x += randomOffset.x;
        //targetPosition.z += randomOffset.y;

        //enemy.transform.forward = targetPosition - enemy.transform.position;
        //enemy.moveDirection = enemy.transform.forward;
        //enemy.moveDirection = (targetPosition - enemy.transform.position).normalized;

        //enemy.moveDirection *= -1;
        //enemy.transform.forward = enemy.moveDirection;
    }

    public virtual void DoSleepAction(Enemy enemy)
    {
        //timer += Time.deltaTime;

        enemy.moveDirection.y = enemy.rig.velocity.y;
        enemy.charController.Move(enemy.moveDirection);
    
        CheckPatrolReverse(enemy);
        CheckShouldAttack(enemy);
    }

}
