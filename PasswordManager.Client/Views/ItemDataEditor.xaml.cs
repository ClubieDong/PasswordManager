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
using PasswordManager.Client.Services;

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
        /// </summary>
        public event Action<ListOperationType, ItemData> OnListOperation;
        /// <summary>
        /// 数据类型被改变
        /// </summary>
        public event Action TypeChanged;

        // 要显示的时候才实例化，以节省内存空间
        private Generator Generator;

        public ItemDataEditor()
        {
            InitializeComponent();
            cbxType.SelectionChanged += (s, e) => SwitchType((ItemDataType)cbxType.SelectedIndex);
            btnMoveUp.Click += (s, e) => OnListOperation?.Invoke(ListOperationType.MoveUp, ItemData);
            btnMoveDown.Click += (s, e) => OnListOperation?.Invoke(ListOperationType.MoveDown, ItemData);
            btnDelete.Click += (s, e) => OnListOperation?.Invoke(ListOperationType.Delete, ItemData);
            btnInsert.Click += (s, e) => OnListOperation?.Invoke(ListOperationType.Insert, ItemData);
            txtData.TextChanged += (s, e) => pwdData.Password = txtData.Text;
            pwdData.PasswordChanged += (s, e) => txtData.Text = pwdData.Password;
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
            ItemData.Key = txtKey.Text;
            ItemData.Data = txtData.Text;
            Generator?.GetData();
        }

        /// <summary>
        /// 将项目数据显示到文本框
        /// </summary>
        public void SetData()
        {
            txtKey.Text = ItemData.Key;
            txtData.Text = ItemData.Data;
            switch (ItemData.Type)
            {
                case ItemDataType.Normal:
                    grdText.Visibility = Visibility.Visible;
                    grdLine.Visibility = Visibility.Hidden;
                    txtData.Visibility = Visibility.Visible;
                    pwdData.Visibility = Visibility.Hidden;
                    btnGenerate.Visibility = Visibility.Hidden;
                    btnShowOrOpen.Visibility = Visibility.Hidden;
                    break;
                case ItemDataType.Password:
                    btnShowOrOpen.Content = "显示";
                    grdText.Visibility = Visibility.Visible;
                    grdLine.Visibility = Visibility.Hidden;
                    txtData.Visibility = Visibility.Hidden;
                    pwdData.Visibility = Visibility.Visible;
                    btnGenerate.Visibility = Visibility.Visible;
                    btnShowOrOpen.Visibility = Visibility.Visible;
                    break;
                case ItemDataType.Link:
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

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            // 根据按钮文本路由方法
            switch ((string)btnGenerate.Content)
            {
                case "生成":
                    if (ItemData.PasswordRule == null)
                        ItemData.PasswordRule = new PasswordRule();
                    if (Generator == null)
                    {
                        Generator = new Generator();
                        Generator.OnGenerated += (password) => txtData.Text = password;
                        Generator.PasswordRule = ItemData.PasswordRule;
                        Generator.Load();
                        Grid.SetRow(Generator, 1);
                        Grid.SetColumn(Generator, 2);
                        grdMain.Children.Add(Generator);
                    }
                    else
                        Generator.Visibility = Visibility.Visible;
                    btnGenerate.Content = "收起";
                    break;
                case "收起":
                    Generator.Visibility = Visibility.Collapsed;
                    btnGenerate.Content = "生成";
                    break;
                default:
                    throw new Exception("按钮文本无效！");
            }
        }

        private void BtnShowOrOpen_Click(object sender, RoutedEventArgs e)
        {
            // 根据按钮文本路由方法
            switch ((string)btnShowOrOpen.Content)
            {
                case "打开":
                    try
                    {
                        Process.Start(ItemData.Data);
                    }
                    catch (Exception)
                    {
                        // 如果打开失败，报错
                        MessageBox.Show("无法打开链接！");
                    }
                    break;
                case "显示":
                    txtData.Visibility = Visibility.Visible;
                    pwdData.Visibility = Visibility.Collapsed;
                    btnShowOrOpen.Content = "隐藏";
                    break;
                case "隐藏":
                    txtData.Visibility = Visibility.Collapsed;
                    pwdData.Visibility = Visibility.Visible;
                    btnShowOrOpen.Content = "显示";
                    break;
                default:
                    throw new Exception("按钮文本无效！");
            }
        }
    }
}
