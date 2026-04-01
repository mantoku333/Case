using UnityEngine;
using Cysharp.Threading.Tasks;

public class UmbrellaAttackController : MonoBehaviour
{
    [Header("攻撃設定")]
    [SerializeField] private float attackDuration = 0.2f;   //攻撃の当たり判定が有効な時間
    [SerializeField, Min(0.01f)] private float attackPerSecond = 4.0f;

    [Header("当たり判定")]
    [SerializeField] private Collider2D attackCollider;     //攻撃の当たり判定用コライダー

    private bool isAttacking = false;   //攻撃中かどうかのフラグ
    private float lastAttackTime = -999.0f;

    private void Awake()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    public void SetAttackPerSecond(float attackPerSecond)
    {
        attackPerSecond = Mathf.Max(0.01f, attackPerSecond);
    }

    //--------Get関数-------
    public float GetAttackPerSecond()
    {
        return attackPerSecond;
    }

    /// <summary>
    /// 傘での攻撃処理を行う関数
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid Attack()
    {
        if (isAttacking)
        {
            return;
        }

        float attackInterval = 1.0f / attackPerSecond;

        if (Time.time < lastAttackTime + attackInterval)
        {
            return;
        }

        if (attackCollider == null)
        {
            return;
        }

        isAttacking = true;
        lastAttackTime = Time.time;

        attackCollider.enabled = true;

        //数秒待つ
        await UniTask.Delay((int)(attackDuration * 1000));

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        isAttacking = false;
    }
}
