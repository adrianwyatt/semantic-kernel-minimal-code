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
    Console.Write("Input: ");
    chatHistory.AddUserMessage(Console.ReadLine());
    chatHistory.AddAssistantMessage(await chat.GenerateMessageAsync(chatHistory));
    Console.WriteLine($"Assistant: {chatHistory.Last()}");
}