using UnityEngine;

[CreateAssetMenu(fileName = "NewRobot", menuName = "Robot/RobotStats")]
public class RobotStats : ScriptableObject
{
    public string robotName;
    public int attack;
    public int defense;
    public int health;
    public float criticalChance;
    public float criticalDamageMultiplier;
    public float attackRange;
    public float moveSpeed;   
}
