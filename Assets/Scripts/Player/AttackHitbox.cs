using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    //[Header("攻撃力")]
    //[SerializeField] private int damage = 1;　//攻撃力

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MonoBehaviour[] behaviours = collision.GetComponentsInParent<MonoBehaviour>();

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is IAttackReceiver receiver)
            {
                receiver.OnAttacked(this, collision);
            }
        }
    }
}
