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

namespace PasswordManager.Client
{
    /// <summary>
    /// 主界面状态
    /// </summary>
    enum MainWindowState
    {
        /// <summary>
        /// 锁定状态
        /// </summary>
        Locked,
        /// <summary>
        /// 空白状态
        /// </summary>
        Blank,
        /// <summary>
        /// 显示项目状态
        /// </summary>
        Showing,
        /// <summary>
        /// 编辑项目状态
        /// </summary>
        Editing,
        /// <summary>
        /// 新建项目状态
        /// </summary>
        Adding,
    }

    /// <summary>
    /// 主界面操作
    /// </summary>
    enum MainWindowOperation
    {
        /// <summary>
        /// 解锁
        /// </summary>
        Unlock,
        /// <summary>
        /// 选择项目
        /// </summary>
        SelectItem,
        /// <summary>
        /// 新建项目
        /// </summary>
        AddItem,
        /// <summary>
        /// 删除项目
        /// </summary>
        DeleteItem,
        /// <summary>
        /// 请求编辑
        /// </summary>
        Edit,
        /// <summary>
        /// 取消编辑
        /// </summary>
        Cancel,
        /// <summary>
        /// 项目以保存
        /// </summary>
        ItemSaved,
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public Data Data = new Data();
        public Settings Settings;

        /// <summary>
        /// 状态
        /// </summary>
        private MainWindowState State = MainWindowState.Locked;
        /// <summary>
        /// 上次显示的项目或正在显示的项目
        /// </summary>
        private Item ItemShown;
        /// <summary>
        /// 上次编辑的项目或正在编辑的项目
        /// </summary>
        private Item ItemEdited;

        public MainWindow()
        {
            InitializeComponent();

            lck.Data = Data;
            // 全都交由状态转移函数完成
            lck.OnUnlock += () => SwitchState(MainWindowOperation.Unlock, null);
            categoryList.OnSelectItem += (n) => SwitchState(MainWindowOperation.SelectItem, n);
            categoryList.OnAddItem += (n) => SwitchState(MainWindowOperation.AddItem, n);
            categoryList.OnDeleteItem += (n) => SwitchState(MainWindowOperation.DeleteItem, n);
            itemShower.OnEdit += (n) => SwitchState(MainWindowOperation.Edit, n);
            itemEditor.OnCancel += () => SwitchState(MainWindowOperation.Cancel, null);
            itemEditor.OnItemSaved += (n) => SwitchState(MainWindowOperation.ItemSaved, n);
        }

