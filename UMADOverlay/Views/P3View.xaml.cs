using System.Windows;
using System.Windows.Controls;
namespace UMADOverlay.Views
{
    public partial class P3View : UserControl
    {
        public static readonly DependencyProperty FlippedProperty =
            DependencyProperty.Register(nameof(Flipped), typeof(bool), typeof(P3View),
                new PropertyMetadata(false, OnFlippedChanged));

        public bool Flipped
        {
            get => (bool)GetValue(FlippedProperty);
            set => SetValue(FlippedProperty, value);
        }

        static void OnFlippedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (P3View)d;
            view.ApplyFlip((bool)e.NewValue);
        }

        public P3View() => InitializeComponent();

        void ApplyFlip(bool flipped)
        {
            // RootGrid columns: 0=answers, 1=spacer, 2=buttons
            // When flipped: swap col 0 and col 2
            var ansCol = RootGrid.ColumnDefinitions[0];
            var btnCol = RootGrid.ColumnDefinitions[2];

            if (flipped)
            {
                Grid.SetColumn(AnsColumn, 2);
                Grid.SetColumn(BtnColumn, 0);
            }
            else
            {
                Grid.SetColumn(AnsColumn, 0);
                Grid.SetColumn(BtnColumn, 2);
            }
        }
    }
}
