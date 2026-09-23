namespace CrudBackend_1_.config
{
    public class JwtSettings
    {
        public const string SectionName = "ExternalAPIS";
        public string JWTSecret { get; set; } = null!;
        public int JWTExpiry { get; set; }
    }
}
