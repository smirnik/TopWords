namespace TopWords.Messages
{
    public class SearchCommandMessage
    {
        public string Path { get; set; }

        public bool SearchInSubfolders { get; set; }

        public SearchCommandMessage(string path, bool searchInSubfolders)
        {
            Path = path;
            SearchInSubfolders = searchInSubfolders;
        }
    }
}
