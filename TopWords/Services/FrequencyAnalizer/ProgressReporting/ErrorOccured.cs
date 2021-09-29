namespace TopWords.Services.FrequencyAnalizer.ProgressReporting
{
    internal class ErrorOccured 
    {
        public string Error { get; set; }

        public ErrorOccured(string error)
        {
            Error = error;
        }
    }
}
