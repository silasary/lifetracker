using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LifeCounter
{
    class CommanderDamage : LifeButton
    {
        private LifeButton linkedButton = null;

        public LifeButton LinkedCounter {
            get {
                if (linkedButton == null && Parent != null)
                {
                    LinkedCounter = (Parent as Grid).Children.OfType<LifeButton>().First();
                }
                return linkedButton;
            }
            set => SetProperty(ref linkedButton, value);
        }

        public CommanderDamage() : base()
        {
            LifeTotal = 0;
        }

        protected override void AddLife_Click(object sender, RoutedEventArgs e)
        {
            base.AddLife_Click(sender, e);
            LinkedCounter.LifeTotal--;
        }

        protected override void RemoveLife_Click(object sender, RoutedEventArgs e)
        {
            base.RemoveLife_Click(sender, e);
            LinkedCounter.LifeTotal++;
        }
    }
}
