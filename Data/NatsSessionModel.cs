using System.ComponentModel.DataAnnotations;

namespace nats_ui.Data
{
    public class NatsSessionModel
    {
        public string Url => $"nats://{Host}:{Port}";

        public bool Selected { get; set; } = false;
        
        [Required]
        public string Name { get; set; } = "localhost";
        
        [Required]
        public string Host { get; set; } = "127.0.0.1";

        [Required]
        public int Port { get; set; } = 4222;

        public override string ToString()
        {
            return $"{Name}, {Host}, {Port}, {Selected}";
        }

        public NatsSessionModel Clone()
        {
            return (NatsSessionModel)MemberwiseClone();
        }
    }
}