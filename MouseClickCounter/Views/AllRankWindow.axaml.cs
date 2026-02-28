using Avalonia.Controls;
using Avalonia.Interactivity;
using MouseClickCounter.ViewModels;

namespace MouseClickCounter.Views
{
    public partial class AllRankWindow : Window
    {
        public AllRankWindow()
        {
            InitializeComponent();
        }

        public AllRankWindow(AllRankViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            // 绑定关闭按钮点击事件
            var closeButton = this.FindControl<Button>("CloseButton");
            if (closeButton != null)
            {
                closeButton.Click += (s, e) => Close();
            }
        }
    }
}
