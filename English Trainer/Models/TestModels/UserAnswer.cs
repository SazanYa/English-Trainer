namespace English_Trainer
{
    public partial class UserAnswer
    {
        public int Id { get; set; }

        public int? ProgressId { get; set; }

        public int? AnswerId { get; set; }

        public virtual Progress Progress { get; set; }
    }
}
