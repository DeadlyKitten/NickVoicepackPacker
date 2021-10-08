using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace VoicepackPacker
{
    class Program
    {
        static readonly Dictionary<string, List<string>> files = new Dictionary<string, List<string>>();

        private static readonly IEnumerable<string> filter = new List<string> { ".ogg", ".wav" };

        public static string path;

        static JsonSerializerSettings options = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.Indented
        };

        static void Main(string[] args)
        {
#if DEBUG
            // test input data (Gir)
            path = ".\\proj_alien_friend";
#else
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Drag a folder on the exe!");
                Console.Error.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            path = GetRelativePath(args[0], AppDomain.CurrentDomain.BaseDirectory);
#endif
            if (!ValidateInput()) return;

            Console.WriteLine("Generating voicepack...");

            var characterId = new DirectoryInfo(path).Name;

            ParseInputDirectory();

            GenerateGroups();

            var package = new PackageJSON
            {
                CharacterId = characterId,
                AudioGroups = Group.allGroups,
                VoiceClips = Clip.allClips
            };

            var json = JsonConvert.SerializeObject(package, options);

            GenerateVoicepackFile(characterId, json);

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static void GenerateVoicepackFile(string characterId, string json)
        {
            var tempFolder = Directory.CreateDirectory($"./temp/{characterId}").FullName;

            var packageJsonPath = Path.Combine(tempFolder, "package.json");

            if (File.Exists(packageJsonPath))
                File.Delete(packageJsonPath);

            using (var writer = new StreamWriter(packageJsonPath))
                writer.Write(json);

            if (File.Exists($"./{characterId}.voicepack")) File.Delete($"./{characterId}.voicepack");

            Directory.CreateDirectory(Path.Combine(tempFolder, "clips"));
            foreach (var clip in Clip.allClips)
            {
                File.Copy(clip.filePath, Path.Combine(tempFolder, clip.Path), true);
            }

            ZipFile.CreateFromDirectory(tempFolder, $"./{characterId}.voicepack");

            Directory.Delete(Directory.GetParent(tempFolder).FullName, true);
        }

        private static void ParseInputDirectory()
        {
            foreach (var file in Directory.GetFiles(path).Where(x => filter.Contains(Path.GetExtension(x))))
            {
                var relativePath = GetRelativePath(file, path);
                files.Add(Path.GetFileNameWithoutExtension(file), new List<string> { relativePath });
                new Clip(Path.GetFileNameWithoutExtension(relativePath), relativePath);
            }

            foreach (var directory in Directory.GetDirectories(path))
            {
                foreach (var file in Directory.GetFiles(directory).Where(x => filter.Contains(Path.GetExtension(x))))
                {
                    var directoryName = Directory.GetParent(file).Name;
                    var relativePath = GetRelativePath(file, path);

                    if (files.TryGetValue(directoryName, out var clip))
                    {
                        clip.Add(relativePath);
                    }
                    else
                    {
                        files.Add(directoryName, new List<string> { relativePath });
                    }

                    new Clip(Path.GetFileNameWithoutExtension(file), relativePath);
                }
            }
        }

        private static bool ValidateInput()
        {
            FileAttributes attr = File.GetAttributes(path);

            if (!attr.HasFlag(FileAttributes.Directory))
            {
                Console.Error.WriteLine("Drag a folder on the exe!");
                Console.Error.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return false;
            }
            return true;
        }

        static void GenerateGroups()
        {
            foreach(var key in files.Keys.Where(x => files[x].Count() > 1))
            {
                new Group(key, Clip.GetClipsForHashArray(HashFileList(files[key])));
            }
        }

        static string[] HashFileList(List<string> list)
        {
            List<string> result = new List<string>();

            foreach(var file in list)
            {
                var bytes = File.ReadAllBytes(file).ToArray();

                using (MD5 md = MD5.Create())
                {
                    var hashBytes = md.ComputeHash(bytes.ToArray());

                    var sb = new StringBuilder();

                    for (int i = 0; i < hashBytes.Length; i++)
                        sb.Append(hashBytes[i].ToString("X2"));

                    var hash = sb.ToString();
                    result.Add(hash);
                }
            }


            return result.ToArray();
        }

        public static string GetRelativePath(string p_fullDestinationPath, string p_startPath)
        {
            string[] l_startPathParts = Path.GetFullPath(p_startPath).Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string[] l_destinationPathParts = p_fullDestinationPath.Split(Path.DirectorySeparatorChar);

            int l_sameCounter = 0;
            while ((l_sameCounter < l_startPathParts.Length) && (l_sameCounter < l_destinationPathParts.Length) && l_startPathParts[l_sameCounter].Equals(l_destinationPathParts[l_sameCounter], StringComparison.InvariantCultureIgnoreCase))
            {
                l_sameCounter++;
            }

            if (l_sameCounter == 0)
            {
                return p_fullDestinationPath; // There is no relative link.
            }

            StringBuilder l_builder = new StringBuilder();
            for (int i = l_sameCounter; i < l_startPathParts.Length; i++)
            {
                l_builder.Append(".." + Path.DirectorySeparatorChar);
            }

            for (int i = l_sameCounter; i < l_destinationPathParts.Length; i++)
            {
                l_builder.Append(l_destinationPathParts[i] + Path.DirectorySeparatorChar);
            }

            l_builder.Length--;

            return l_builder.ToString();
        }

    }
}
