using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.Events;
using HoloToolkit.UI.Keyboard;
using UnityEngine.UI;

namespace CustomButton
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Button))]
    public class HololensButton : MonoBehaviour, IInputClickHandler, IFocusable
    {

        public ButtonVariables FocusEnter;
        public ButtonVariables FocusExit;

        public UnityEvent customEvent;
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            if (FocusExit.GetDefault)
            {
                FocusExit = new ButtonVariables()
                {
                    SetColor = button.colors.normalColor,
                    Opacity = button.colors.normalColor.a,

                    OnClickText = button.GetComponentInChildren<Text>() != null ? button.GetComponentInChildren<Text>().text : null,

                };
            }
            if (FocusEnter.GetDefault)
            {
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
            ColorBlock cBlock = button.colors;
            cBlock.normalColor = vars.SetColor;
            button.colors = cBlock;
        }
    }
}

