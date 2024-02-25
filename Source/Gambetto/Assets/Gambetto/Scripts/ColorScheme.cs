using UnityEngine;

namespace Gambetto.Scripts
{
    public class ColorScheme : MonoBehaviour
    {
        [SerializeField]
        public Material transitionMaterial;

        [SerializeField]
        private Color trasitionColor;

        [SerializeField]
        public Material fogMaterial;

        [SerializeField]
        private Color fogColor;

        [SerializeField]
        public Material endLevelMaterial;

        [SerializeField]
        private Color endLevelColor;

        [SerializeField]
        public Material selectedSquareMaterial;

        [SerializeField]
        private Color selectedSquareColor;

        [SerializeField]
        [Min(0)]
        private float selectedSquareColorIntensity;

        [SerializeField]
        public Material lateralMaterial;

        [SerializeField]
        private Color lateralColor;

        [SerializeField]
        public Material lightMaterial;

        [SerializeField]
        private Color lightColor;

        [SerializeField]
        public Material darkMaterial;

        [SerializeField]
        private Color darkColor;

        public bool liveEdit;

        private void Start()
        {
            UpdateMaterialColors(); // Initialize material colors
        }

        private void Update()
        {
            if (liveEdit)
            {
                UpdateMaterialColors();
            }
        }

        private void UpdateMaterialColors()
        {
            transitionMaterial.color = trasitionColor;
            fogMaterial.SetColor("_FogColor", fogColor);
            endLevelMaterial.color = endLevelColor;
            lateralMaterial.color = lateralColor;
            lightMaterial.color = lightColor;
            darkMaterial.color = darkColor;

            //change emission color
            selectedSquareMaterial.SetColor(
                "_EmissionColor",
                selectedSquareColor * selectedSquareColorIntensity
            );
        }
    }
}
