namespace TechnicalRevisionRequestSystem.Services.AI
{
	public interface IAIService
	{
		Task<string> AskAsync(string question);


		Task<string> AskAboutRequestAsync(
			int trrsId,
			string question);


		Task<string> AskKnowledgeAsync(
			string question);
	}
}
