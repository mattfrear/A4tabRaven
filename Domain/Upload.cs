using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Upload
    {
        [Required]
        [StringLength(50)]
        public string Artist { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Song Name")]
        public string Name { get; set; }

        public int? BookId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Tab { get; set; }
    }
}