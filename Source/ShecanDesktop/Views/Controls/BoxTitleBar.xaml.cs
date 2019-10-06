using System;
using System.Windows;

namespace ShecanDesktop.Views.Controls
{
    public partial class BoxTitleBar
    {
        public BoxTitleBar()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void CloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            CloseButtonCallback.Invoke();
        }

        #region Dependency Properties

        public static readonly DependencyProperty CloseButtonCallbackProperty = DependencyProperty.Register(
            nameof(CloseButtonCallback), typeof(Action), 
            typeof(BoxTitleBar),
            new PropertyMetadata(default(Action)));

        public Action CloseButtonCallback
        {
            get { return (Action) GetValue(CloseButtonCallbackProperty); }
            set { SetValue(CloseButtonCallbackProperty, value); }
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(BoxTitleBar),
            new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty UseBackButtonProperty = DependencyProperty.Register(
            "UseBackButton", typeof(bool),
            typeof(BoxTitleBar), 
            new PropertyMetadata(default(bool)));

        public bool UseBackButton
        {
            get { return (bool) GetValue(UseBackButtonProperty); }
            set { SetValue(UseBackButtonProperty, value); }
        }

        #endregion Dependency Properties
    }
}
