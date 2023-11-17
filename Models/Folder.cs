using System.ComponentModel.DataAnnotations;

namespace TestTask.Models
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, 20)]
        public string Name { get; set; }
        public int? ParentFolderId { get; set; }
    }
}
