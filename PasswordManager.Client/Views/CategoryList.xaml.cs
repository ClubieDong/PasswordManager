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
    /// CategoryList.xaml 的交互逻辑
    /// </summary>
    public partial class CategoryList : UserControl
    {
        public Data Data;
        public Settings Settings;
        public List<Category> Categories;

        private List<CategoryShower> CategoryShowers;
        // 要显示的时候才实例化，以节省内存空间
        private TextBlock tbEmpty;

        /// <summary>
        /// 项目被选中
        /// </summary>
        public event Action<Item> OnSelectItem;
        /// <summary>
        /// 项目被添加
        /// </summary>
        public event Action<Item> OnAddItem;
        /// <summary>
        /// 项目被删除
        /// </summary>
        public event Action<Item> OnDeleteItem;

        public CategoryList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 加载并显示类别列表
        /// </summary>
        public void Load()
        {
            // 如果CategoryShowers为空，则创建一个对象
            if (CategoryShowers == null)
                CategoryShowers = new List<CategoryShower>();
            Refresh();
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public void Refresh()
        {
            // 创建临时列表
            List<CategoryShower> localShowers = new List<CategoryShower>();
            foreach (Category i in Categories)
            {
                // 如果已经加载过了，对象尽量从以前的列表中获取
                CategoryShower shower = CategoryShowers.Find(n => n.Category == i);
                // 如果是第一次加载，自然找不到以前创建过的对象，则新建一个
                if (shower == null)
                {
                    shower = new CategoryShower()
                    {
                        Data = Data,
                        Settings = Settings,
                        Category = i,
                        Margin = new Thickness(0, 0, 0, 5),
                    };
                    shower.OnSelectItem += OnSelectItem;
                    shower.OnAddItem += OnAddItem;
                    shower.OnDeleteItem += OnDeleteItem;
                    shower.OnDeleteCategory += DeleteCategory;
                    shower.OnMoveItem += MoveItem;
                    shower.OnRenameCategory += n => Refresh();
                }
                // shower也要重新加载一次
                shower.Load();
                // 添加进临时列表
                localShowers.Add(shower);
            }
            // 临时列表转正
            CategoryShowers = localShowers;

            stpCategory.Children.Clear();
            // 如果没有类别，提示创建一个类别
            if (Categories.Count == 0)
            {
                // 如果提示文本框没有被创建过，则创建一个
                if (tbEmpty == null)
                {
                    tbEmpty = new TextBlock()
                    {
                        Text = "空",
                        Margin = new Thickness(0, 20, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    grdCategory.Children.Add(tbEmpty);
                }
                tbEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                if (tbEmpty != null)
                    tbEmpty.Visibility = Visibility.Collapsed;
                // 根据搜索内容进行显示
                Search();
                // 将列表显示到UI
                foreach (CategoryShower i in CategoryShowers)
                    stpCategory.Children.Add(i);
            }
        }

        /// <summary>
        /// 卸载类别列表
        /// </summary>
        public void Unload()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行搜索
        /// </summary>
        private void Search()
        {
            // 去除搜索文本两端的空格
            string searchText = txtSearch.Text.Trim();
            // 对每一个类别的项目进行筛选
            foreach (CategoryShower i in CategoryShowers)
                i.Search(searchText);
        }

        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="category">要删除的Category</param>
        private void DeleteCategory(Category category)
        {
            // 必须保证类别下无项目
            if (category.Items.Count > 0)
            {
                MessageBox.Show("请先移除该类别下的所有项目！");
                return;
            }
            // 用户确认操作
            MessageBoxResult result = MessageBox.Show("此操作无法撤销，确定删除吗？", "删除警告", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                return;
            // 删除数据
            Data.RemoveCategory(category);
            // 重新加载
            Refresh();
        }

        /// <summary>
        /// 把项目从一个类别移动到另一个类别
        /// </summary>
        /// <param name="itemToMove">要移动的项目</param>
        /// <param name="categoryFrom">移出的类别</param>
        /// <param name="categoryTo">移入的类别</param>
        private void MoveItem(Item itemToMove, Category categoryTo)
        {
            Data.MoveItem(itemToMove, categoryTo);
            Refresh();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            // 如果按下的不是回车键，直接退出
            if (e.Key != Key.Enter)
                return;
            Search();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 如果设置了文本变动就搜索
            if (Settings.SearchWhenTextChange)
                Search();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // 新建类别
            Category category = ModelProvider.GetCategory("展开的类别", Categories);
            // 写入到数据库
            Data.AddCategory(category, Categories);
            // 重新加载
            Refresh();
        }
    }
}