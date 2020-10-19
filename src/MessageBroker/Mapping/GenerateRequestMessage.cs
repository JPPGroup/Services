using System.ComponentModel.DataAnnotations;

namespace Jpp.MessageBroker.Mapping
{
    public class GenerateRequestMessage
    {
        [Required]
        [EmailAddress]
        public string Email;
        [Required]
        public string Client;
        [Required]
        public string Project;
        [Required]
        public double Latitude;
        [Required]
        public double Longitude;
        [Required]
        public string Id;
    }
}
