using Passwords.Client.Services;
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

namespace Passwords.Client.Views
{
    /// <summary>
    /// Lock.xaml 的交互逻辑
    /// </summary>
    public partial class Lock : UserControl
    {
        public Data Data;
        /// <summary>
        /// 解锁成功时触发
        /// </summary>
        public event Action OnUnlock;

        public Lock()
        {
            InitializeComponent();
        }

        private void Pwb_KeyDown(object sender, KeyEventArgs e)
        {
            // 如果按下的不是回车键，直接退出
            if (e.Key != Key.Enter)
                return;
            // 尝试连接
            if (Data.Connect(pwb.Password))
                OnUnlock?.Invoke();
            else
                MessageBox.Show("密码不正确！");
            // 无论是否成功，清空密码框，防止密码泄露
            pwb.Password = string.Empty;
        }
    }
}
