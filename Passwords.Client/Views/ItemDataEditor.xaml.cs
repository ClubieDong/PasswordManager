using Passwords.Client.Models;
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

namespace Passwords.Client.Views
{
    /// <summary>
    /// ItemDataEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ItemDataEditor : UserControl
    {
        public ItemData ItemData;

        /// <summary>
        /// 请求进行列表操作
        /// 0: 上移
        /// 1: 下移
        /// 2: 删除
        /// 3: 插入
        /// </summary>
        public event Action<int, ItemData> OnListOperation;
        /// <summary>
        /// 数据类型被改变
        /// </summary>
        public event Action TypeChanged;

        public ItemDataEditor()
        {
            InitializeComponent();
            cbxType.SelectionChanged += (s, e) => ChangeType(cbxType.SelectedIndex);
            btnMoveUp.Click += (s, e) => OnListOperation?.Invoke(0, ItemData);
            btnMoveDown.Click += (s, e) => OnListOperation?.Invoke(1, ItemData);
            btnDelete.Click += (s, e) => OnListOperation?.Invoke(2, ItemData);
            btnInsert.Click += (s, e) => OnListOperation?.Invoke(3, ItemData);
        }

        /// <summary>
        /// 加载并显示项目数据
        /// </summary>
        public void Load()
        {
            // 普通数据
            if (!ItemData.IsPassword && !ItemData.IsLink && !ItemData.IsSplitter)
                cbxType.SelectedIndex = 0;
            // 密码
            else if (ItemData.IsPassword && !ItemData.IsLink && !ItemData.IsSplitter)
                cbxType.SelectedIndex = 1;
            // 链接
            else if (!ItemData.IsPassword && ItemData.IsLink && !ItemData.IsSplitter)
                cbxType.SelectedIndex = 2;
            // 分割线
            else if (!ItemData.IsPassword && !ItemData.IsLink && ItemData.IsSplitter)
                cbxType.SelectedIndex = 3;
            else
                throw new Exception("项目数据类型无效！");
            btnMoveUp.IsEnabled = ItemData.Order > 0;
            btnMoveDown.IsEnabled = ItemData.Order < ItemData.Item.ItemData.Count - 1;
            btnDelete.IsEnabled = ItemData.Item.ItemData.Count > 1;
        }

        /// <summary>
        /// 更改项目数据的类型
        /// </summary>
        /// <param name="comboBoxIndex"></param>
        private void ChangeType(int comboBoxIndex) 
        {
            switch (comboBoxIndex)
            {
                // 普通文本
                case 0:
                    ItemData.IsPassword = false;
                    ItemData.IsLink = false;
                    ItemData.IsSplitter = false;
                    grdText.Visibility = Visibility.Visible;
                    grdLine.Visibility = Visibility.Hidden;
                    txtData.Visibility = Visibility.Visible;
                    pwdData.Visibility = Visibility.Hidden;
                    btnGenerate.Visibility = Visibility.Hidden;
                    btnShowOrOpen.Visibility = Visibility.Hidden;
                    txtKey.Text = ItemData.Key;
                    txtData.Text = ItemData.Data;
                    break;
                // 密码
                case 1:
                    ItemData.IsPassword = true;
                    ItemData.IsLink = false;
                    ItemData.IsSplitter = false;
                    grdText.Visibility = Visibility.Visible;
                    grdLine.Visibility = Visibility.Hidden;
                    txtData.Visibility = Visibility.Hidden;
                    pwdData.Visibility = Visibility.Visible;
                    btnGenerate.Visibility = Visibility.Visible;
                    btnShowOrOpen.Visibility = Visibility.Visible;
                    btnShowOrOpen.Content = "显示";
                    txtKey.Text = ItemData.Key;
                    pwdData.Password = ItemData.Data;
                    break;
                // 链接
                case 2:
                    ItemData.IsPassword = false;
                    ItemData.IsLink = true;
                    ItemData.IsSplitter = false;
                    grdText.Visibility = Visibility.Visible;
                    grdLine.Visibility = Visibility.Hidden;
                    txtData.Visibility = Visibility.Visible;
                    pwdData.Visibility = Visibility.Hidden;
                    btnGenerate.Visibility = Visibility.Hidden;
                    btnShowOrOpen.Visibility = Visibility.Visible;
                    btnShowOrOpen.Content = "打开";
                    txtKey.Text = ItemData.Key;
                    txtData.Text = ItemData.Data;
                    break;
                // 分割线
                case 3:
                    ItemData.IsPassword = false;
                    ItemData.IsLink = false;
                    ItemData.IsSplitter = true;
                    grdText.Visibility = Visibility.Hidden;
                    grdLine.Visibility = Visibility.Visible;
                    btnGenerate.Visibility = Visibility.Hidden;
                    btnShowOrOpen.Visibility = Visibility.Hidden;
                    break;
                default:
                    throw new Exception("类型索性无效！");
            }
            TypeChanged?.Invoke();
        }

        /// <summary>
        /// 显示或隐藏密码
        /// </summary>
        private void ShowOrHidePassword()
        {
            // 通过按钮标题来判断现在密码是否显示
            switch ((string)btnShowOrOpen.Content)
            {
                case "显示":
                    txtData.Visibility = Visibility.Visible;
                    pwdData.Visibility = Visibility.Collapsed;
                    txtData.Text = ItemData.Data;
                    btnShowOrOpen.Content = "隐藏";
                    break;
                case "隐藏":
                    txtData.Visibility = Visibility.Collapsed;
                    pwdData.Visibility = Visibility.Visible;
                    pwdData.Password = ItemData.Data;
                    btnShowOrOpen.Content = "显示";
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

        private void BtnShowOrOpen_Click(object sender, RoutedEventArgs e)
        {
            // 根据按钮文本路由方法
            switch ((string)btnShowOrOpen.Content)
            {
                case "打开":
                    OpenLink();
                    break;
                case "显示":
                case "隐藏":
                    ShowOrHidePassword();
                    break;
                default:
                    throw new Exception("按钮文本无效！");
            }
        }

        private void TxtKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            ItemData.Key = txtKey.Text;
        }

        private void PwdData_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ItemData.Data = pwdData.Password;
        }

        private void TxtData_TextChanged(object sender, TextChangedEventArgs e)
        {
            ItemData.Data = txtData.Text;
        }
    }
}
