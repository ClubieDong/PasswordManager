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
    /// ItemShower.xaml 的交互逻辑
    /// </summary>
    public partial class ItemShower : UserControl
    {
        public Data Data;
        public Item Item;

        /// <summary>
        /// 项目请求编辑时触发
        /// </summary>
        public event Action<Item> OnEdit;
         
        private List<ItemDataShower> ItemDataShowers;
        /// <summary>
        /// 项目是否已加载
        /// </summary>
        private bool IsItemLoaded;

        public ItemShower()
        {
            InitializeComponent();
            btnEdit.Click += (s, e) => OnEdit?.Invoke(Item);
        }

        /// <summary>
        /// 加载并显示项目
        /// </summary>
        public void Load()
        {
            // 如果ItemDataShowers为空，则创建一个对象
            if (ItemDataShowers == null)
                ItemDataShowers = new List<ItemDataShower>();
            // 加载项目属性
            tbTitle.Text = Item.ItemName;
            //创建临时列表
            List<ItemDataShower> localShowers = new List<ItemDataShower>();
            foreach (ItemData i in Item.ItemData)
            {
                // 如果已经Load过了，对象尽量从以前的列表中获取
                ItemDataShower shower = ItemDataShowers.Find(n => n.ItemData == i);
                // 如果是第一次Load，自然找不到以前创建过的对象，则新建一个
                if (shower == null)
                    shower = new ItemDataShower()
                    {
                        ItemData = i,
                    };
                // shower也要（重新）加载一次
                shower.Load();
                // 添加进临时列表
                localShowers.Add(shower);
            }
            // 临时列表转正
            ItemDataShowers = localShowers;
            // 将列表显示到UI
            stpItemData.Children.Clear();
            foreach (ItemDataShower i in ItemDataShowers)
                stpItemData.Children.Add(i);
            IsItemLoaded = true;
        }

        /// <summary>
        /// 卸载项目
        /// </summary>
        public void Unload()
        {
            // 如果未加载，则直接退出
            if (!IsItemLoaded)
                return;
            tbTitle.Text = string.Empty;
            stpItemData.Children.Clear();
            Item = null;
            ItemDataShowers = null;
            IsItemLoaded = false;
        }
    }
}