using System.Windows;
using System.Windows.Controls;

namespace Arduino.UserControls
{
    /// <summary>
    /// Interaction logic for TemperatureGauge.xaml
    /// </summary>
    public partial class TemperatureGauge : UserControl
    {
        public static readonly DependencyProperty TemperatureProperty = DependencyProperty.Register(
            "Temperature", typeof(float?),
            typeof(TemperatureGauge));
        public static readonly DependencyProperty MinTemperatureProperty = DependencyProperty.Register(
            "MinTemperature", typeof(int),
            typeof(TemperatureGauge), new PropertyMetadata(-15));
        public static readonly DependencyProperty MaxTemperatureProperty = DependencyProperty.Register(
            "MaxTemperature", typeof(int),
            typeof(TemperatureGauge), new PropertyMetadata(45));
        public static readonly DependencyProperty DisplayTemperaturesProperty = DependencyProperty.Register(
            "DisplayTemperatures", typeof(bool),
            typeof(TemperatureGauge), new PropertyMetadata(true));

        public float? Temperature
        {
            get => (float?)GetValue(TemperatureProperty);
            set => SetValue(TemperatureProperty, value);
        }

        public int MinTemperature
        {
            get => (int)GetValue(MinTemperatureProperty);
            set => SetValue(MinTemperatureProperty, value);
        }

        public int MaxTemperature
        {
            get => (int)GetValue(MaxTemperatureProperty);
            set => SetValue(MaxTemperatureProperty, value);
        }

        public bool DisplayTemperatures
        {
            get => (bool)GetValue(DisplayTemperaturesProperty);
            set => SetValue(DisplayTemperaturesProperty, value);
        }

        public TemperatureGauge()
        {
            InitializeComponent();
        }
    }
}
