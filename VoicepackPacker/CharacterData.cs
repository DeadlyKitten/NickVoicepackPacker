using System.Linq;

namespace VoicepackPacker
{
    class CharacterData
    {
        public string Id { get; set; }
        public MoveData[] Moves { get; set; }
        public string Hash { get; set; }

        public string GetPathFromMoveId(string moveId) => Moves.FirstOrDefault(x => x.Id == moveId)?.Path;
    }
}
