namespace UserStartBot.Services
{
    public class UserControlledBackgroundTask
    {
        private readonly ILogger<UserControlledBackgroundTask> _logger;
        private CancellationTokenSource? _cts;
        private Task? _executingTask;
        public bool IsRunning { get; private set; }
        private readonly object _lock = new();

        public UserControlledBackgroundTask(ILogger<UserControlledBackgroundTask> logger)
        {
            _logger = logger;
        }

        public void StartTask()
        {
            lock (_lock)
            {
                if (_executingTask == null || _executingTask.IsCompleted)
                {
                    _cts = new CancellationTokenSource();
                    _executingTask = Task.Run(() => RunTask(_cts.Token), _cts.Token).ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            _logger.LogError("Background Task encountred an error. Error Message: {ErrorMessage} -- Stack Trace: {StackTrace}", t.Exception.Message, t.Exception.StackTrace);
                        }
                        IsRunning = false;
                    }, TaskContinuationOptions.OnlyOnFaulted);
                    IsRunning = true;
                    _logger.LogInformation("Background Task started");
                }
            }
        }

        public void StopTask()
        {
            lock (_lock)
            {
                if (_cts != null && !_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                    IsRunning = false;
                    _logger.LogInformation("Background Task stopped");
                }
            }
        }

        private async Task RunTask(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    _logger.LogInformation("Backhround Task is running after user click, current time: {CurrentTime}", DateTimeOffset.Now);
                    await Task.Delay(1000, token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An Error occurred when running the background task. Error Message: {ErrorMessage}  -- Stack Trace: {StackTrace}", ex.Message, ex.StackTrace);
            }
            finally
            {
                //perform cleanup here if any
                _logger.LogInformation("Background task is finally stopping.");
            }
        }
    }
}