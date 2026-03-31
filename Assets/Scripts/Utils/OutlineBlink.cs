using System.Collections;
using UnityEngine;

namespace Utils
{
    public class OutlineBlink : MonoBehaviour
    {
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");

        [ColorUsage(true, true)]
        public Color[] colors;
        public float delay;
        public Material outlineMaterial;

        private void Start()
        {
            StartCoroutine(BlinkFlow());
        }

        private IEnumerator BlinkFlow()
        {
            while (true)
            {
                foreach (var color in colors)
                {
                    outlineMaterial.SetColor(ColorProperty, color);
                    yield return new WaitForSeconds(delay);
                }
            }
        }
    }
}
