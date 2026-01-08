using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace GLTFast.Loading
{
    /// <summary>
    /// Simplified texture loader used by the importer.
    /// Implementations create and upload a Texture2D from raw image bytes.
    /// The importer will set the texture's name after the loader returns.
    /// </summary>
    public interface ITextureLoader
    {
        /// <summary>
        /// Create a Texture2D from a native byte array.
        /// </summary>
        /// <param name="data">Image bytes (e.g. JPEG/PNG) as a <see cref="NativeArray{Byte}.ReadOnly"/></param>
        /// <param name="markReadable">If true, the created texture should be left readable</param>
        /// <param name="forceSampleLinear">If true, the texture should be created using a linear format.</param>
        /// <param name="importSettings">Current import settings (can be used for mipmap/aniso settings).</param>
        /// <returns>Task that completes with the created <see cref="Texture2D"/>.</returns>
        Task<Texture2D> LoadTextureFromNativeArrayAsync(
            NativeArray<byte>.ReadOnly data,
            bool markReadable,
            bool forceSampleLinear,
            ImportSettings importSettings
        );

        /// <summary>
        /// Create a Texture2D from a managed byte[].
        /// </summary>
        /// <param name="data">Image bytes (e.g. JPEG/PNG) as a managed array.</param>
        /// <param name="markNonReadable">If true, the created texture should be left readable</param>
        /// <param name="forceSampleLinear">If true, the texture should be created using a linear format.</param>
        /// <param name="importSettings">Current import settings (can be used for mipmap/aniso settings).</param>
        /// <returns>Task that completes with the created <see cref="Texture2D"/>.</returns>
        Task<Texture2D> LoadTextureFromManagedArrayAsync(
            byte[] data,
            bool markReadable,
            bool forceSampleLinear,
            ImportSettings importSettings
        );
    }
}