        /// <summary>
        /// 转移状态
        /// </summary>
        /// <param name="operation">进行的操作</param>
        private void SwitchState(MainWindowOperation operation, Item item)
        {
            // 详细的状态转移表见Excel文档
            switch (operation)
            {
                case MainWindowOperation.Unlock:
                    switch (State)
                    {
                        case MainWindowState.Locked:
                            Load();
                            State = MainWindowState.Blank;
                            break;
                        case MainWindowState.Blank:
                        case MainWindowState.Showing:
                        case MainWindowState.Editing:
                        case MainWindowState.Adding:
                            throw new Exception("非法状态转移！");
                        default:
                            throw new Exception("无效状态！");
                    }
                    break;

                case MainWindowOperation.SelectItem:
                    switch (State)
                    {
                        case MainWindowState.Locked:
                            throw new Exception("非法状态转移！");
                        case MainWindowState.Blank:
                            ShowItem(item);
                            State = MainWindowState.Showing;
                            break;
                        case MainWindowState.Showing:
                            if (item != ItemShown)
                                ShowItem(item);
                            break;
                        case MainWindowState.Editing:
                        case MainWindowState.Adding:
                            if (ItemEdited != item && itemEditor.CanUnload())
                            {
                                Unload();
                                ShowItem(item);
                                State = MainWindowState.Showing;
                            }
                            break;
                        default:
                            throw new Exception("无效状态！");
                    }
                    break;
                case MainWindowOperation.AddItem:
                    switch (State)
                    {
                        case MainWindowState.Locked:
                            throw new Exception("非法状态转移！");
                        case MainWindowState.Blank:
                        case MainWindowState.Showing:
                            Unload();
                            EditItem(item);
                            State = MainWindowState.Adding;
                            break;
                        case MainWindowState.Editing:
                        case MainWindowState.Adding:
                            if (itemEditor.CanUnload())
                            {
                                Unload();
                                EditItem(item);
                                State = MainWindowState.Adding;
                            }
                            break;
                        default:
                            throw new Exception("无效状态！");
                    }
                    break;
                case MainWindowOperation.DeleteItem:
                    switch (State)
                    {
                        case MainWindowState.Locked:
                            throw new Exception("非法状态转移！");
                        case MainWindowState.Blank:
                        case MainWindowState.Adding:
                            categoryList.Refresh();
                            break;
                        case MainWindowState.Showing:
                            if (item == ItemShown)
                            {
                                Unload();
                                State = MainWindowState.Blank;
                            }
                            categoryList.Refresh();
                            break;
                        case MainWindowState.Editing:
                            if (item == ItemEdited)
                            {
                                Unload();
                                State = MainWindowState.Blank;
                            }
                            categoryList.Refresh();
                            break;
                        default:
                            throw new Exception("无效状态！");
                    }
                    break;
                case MainWindowOperation.Edit:
                    switch (State)
                    {
                        case MainWindowState.Locked:
                        case MainWindowState.Blank:
                        case MainWindowState.Editing:
                        case MainWindowState.Adding:
                            throw new Exception("非法状态转移！");
                        case MainWindowState.Showing:
                            Unload();
                            EditItem(item);
                            State = MainWindowState.Editing;
                            break;
                        default:
                            throw new Exception("无效状态！");
                    }
                    break;
                case MainWindowOperation.Cancel:
                    switch (State)
                    {
                        case MainWindowState.Locked:
                        case MainWindowState.Blank:
                        case MainWindowState.Showing:
                            throw new Exception("非法状态转移！");
                        case MainWindowState.Editing:
                        case MainWindowState.Adding:
                            Unload();
                            ShowItem(ItemShown);
                            State = MainWindowState.Showing;
                            break;
                        default:
                            throw new Exception("无效状态！");
                    }
                    break;
                case MainWindowOperation.ItemSaved:
                    switch (State)
                    {
                        case MainWindowState.Locked:
                        case MainWindowState.Blank:
                        case MainWindowState.Showing:
                            throw new Exception("非法状态转移！");
                        case MainWindowState.Editing:
                        case MainWindowState.Adding:
                            Unload();
                            categoryList.Refresh();
                            ShowItem(item);
                            State = MainWindowState.Showing;
                            break;
                        default:
                            throw new Exception("无效状态！");
                    }
                    break;
                default:
                    throw new Exception("无效操作类型！");
            }
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

            // 传入并加载数据
            categoryList.Data = Data;
            categoryList.Settings = Settings;
            categoryList.Categories = categories;
            categoryList.Load();
            itemShower.Data = Data;
            itemEditor.Data = Data;

            // 设置可见性
            lck.Visibility = Visibility.Collapsed;
            dpnMain.Visibility = Visibility.Visible;
            itemShower.Visibility = Visibility.Collapsed;
            itemEditor.Visibility = Visibility.Collapsed;
            tbItemShowerHint.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 卸载ItemShower和ItemEditor，转移到Blank状态
        /// </summary>
        private void Unload()
        {
            itemShower.Unload();
            itemEditor.Unload();

            // 设置可见性
            itemShower.Visibility = Visibility.Collapsed;
            itemEditor.Visibility = Visibility.Collapsed;
            tbItemShowerHint.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 在ItemShower中显示项目
        /// </summary>
        /// <param name="item">要显示的项目</param>
        private void ShowItem(Item item)
        {
            itemShower.Item = item;
            itemShower.Load();
            ItemShown = item;
            Data.UpdateLastOpenTime(item);

            // 设置可见性
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
            itemEditor.Item = item;
            itemEditor.Load();
            ItemEdited = item;
            Data.UpdateLastOpenTime(item);

            // 设置可见性
            itemShower.Visibility = Visibility.Collapsed;
            itemEditor.Visibility = Visibility.Visible;
            tbItemShowerHint.Visibility = Visibility.Collapsed;
        }
    }
}
