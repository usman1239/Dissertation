using Microsoft.AspNetCore.Components;

namespace Dissertation.Components.Pages.Learn
{
    public class OptionCardBase : ComponentBase
    {
        [Parameter]
        public string Option { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> OnOptionSelectedCallback { get; set; }

        protected async void SelectOption()
        {
            try
            {
                await OnOptionSelectedCallback.InvokeAsync(Option);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
