namespace MySecureBackend.WebApi.Models
{
    public class Object2D
    {
        public string Id { get; set; } = string.Empty;
        public string EnvironmentId { get; set; } = string.Empty;
        public string PrefabId { get; set; } = string.Empty;
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float RotationZ { get; set; }
        public int SortingLayer { get; set; }
    }
}
