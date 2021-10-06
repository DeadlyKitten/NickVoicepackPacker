using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace VoicepackPacker
{
    [System.Serializable]
    class Group
    {
        [JsonIgnore]
        public static List<Group> allGroups = new List<Group>();

        public string Name { get; set; }
        public List<GroupMember> Clips { get; set; }

        public List<string> Moves { get; set; }
        [JsonIgnore]
        public string hash;

        public Group(string move, List<Clip> clips)
        {
            Name = $"Group{allGroups.Count() + 1}";
            Moves ??= new List<string>();
            Moves.Add(move);
            this.Clips = GroupMember.FromClipList(clips);

            ComputeHash();

            if (!allGroups.Any(x => x.hash == this.hash))
                allGroups.Add(this);
            else
                allGroups.First(x => x.hash == this.hash).Moves.Add(move);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"Group ID: {Name} - Hash: {hash}\n");
            sb.Append($"Moves: ");
            Moves.ForEach(x => sb.Append(x + " "));
            sb.Append("\n");
            Clips.ForEach(x => sb.Append(x.ToString() + "\n"));

            return sb.ToString();
        }

        void ComputeHash()
        {
            var byteArray = new List<byte>();
            foreach (var clip in Clips)
            {
                byteArray.AddRange(File.ReadAllBytes(clip.Path));
            }

            using (MD5 md = MD5.Create())
            {
                var hashBytes = md.ComputeHash(byteArray.ToArray());

                var sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString("X2"));

                hash = sb.ToString();
            }
        }
    }

    class GroupMember
    {
        [JsonIgnore]
        public Clip clip;
        [JsonIgnore]
        public string Path { get => clip.filePath; }
        public string Id { get => clip.Id; }
        public float Weight { get; set; }

        public GroupMember(Clip clip, float weight = 1)
        {
            this.clip = clip;
            this.Weight = weight;
        }

        public override string ToString() => clip.ToString();

        public static List<GroupMember> FromClipList(List<Clip> clips)
        {
            var result = new List<GroupMember>();

            clips.ForEach(x => result.Add(new GroupMember(x)));

            return result;
        }
    }
}
