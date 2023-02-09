using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBrainDash_", menuName = "Enemy Brains/Dash Attack", order = 51)]
public class EnemyBrainDash :EnemyBrain
{
    [SerializeField] protected float dashLength = 1.25f;
    [SerializeField] protected float dashPause = .75f;
    [SerializeField] protected Color flashColor = Color.white;

    public override void DoAttackAction(Enemy enemy)
    {
        enemy.dashTimer += Time.deltaTime;
        enemy.pauseTimer += Time.deltaTime;

        // dash for a certain length of time after pausing
        if (enemy.dashTimer > dashLength + dashPause && enemy.pauseTimer > dashPause)
        {
            // reset color after dash ends
            enemy.material.color = enemy.originalColor;
            enemy.dashTimer = 0;
            enemy.pauseTimer = 0;

            // only go back to patrol after a dash has ended
           CheckShouldSleep(enemy);
        }
        else if (enemy.pauseTimer < dashPause)
        {
            // lerp to charging color to show player we're coming
            enemy.material.color = Color.Lerp(enemy.originalColor, flashColor, enemy.pauseTimer/dashPause);
            
            // follow the player while paused
            enemy.transform.forward = GetPlayerDirection(enemy);
            enemy.charController.Pause();
        }
        // otherwise move forward until told not to
        else
        {
            enemy.moveDirection = enemy.transform.forward * chaseSpeed;
            SetMoveDirectionY(enemy);
            enemy.charController.Move(enemy.moveDirection);
        }
    }

    // broken out to override in Flying enemy
    protected virtual void SetMoveDirectionY(Enemy enemy)
    {
        enemy.moveDirection.y = enemy.rig.velocity.y;
    }
}
