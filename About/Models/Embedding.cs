using System.ComponentModel.DataAnnotations;

namespace About.Models
{
    public class Embedding
    {
        [Key]
        public string EmbeddingID { get; set; }

        public string? EmbeddingQuestion { get; set; }

        public string? EmbeddingAnswer { get; set; }

        public string QA { get; set; }

        public string EmbeddingVectors { get; set; }

    }
}
