using Dissertation.Models.Learn;
using Dissertation.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Dissertation.Services;

public class QuizCardBase : ComponentBase
{
    protected int QuestionIndex;
    protected int Score;
    [Inject] public required IQuestionProvider QuestionProvider { get; set; }

    [Parameter] public string Topic { get; set; } = string.Empty;

    public List<Question> Questions { get; set; } = [];

    protected override Task OnInitializedAsync()
    {
        LoadQuestions();
        return base.OnInitializedAsync();
    }

    protected void OptionSelected(string option)
    {
        if (option.Equals(Questions[QuestionIndex].Answer, StringComparison.OrdinalIgnoreCase))
            Score++;

        QuestionIndex++;
    }

    protected void RestartQuiz()
    {
        Score = 0;
        QuestionIndex = 0;
    }

    private void LoadQuestions()
    {
        Questions = QuestionProvider.GetQuestions(Topic);
    }
}