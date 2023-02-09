using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBrainPassive", menuName = "Enemy Brains/Passive", order=51)]
public class EnemyBrainPassive : EnemyBrain
{
    protected override void CheckShouldAttack(Enemy enemy)
    { }
}
