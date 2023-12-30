using UnityEngine;

namespace Gambetto.Scripts
{
    public class ColorScheme : MonoBehaviour
    {
    
        [SerializeField]
        public Material transitionMaterial;
        
        [SerializeField]
        public Material fogMaterial;
        
        [SerializeField]
        private Color fogColor;
    
        [SerializeField]
        public Material endLevelMaterial;
        
        [SerializeField]
        private Color endLevelColor;
        
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

        public void Start()
        {
            transitionMaterial.color = fogColor;
            fogMaterial.SetColor("_FogColor", fogColor);
            endLevelMaterial.color = endLevelColor;
            lateralMaterial.color = lateralColor;
            lightMaterial.color = lightColor;
            darkMaterial.color = darkColor;
        }
    }
}