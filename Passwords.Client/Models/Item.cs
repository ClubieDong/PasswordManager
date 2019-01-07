using Passwords.Client.Models.ModelBases;
using System;
using System.Collections.Generic;

namespace Passwords.Client.Models
{
    /// <summary>
    /// 项目
    /// </summary>
    public class Item : BaseOrder
    {
        /// <summary>
        /// 此项目的父类别
        /// </summary>
        public Category Category;
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName;
        /// <summary>
        /// 该项目下的数据
        /// </summary>
        public List<ItemData> ItemData;
        /// <summary>
        /// 上次浏览或编辑此项目的时间
        /// </summary>
        public DateTime LastOpenTime;
        /// <summary>
        /// 创建项目的时间
        /// </summary>
        public DateTime CreateTime;
    }
}
