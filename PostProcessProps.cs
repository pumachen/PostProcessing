using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fuxi.Rendering.PostProcessing
{
    public static class PostProcessProperties
    {
        public static readonly int mainTex         = Shader.PropertyToID("_MainTex");

        #region FXAA
        public static readonly int fxaaParams      = Shader.PropertyToID("_FXAAParams");
        #endregion

        #region Bloom
        public static readonly int bloomTex        = Shader.PropertyToID("_BloomTex");
        public static readonly int bloomParams     = Shader.PropertyToID("_BloomParams");
        public static readonly int filterParams    = Shader.PropertyToID("_FilterParams");
        #endregion

        #region Chromatic Aberration
        public static readonly int spectralLut     = Shader.PropertyToID("_ChromaticAberration_SpectralLut");
        public static readonly int chromaticAmount = Shader.PropertyToID("_ChromaticAberration_Amount");
        #endregion

        #region Color Grading
        public static readonly int LUT             = Shader.PropertyToID("_LUT");
        public static readonly int brightness      = Shader.PropertyToID("_Brightness");
        #endregion

        #region Vignette
        public static readonly int vignetteMask    = Shader.PropertyToID("_Vignette_Mask");
        public static readonly int vignetteColor   = Shader.PropertyToID("_Vignette_Color");
        public static readonly int vignetteOpacity = Shader.PropertyToID("_Vignette_Opacity");
        public static readonly int vignetteCenter  = Shader.PropertyToID("_Vignette_Center");
        public static readonly int vignetteParams  = Shader.PropertyToID("_Vignette_Params");
        #endregion
    }
}