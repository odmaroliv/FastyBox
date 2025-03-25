using Microsoft.AspNetCore.Components;

namespace FastyBox.Web.Services
{
    public interface IDialogService
    {
        Task<DialogResult> ShowAsync<TComponent>(string title, DialogParameters parameters = null) where TComponent : ComponentBase;
        void Close(DialogResult result = null);
    }

    public class DialogService : IDialogService
    {
        private RenderFragment _content;
        private TaskCompletionSource<DialogResult> _resultCompletion;
        private bool _isVisible;
        private string _title;
        private event Action<RenderFragment, string, bool> OnDialogChanged;

        public void OnDialogCloseRequested(DialogResult result)
        {
            _resultCompletion?.TrySetResult(result ?? DialogResult.Cancel());
            _isVisible = false;
            OnDialogChanged?.Invoke(null, null, _isVisible);
        }

        public Task<DialogResult> ShowAsync<TComponent>(string title, DialogParameters parameters = null) where TComponent : ComponentBase
        {
            _resultCompletion = new TaskCompletionSource<DialogResult>();

            parameters ??= new DialogParameters();
            parameters.Add("CloseDialog", new Action<bool>((bool result) =>
            {
                OnDialogCloseRequested(new DialogResult { Canceled = !result });
            }));

            _content = builder =>
            {
                builder.OpenComponent<TComponent>(0);

                foreach (var param in parameters.Parameters)
                {
                    builder.AddAttribute(1, param.Key, param.Value);
                }

                builder.CloseComponent();
            };

            _title = title;
            _isVisible = true;
            OnDialogChanged?.Invoke(_content, _title, _isVisible);

            return _resultCompletion.Task;
        }

        public void Close(DialogResult result = null)
        {
            OnDialogCloseRequested(result);
        }

        public event Action<RenderFragment, string, bool> DialogChanged
        {
            add => OnDialogChanged += value;
            remove => OnDialogChanged -= value;
        }
    }

    // The DialogParameters class needs to implement IEnumerable to support collection initializers
    public class DialogParameters : IEnumerable<KeyValuePair<string, object>>
    {
        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public void Add(string key, object value)
        {
            Parameters[key] = value;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Parameters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Parameters.GetEnumerator();
        }
    }

    public class DialogResult
    {
        public bool Canceled { get; set; }
        public object Data { get; set; }


        public object Result => Data;

        public static DialogResult Ok(object data = null)
        {
            return new DialogResult { Canceled = false, Data = data };
        }

        public static DialogResult Cancel()
        {
            return new DialogResult { Canceled = true };
        }
    }
}