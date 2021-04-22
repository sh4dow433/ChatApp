namespace ChatApi.DTOs
{
    public class MessageCreateDto
    {
        public string SenderId { get; set; }
        public int ChatId { get; set; }
        public string Text { get; set; }
        public bool HasFile { get; set; }
        public int? FileId { get; set; } = null;
    }
}
