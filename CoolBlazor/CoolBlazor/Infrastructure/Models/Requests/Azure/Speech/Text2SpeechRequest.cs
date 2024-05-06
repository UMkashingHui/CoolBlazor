using System.ComponentModel.DataAnnotations;

namespace CoolBlazor.Infrastructure.Models.Requests.Azure.Speech
{
    public class Text2SpeechRequest
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string Speaker { get; set; }
    }
}