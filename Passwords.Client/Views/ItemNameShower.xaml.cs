using Passwords.Client.Models;
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
    /// ItemNameShower.xaml 的交互逻辑
    /// </summary>
    public partial class ItemNameShower : UserControl
    {
        public Item Item;

        /// <summary>
        /// 请求移动到另一个类别
        /// </summary>
        public event Action<Item, Category> MoveTo;
        /// <summary>
        /// 请求删除此项目
        /// </summary>
        public event Action<Item> OnDelete;

        public ItemNameShower()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 加载并显示项目名称
        /// </summary>
        public void Load()
        {
            tbItemName.Text = Item.ItemName;

            // 创建类别名菜单
            menuItemMoveTo.Items.Clear();
            foreach (Category i in Item.Category.Categories)
            {
                // 移动到自己的类别没有意义，所以不创建自己的类别对应的MenuItem
                if (i == Item.Category)
                    continue;
                MenuItem menuItem = new MenuItem()
                {
                    Header = i.CategoryName,
                };
                menuItem.Click += (s, e) => MoveTo?.Invoke(Item, i);
                menuItemMoveTo.Items.Add(menuItem);
            }
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            OnDelete?.Invoke(Item);
        }
    }
}
