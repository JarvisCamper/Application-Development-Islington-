using System;
using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }

        [Required]
        public DateTime EntryDate { get; set; } = DateTime.Today;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
