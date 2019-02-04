using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;

namespace ImgDiffTool.ViewModels
{
    sealed class ShellViewModel : Conductor<object>
    {
        private readonly IWindowManager _windowManager;

        public ShellViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;

            Activated += OnActivated;
        }

        private void OnActivated(object sender, ActivationEventArgs e)
        {
            Execute.OnUIThreadAsync(() =>
            {
                var settings = new Dictionary<string, object>
                {
                    ["ShowCloseButton"] = false,
                    ["IsCloseButtonEnabled"] = false,
                    ["ShowMaximizeButton"] = false,
                    ["IsMaximizeButtonEnabled"] = false,
                    ["ShowMinimizeButton"] = false,
                    ["IsMinimizeButtonEnabled"] = false,

                };

                var dialogResult = _windowManager.ShowDialog(IoC.Get<ConfigViewModel>(), settings: settings);

                if (dialogResult.HasValue && dialogResult.Value)
                {
                    ActivateItem(IoC.Get<ImgDiffViewModel>());
                }
                else
                {
                    Application.Current.Shutdown(0);
                }
            });
        }
    }
}
