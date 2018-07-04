using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LifeCounter
{
    /// <summary>
    /// Interaction logic for LifeButton.xaml
    /// </summary>
    public partial class LifeButton : UserControl, INotifyPropertyChanged
    {
        public LifeButton()
        {
            InitializeComponent();
        }

        protected int lifeTotal = 40;

        public int LifeTotal { get => lifeTotal; set => SetProperty(ref lifeTotal, value); }

        #region INotifyPropertyChanged implementation

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void AddLife_Click(object sender, RoutedEventArgs e)
        {
            LifeTotal++;
        }

        protected virtual void RemoveLife_Click(object sender, RoutedEventArgs e)
        {
            LifeTotal--;
        }
    }
}
