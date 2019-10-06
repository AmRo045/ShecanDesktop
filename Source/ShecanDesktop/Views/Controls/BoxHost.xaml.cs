using System.Windows;
using System.Windows.Input;
using ShecanDesktop.Contracts;

namespace ShecanDesktop.Views.Controls
{
    public partial class BoxHost
    {
        public BoxHost()
        {
            InitializeComponent();
        }

        public BoxHost(bool closeOnClickAway) 
            : this()
        {
            CloseOnClickAway = closeOnClickAway;
        }

        public bool CloseOnClickAway { get; set; }
        public FrameworkElement ParentElement { get; set; }

        public void Close()
        {
            BoxContainerCanvas.UpdateLayout();
            BoxContentFrame.Content = null;

            SetParentStatus(true);

            if (BoxContainerCanvas.Visibility.Equals(Visibility.Visible))
                BoxContainerCanvas.Visibility = Visibility.Collapsed;
        }

        public void Show(IBox box)
        {
            if (!CanShow(box))
                return;

            SetParentStatus(false);
            BoxContentFrame.Navigate(box);

            if (BoxContainerCanvas.Visibility.Equals(Visibility.Collapsed))
                BoxContainerCanvas.Visibility = Visibility.Visible;

            if (BoxContentFrame.CanGoBack)
                BoxContentFrame.RemoveBackEntry();
        }

        private bool CanShow(IBox box)
        {
            var currentSource = (IBox)BoxContentFrame.Content;

            if (currentSource == null) return true;
            return !currentSource.GetType().Name.Equals(box.GetType().Name);
        }

        private void SetParentStatus(bool status)
        {
            if (ParentElement == null) return;

            ParentElement.IsEnabled = status;
            ParentElement.Focusable = status;
        }

        private void BackgroundOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CloseOnClickAway)
                Close();
        }
    }
}
