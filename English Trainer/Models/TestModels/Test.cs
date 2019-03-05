namespace English_Trainer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Test
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Test()
        {
            Questions = new ObservableCollection<Question>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Creator { get; set; }

        public DateTime? CreationDate { get; set; }
        public DateTime? ChangingDate { get; set; }

        [StringLength(2)]
        public string Level { get; set; }

        public int TotalWeight { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsReady { get; set; }
        public bool CanUserSeeResult { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Question> Questions { get; set; }
    }
}
