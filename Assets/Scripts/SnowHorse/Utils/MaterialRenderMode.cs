namespace SnowHorse.Utils
{
    public static class MaterialRenderMode
    {
        /// <summary>
        /// Sets a material to use the Opaque render mode.
        /// The material will be rendered with a 1:0 blend mode, and will write to the depth buffer.
        /// </summary>
        /// <param name="material">The material to set the render mode for.</param>
        public static void Opaque(UnityEngine.Material material)
        {
            material.SetOverrideTag("RenderType", "");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
        }

        /// <summary>
        /// Sets a material to use the Fade render mode.
        /// This mode is used for transparent rendering with alpha blending,
        /// where the material will not write to the depth buffer.
        /// </summary>
        /// <param name="material">The material to set the render mode for.</param>
        public static void Fade(UnityEngine.Material material)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
    }
}