namespace MySecureBackend.WebApi.Models
{
    public class Environment2D
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string OwnerUserId { get; set; } = string.Empty;
        public int MaxLength { get; set; }
        public int MaxHeight { get; set; }
        public int BackgroundIndex { get; set; } = -1; // -1 = geen background, 0-3 = specifieke backgrounds
    }
}

