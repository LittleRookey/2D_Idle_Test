using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;
using Litkey.InventorySystem;
using Litkey.Utility;

public class ResourceInteractor : MonoBehaviour
{
    [SerializeField] private LayerMask resourceLayer;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private float searchRange = 1f;
    [SerializeField] private SpriteRenderer resourceGetterSprite;
    
    private PlayerController _player;
    private IInteractable _currentTarget;


    public bool IsInteracting { get; private set; }

    private void Awake()
    {
        if (!TryGetComponent<PlayerController>(out _player))
        {
            Debug.LogError("No PlayerController attached to ResourceInteractor gameObject");
        }
    }

    private void Update()
    {
        SearchForInteractable();
    }

    public void SearchForInteractable()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, searchRange, resourceLayer);

        IInteractable nearestInteractable = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<IInteractable>(out var interactable))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestInteractable = interactable;
                    nearestDistance = distance;
                }
            }
        }

        HandleInteractableChange(nearestInteractable);
    }

    private void HandleInteractableChange(IInteractable newTarget)
    {
        if (_currentTarget != null && (_currentTarget != newTarget || Vector2.Distance(transform.position, ((MonoBehaviour)_currentTarget).transform.position) > searchRange))
        {
            if (_currentTarget is IDeselectable deselectable)
            {
                deselectable.Deselect();
            }
            _currentTarget = null;
            InteractorUI.Instance.DisableOrientation();
        }

        if (newTarget != null && newTarget != _currentTarget)
        {
            _currentTarget = newTarget;
            if (_currentTarget is ISelectable selectable)
            {
                selectable.Select();
            }
            SetupInteractionUI();
        }
    }

    private void SetupInteractionUI()
    {
        // This is a simplified version. You might need to adjust based on your specific UI setup
        InteractorUI.Instance.SetInteractor(GetResourceType(_currentTarget), () =>
        {
            if (_currentTarget.CanInteract(_player))
            {
                IsInteracting = true;
                resourceGetterSprite.sprite = GetResourceGetterSprite(GetResourceType(_currentTarget));
                _currentTarget.Interact(_player, () => IsInteracting = false);
            }
        });
        InteractorUI.Instance.EnableOrientation();
    }

    private Sprite GetResourceGetterSprite(eResourceType resourceType)
    {
        if (resourceType == eResourceType.±¤¼®)
        {
            return _inventory.GetEquippedMiningItem().EquipmentData.IconSprite;
        } 
        else if (resourceType == eResourceType.³ª¹«)
        {
            return _inventory.GetEquippedAxingItem().EquipmentData.IconSprite;
        } 
        else if (resourceType == eResourceType.¹°°í±â)
        {
            return _inventory.GetEquippedFishingItem().EquipmentData.IconSprite;
        }
        return _inventory.GetEquippedMiningItem().EquipmentData.IconSprite;
    }
    private eResourceType GetResourceType(IInteractable interactable)
    {
        // Implement logic to determine resource type based on the interactable
        // This is just a placeholder implementation
        if (interactable is MineInteractor)
        {
            return eResourceType.±¤¼®;
        }
        else if (interactable is NPCInteractor)
        {
            return eResourceType.¸»°É±â;
        }
        else if (interactable is TreeInteractor)
        {
            return eResourceType.³ª¹«;
        }
        
        // Add other types as needed
        return eResourceType.±¤¼®;
    }
}