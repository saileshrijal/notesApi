using System.ComponentModel.DataAnnotations;

namespace Notes.ViewModels
{
    public class NoteVM
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
