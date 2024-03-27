using BatchRename_MainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BatchRename_UtilClasses;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BatchRename_MainUI
{
    /// <summary>
    /// Interaction logic for RuleSettingUI.xaml
    /// </summary>
    public partial class RuleSettingUI : Window, INotifyPropertyChanged
    {
        public delegate void RuleParamValueChanged(string key, string value);
        public event RuleParamValueChanged? RuleParamValueChangedEvent;
        public ObservableCollection<BindableRuleParameter> RuleParameters { get; set; } 
            = new ObservableCollection<BindableRuleParameter>() { };
        private IRule _rule { get; set; }
        public RuleSettingUI(IRule rule)
        {
            InitializeComponent();

            _rule = (IRule)rule.Clone();

            foreach (var key in rule.RuleParameters.Keys)
            {
                RuleParameters.Add(
                    new BindableRuleParameter()
                    {
                        ParamKey = key,
                        ParamValue = rule.RuleParameters[key],
                        RuleError = ""
                    }
                );
            }    

            DisplayingText.Text = rule.TextToDisplay;
            RuleParameterList.ItemsSource = RuleParameters;
        }

        private void RuleParameterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            int selectedIndex = RuleParameterList.SelectedIndex;
            
            bool success = true;
            string message = "";

            TextBox paramValueTextBox = (TextBox)sender;
            
            RuleParameters[selectedIndex].ParamValue = paramValueTextBox.Text;

            (success, message) = _rule.ValidateParameters(
                RuleParameters[selectedIndex].ParamKey,
                RuleParameters[selectedIndex].ParamValue 
            );

            if (success) 
            {
                RuleParameters[selectedIndex].RuleError = "";

                RuleParamValueChangedEvent!.Invoke(
                    RuleParameters[selectedIndex].ParamKey,
                    RuleParameters[selectedIndex].ParamValue
                );
            }
            else
            {
                RuleParameters[selectedIndex].RuleError = message;
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = true;
            string message = "";

            foreach (var param in RuleParameters)
            {
                (success, message) = _rule.ValidateParameters(
                    param.ParamKey,
                    param.ParamValue
                );

                if (!success)
                {
                    break;
                }
                else
                {
                    // Do nothing
                }
            }

            if (success)
            {
                DialogResult = true;
            }
            else
            {
                foreach (var param in RuleParameters)
                {
                    (success, message) = _rule.ValidateParameters(
                        param.ParamKey,
                        param.ParamValue
                    );

                    if (success)
                    {
                        param.RuleError = "";
                    }
                    else
                    {
                        param.RuleError = message;
                    }
                }

                MessageBox.Show(
                    "The parameters seem to be incompatible.", 
                    "Rule Setting Warning"
                );
            }
        }
    }
}
