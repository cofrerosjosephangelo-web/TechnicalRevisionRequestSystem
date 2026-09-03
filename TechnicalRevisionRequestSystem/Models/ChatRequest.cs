namespace TechnicalRevisionRequestSystem.Models
{
	public class ChatRequest
	{
		public string model { get; set; }
		public List<Message> messages { get; set; }
		public double temperature { get; set; } = 0.2;
	}

	public class Message
	{
		public string role { get; set; }
		public string content { get; set; }
	}
}
