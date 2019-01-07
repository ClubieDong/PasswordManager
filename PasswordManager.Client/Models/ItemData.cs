using PasswordManager.Client.Models.ModelBases;

namespace PasswordManager.Client.Models
{
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
        /// <summary>
        /// 是否是密码
        /// </summary>
        public bool IsPassword = false;
        /// <summary>
        /// 是否是链接
        /// </summary>
        public bool IsLink = false;
        /// <summary>
        /// 是否是分割线
        /// </summary>
        public bool IsSplitter = false;
        /// <summary>
        /// 自动生成密码的规则。空值表示该数据不是密码或密码不是自动生成的。
        /// </summary>
        public PasswordRule PasswordRule;
    }
}
