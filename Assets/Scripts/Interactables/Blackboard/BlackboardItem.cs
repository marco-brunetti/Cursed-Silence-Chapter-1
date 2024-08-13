using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackboardItem : MonoBehaviour, IBehaviour
{
    public bool IsOnBlackboard;
    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(isInteracting && IsOnBlackboard)
        {
            var playerController = PlayerController.Instance;

            UIManager.Instance.ShowBlackboardImage(true, GetComponent<SpriteRenderer>().sprite);
            playerController.FreezePlayerMovement = true;
            playerController.FreezePlayerRotation = true;
            playerController.ActivateDepthOfField(true);
            StartCoroutine(WaitForRightMouse(playerController));
        }
    }

    private IEnumerator WaitForRightMouse(PlayerController playerController)
    {
        yield return new WaitUntil(()=> Input.GetMouseButtonDown(1));

        UIManager.Instance.ShowBlackboardImage(false);
        playerController.ActivateDepthOfField(false);
        playerController.FreezePlayerMovement = false;
        playerController.FreezePlayerRotation = false;
    }

    public bool IsInspectable()
    {
        return false;
    }

    public bool IsInteractable()
    {
        return true;
    }
}
