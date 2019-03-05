namespace English_Trainer
{
    using System.ComponentModel.DataAnnotations;

    public partial class Answer
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public bool IsRight { get; set; }

        public int? QuestionId { get; set; }

        public virtual Question Question { get; set; }
    }
}
