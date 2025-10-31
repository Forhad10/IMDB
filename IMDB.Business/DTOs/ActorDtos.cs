using System.Collections.Generic;

namespace IMDB.Business.DTOs
{
    public class PersonWordResult
    {
        public string Word { get; set; } = string.Empty;
        public int Frequency { get; set; }
    }

    public class ExactMatchTitleResult
    {
        public string TitleId { get; set; } = string.Empty;
        public string PrimaryTitle { get; set; } = string.Empty;
    }

    public class BestMatchTitleResult
    {
        public string TitleId { get; set; } = string.Empty;
        public string PrimaryTitle { get; set; } = string.Empty;
        public int MatchedCount { get; set; }
        public string[] MatchedWords { get; set; } = System.Array.Empty<string>();
    }

    public class KeywordWordResult
    {
        public string Word { get; set; } = string.Empty;
        public int Frequency { get; set; }
    }
}
