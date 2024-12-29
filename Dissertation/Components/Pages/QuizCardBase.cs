using Dissertation.Models;
using Microsoft.AspNetCore.Components;

namespace Dissertation.Components.Pages;

public class QuizCardBase : ComponentBase
{
    public List<Question> Questions { get; set; } = [];
    protected int QuestionIndex;
    protected int Score;

    protected override Task OnInitializedAsync()
    {
        LoadQuestions();

        return base.OnInitializedAsync();
    }

    protected void OptionSelected(string option)
    {
        if (option.Equals(Questions[QuestionIndex].Answer))
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
        var questionOne = new Question
        {
            QuestionTitle = "How many letters does Scrum have?",
            Answer = "5",
            PossibleOptions = ["1", "2", "3", "5"]
        };

        var questionTwo = new Question
        {
            QuestionTitle = "What does Scrum mean?",
            Answer = "44",
            PossibleOptions = ["12", "22", "44", "50"]
        };

        Questions.AddRange(
            new List<Question>
                { questionOne, questionTwo }
        );
    }
}