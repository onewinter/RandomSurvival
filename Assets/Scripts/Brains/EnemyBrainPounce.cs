using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBrainPounce_", menuName = "Enemy Brains/Pounce Attack", order = 51)]
public class EnemyBrainPounce : EnemyBrainDash
{
    // broken out to override in Flying enemy
    protected override void SetMoveDirectionY(Enemy enemy)
    {
        if(enemy.charController.IsGrounded()) enemy.charController.DoJump();
        
        base.SetMoveDirectionY(enemy);
    }
}