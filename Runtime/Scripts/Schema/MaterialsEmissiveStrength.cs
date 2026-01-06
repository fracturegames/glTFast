


namespace GLTFast.Schema
{
    /// <summary>
    /// Schema for KHR_materials_emissive_strength extension
    /// See: https://github.com/KhronosGroup/glTF/tree/main/extensions/2.0/Khronos/KHR_materials_emissive_strength
    /// </summary>
    [System.Serializable]
    public class MaterialsEmissiveStrength
    {
        /// <summary>
        /// The emissive strength multiplier.
        /// Default is 1.0
        /// </summary>
        public float emissiveStrength = 1.0f;

        internal void GltfSerialize(JsonWriter writer)
        {
            writer.AddObject();
            writer.AddProperty("emissiveStrength", emissiveStrength);
            writer.Close();
        }
    }
}