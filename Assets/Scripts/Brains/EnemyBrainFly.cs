using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBrainFly_", menuName = "Enemy Brains/Flying", order = 51)]

public class EnemyBrainFly : EnemyBrainDash
{
    
  [SerializeField] private float resetSpeed;

  public override void OnStart(Enemy enemy)
  {
      base.OnStart(enemy);
      
      enemy.rig.useGravity = false;
      enemy.startingPosition.y = 2.5f; // todo: add raycast to offset from ground in case starts over hill or mountain
  }

  protected override void CheckShouldSleep(Enemy enemy)
  {
      // move back to starting position
      enemy.transform.forward = enemy.startingPosition - enemy.transform.position;
      enemy.moveDirection = enemy.transform.forward * patrolSpeed;
      enemy.currentState = EnemyState.Sleeping;
  }

  protected override void CheckShouldAttack(Enemy enemy)
  {
      // don't start a new attack until we return to start and pause
      if(Vector3.Distance(enemy.startingPosition,enemy.transform.position) > .1f) return;
   
      // once back at start, use base class distance logic to determine attack
      base.CheckShouldAttack(enemy);
  }

  public override void DoSleepAction(Enemy enemy)
  {
      //enemy.timer += Time.deltaTime;

      // keep moving until back at starting point, then hang out
      if(Vector3.Distance(enemy.startingPosition,enemy.transform.position) > .1f)
      {
          enemy.moveDirection = (enemy.startingPosition - enemy.transform.position).normalized * resetSpeed;
          enemy.charController.Move(enemy.moveDirection);
      }
      else
      {
          enemy.transform.forward = enemy.startingForward;
          enemy.charController.Pause();
      }
  
      CheckShouldAttack(enemy);
  }

  // don't set the Y for the move direction, we need to move vertically to fly
  protected override void SetMoveDirectionY(Enemy enemy) { }
  
  // enemy needs to move on the Y axis to hit the player
  protected override Vector3 GetPlayerDirection(Enemy enemy)
  {
      return enemy.player.transform.position - enemy.transform.position;
  }
  
}
