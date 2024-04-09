using System.ComponentModel.DataAnnotations;

namespace git_test.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(22)]
        public string Name { get; set; }
        public string Email { get; set; }
        [MinLength(10)]
        [MaxLength(10)]
        public string Number { get; set; }
    }
}
