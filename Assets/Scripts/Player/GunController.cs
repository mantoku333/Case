using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("銃の設定")]
    [SerializeField] private float coolTime = 3.0f;           //銃のクールタイム
    [SerializeField] private float groundRecoilPower = 15.0f; //地面似る時の弾の反動
    [SerializeField] private float airRecoilPower = 25.0f;    //空中にいる時の弾の反動

    [Header("地面判定")]
    [SerializeField] private GroundCheck groundCheck;

    private bool isRecoiling = false;   //反動が起きているかどうか

    private float currentCoolTime = 0.0f; //クールタイムの残り時間    

    private Rigidbody2D rigidBody2d;      //反動を加えるためのRigidbody2D

    void Start()
    {
        rigidBody2d = GetComponentInParent<Rigidbody2D>();
    }

    void Update()
    {
        if (currentCoolTime > 0)
        {
            currentCoolTime -= Time.deltaTime;
        }
    }

    public void Shoot(Vector2 direction)
    {
        if (currentCoolTime > 0) { return; }

        ApplyRecoil(direction);

        currentCoolTime = coolTime;

        Debug.Log("Shoot! Cooldown started.");
    }

    void ApplyRecoil(Vector2 direction)
    {
        if (rigidBody2d == null) { return; }

        isRecoiling = true;

        rigidBody2d.linearDamping = 5.0f;

        Vector2 velocity = rigidBody2d.linearVelocity;

        float recoilPower = airRecoilPower;

        //地面で下撃ち（＝ジャンプ）
        if (direction == Vector2.down && groundCheck.IsGround())
        {
            recoilPower = airRecoilPower * 1.8f;

            velocity.y = 0;
            velocity.x *= 0.5f;
        }

        rigidBody2d.linearVelocity = velocity;

        Vector2 recoil = -direction.normalized * recoilPower;
        rigidBody2d.AddForce(recoil, ForceMode2D.Impulse);

        Invoke(nameof(EndRecoil), 0.1f);
    }

    void EndRecoil()
    {
        isRecoiling = false;
        rigidBody2d.linearDamping = 0.0f;
    }

    /// <summary>
    /// 反動が起きているかどうかを返すGet関数
    /// </summary>
    /// <returns>反動が起きているかのbool値</returns>
    public bool GetRecoiling()
    {
        return isRecoiling;
    }
}
