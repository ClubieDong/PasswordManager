using PasswordManager.Client.Models;
using PasswordManager.Client.Services;
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

namespace PasswordManager.Client.Views
{
    /// <summary>
    /// CategoryShower.xaml 的交互逻辑
    /// </summary>
    public partial class CategoryShower : UserControl
    {
        public Data Data;
        public Settings Settings;
        public Category Category;

        private List<ItemNameShower> ItemNameShowers;
        // 需要显示的时候才进行实例化，以节省内存
        private TextBox txtRename;
        private TextBlock tbEmpty;

        /// <summary>
        /// 是否是处于重命名类别名称的模式
        /// </summary>
        private bool InRenameMode = false;

        /// <summary>
        /// 请求显示项目
        /// </summary>
        public event Action<Item> OnSelectItem;
        /// <summary>
        /// 请求添加项目
        /// </summary>
        public event Action<Item> OnAddItem;
        /// <summary>
        /// 项目被删除后触发
        /// </summary>
        public event Action<Item> OnDeleteItem;
        /// <summary>
        /// 请求删除此类别
        /// </summary>
        public event Action<Category> OnDeleteCategory;
        /// <summary>
        /// 项目请求从一个类别移动到另一个类别
        /// </summary>
        public event Action<Item, Category> OnMoveItem;
        /// <summary>
        /// 类别重命名后触发
        /// </summary>
        public event Action<Category> OnRenameCategory;

        public CategoryShower()
        {
            InitializeComponent();

            // 创建默认添加项
            menuItemAdd.Items.Clear();
            foreach (string i in ModelProvider.ItemNames)
            {
                MenuItem menuItem = new MenuItem()
                {
                    Header = i,
                };
                menuItem.Click += (s, e2) => AddItem((s as MenuItem).Header as string);
                menuItemAdd.Items.Add(menuItem);
            }
        }

        /// <summary>
        /// 加载并显示类别
        /// </summary>
        public void Load()
        {
            // 如果ItemNameShowers为空，则创建一个对象
            if (ItemNameShowers == null)
                ItemNameShowers = new List<ItemNameShower>();

            // 加载类别属性
            exp.IsExpanded = Category.IsExpanded;
            tbName.Text = Category.CategoryName;

            // 创建临时列表
            List<ItemNameShower> localShowers = new List<ItemNameShower>();
            foreach (Item i in Category.Items)
            {
                // 如果已经Load过了，对象尽量从以前的列表中获取
                ItemNameShower shower = ItemNameShowers.Find(n => n.Item == i);
                // 如果是第一次Load，自然找不到以前创建过的对象，则新建一个
                if (shower == null)
                {
                    shower = new ItemNameShower()
                    {
                        Item = i,
                    };
                    shower.MoveTo += OnMoveItem;
                    shower.OnDelete += DeleteItem;
                }
                // shower也要（重新）加载一次
                shower.Load();
                // 添加进临时列表
                localShowers.Add(shower);
            }
            // 临时列表转正
            ItemNameShowers = localShowers;
            // 将列表显示到UI
            ShowItemName(ItemNameShowers);
        }

        /// <summary>
        /// 执行搜索
        /// </summary>
        /// <param name="text">搜索文本</param>
        public void Search(string text)
        {
            // 筛选出满足搜索条件的项目
            // 如果类别名称包含搜索文本，则全部显示
            // 遍历该类别下的每个项目，如果项目名称包含搜索文本，则显示该项目
            // 如果设置了搜索包含数据，判断该项目的数据是否包含搜索文本，如果包含，则显示该项目
            List<ItemNameShower> searchResult = (Data.IsTextIncluded(Category.CategoryName, text, Settings.SearchIgnoreCase)) ? ItemNameShowers : ItemNameShowers.Where(n => (Data.IsTextIncluded(n.Item.ItemName, text, Settings.SearchIgnoreCase)) || (Settings.SearchIncludeData && Data.ContainText(n.Item, text, Settings.SearchIgnoreCase))).ToList();
            // 显示项目
            ShowItemName(searchResult);
        }

