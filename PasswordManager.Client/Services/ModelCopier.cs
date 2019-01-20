using PasswordManager.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Client.Services
{
    /// <summary>
    /// 模型复制类
    /// </summary>
    public static class ModelCopier
    {
        /// <summary>
        /// 复制一份项目
        /// </summary>
        public static Item CopyItem(Item item)
        {
            if (item == null)
                return null;
            return new Item()
            {
                ID = item.ID,
                Order = item.Order,
                Category = item.Category,
                ItemName = item.ItemName,
                ItemData = CopyList(item.ItemData, CopyItemData),
                LastOpenTime = item.LastOpenTime,
                CreateTime = item.CreateTime,
            };
        }

        /// <summary>
        /// 复制一份项目
        /// </summary>
        public static void CopyItem(Item itemFrom, Item itemTo)
        {
            if (itemFrom == null)
            {
                itemTo = null;
                return;
            }
            itemTo.ID = itemFrom.ID;
            itemTo.Order = itemFrom.Order;
            itemTo.Category = itemFrom.Category;
            itemTo.ItemName = itemFrom.ItemName;
            itemTo.ItemData = CopyList(itemFrom.ItemData, CopyItemData);
            itemTo.LastOpenTime = itemFrom.LastOpenTime;
            itemTo.CreateTime = itemFrom.CreateTime;
        }

        /// <summary>
        /// 复制一份项目
        /// </summary>
        public static ItemData CopyItemData(ItemData itemData)
        {
            if (itemData == null)
                return null;
            return new ItemData()
            {
                ID = itemData.ID,
                Order = itemData.Order,
                Item = itemData.Item,
                Key = itemData.Key,
                Data = itemData.Data,
                Type = itemData.Type,
                PasswordRule = CopyPasswordRule(itemData.PasswordRule),
            };
        }

        /// <summary>
        /// 复制一份项目
        /// </summary>
        public static PasswordRule CopyPasswordRule(PasswordRule passwordRule)
        {
            if (passwordRule == null)
                return null;
            return new PasswordRule()
            {
                ID = passwordRule.ID,
                ItemData = passwordRule.ItemData,
                Length = passwordRule.Length,
                AllowNumbers = passwordRule.AllowNumbers,
                AllowUpperCases = passwordRule.AllowUpperCases,
                AllowLowerCases = passwordRule.AllowLowerCases,
                AllowSpecialCharacters = passwordRule.AllowSpecialCharacters,
                AllowNumbersFirst = passwordRule.AllowNumbersFirst,
                AllowUpperCasesFirst = passwordRule.AllowUpperCasesFirst,
                AllowLowerCasesFirst = passwordRule.AllowLowerCasesFirst,
                AllowSpecialCharactersFirst = passwordRule.AllowSpecialCharactersFirst,
                LeastNumberCount = passwordRule.LeastNumberCount,
                LeastUpperCaseCount = passwordRule.LeastUpperCaseCount,
                LeastLowerCaseCount = passwordRule.LeastLowerCaseCount,
                LeastSpecialCharacterCount = passwordRule.LeastSpecialCharacterCount,
                BannedCharacters = passwordRule.BannedCharacters,
                BannedStrings = CopyList(passwordRule.BannedStrings),
                BannedContinuousCharacters = CopyList(passwordRule.BannedContinuousCharacters),
                BanRepeatedCharacters = passwordRule.BanRepeatedCharacters,
                BanContinuousRepeatedCharacters = passwordRule.BanContinuousRepeatedCharacters,
                BanContinuousRepeatedCharacterCount = passwordRule.BanContinuousRepeatedCharacterCount,
                BannedCharactersIgnoreCase = passwordRule.BannedCharactersIgnoreCase,
                BannedStringsIgnoreCase = passwordRule.BannedStringsIgnoreCase,
                BannedContinuousCharactersIgnoreCase = passwordRule.BannedContinuousCharactersIgnoreCase,
            };
        }

        /// <summary>
        /// 复制一份项目，使用浅层拷贝进行复制
        /// </summary>
        public static List<T> CopyList<T>(List<T> list)
        {
            if (list == null)
                return null;
            List<T> result = new List<T>();
            foreach (T i in list)
                result.Add(i);
            return result;
        }

        /// <summary>
        /// 复制一份列表，使用copier进行复制
        /// </summary>
        public static List<T> CopyList<T>(List<T> list, Func<T, T> copier)
        {
            if (list == null)
                return null;
            List<T> result = new List<T>();
            foreach (T i in list)
                result.Add(copier(i));
            return result;
        }
    }
}
