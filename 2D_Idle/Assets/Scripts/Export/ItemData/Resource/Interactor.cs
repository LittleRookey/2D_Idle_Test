using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;
using Litkey.InventorySystem;
using Litkey.Character.Cooldowns;
using Litkey.Utility;
using UnityEngine.Events;

public abstract class Interactor : MonoBehaviour, IInteractable, ISelectable, IDeselectable, IHasCooldown
{
    [Header("Interactor Settings")]
    [SerializeField] protected float _cooldownTime = 5f;
    [SerializeField] protected float _interactionTime = 3f;
    [SerializeField] protected bool enableOutlineOnSelected = true;
    [SerializeField] protected bool enableGlowOnSelected = true;
    [SerializeField] protected bool enableHandDrawn = false;

    protected static readonly int OutlineProperty = Shader.PropertyToID("_OutlineAlpha");
    protected static readonly int GlowProperty = Shader.PropertyToID("_Glow");
    protected static readonly int HandDrawnProperty = Shader.PropertyToID("_HandDrawnAmount");

    protected MaterialPropertyBlock _mpb;
    protected SpriteRenderer _spriteRenderer;
    protected CooldownSystem _cooldown;

    public float CooldownDuration => _cooldownTime;
    public bool IsSelected { get; private set; }
    public string ID { get; protected set; }

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _mpb = new MaterialPropertyBlock();
        _cooldown = GetComponent<CooldownSystem>();
        DisableGlow();
        DisableOutline();
        DisableHandDrawn();
        if (enableHandDrawn) EnableHandDrawn();

    }

    public virtual void Deselect()
    {
        if (enableOutlineOnSelected) DisableOutline();
        if (enableGlowOnSelected) DisableGlow();
        IsSelected = false;
    }

    public virtual void Select()
    {
        if (enableOutlineOnSelected) EnableOutline();
        if (enableGlowOnSelected) EnableGlow();
        IsSelected = true;
    }

    public abstract void Interact(PlayerController player, UnityAction OnEnd = null);

    public virtual bool CanInteract(PlayerController player)
    {
        return !IsOnCooldown();
    }

    public virtual bool IsOnCooldown()
    {
        return _cooldown.IsOnCooldown(ID);
    }

    public virtual float GetRemainingDuration()
    {
        return _cooldown.GetRemainingDuration(ID);
    }

    public virtual float GetInteractionTime()
    {
        return _interactionTime;
    }

    protected virtual void SetToCooldown(float duration)
    {
        _cooldown.PutOnColdown(ID, duration);
    }

    protected void EnableOutline()
    {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(OutlineProperty, 1f);
        _spriteRenderer.SetPropertyBlock(_mpb);
    }

    protected void DisableOutline()
    {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(OutlineProperty, 0f);
        _spriteRenderer.SetPropertyBlock(_mpb);
    }

    protected void EnableGlow()
    {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(GlowProperty, 0.5f);
        _spriteRenderer.SetPropertyBlock(_mpb);
    }

    protected void DisableGlow()
    {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(GlowProperty, 0f);
        _spriteRenderer.SetPropertyBlock(_mpb);
    }

    protected void EnableHandDrawn()
    {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(HandDrawnProperty, 3f);
        _spriteRenderer.SetPropertyBlock(_mpb);
    }

    protected void DisableHandDrawn()
    {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(HandDrawnProperty, 0f);
        _spriteRenderer.SetPropertyBlock(_mpb);
    }


    protected IEnumerator InteractionCoroutine(PlayerController player, UnityAction OnInteractionComplete)
    {
        SetToCooldown(_cooldownTime);
        var barProgress = BarCreator.CreateFillBar(transform.position + Vector3.up * 0.5f);
        barProgress.SetBar(false)
            .SetInnerColor(Color.green)
            .SetOuterColor(Color.black);
        player.DisableMovement();
        barProgress.StartFillBar(_interactionTime, () =>
        {
            OnInteractionComplete?.Invoke();
            player.EnableMovement();
        });
        if (_interactionTime <= 0f) yield break;
        for (int i = 0; i < Mathf.FloorToInt(_interactionTime); i++)
        {
            player.PlayMineInteract();
            yield return new WaitForSeconds(1f);
            OnInteractionTick(player);
        }
    }

    protected virtual void OnInteractionTick(PlayerController player) { }

    public virtual void Initialize() { }
}
