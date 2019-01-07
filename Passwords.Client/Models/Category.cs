using Passwords.Client.Models.ModelBases;
using System.Collections.Generic;

namespace Passwords.Client.Models
{
    /// <summary>
    /// 类别
    /// </summary>
    public class Category : BaseOrder
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName;
        /// <summary>
        /// 包含此类别的类别列表
        /// </summary>
        public List<Category> Categories;
        /// <summary>
        /// 列表框的Expander是否展开
        /// </summary>
        public bool IsExpanded;
        /// <summary>
        /// 该类别下的项目
        /// </summary>
        public List<Item> Items;
    }
}
