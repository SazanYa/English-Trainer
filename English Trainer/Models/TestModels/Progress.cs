namespace English_Trainer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Progress")]
    public partial class Progress
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Progress()
        {
            UserAnswers = new HashSet<UserAnswer>();
        }

        public int Id { get; set; }

        public string TestTitle { get; set; }

        public string TestCreator { get; set; }

        [StringLength(20)]
        public string Username { get; set; }

        public int? TestId { get; set; }

        public int? UserId { get; set; }

        public int? Mark { get; set; }

        public DateTime? Date { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
