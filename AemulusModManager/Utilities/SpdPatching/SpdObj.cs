using System.Collections.Generic;

namespace AemulusModManager.Utilities.SpdPatching
{
    public class SpdPatch
    {
        public string SpdPath { get; set; }
        public string TextureName { get; set; }
        public uint TextureID { get; set; }
        public string SpriteIDs { get; set; }
    }

    public class SpdPatches
    {
        public int Version;
        public List<SpdPatch> Patches { get; set; }
    }
}