        /// <summary>
        /// 显示项目
        /// </summary>
        /// <param name="itemNameShowers">项目控件列表</param>
        private void ShowItemName(List<ItemNameShower> itemNameShowers)
        {
            // 清空原有项目
            lstItemName.Items.Clear();
            // 判断是否有项目
            if (itemNameShowers.Count == 0)
            {
                // 如果没有项目，则显示空
                // 如果“空”文本框没有被创建过，则创建一个
                if (tbEmpty == null)
                {
                    tbEmpty = new TextBlock()
                    {
                        Text = "空",
                        Margin = new Thickness(30, 0, 0, 0),
                    };
                    grdContent.Children.Add(tbEmpty);
                }
                tbEmpty.Visibility = Visibility.Visible;
                lstItemName.Visibility = Visibility.Collapsed;
            }
            else
            {
                // 如果有项目，则都添加到List
                foreach (ItemNameShower i in itemNameShowers)
                    lstItemName.Items.Add(i);
                lstItemName.Visibility = Visibility.Visible;
                if (tbEmpty != null)
                    tbEmpty.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 切换到重命名模式
        /// </summary>
        public void SwitchToRenameMode()
        {
            // 如果已经在重命名模式，则直接退出
            if (InRenameMode)
                return;
            // 如果是第一次进入重命名模式，创建实例
            if (txtRename == null)
            {
                txtRename = new TextBox();
                txtRename.LostFocus += TxtRename_LostFocus;
                txtRename.KeyDown += TxtRename_KeyDown;
                grdHeader.Children.Add(txtRename);
            }
            txtRename.Text = Category.CategoryName;
            txtRename.Visibility = Visibility.Visible;
            tbName.Visibility = Visibility.Collapsed;
            txtRename.SelectAll();
            txtRename.Focus();
            InRenameMode = true;
        }

        /// <summary>
        /// 切换到正常模式
        /// </summary>
        public void SwitchToNormalMode()
        {
            // 如果已经在正常模式，则直接退出
            if (!InRenameMode)
                return;

            // 以防万一进行空值判断
            if (txtRename != null)
            {
                txtRename.Text = string.Empty;
                txtRename.Visibility = Visibility.Collapsed;
            }
            tbName.Visibility = Visibility.Visible;
            InRenameMode = false;
        }

        /// <summary>
        /// 重命名
        /// </summary>
        private void Rename()
        {
            // 去除文本两端空格
            string newName = txtRename.Text.Trim();
            // 如果新名字为空，报错
            if (newName == string.Empty)
            {
                MessageBox.Show("类别名不能为空！");
                return;
            }
            // 更改数据
            Data.RenameCategory(Category, newName);
            // 切换回非重命名模式
            SwitchToNormalMode();
            // 向上级传递
            OnRenameCategory?.Invoke(Category);
            // 上级CategoryList会重新加载，所以此处不再Load
        }

        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="name">项目的默认名字</param>
        private void AddItem(string name)
        {
            // 获取默认项目
            Item newItem = ModelProvider.GetItem(name, Category);
            OnAddItem?.Invoke(newItem);
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="item">要删除的项目</param>
        private void DeleteItem(Item item)
        {
            // 用户确认操作
            MessageBoxResult result = MessageBox.Show("此操作无法撤销，确定删除吗？", "删除警告", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                return;
            // 向上级传递
            OnDeleteItem?.Invoke(item);
            // 删除数据
            Data.RemoveItem(item);
            // 重新加载
            Load();
        }

        private void LstItemName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果没有选择内容，应该不是用户操作的，就直接退出
            if (lstItemName.SelectedIndex==-1)
                return;
            Item item = (lstItemName.SelectedItem as ItemNameShower).Item;
            OnSelectItem?.Invoke(item);
            
            // 每次点击之后都不选中，避免问题
            lstItemName.SelectedIndex = -1;
        }

        private void MenuItemRename_Click(object sender, RoutedEventArgs e)
        {
            SwitchToRenameMode();
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            OnDeleteCategory?.Invoke(Category);
        }

        private void TxtRename_KeyDown(object sender, KeyEventArgs e)
        {
            if (!InRenameMode)
                return;
            // 按下回车即重命名
            if (e.Key == Key.Enter)
                Rename();
            // 按下Esc即取消重命名
            else if (e.Key == Key.Escape)
                SwitchToNormalMode();
        }

        private void TxtRename_LostFocus(object sender, RoutedEventArgs e)
        {
            // 离开焦点即取消重命名
            SwitchToNormalMode();
        }

        private void ExpExpanded(object sender, RoutedEventArgs e)
        {
            Category.IsExpanded = true;
        }

        private void ExpCollapsed(object sender, RoutedEventArgs e)
        {
            Category.IsExpanded = false;
        }
    }
}
