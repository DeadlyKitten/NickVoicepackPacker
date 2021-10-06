using System.Collections.Generic;

namespace VoicepackPacker
{
    [System.Serializable]
    class PackageJSON
    {
        public string CharacterId { get; set; }
        public List<Group> AudioGroups { get; set; }
        public List<Clip> VoiceClips { get; set; }
    }
}
