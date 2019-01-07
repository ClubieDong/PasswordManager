using Passwords.Client.Models;
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

namespace Passwords.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public Data Data = new Data();
        public Settings Settings;
        // test

        public MainWindow()
        {
            InitializeComponent();

            lck.Data = Data;
            lck.OnUnlock += Load;
            categoryList.OnSelectItem += ShowItem;
            categoryList.OnAddItem += EditItem;
            categoryList.OnDeleteItem += DeleteItem;
            itemShower.OnEdit += EditItem;
            itemEditor.OnCancel += ItemEditorCancelled;
            itemEditor.OnItemSaved += ItemSaved;
        }

        /// <summary>
        /// 解锁时加载
        /// </summary>
        private void Load()
        {
            // 获取设置
            Settings = Data.GetSettings();
            // 获取类别列表及每个类别下的项目
            List<Category> categories = Data.GetCategoriesWithItems();

            categoryList.Data = Data;
            categoryList.Settings = Settings;
            categoryList.Categories = categories;
            categoryList.Load();
            itemShower.Data = Data;
            itemEditor.Data = Data;

            lck.Visibility = Visibility.Collapsed;
            dpnMain.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 尝试卸载项目
        /// </summary>
        /// <returns>是否卸载成功</returns>
        private bool TryUnloadItem()
        {
            if (itemShower.Visibility == Visibility.Visible && !itemShower.CanUnload())
            {
                categoryList.SelectItem(itemShower.Item);
                return false;
            }
            if (itemEditor.Visibility == Visibility.Visible && !itemEditor.CanUnload())
            {
                categoryList.SelectItem(itemEditor.Item);
                return false;
            }
            itemShower.Unload();
            itemEditor.Unload();
            return true;
        }

        /// <summary>
        /// 在ItemShower中显示项目
        /// </summary>
        /// <param name="item">要显示的项目</param>
        private void ShowItem(Item item)
        {
            // 卸载之前显示的项目
            if (!TryUnloadItem())
                return;
            // 加载项目
            itemShower.Item = item;
            categoryList.SelectItem(item);
            itemShower.Load();
            Data.UpdateLastOpenTime(item);
            itemShower.Visibility = Visibility.Visible;
            itemEditor.Visibility = Visibility.Collapsed;
            tbItemShowerHint.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 在ItemEditor中编辑项目
        /// </summary>
        /// <param name="item">要编辑的项目</param>
        private void EditItem(Item item)
        {
            // 卸载之前显示的项目
            if (!TryUnloadItem())
                return;
            // 加载项目
            itemEditor.Item = item;
            categoryList.SelectItem(item);
            itemEditor.Load();
            Data.UpdateLastOpenTime(item);
            itemShower.Visibility = Visibility.Collapsed;
            itemEditor.Visibility = Visibility.Visible;
            tbItemShowerHint.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 删除Item
        /// </summary>
        /// <param name="item">被删除的项目</param>
        private void DeleteItem(Item item)
        {
            // 如果ItemShower显示着Item或ItemEditor显示着Item
            if ((itemShower.Visibility == Visibility.Visible && item == itemShower.Item) || (itemEditor.Visibility == Visibility.Visible && item == itemEditor.Item))
            {
                itemShower.Unload();
                itemEditor.Unload();
                itemShower.Visibility = Visibility.Collapsed;
                itemShower.Visibility = Visibility.Collapsed;
                tbItemShowerHint.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 项目被刷新后触发
        /// </summary>
        private void ItemSaved()
        {
            // 重新加载类别列表，以响应更改
            categoryList.Load();
            ShowItem(itemEditor.Item);
        }

        /// <summary>
        /// 项目编辑器取消编辑时触发
        /// </summary>
        private void ItemEditorCancelled()
        {
            // ID等于0表示是新建项目
            if (itemEditor.Item.ID == 0)
            {
                itemEditor.Unload();
                itemShower.Visibility = Visibility.Collapsed;
                itemShower.Visibility = Visibility.Collapsed;
                tbItemShowerHint.Visibility = Visibility.Visible;
            }
            else
                ShowItem(itemEditor.Item);
        }
    }
}
