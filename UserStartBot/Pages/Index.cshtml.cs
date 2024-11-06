using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserStartBot.Services;

namespace UserStartBot.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserControlledBackgroundTask _backgroundTask;

        public IndexModel(ILogger<IndexModel> logger, UserControlledBackgroundTask backgroundTask)
        {
            _logger = logger;
            _backgroundTask = backgroundTask;
        }

        public void OnGet()
        {
            _logger.LogInformation("Arrived at page one");
        }

        public IActionResult OnPostStartTask()
        {
            _backgroundTask.StartTask();
            _logger.LogInformation("User has started the background task");
            return RedirectToPage();
        }

        public IActionResult OnPostStopTask()
        {
            _backgroundTask.StopTask();
            _logger.LogInformation("User has stopped the background task");
            return RedirectToPage();
        }

        public IActionResult OnGetServiceState()
        {
            return new JsonResult(new { _backgroundTask.IsRunning });
        }
    }
}