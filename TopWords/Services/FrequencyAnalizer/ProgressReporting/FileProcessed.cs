namespace TopWords.Services.FrequencyAnalizer.ProgressReporting
{
    internal class FileProcessed
    {
        public string FileName { get; set; }

        public int WordsCountInFile { get; set; }
        public int WordsCountTotal { get; set; }

        public FileProcessed(string fileName, int wordsCountInFile, int wordsCountTotal)
        {
            FileName = fileName;
            WordsCountInFile = wordsCountInFile;
            WordsCountTotal = wordsCountTotal;
        }
    }
}
