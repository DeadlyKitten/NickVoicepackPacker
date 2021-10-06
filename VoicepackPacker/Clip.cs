using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace VoicepackPacker
{
    [System.Serializable]
    class Clip
    {
        [JsonIgnore]
        public static List<Clip> allClips = new List<Clip>();

        public string Id { get; set; }
        [JsonIgnore]
        public string filePath;
        public string Path { get; set; }
        public float Volume { get; set; }
        [JsonIgnore]
        public string hash;

        public Clip(string id, string path, float volume = 1)
        {
            this.Id = id ?? $"Clip{allClips.Count() + 1}";
            this.filePath = path;
            this.Volume = volume;
            Path = $"clips/clip{allClips.Count() + 1}{System.IO.Path.GetExtension(filePath)}";

            ComputeHash();

            if (!allClips.Any(x => x.hash == this.hash)) allClips.Add(this);
        }

        public static List<Clip> GetClipsForHashArray(string[] hashes)
        {
            List <Clip> result = new List<Clip>();

            foreach(var hash in hashes)
            {
                result.Add(allClips.FirstOrDefault(x => x.hash == hash));
            }

            return result;
        }

        public override string ToString() => $"{Id} - {filePath} - {hash}";

        void ComputeHash()
        {
            var bytes = File.ReadAllBytes(filePath).ToArray();

            using (MD5 md = MD5.Create())
            {
                var hashBytes = md.ComputeHash(bytes.ToArray());

                var sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString("X2"));

                hash = sb.ToString();
            }
        }
    }
}
