using Passwords.Client.Models.ModelBases;
using System.Collections.Generic;

namespace Passwords.Client.Models
{
    /// <summary>
    /// 自动生成密码的规则
    /// </summary>
    public class PasswordRule : BaseID
    {
        /// <summary>
        /// 此规则的父项目数据
        /// </summary>
        public ItemData ItemData;

        /// <summary>
        /// 密码长度
        /// </summary>
        public int Length;

        /// <summary>
        /// 是否允许出现数字
        /// </summary>
        public bool AllowNumbers;
        /// <summary>
        /// 是否允许出现大写字母
        /// </summary>
        public bool AllowUpperCases;
        /// <summary>
        /// 是否允许出现小写字母
        /// </summary>
        public bool AllowLowerCases;
        /// <summary>
        /// 是否允许出现特殊符号
        /// </summary>
        public bool AllowSpecialCharacters;

        /// <summary>
        /// 首位是否允许出现数字
        /// </summary>
        public bool AllowNumbersFirst;
        /// <summary>
        /// 首位是否允许出现大写字母
        /// </summary>
        public bool AllowUpperCasesFirst;
        /// <summary>
        /// 首位是否允许出现小写字母
        /// </summary>
        public bool AllowLowerCasesFirst;
        /// <summary>
        /// 首位是否允许出现特殊字符
        /// </summary>
        public bool AllowSpecialCharactersFirst;

        /// <summary>
        /// 至少出现多少数字
        /// </summary>
        public int LeastNumberCount;
        /// <summary>
        /// 至少出现多少大写字母
        /// </summary>
        public int LeastUpperCaseCount;
        /// <summary>
        /// 至少出现多少小写字母
        /// </summary>
        public int LeastLowerCaseCount;
        /// <summary>
        /// 至少出现多少特殊字符
        /// </summary>
        public int LeastSpecialCharacterCount;

        /// <summary>
        /// 禁止出现的字符列表
        /// </summary>
        public string BannedCharacters;
        /// <summary>
        /// 禁止出现的字符串列表
        /// </summary>
        public List<string> BannedStrings;
        /// <summary>
        /// 禁止出现连续字符串列表。此列表中的字符串的任意连续子字符串都不允许出现。
        /// </summary>
        public List<string> BannedContinuousCharacters;

        /// <summary>
        /// 是否禁止出现相同的字符
        /// </summary>
        public bool BanRepeatedCharacters;
        /// <summary>
        /// 是否禁止出现连续相同的字符
        /// </summary>
        public bool BanContinuousRepeatedCharacters;
        /// <summary>
        /// 最长允许出现的连续相同字符数量
        /// </summary>
        public int BanContinuousRepeatedCharacterCount;

        /// <summary>
        /// 禁止出现的字符是否忽略大小写
        /// </summary>
        public bool BannedCharactersIgnoreCase;
        /// <summary>
        /// 禁止出现的字符串是否忽略大小写
        /// </summary>
        public bool BannedStringsIgnoreCase;
        /// <summary>
        /// 禁止出现的连续字符串是否忽略大小写
        /// </summary>
        public bool BannedContinuousCharactersIgnoreCase;
    }
}
