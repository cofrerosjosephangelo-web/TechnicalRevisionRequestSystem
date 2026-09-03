namespace TechnicalRevisionRequestSystem.Models
{
	public class ChatResponse
	{
		public List<Choice> choices { get; set; }
	}

	public class Choice
	{
		public Message message { get; set; }
	}
}
