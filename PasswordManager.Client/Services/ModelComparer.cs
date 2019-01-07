using PasswordManager.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Client.Services
{
    /// <summary>
    /// 模型比较类
    /// </summary>
    public static class ModelComparer
    {
        /// <summary>
        /// 比较项目是否相同（仅比较ItemName、ItemData）
        /// </summary>
        public static bool CompareItem(Item item1, Item item2)
        {
            // 同为null则判断为相同
            if (item1 == null && item2 == null)
                return true;
            // 一个为null而另一个不为null则判断为不同
            if (item1 == null || item1 == null)
                return false;
            // 比较属性
            if (!(item1.ItemName == item2.ItemName && CompareList(item1.ItemData, item2.ItemData, CompareItemData)))
                return false;
            return true;
        }

        /// <summary>
        /// 比较项目数据是否相同（仅比较Key、Data、IsPassword、IsLink、IsSplitter、PasswordRule）
        /// </summary>
        public static bool CompareItemData(ItemData itemData1, ItemData itemData2)
        {
            // 同为null则判断为相同
            if (itemData1 == null && itemData2 == null)
                return true;
            // 一个为null而另一个不为null则判断为不同
            if (itemData1 == null || itemData2 == null)
                return false;
            // 比较属性
            if (!(itemData1.Key == itemData2.Key && itemData1.Data == itemData2.Data && itemData1.IsPassword == itemData2.IsPassword && itemData1.IsLink == itemData2.IsLink && itemData1.IsSplitter == itemData2.IsSplitter && ComparePasswordRule(itemData1.PasswordRule, itemData2.PasswordRule)))
                return false;
            return true;
        }

        /// <summary>
        /// 比较密码生成规则是否相同（不比较ID、ItemData）
        /// </summary>
        public static bool ComparePasswordRule(PasswordRule passwordRule1, PasswordRule passwordRule2)
        {
            // 同为null则判断为相同
            if (passwordRule1 == null && passwordRule2 == null)
                return true;
            // 一个为null而另一个不为null则判断为不同
            if (passwordRule1 == null || passwordRule2 == null)
                return false;
            // 比较属性
            if (!(passwordRule1.Length == passwordRule2.Length && passwordRule1.AllowNumbers == passwordRule2.AllowNumbers && passwordRule1.AllowUpperCases == passwordRule2.AllowUpperCases && passwordRule1.AllowLowerCases == passwordRule2.AllowLowerCases && passwordRule1.AllowSpecialCharacters == passwordRule2.AllowSpecialCharacters && passwordRule1.AllowNumbersFirst == passwordRule2.AllowNumbersFirst && passwordRule1.AllowUpperCasesFirst == passwordRule2.AllowUpperCasesFirst && passwordRule1.AllowLowerCasesFirst == passwordRule2.AllowLowerCasesFirst && passwordRule1.AllowSpecialCharactersFirst == passwordRule2.AllowSpecialCharactersFirst && passwordRule1.LeastNumberCount == passwordRule2.LeastNumberCount && passwordRule1.LeastUpperCaseCount == passwordRule2.LeastUpperCaseCount && passwordRule1.LeastLowerCaseCount == passwordRule2.LeastLowerCaseCount && passwordRule1.LeastSpecialCharacterCount == passwordRule2.LeastSpecialCharacterCount && passwordRule1.BannedCharacters == passwordRule2.BannedCharacters && CompareList(passwordRule1.BannedStrings, passwordRule2.BannedStrings) && CompareList(passwordRule1.BannedContinuousCharacters, passwordRule2.BannedContinuousCharacters) && passwordRule1.BanRepeatedCharacters == passwordRule2.BanRepeatedCharacters && passwordRule1.BanContinuousRepeatedCharacters == passwordRule2.BanContinuousRepeatedCharacters && passwordRule1.BanContinuousRepeatedCharacterCount == passwordRule2.BanContinuousRepeatedCharacterCount && passwordRule1.BannedCharactersIgnoreCase == passwordRule2.BannedCharactersIgnoreCase && passwordRule1.BannedStringsIgnoreCase == passwordRule2.BannedStringsIgnoreCase && passwordRule1.BannedContinuousCharactersIgnoreCase == passwordRule2.BannedContinuousCharactersIgnoreCase))
                return false;
            return true;
        }

        /// <summary>
        /// 比较列表是否相同，使用object.Equals()进行比较
        /// </summary>
        public static bool CompareList<T>(List<T> list1, List<T> list2)
        {
            return CompareList(list1, list2, (n1, n2) => n1.Equals(n2));
        }

        /// <summary>
        /// 比较列表是否相同，使用comparer进行比较
        /// </summary>
        /// <param name="comparer">列表中的项目的比较器</param>
        public static bool CompareList<T>(List<T> list1, List<T> list2, Func<T, T, bool> comparer)
        {
            // 同为null则判断为相同
            if (list1 == null && list2 == null)
                return true;
            // 一个为null而另一个不为null则判断为不同
            if (list1 == null || list2 == null)
                return false;
            // 列表长度相同
            if (list1.Count != list2.Count)
                return false;
            // 列表每一个都相同
            for (int i = 0; i < list1.Count; i++)
                if (!comparer(list1[i], list2[i]))
                    return false;
            return true;
        }
    }
}
