using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.Events;
using HoloToolkit.UI.Keyboard;

namespace CustomButton
{
    [RequireComponent(typeof(Collider))]
    public class Hololens3DButton : MonoBehaviour, IInputClickHandler, IFocusable
    {

        public ButtonVariables FocusEnter;
        public ButtonVariables FocusExit;
        public UnityEvent customEvent;
        private MeshRenderer mRenderer;

        /// <summary>
        /// Checks FocusEnter and FocusExit for GetDefault boolean, if true, function assigns default values from button components.
        /// </summary>
        public void Start()
        {
            mRenderer = GetComponent<MeshRenderer>();

            if (FocusExit.GetDefault)
            {
#if UNITY_EDITOR
                Debug.Log("Filling values in FocusExit");
#endif
                FocusExit = new ButtonVariables()
                {
                    SetMaterial = mRenderer.material,
                    SetColor = mRenderer.material.color,
                    Opacity = mRenderer.material.color.a,
                    Animate = false,
                    OnClickText = GetComponentInChildren<TextMesh>().text
                };
            }
            if (FocusEnter.GetDefault)
            {
#if UNITY_EDITOR
                Debug.Log("Filling values in FocusEnter");
#endif
                FocusEnter = FocusExit;
            }
        }

        public void OnFocusEnter()
        {
            OnFocusChanged(FocusEnter);
        }

        public void OnFocusExit()
        {
            OnFocusChanged(FocusExit);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (customEvent == null)
                return;
#if UNITY_EDITOR
            Debug.Log("Event is about to invoke: " + customEvent.GetPersistentMethodName(0));
#endif
            customEvent.Invoke();
        }


        private void OnFocusChanged(ButtonVariables vars)
        {
            mRenderer.material = vars.SetMaterial;
            var tempColor = vars.SetColor;
            tempColor.a = vars.Opacity;
            if (vars.Animate)
            {
                //TODO: A better way to disable single coroutine(without passing parameters)
                StopAllCoroutines();
                StartCoroutine(ChangeRendererColor(mRenderer, tempColor));
            }
            if (vars.OnClickText != null)
            {
                GetComponentInChildren<TextMesh>().text = vars.OnClickText;
            }
        }

        private IEnumerator ChangeRendererColor(MeshRenderer mr, Color toColor)
        {
            var wait = new WaitForEndOfFrame();

            while (mr.material.color != toColor)
            {
                mr.material.color = Color.Lerp(mr.material.color, toColor, Time.deltaTime * 2f);
                yield return wait;
            }
        }
    }
}


