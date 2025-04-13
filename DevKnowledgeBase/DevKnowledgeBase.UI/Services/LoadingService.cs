namespace DevKnowledgeBase.UI.Services
{
    public class LoadingService
    {
        public event Action? OnShow;
        public event Action? OnHide;

        public void Show()
        {
            OnShow?.Invoke();
        }

        public void Hide()
        {
            OnHide?.Invoke();
        }

        public async Task RunWithLoading(Func<Task> action)
        {
            Show();
            try
            {
                await action();
            }
            finally
            {
                Hide();
            }
        }

        public async Task<T> RunWithLoading<T>(Func<Task<T>> action)
        {
            Show();
            try
            {
                return await action();
            }
            finally
            {
                Hide();
            }
        }
    }
}
