namespace BackEnd.Configurations
{
    public class JwtSettings
    {
        public required string Key { get; set; }
        public  string Issuer { get; set; }
        //public string Audience { get; set; }
        // Outras propriedades conforme necessário
    }
}
