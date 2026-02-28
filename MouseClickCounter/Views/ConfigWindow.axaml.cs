using Avalonia.Controls;
using Avalonia.Interactivity;
using MouseClickCounter.ViewModels;

namespace MouseClickCounter.Views
{
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
        }

        public ConfigWindow(ConfigViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            // 绑定取消按钮点击事件
            var cancelButton = this.FindControl<Button>("CancelButton");
            if (cancelButton != null)
            {
                cancelButton.Click += (s, e) => Close();
            }

            // 绑定保存命令完成后关闭窗口
            if (DataContext is ConfigViewModel viewModel)
            {
                viewModel.SaveCommand.Execute(null);
            }
        }
    }
}
