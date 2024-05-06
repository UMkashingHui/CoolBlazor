using System.ComponentModel.DataAnnotations;

namespace CoolBlazor.Infrastructure.Models.Responses.Azure.Speech
{
    public class Text2SpeechResponse
    {
        public string Reason { get; set; }

        public string AudioDuration { get; set; }

        public string Properties { get; set; }

        public string AudioData { get; set; }

        public string ResultId { get; set; }
    }
}