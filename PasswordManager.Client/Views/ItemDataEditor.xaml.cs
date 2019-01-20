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
            cbxType.SelectionChanged += (s, e) => SwitchType((ItemDataType)cbxType.SelectedIndex);
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
            SetData();
            btnMoveUp.IsEnabled = ItemData.Order > 0;
            btnMoveDown.IsEnabled = ItemData.Order < ItemData.Item.ItemData.Count - 1;
            btnDelete.IsEnabled = ItemData.Item.ItemData.Count > 1;
        }

        /// <summary>
        /// 将文本框的数据保存到项目数据实例
        /// </summary>
        public void GetData()
        {
            switch (ItemData.Type)
            {
                case ItemDataType.Normal:
                case ItemDataType.Link:
                    ItemData.Key = txtKey.Text;
                    ItemData.Data = txtData.Text;
                    break;
                case ItemDataType.Password:
                    ItemData.Key = txtKey.Text;
                    ItemData.Data = pwdData.Password;
                    break;
                case ItemDataType.Splitter:
                    break;
                default:
                    throw new Exception("项目数据类型无效！");
            }
        }

        /// <summary>
        /// 将项目数据显示到文本框
        /// </summary>
        public void SetData()
        {
            switch (ItemData.Type)
            {
                case ItemDataType.Normal:
                    txtKey.Text = ItemData.Key;
                    txtData.Text = ItemData.Data;
                    grdText.Visibility = Visibility.Visible;
                    grdLine.Visibility = Visibility.Hidden;
                    txtData.Visibility = Visibility.Visible;
                    pwdData.Visibility = Visibility.Hidden;
                    btnGenerate.Visibility = Visibility.Hidden;
                    btnShowOrOpen.Visibility = Visibility.Hidden;
                    break;
                case ItemDataType.Password:
                    txtKey.Text = ItemData.Key;
                    pwdData.Password = ItemData.Data;
                    btnShowOrOpen.Content = "显示";
                    grdText.Visibility = Visibility.Visible;
                    grdLine.Visibility = Visibility.Hidden;
                    txtData.Visibility = Visibility.Hidden;
                    pwdData.Visibility = Visibility.Visible;
                    btnGenerate.Visibility = Visibility.Visible;
                    btnShowOrOpen.Visibility = Visibility.Visible;
                    break;
                case ItemDataType.Link:
                    txtKey.Text = ItemData.Key;
                    txtData.Text = ItemData.Data;
                    btnShowOrOpen.Content = "打开";
                    grdText.Visibility = Visibility.Visible;
                    grdLine.Visibility = Visibility.Hidden;
                    txtData.Visibility = Visibility.Visible;
                    pwdData.Visibility = Visibility.Hidden;
                    btnGenerate.Visibility = Visibility.Hidden;
                    btnShowOrOpen.Visibility = Visibility.Visible;
                    break;
                case ItemDataType.Splitter:
                    grdText.Visibility = Visibility.Hidden;
                    grdLine.Visibility = Visibility.Visible;
                    btnGenerate.Visibility = Visibility.Hidden;
                    btnShowOrOpen.Visibility = Visibility.Hidden;
                    break;
                default:
                    throw new Exception("项目数据类型无效！");
            }
            cbxType.SelectedIndex = (int)ItemData.Type;
        }

        /// <summary>
        /// 更改项目数据的类型
        /// </summary>
        /// <param name="comboBoxIndex"></param>
        private void SwitchType(ItemDataType type)
        {
            // 如果是同一种类型就直接退出，一般遇到这种情况就是由SetData()触发的。
            if (type == ItemData.Type)
                return;
            GetData();
            ItemData.Type = type;
            SetData();
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
    }
}
