using PasswordManager.Client.Models.ModelBases;

namespace PasswordManager.Client.Models
{
    /// <summary>
    /// 项目数据类型
    /// </summary>
    public enum ItemDataType
    {
        /// <summary>
        /// 普通文本
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 密码
        /// </summary>
        Password = 1,
        /// <summary>
        /// 链接
        /// </summary>
        Link = 2,
        /// <summary>
        /// 分割线
        /// </summary>
        Splitter = 3,
    }

    /// <summary>
    /// 数据
    /// </summary>
    public class ItemData : BaseOrder
    {
        /// <summary>
        /// 此项目的父项目
        /// </summary>
        public Item Item;
        public string Key;
        public string Data;
        public ItemDataType Type;
        /// <summary>
        /// 自动生成密码的规则。空值表示该数据不是密码或密码不是自动生成的。
        /// </summary>
        public PasswordRule PasswordRule;
    }
}
