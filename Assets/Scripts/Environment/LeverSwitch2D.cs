using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LeverSwitch2D : MonoBehaviour, IAttackReceiver
{
    [SerializeField] private ShutterWallBlockRise shutterWall;
    [SerializeField] private SpriteRenderer leverRenderer;
    [SerializeField] private bool oneShot = true;

    private bool initialized;
    private bool isOn;
    private bool consumed;
    private bool initialFlipX;

    private void Awake()
    {
        InitializeIfNeeded();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (leverRenderer == null)
        {
            leverRenderer = GetComponent<SpriteRenderer>();
        }
    }
#endif

    public void ActivateFromAttack()
    {
        InitializeIfNeeded();

        if (oneShot && consumed)
        {
            return;
        }

        if (oneShot)
        {
            consumed = true;
        }

        isOn = !isOn;

        if (leverRenderer != null)
        {
            leverRenderer.flipX = isOn ? !initialFlipX : initialFlipX;
        }

        if (isOn)
        {
            if (shutterWall != null)
            {
                shutterWall.Open();
            }
            else
            {
                Debug.LogWarning("ShutterWallBlockRise is not assigned on LeverSwitch2D.", this);
            }
        }
        else
        {
            if (shutterWall != null)
            {
                shutterWall.Close();
            }
            else
            {
                Debug.LogWarning("ShutterWallBlockRise is not assigned on LeverSwitch2D.", this);
            }
        }
    }

    public void OnAttacked(AttackHitbox attacker, Collider2D hitCollider)
    {
        ActivateFromAttack();
    }

    private void InitializeIfNeeded()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        if (leverRenderer == null)
        {
            leverRenderer = GetComponent<SpriteRenderer>();
        }

        if (leverRenderer != null)
        {
            initialFlipX = leverRenderer.flipX;
        }

        if (shutterWall == null)
        {
            GameObject shutterWallObject = GameObject.Find("ShutterWall");
            if (shutterWallObject != null)
            {
                shutterWall = shutterWallObject.GetComponent<ShutterWallBlockRise>();
            }
        }
    }
}
