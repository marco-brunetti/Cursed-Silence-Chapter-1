using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrompts : MonoBehaviour
{
    public void ManagePrompts(UIData UIData)
    {
        if(PlayerController.Instance != null && PlayerController.Instance.InteractableInSight != null)
        {
            Interactable interactable = PlayerController.Instance.InteractableInSight;

            if(PlayerController.Instance.Inspector.IsInspecting)
            {
                UIData.InteractPrompt.SetActive(false);
                UIData.InspectPrompt.SetActive(false);

                if(interactable.InspectableOnly)
                {
                    UIData.ReturnPrompt.SetActive(true);
                    UIData.InteractOrReturnPrompt.SetActive(false);
                }
                else
                {
                    UIData.InteractOrReturnPrompt.SetActive(true);
                    UIData.ReturnPrompt.SetActive(false);
                }
            }
            else
            {
                if(interactable.NonInspectable)
                {
                    UIData.InteractPrompt.SetActive(true);
                    UIData.InspectPrompt.SetActive(false);
                }
                else
                {
                    UIData.InspectPrompt.SetActive(true);
                    UIData.InteractPrompt.SetActive(false);
                }    
            }
        }
        else
        {
            DeactivatePrompts(UIData);
        }
    }

    private void DeactivatePrompts(UIData uIData)
    {
        uIData.InspectPrompt.SetActive(false);
        uIData.InteractOrReturnPrompt.SetActive(false);
        uIData.InteractPrompt.SetActive(false);
        uIData.ReturnPrompt.SetActive(false);
    }
}
