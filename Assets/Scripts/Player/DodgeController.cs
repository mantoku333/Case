using UnityEngine;
using Cysharp.Threading.Tasks;

public class DodgeController : MonoBehaviour
{
    [Header("回避距離")]
    [SerializeField] private float dodgeDistance = 3.0f;

    [Header("回避時間")]
    [SerializeField] private float dodgeDuration = 0.1f;

    private bool isDodging = false;

    private Rigidbody2D rigidBody2d;

    private void Awake()
    {
        rigidBody2d = GetComponent<Rigidbody2D>();
    }

    public async UniTaskVoid Dodge(Vector2 direction)
    {
        if (isDodging){ return; }

        if (direction == Vector2.zero)
        {
            return;
        }

        isDodging = true;

        Vector2 startPos = rigidBody2d.position;
        Vector2 targetPos = startPos + direction.normalized * dodgeDistance;

        float elapsedTime = 0f;

        while (elapsedTime < dodgeDuration)
        {
            float t = elapsedTime / dodgeDuration;

            Vector2 newPos = Vector2.Lerp(startPos, targetPos, t);
            rigidBody2d.MovePosition(newPos);

            elapsedTime += Time.deltaTime;

            await UniTask.Yield();
        }

        rigidBody2d.MovePosition(targetPos);

        isDodging = false;
    }

    public bool IsDodging()
    {
        return isDodging;
    }
}
