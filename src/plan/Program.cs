using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Planning.Stepwise;
using Microsoft.SemanticKernel.Skills.Core;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>().AddEnvironmentVariables().Build();

var kernel = new KernelBuilder()
    //.WithOpenAIChatCompletionService("gpt-3.5-turbo", config["OPENAI_KEY"])
    .WithAzureChatCompletionService("gpt-35-turbo", config["AZUREOPENAI_ENDPOINT"], config["AZUREOPENAI_KEY"])
    .Build();

// TODO web search skill
kernel.ImportSkill(new LanguageCalculatorSkill(kernel), "advancedCalculator");
kernel.ImportSkill(new TimeSkill(), "time");

var planConfig = new StepwisePlannerConfig { MinIterationTimeMs = 1500, MaxTokens = 4000 };
//planConfig.ExcludedFunctions.Add("TranslateMathProblem");

StepwisePlanner planner = new(kernel, planConfig);

var chat = kernel.GetService<IChatCompletion>();

var chatHistory = chat.CreateNewChat("You are a helpful assistant.");

Console.WriteLine("Ask a question, such as \"What is the current president of the United States' age is Mars years?\"");

while (true)
{
    Console.Write($"{Environment.NewLine}Input: ");
    chatHistory.AddUserMessage(Console.ReadLine());
    var plan = planner.CreatePlan(chatHistory.Last().Content);

    Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));

    //while (plan.HasNextStep)
    //{
    //    await plan.InvokeNextStepAsync(kernel.CreateNewContext());
    //}
    
    //chatHistory.AddAssistantMessage((await plan.InvokeAsync(kernel.CreateNewContext())).Result);
    //Console.WriteLine($"{Environment.NewLine}Assistant: {chatHistory.Last()}");
}