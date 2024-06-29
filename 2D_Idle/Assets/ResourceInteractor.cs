using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;


public class ResourceInteractor : MonoBehaviour
{
    [SerializeField] private LayerMask resourceLayer;
    
    PlayerController player;
    private MineInteractor interactableTarget;
    public float searchRange = 1f;


    private void Awake()
    {
        if (!TryGetComponent<PlayerController>(out player))
        {
            Debug.LogError("No PlayerController attached to ResourceInteractor gameObject");
        }
    }

    public void SearchForInteractable()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, searchRange, resourceLayer);

        MineInteractor nearestMine = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<MineInteractor>(out var mine))
            {
                float distance = Vector2.Distance(transform.position, mine.transform.position);
                if (distance < nearestDistance)
                {
                    nearestMine = mine;
                    nearestDistance = distance;
                }
            }
        }

        // Deselection logic
        if (interactableTarget != null)
        {
            bool shouldDeselect = false;

            // Case 1: There is another ISelectable nearby which is not the previous interactableTarget
            if (nearestMine != null && nearestMine != interactableTarget)
            {
                shouldDeselect = true;
            }
            // Case 2: Previous IInteractable's distance is greater than searchRange
            else if (Vector2.Distance(transform.position, interactableTarget.transform.position) > searchRange)
            {
                shouldDeselect = true;
            }

            if (shouldDeselect)
            {
                interactableTarget.Deselect();
                interactableTarget = null;
                Debug.Log("Previous target deselected");
            }
        }

        // Selection and interaction logic
        if (nearestMine != null && nearestMine != interactableTarget)
        {
            nearestMine.Select();
            interactableTarget = nearestMine;
            Debug.Log("New target selected");


            // 버튼에 Interact 넣기

            // Add Interact to interact button changing sprite of button
            // Interact with the object
            // Pass interaction time of 
            InteractorUI.Instance.SetInteractor(eResourceType.광석, () => interactableTarget.Interact(5));
            
        }
        else if (interactableTarget == null)
        {
            Debug.Log("No interactable object found in range");
        }
    }

   


}
