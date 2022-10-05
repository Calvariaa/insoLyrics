namespace insoLyrics.Beatmap
{
    public class BeatmapMetadata
    {
        public string Title { get; set; } = string.Empty;
        public string TitleUnicode { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string ArtistUnicode { get; set; } = string.Empty;
        public string AuthorString { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public int PreviewTime { get; set; }
        public string AudioFile { get; set; } = string.Empty;
        public string BackgroundFile { get; set; } = string.Empty;

        public BeatmapMetadata(BeatmapMetadata? original = null)
        {
            if (original == null)
            {
                Title = @"Unknown";
                Artist = @"Unknown";
                AuthorString = @"Unknown Creator";
            }
        }
    }
}
