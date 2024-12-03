using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Player;

namespace Interactables.Behaviours
{
    public class PhoneController : Behaviour
    {
        [SerializeField] private GameObject _canvas;

        [SerializeField] private TextMeshProUGUI _timeText;

        [Header("App buttons")]
        [SerializeField] private Button _messageAppButton;


        private bool _isInteracting;
        private Coroutine phoneLoop;

        public override void Activate()
        {
            phoneLoop ??= StartCoroutine(PhoneLoop());
        }

        private IEnumerator PhoneLoop()
        {
            var playerController = PlayerController.Instance;

            playerController.DeactivateInteraction();
            playerController.FreezePlayer(true);
            
            yield return StartCoroutine(ActivateCanvas(true));

            _isInteracting = true;

            while (_isInteracting)
            {
                _timeText.text = $"{DateTime.Now:hh:mm tt}";
                
                //Add this to button
                if (Input.GetMouseButtonDown(1))
                {
                    _isInteracting = false;
                    playerController.ReactivateInteraction();
                }

                yield return null;
            }
            
            StartCoroutine(ActivateCanvas(false));
            playerController.FreezePlayer(false);

            phoneLoop = null;
        }

        private IEnumerator ActivateCanvas(bool enable)
        {
            yield return new WaitForSecondsRealtime(0.4f);
            Interactable.showUICursor?.Invoke(this, enable);
            _canvas.SetActive(enable);
        }


        private void OnEnable()
        {
            //add camera to canvas
        }
    }
}