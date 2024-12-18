namespace FileService.Models
{
    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string FilePath { get; set; }
        public string Url { get; set; }
        public string ErrorMessage { get; set; }
        public StorageType StorageType { get; set; }

    }
    public enum StorageType
    {
        Local,
        GoogleDrive
    }
}
