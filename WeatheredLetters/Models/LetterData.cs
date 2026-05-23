namespace WeatheredLetters.Models
{
    public enum SpawnLocationType
    {
        Farm,
        Beach,
        Trash,
        Mines,
        Fishing
    }

    public class LetterData
    {
        public string Id { get; set; } = "";
        public string Author { get; set; } = "";
        public string TranslationKey { get; set; } = "";
        public SpawnLocationType LocationType { get; set; }
        public string? SeasonHint { get; set; }
        public int Order { get; set; }
    }
}
