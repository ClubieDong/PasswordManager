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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PasswordManager.Client.Models;
using PasswordManager.Client.Services;

namespace PasswordManager.Client.Views
{
    /// <summary>
    /// PasswordGenerator.xaml 的交互逻辑
    /// </summary>
    public partial class Generator : UserControl
    {
        public PasswordRule PasswordRule;

        /// <summary>
        /// 密码已生成
        /// </summary>
        public Action<string> OnGenerated;

        public Generator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 加载并显示密码规则
        /// </summary>
        public void Load()
        {
            txtLength.Text = Convert.ToString(PasswordRule.Length);

            chkAllowNumbers.IsChecked = PasswordRule.AllowNumbers;
            chkAllowUpperCases.IsChecked = PasswordRule.AllowUpperCases;
            chkAllowLowerCases.IsChecked = PasswordRule.AllowLowerCases;
            chkAllowSpecialCharacters.IsChecked = PasswordRule.AllowSpecialCharacters;

            chkAllowNumbersFirst.IsChecked = PasswordRule.AllowNumbersFirst;
            chkAllowUpperCasesFirst.IsChecked = PasswordRule.AllowUpperCasesFirst;
            chkAllowLowerCasesFirst.IsChecked = PasswordRule.AllowLowerCasesFirst;
            chkAllowSpecialCharactersFirst.IsChecked = PasswordRule.AllowSpecialCharactersFirst;

            txtLeastNumberCount.Text = Convert.ToString(PasswordRule.LeastNumberCount);
            txtLeastUpperCaseCount.Text = Convert.ToString(PasswordRule.LeastUpperCaseCount);
            txtLeastLowerCaseCount.Text = Convert.ToString(PasswordRule.LeastLowerCaseCount);
            txtLeastSpecialCharacterCount.Text = Convert.ToString(PasswordRule.LeastSpecialCharacterCount);

            txtBannedCharacters.Text = PasswordRule.BannedCharacters;
            chkBannedCharactersIgnoreCase.IsChecked = PasswordRule.BannedCharactersIgnoreCase;

            txtBannedStrings.Text = string.Join(Environment.NewLine, PasswordRule.BannedStrings);
            chkBannedStringsIgnoreCase.IsChecked = PasswordRule.BannedStringsIgnoreCase;

            txtBannedContinuousCharacters.Text = string.Join(Environment.NewLine, PasswordRule.BannedContinuousCharacters);
            chkBannedContinuousCharactersIgnoreCase.IsChecked = PasswordRule.BannedContinuousCharactersIgnoreCase;

            chkBanRepeatedCharacters.IsChecked = PasswordRule.BanRepeatedCharacters;
            chkBanContinuousRepeatedCharacters.IsChecked = PasswordRule.BanContinuousRepeatedCharacters;
            txtBanContinuousRepeatedCharacterCount.Text = Convert.ToString(PasswordRule.BanContinuousRepeatedCharacterCount);
        }

        /// <summary>
        /// 将控件的数据保存到密码规则实例
        /// </summary>
        public void GetData()
        {
            PasswordRule.Length = Convert.ToInt32(txtLength.Text);

            PasswordRule.AllowNumbers = chkAllowNumbers.IsChecked.Value;
            PasswordRule.AllowUpperCases = chkAllowUpperCases.IsChecked.Value;
            PasswordRule.AllowLowerCases = chkAllowLowerCases.IsChecked.Value;
            PasswordRule.AllowSpecialCharacters = chkAllowSpecialCharacters.IsChecked.Value;

            PasswordRule.AllowNumbersFirst = chkAllowNumbersFirst.IsChecked.Value;
            PasswordRule.AllowUpperCasesFirst = chkAllowUpperCasesFirst.IsChecked.Value;
            PasswordRule.AllowLowerCasesFirst = chkAllowLowerCasesFirst.IsChecked.Value;
            PasswordRule.AllowSpecialCharactersFirst = chkAllowSpecialCharactersFirst.IsChecked.Value;

            PasswordRule.LeastNumberCount = Convert.ToInt32(txtLeastNumberCount.Text);
            PasswordRule.LeastUpperCaseCount = Convert.ToInt32(txtLeastUpperCaseCount.Text);
            PasswordRule.LeastLowerCaseCount = Convert.ToInt32(txtLeastLowerCaseCount.Text);
            PasswordRule.LeastSpecialCharacterCount = Convert.ToInt32(txtLeastSpecialCharacterCount.Text);

            PasswordRule.BannedCharacters = txtBannedCharacters.Text;
            PasswordRule.BannedCharactersIgnoreCase = chkBannedCharactersIgnoreCase.IsChecked.Value;

            PasswordRule.BannedStrings = txtBannedStrings.Text.Split(Environment.NewLine.ToCharArray()).Where(n => n != string.Empty).ToList();
            PasswordRule.BannedStringsIgnoreCase = chkBannedStringsIgnoreCase.IsChecked.Value;

            PasswordRule.BannedContinuousCharacters = txtBannedContinuousCharacters.Text.Split(Environment.NewLine.ToCharArray())
                .Where(n => n != string.Empty).ToList();
            PasswordRule.BannedContinuousCharactersIgnoreCase = chkBannedContinuousCharactersIgnoreCase.IsChecked.Value;

            PasswordRule.BanRepeatedCharacters = chkBanRepeatedCharacters.IsChecked.Value;
            PasswordRule.BanContinuousRepeatedCharacters = chkBanContinuousRepeatedCharacters.IsChecked.Value;
            PasswordRule.BanContinuousRepeatedCharacterCount = Convert.ToInt32(txtBanContinuousRepeatedCharacterCount.Text);
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            GetData();
            OnGenerated?.Invoke(PasswordGenerator.Generate(PasswordRule, 1000));
        }
    }
}