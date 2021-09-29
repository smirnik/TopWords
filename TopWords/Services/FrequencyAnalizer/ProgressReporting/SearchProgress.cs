namespace TopWords.Services.FrequencyAnalizer.ProgressReporting
{
    internal class SearchProgress
    {
        public static ErrorOccured Error(string error) => new(error);

        public static FileProcessed FileProcessed(string fileName, int wordsCount, int wordsCountTotal) => new(fileName, wordsCount, wordsCountTotal);

        public static FoundFilesCount FilesCount(int filesCount) => new(filesCount);

        public static StatusChanged Status(string status, string details) => new(status, details);

        public static StatusChanged Status(string status) => new(status);

    }
}
