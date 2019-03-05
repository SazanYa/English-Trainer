namespace English_Trainer
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Username { get; set; }

        [Required]
        public string Salt { get; set; }

        [Required]
        public string Hash { get; set; }

        [StringLength(2)]
        public string Level { get; set; }

        public bool IsTeacher { get; set; }
    }
}
