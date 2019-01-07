using PasswordManager.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Client.Services
{
    /// <summary>
    /// 模型验证器和去冗余类。
    /// </summary>
    public static class ModelValidator
    {
        /// <summary>
        /// 验证项目是否合法，并去除冗余
        /// </summary>
        /// <returns>返回第一个不合法处的错误信息</returns>
        public static string ValidateItem(Item item)
        {
            // 为空值创建实例
            if (item.ItemName == null)
                item.ItemName = string.Empty;
            // 去除首尾空格
            item.ItemName = item.ItemName.Trim();
            // 如果数据不合法，返回错误信息
            if (item.ItemName == string.Empty)
            {
                return "项目名称不能为空";
            }
            for (int i = 0; i < item.ItemData.Count; i++)
            {
                string errorMessage = ValidateItemData(item.ItemData[i]);
                if (errorMessage != null)
                    return $"第{i + 1}条数据" + errorMessage;
            }
            return null;
        }

        /// <summary>
        /// 验证项目数据是否合法，并去除冗余
        /// </summary>
        /// <returns>返回第一个不合法处的错误信息</returns>
        public static string ValidateItemData(ItemData itemData)
        {
            // 如果是分割线
            if (!itemData.IsPassword && !itemData.IsLink && itemData.IsSplitter)
            {
                // 去除冗余
                itemData.Key = string.Empty;
                itemData.Data = string.Empty;
                itemData.PasswordRule = null;
                return null;
            }
            // 如果是普通文本、密码或链接
            if ((!itemData.IsPassword && !itemData.IsLink && !itemData.IsSplitter) || (itemData.IsPassword && !itemData.IsLink && !itemData.IsSplitter) || (!itemData.IsPassword && itemData.IsLink && !itemData.IsSplitter))
            {
                // 为空值创建实例
                if (itemData.Key == null)
                    itemData.Key = string.Empty;
                if (itemData.Data == null)
                    itemData.Data = string.Empty;
                // 去除首尾空格
                itemData.Key = itemData.Key.Trim();
                // 如果数据不合法，返回错误信息
                if (itemData.Key == string.Empty)
                    return "数据名称不能为空！";
                if (itemData.PasswordRule != null)
                {
                    string errorMessage = ValidatePasswordRule(itemData.PasswordRule);
                    if (errorMessage != null)
                        return "密码生成规则：" + errorMessage;
                }
                return null;
            }
            // 正常执行的代码不应该执行到这里，所以此处直接报错而不是返回错误信息
            throw new Exception("项目类别无效！");
        }

        /// <summary>
        /// 验证密码生成规则是否合法，并去除冗余
        /// </summary>
        /// <returns>返回第一个不合法处的错误信息</returns>
        public static string ValidatePasswordRule(PasswordRule passwordRule)
        {
            // 如果数据不合法，返回错误信息
            if (passwordRule.ID <= 0)
                return "密码长度至少为1";
            if (passwordRule.ID >= 64000)
                return "密码长度至多为63999";
            if (!passwordRule.AllowNumbers && !passwordRule.AllowNumbersFirst && passwordRule.LeastNumberCount > 0)
                return "如果不允许出现数字，则至少出现的数字个数必须为0";
            if (!passwordRule.AllowUpperCases && !passwordRule.AllowUpperCasesFirst && passwordRule.LeastUpperCaseCount > 0)
                return "如果不允许出现大写字母，则至少出现的大写字母个数必须为0";
            if (!passwordRule.AllowLowerCases && !passwordRule.AllowLowerCasesFirst && passwordRule.LeastLowerCaseCount > 0)
                return "如果不允许出现小写字母，则至少出现的小写字母个数必须为0";
            if (!passwordRule.AllowSpecialCharacters && !passwordRule.AllowSpecialCharactersFirst && passwordRule.LeastSpecialCharacterCount > 0)
                return "如果不允许出现特殊字符，则至少出现的特殊字符个数必须为0";
            if (!passwordRule.AllowNumbers && passwordRule.AllowNumbersFirst && passwordRule.LeastNumberCount > 1)
                return "如果除首位不允许出现数字，则至少出现的数字个数至多为1";
            if (!passwordRule.AllowUpperCases && passwordRule.AllowUpperCasesFirst && passwordRule.LeastUpperCaseCount > 1)
                return "如果除首位不允许出现大写字母，则至少出现的大写字母个数至多为1";
            if (!passwordRule.AllowLowerCases && passwordRule.AllowLowerCasesFirst && passwordRule.LeastLowerCaseCount > 1)
                return "如果除首位不允许出现小写字母，则至少出现的小写字母个数至多为1";
            if (!passwordRule.AllowSpecialCharacters && passwordRule.AllowSpecialCharactersFirst && passwordRule.LeastSpecialCharacterCount > 1)
                return "如果除首位不允许出现特殊字符，则至少出现的特殊字符个数至多为1";
            if (passwordRule.LeastNumberCount < 0)
                return "至少出现的数字个数至少为0";
            if (passwordRule.LeastUpperCaseCount < 0)
                return "至少出现的大写字母个数至少为0";
            if (passwordRule.LeastLowerCaseCount < 0)
                return "至少出现的小写字母个数至少为0";
            if (passwordRule.LeastSpecialCharacterCount < 0)
                return "至少出现的特殊字符个数至少为0";
            int leastSum = passwordRule.LeastNumberCount + passwordRule.LeastUpperCaseCount + passwordRule.LeastLowerCaseCount + passwordRule.LeastSpecialCharacterCount;
            if (leastSum > passwordRule.Length)
                return $"至少出现的数字、大写字母、小写字母、特殊符号个数总和（{leastSum}）至多与密码长度（{passwordRule.Length}）相同";
            if (passwordRule.BanContinuousRepeatedCharacterCount < 2)
                return "不允许出现的连续相同字符数量至少为2";

            // 为空对象创建实例
            if (passwordRule.BannedCharacters == null)
                passwordRule.BannedCharacters = string.Empty;
            if (passwordRule.BannedStrings == null)
                passwordRule.BannedStrings = new List<string>();
            if (passwordRule.BannedContinuousCharacters == null)
                passwordRule.BannedContinuousCharacters = new List<string>();

            // 去除冗余
            passwordRule.BannedCharacters = new string(passwordRule.BannedCharacters.Distinct().ToArray());
            passwordRule.BannedStrings = passwordRule.BannedStrings.Where(n => string.IsNullOrEmpty(n)).Distinct().ToList();
            passwordRule.BannedContinuousCharacters = passwordRule.BannedContinuousCharacters.Where(n => string.IsNullOrEmpty(n)).Distinct().ToList();

            return null;
        }
    }
}
