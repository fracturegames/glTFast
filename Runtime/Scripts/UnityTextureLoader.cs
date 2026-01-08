using System;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;

namespace GLTFast.Loading
{
    /// <summary>
    /// Default texture loader that uses Unity's Texture2D.LoadImage to decode JPG/PNG bytes.
    /// Returns textures synchronously wrapped in a Task (keeps caller async flow unchanged).
    /// The loader does not set the texture name; caller is responsible for that.
    /// </summary>
    public class UnityTextureLoader : ITextureLoader
    {
        public Task<Texture2D> LoadTextureFromNativeArrayAsync(
            NativeArray<byte>.ReadOnly data,
            bool markReadable,
            bool forceSampleLinear,
            ImportSettings importSettings
        )
        {
            Debug.Log("Loading Unity Texture 2D");
            
            var txt = CreateEmptyTexture(forceSampleLinear, importSettings);

#if UNITY_6000_0_OR_NEWER
            Profiler.BeginSample("Texture2D.LoadImage");
            // NativeArray<byte>.ReadOnly supports AsReadOnlySpan() on newer Unity versions
            txt.LoadImage(data.AsReadOnlySpan(), !markReadable);
            Profiler.EndSample();
#else
            // Fallback for older Unity: copy to managed then call LoadImage
            var managed = new byte[data.Length];
            for (int i = 0; i < data.Length; i++) managed[i] = data[i];
            Profiler.BeginSample("Texture2D.LoadImage");
            txt.LoadImage(managed, !markReadable);
            Profiler.EndSample();
#endif

            return Task.FromResult(txt);
        }

        public Task<Texture2D> LoadTextureFromManagedArrayAsync(
            byte[] data,
            bool markReadable,
            bool forceSampleLinear,
            ImportSettings importSettings
        )
        {

            var txt = CreateEmptyTexture(forceSampleLinear, importSettings);
            Profiler.BeginSample("Texture2D.LoadImage");
            txt.LoadImage(data, !markReadable);
            Profiler.EndSample();
            return Task.FromResult(txt);
        }

        static Texture2D CreateEmptyTexture(bool forceSampleLinear, ImportSettings importSettings)
        {
            var textureCreationFlags = TextureCreationFlags.DontUploadUponCreate | TextureCreationFlags.DontInitializePixels;
            // ImportSettings is expected to include GenerateMipMaps in the project (used by importer)
            if (importSettings != null)
            {
                // Try to detect and respect a GenerateMipMaps property if present.
                // This avoids a hard dependency on a specific ImportSettings shape in compile-time signatures
                // while keeping behavior consistent with the importer when that property exists.
                var genMip = false;
                try
                {
                    var prop = importSettings.GetType().GetProperty("GenerateMipMaps");
                    if (prop != null && prop.PropertyType == typeof(bool))
                    {
                        genMip = (bool)prop.GetValue(importSettings);
                    }
                }
                catch
                {
                    genMip = false;
                }

                if (genMip)
                {
                    textureCreationFlags |= TextureCreationFlags.MipChain;
                }
            }

            var txt = new Texture2D(
                4, 4,
                forceSampleLinear ? GraphicsFormat.R8G8B8A8_UNorm : GraphicsFormat.R8G8B8A8_SRGB,
                textureCreationFlags
            );

            // Apply anisotropic level from ImportSettings if present
            if (importSettings != null)
            {
                try
                {
                    var prop = importSettings.GetType().GetProperty("AnisotropicFilterLevel");
                    if (prop != null && prop.PropertyType == typeof(int))
                    {
                        txt.anisoLevel = (int)prop.GetValue(importSettings);
                    }
                }
                catch
                {
                    // ignore reflection failures
                }
            }

            return txt;
        }
    }
}