using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>().AddEnvironmentVariables().Build();

var chat = new KernelBuilder()
    .WithOpenAIChatCompletionService("gpt-3.5-turbo", config["OPENAI_KEY"]).Build()
    .GetService<IChatCompletion>();

var chatHistory = chat.CreateNewChat("You are a helpful assistant.");

while (true)
{
    Console.Write($"{Environment.NewLine}Input: ");
    chatHistory.AddUserMessage(Console.ReadLine());

    Console.Write($"{Environment.NewLine}Assistant: ");
    chatHistory.AddAssistantMessage(string.Empty);
    await foreach (var message in chat.GenerateMessageStreamAsync(chatHistory))
    {
        chatHistory.Last().Content += message;
        Console.Write(message);
    }
    Console.WriteLine();
}