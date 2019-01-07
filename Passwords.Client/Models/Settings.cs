using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwords.Client.Models
{
    /// <summary>
    /// 设置
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// 当搜索框文字变化是是否执行搜索
        /// </summary>
        public bool SearchWhenTextChange;
        /// <summary>
        /// 搜索时是否包含数据内容
        /// </summary>
        public bool SearchIncludeData;
        /// <summary>
        /// 搜索时是否忽略大小写
        /// </summary>
        public bool SearchIgnoreCase;
    }
}
