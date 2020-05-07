using System.ComponentModel.DataAnnotations;

namespace nats_ui.Data
{
    public class NatsSessionModel
    {
        public string Url => $"nats://{Host}:{Port}";

        [Required]
        public string Host { get; set; } = "127.0.0.1";

        [Required]
        public int Port { get; set; } = 4222;
    }
}