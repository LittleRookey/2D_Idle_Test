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
    PlayerController player;
    [SerializeField] private MineInteractor interactableTarget;
    public float searchRange = 1f;

    public bool IsMining => _isMining;

    [SerializeField] private bool _isMining;

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
                InteractorUI.Instance.DisableOrientation();
                Debug.Log("Previous target deselected");
            }
        }

        // Selection and interaction logic
        if (nearestMine != null && nearestMine != interactableTarget)
        {
            nearestMine.Select();
            interactableTarget = nearestMine;
            Debug.Log("New target selected + equipped? " + _inventory.IsMiningEquipped());
            if (!_inventory.IsMiningEquipped())
            {
                InteractorUI.Instance.SetUnInteractable();
            } else
            {
                InteractorUI.Instance.SetInteractable();
            }

            // ��ư�� Interact �ֱ�

            // Add Interact to interact button changing sprite of button
            // Interact with the object
            // Pass interaction time of 
            // check if player has interactor equipment equipped
            InteractorUI.Instance.SetInteractor(eResourceType.����, () =>
            {
                if (_inventory.IsMiningEquipped())
                {
                    Debug.Log("Used Mining Item");

                    // ������ 1��
                    if (interactableTarget.IsOnCooldown())
                    {
                        // ��Ÿ�����̸� �޽�������
                        WarningMessageInvoker.Instance.ShowMessage($"�ش� �ڿ��� ��Ÿ�ӿ� �ֽ��ϴ�: {interactableTarget.GetRemainingDuration().ToString("F1")}��");
                        return;
                    }
                    
                    if (interactableTarget.IsEmpty)
                    {
                        WarningMessageInvoker.Instance.ShowMessage($"�ش� �ڿ��� ����ֽ��ϴ�");
                        return;
                    }

                    // �ֺ��� ���� ������ 
                    _inventory.UseResourceEquipmentItem(eResourceType.����);
                    //_inventory.GetEquippedMiningItem().Use();
                    _isMining = true;
                    interactableTarget.Interact(5, player, () => _isMining = false);
                }
                else
                {
                    // TODO show warning message that mining is not equipped
                    Debug.Log("Not Equipped Mining Item");
                    WarningMessageInvoker.Instance.ShowMessage("��̸� �������� �ʾҽ��ϴ�");
                    // disable interaction UI button of image
                }
            });
            InteractorUI.Instance.EnableOrientation();
        }
        else if (interactableTarget == null)
        {
            Debug.Log("No interactable object found in range");
        }
    }

    

    private void Update()
    {
        SearchForInteractable();
    }

}
