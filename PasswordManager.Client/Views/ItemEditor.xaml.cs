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
    /// ItemEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ItemEditor : UserControl
    {
        public Data Data;
        public Item Item;
        
        /// <summary>
        /// 项目被保存后触发
        /// </summary>
        public event Action<Item> OnItemSaved;
        /// <summary>
        /// 项目编辑结束，请求显示
        /// </summary>
        public event Action OnCancel;

        private List<ItemDataEditor> ItemDataEditors;
        /// <summary>
        /// 项目是否已加载
        /// </summary>
        private bool IsItemLoaded;
        /// <summary>
        /// 保存加载时传进来的项目
        /// </summary>
        private Item LoadedItem;

        public ItemEditor()
        {
            InitializeComponent();
            // 加载添加数据下拉框
            foreach (string i in ModelProvider.ItemDataNames)
                cbxAdd.Items.Add(i);
        }

        /// <summary>
        /// 加载并显示项目
        /// </summary>
        public void Load()
        {
            LoadedItem = ModelCopier.CopyItem(Item);
            // 如果ItemDataEditors为空，则创建一个对象
            if (ItemDataEditors == null)
                ItemDataEditors = new List<ItemDataEditor>();
            Refresh();
            IsItemLoaded = true;
        }

        public void Refresh()
        {
            // 加载项目属性
            txtItemName.Text = Item.ItemName;
            // 显示自动输入提示文本
            ShowHint();
            //创建临时列表
            List<ItemDataEditor> localEditors = new List<ItemDataEditor>();
            foreach (ItemData i in Item.ItemData)
            {
                // 如果已经Load过了，对象尽量从以前的列表中获取
                ItemDataEditor editor = ItemDataEditors.Find(n => n.ItemData == i);
                // 如果是第一次Load，自然找不到以前创建过的对象，则新建一个
                if (editor == null)
                {
                    editor = new ItemDataEditor()
                    {
                        ItemData = i,
                        Margin = new Thickness(0, 0, 0, 5),
                    };
                    editor.OnListOperation += ItemDataListOperation;
                    editor.TypeChanged += ShowHint;
                }
                // editor也要（重新）加载一次
                editor.Load();
                // 添加进临时列表
                localEditors.Add(editor);
            }
            // 临时列表转正
            ItemDataEditors = localEditors;
            // 将列表显示到UI
            stpItemData.Children.Clear();
            foreach (ItemDataEditor i in ItemDataEditors)
                stpItemData.Children.Add(i);
        }

        /// <summary>
        /// 判断能否卸载项目（如果没有保存，询问用户是否保存），并不会真的卸载
        /// </summary>
        public bool CanUnload()
        {
            // 如果项目和加载时一致，则不用保存，直接退出
            if (ModelComparer.CompareItem(Item, LoadedItem))
                return true;
            MessageBoxResult result = MessageBox.Show("是否保存更改？", "未保存退出确认", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Cancel:
                    return false;
                case MessageBoxResult.Yes:
                    // 如果保存成功，则允许退出，否则返回退出不成功
                    return Save();
                case MessageBoxResult.No:
                    return true;
                default:
                    throw new Exception("对话框返回结果无效！");
            }
        }

        /// <summary>
        /// 卸载项目
        /// </summary>
        public void Unload()
        {
            if (!IsItemLoaded)
                return;
            Item = null;
            ItemDataEditors = null;
            stpItemData.Children.Clear();
            txtItemName.Text = string.Empty;
            tbHint.Text = string.Empty;
            IsItemLoaded = false;
        }

        /// <summary>
        /// 保存项目
        /// </summary>
        public bool Save()
        {
            string errorMessage = ModelValidator.ValidateItem(Item);
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage);
                return false;
            }
            Data.SaveItem(Item);
            ModelCopier.CopyItem(Item, LoadedItem);
            OnItemSaved?.Invoke(Item);
            return true;
        }

        /// <summary>
        /// 显示自动输入提示文本
        /// </summary>
        private void ShowHint()
        {
            // 如果没有数据，则提示添加数据
            if (Item.ItemData.Count == 0)
            {
                tbHint.Text = "请在下方下拉框处选择一项添加。";
                return;
            }
            // 找到第一个分割线前有几项
            int count = 0;
            foreach (ItemData i in Item.ItemData)
            {
                if (i.IsSplitter)
                {
                    if (count == 0)
                        tbHint.Text = "由于分割线出现在首位，自动登录将被禁用。";
                    else
                        tbHint.Text = $"第一个分割线之前的 {count} 项将被应用于自动登录。";
                    return;
                }
                count++;
            }
            // 程序运行到这里表示所有数据中没有分割线
            tbHint.Text = $"全部的 {count} 项都将被应用于自动登录。";
        }

        /// <summary>
        /// 项目数据的列表操作
        /// </summary>
        /// <param name="operationType">操作类型。0: 上移 1: 下移 2: 删除 3: 插入</param>
        /// <param name="itemData">操作相关的项目数据</param>
        private void ItemDataListOperation(int operationType, ItemData itemData)
        {
            int index = Item.ItemData.IndexOf(itemData);
            if (index == -1)
                throw new Exception("找不到项目数据！");

            switch (operationType)
            {
                // 上移
                case 0:
                    Data.MoveUp(Item.ItemData, index);
                    break;
                // 下移
                case 1:
                    Data.MoveDown(Item.ItemData, index);
                    break;
                // 删除
                case 2:
                    // 用户确认操作
                    MessageBoxResult result = MessageBox.Show("此操作无法撤销，确定删除吗？", "删除警告", MessageBoxButton.OKCancel);
                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            Item.ItemData.RemoveAt(index);
                            for (int i = index; i < Item.ItemData.Count; i++)
                                Item.ItemData[i].Order = i;
                            break;
                        case MessageBoxResult.Cancel:
                            return;
                        default:
                            throw new Exception("对话框返回结果无效！");
                    }
                    break;
                // 插入
                case 3:
                    Item.ItemData.Insert(index, ModelProvider.GetItemData("空白项", Item));
                    for (int i = index; i < Item.ItemData.Count; i++)
                        Item.ItemData[i].Order = i;
                    break;
                default:
                    throw new Exception("列表操作类型无效！");
            }

            Refresh();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ModelCopier.CopyItem(LoadedItem, Item);
            OnCancel?.Invoke();
        }

        private void TxtItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Item != null)
                Item.ItemName = txtItemName.Text;
        }
    }
}