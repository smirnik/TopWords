namespace TopWords.Services.FrequencyAnalizer.ProgressReporting
{
    internal class FoundFilesCount
    {
        public int FilesCount { get; set; }

        public FoundFilesCount(int filesCount)
        {
            FilesCount = filesCount;
        }
    }
}
