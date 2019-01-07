using PasswordManager.Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace PasswordManager.Client.Views
{
    /// <summary>
    /// ItemDataShower.xaml 的交互逻辑
    /// </summary>
    public partial class ItemDataShower : UserControl
    {
        public ItemData ItemData;

        public ItemDataShower()
        {
            InitializeComponent();
            btnCopy.Click += (s, e) => Copy();
        }

        /// <summary>
        /// 加载并显示数据
        /// </summary>
        public void Load()
        {
            tbKey.Text = ItemData.Key + "：";
            if (!ItemData.IsPassword && !ItemData.IsLink && !ItemData.IsSplitter)
            {
                // 如果是普通数据
                tbData.Text = ItemData.Data;
                btnShowOrOpen.Visibility = Visibility.Hidden;
            }
            else if (ItemData.IsPassword && !ItemData.IsLink && !ItemData.IsSplitter)
            {
                // 如果是密码，显示密码框
                tbData.Text = ItemData.Data;
                tbData.Visibility = Visibility.Collapsed;
                pwdData.Visibility = Visibility.Visible;
                btnShowOrOpen.Content = "显示密码";
                btnShowOrOpen.Click += (s, e) => ShowOrHidePassword();
            }
            else if (!ItemData.IsPassword && ItemData.IsLink && !ItemData.IsSplitter)
            {
                // 如果是链接，创建超链接
                Hyperlink link = new Hyperlink();
                link.Inlines.Add(ItemData.Data);
                tbData.Inlines.Add(link);
                btnShowOrOpen.Content = "打开链接";
                btnShowOrOpen.Click += (s, e) => OpenLink();
                link.Click += (s, e) => OpenLink();
            }
            else if (!ItemData.IsPassword && !ItemData.IsLink && ItemData.IsSplitter)
            {
                // 如果是分割线，显示分割线
                grdNotSplitter.Visibility = Visibility.Collapsed;
                splitter.Visibility = Visibility.Visible;
            }
            else
                throw new Exception("项目数据类型无效！");
        }

        /// <summary>
        /// 显示和隐藏密码
        /// </summary>
        private void ShowOrHidePassword()
        {
            // 通过按钮标题来判断现在密码是否显示
            switch ((string)btnShowOrOpen.Content)
            {
                case "显示密码":
                    tbData.Visibility = Visibility.Visible;
                    pwdData.Visibility = Visibility.Collapsed;
                    btnShowOrOpen.Content = "隐藏密码";
                    break;
                case "隐藏密码":
                    tbData.Visibility = Visibility.Collapsed;
                    pwdData.Visibility = Visibility.Visible;
                    btnShowOrOpen.Content = "显示密码";
                    break;
                default:
                    throw new Exception("按钮文本无效！");
            }
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        private void OpenLink()
        {
            try
            {
                Process.Start(ItemData.Data);
            }
            catch (Exception)
            {
                // 如果打开失败，报错
                MessageBox.Show("无法打开链接！");
            }
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        private void Copy()
        {
            // 需要完善
            // 由于复制时可能会出现莫名其妙的错误，这里需要加异常处理
            try
            {
                Clipboard.SetDataObject(ItemData.Data);
            }
            catch
            {
                throw new Exception("复制失败，请重新操作！");
            }
        }
    }
}
