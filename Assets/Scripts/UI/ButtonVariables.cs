using UnityEngine;

namespace CustomButton
{
    /// <summary>
    /// This class contains customizable values for button component
    /// </summary>
    [System.Serializable]
    public class ButtonVariables
    {
        [Tooltip("Set false if you want to use these values | True by default")]
        [SerializeField]
        public bool GetDefault = true;

        [Tooltip("Set true if you want to animate color change")]
        [SerializeField]
        public bool Animate = true;

        [SerializeField]
        public Material SetMaterial;

        [SerializeField]
        public Color SetColor;

        [Range(0f, 1f)]
        [SerializeField]
        public float Opacity = 1f;

        [Tooltip("Leave empty if you dont want to change buttons text after click")]
        [SerializeField]
        public string OnClickText;
    }
}

