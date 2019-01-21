using PasswordManager.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordManager.Client.Services
{
    /// <summary>
    /// 密码生成类
    /// </summary>
    public static class PasswordGenerator
    {
        /// <summary>
        /// 字符集合
        /// </summary>
        private class Characters
        {
            public List<char> Numbers;
            public List<char> UpperCases;
            public List<char> LowerCases;
            public List<char> SpecialCharacters;
        }

        /// <summary>
        /// 禁止出现的字符串集合
        /// </summary>
        private class BannedStrings
        {
            /// <summary>
            /// 忽略大小写的字符串集合
            /// </summary>
            public HashSet<string> IgnoreCase;
            /// <summary>
            /// 区分大小写的字符串集合
            /// </summary>
            public HashSet<string> DistinguishCase;
        }

        private const string NumbersString = "0123456789";
        private const string UpperCasesString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowerCasesString = "abcdefghijklmnopqrstuvwxyz";
        private const string SpecialCharactersString = "`~!@#$%^&*()-_=+[{]};:'\"\\|,<.>/?";

        private static readonly Random Random = new Random();
        private static readonly List<char> EmptyListOfChar = new List<char>();

        /// <summary>
        /// 生成密码
        /// </summary>
        /// <param name="passwordRule">生成密码的规则</param>
        /// <param name="tryCount">最多尝试的次数</param>
        /// <returns>生成的密码</returns>
        public static string Generate(PasswordRule passwordRule, int tryCount)
        {
            // 获取允许出现的字符集合
            Characters allowedCharacters = GetAllowedCharacters(passwordRule);
            // 如果不允许出现重复字符，而所有允许出现的字符数量不足以达到所需的密码长度，报错
            if (passwordRule.BanRepeatedCharacters && allowedCharacters.Numbers.Count + allowedCharacters.UpperCases.Count + allowedCharacters.LowerCases.Count + allowedCharacters.SpecialCharacters.Count < passwordRule.Length)
                throw new Exception("没有足够的不重复字符来生成该长度的字符串！");

            // 获取禁止出现的字符串集合
            BannedStrings bannedStrings = GetBannedStrings(passwordRule);

            // 尝试生成tryCount次
            for (int i = 0; i < tryCount; i++)
            {
                // 尝试生成
                string password = TryGenerate(passwordRule, allowedCharacters);
                // 判断是否生成成功
                if (CheckValidity(passwordRule, password, bannedStrings))
                    return password;
            }

            // 如果尝试生成tryCount次后仍没有生成，报错
            throw new Exception($"要求过于苛刻，{tryCount}次尝试也无法找到满足条件的密码！");
        }

        /// <summary>
        /// 获取所有允许出现的字符集合
        /// </summary>
        /// <param name="passwordRule">生成密码的规则</param>
        private static Characters GetAllowedCharacters(PasswordRule passwordRule)
        {
            // 如果禁止出现的字符忽略大小写，则将大写和小写都添加进禁止出现的字符集合
            string bannedCharacters = passwordRule.BannedCharactersIgnoreCase ? passwordRule.BannedCharacters.ToUpper() + passwordRule.BannedCharacters.ToLower() : passwordRule.BannedCharacters;

            List<char> allowedNumbers = new List<char>();
            List<char> allowedUpperCases = new List<char>();
            List<char> allowedLowerCases = new List<char>();
            List<char> allowedSpecialCharacters = new List<char>();

            // 如果允许出现数字，则添加除去禁止出现的字符后的剩余数字
            if (passwordRule.AllowNumbers)
                allowedNumbers.AddRange(NumbersString.Except(bannedCharacters));
            // 如果允许出现大写字母，则添加除去禁止出现的字符后的剩余大写字母
            if (passwordRule.AllowUpperCases)
                allowedUpperCases.AddRange(UpperCasesString.Except(bannedCharacters));
            // 如果允许出现小写字母，则添加除去禁止出现的字符后的剩余小写字母
            if (passwordRule.AllowLowerCases)
                allowedLowerCases.AddRange(LowerCasesString.Except(bannedCharacters));
            // 如果允许出现特殊符号，则添加除去禁止出现的字符后的剩余特殊符号
            if (passwordRule.AllowSpecialCharacters)
                allowedSpecialCharacters.AddRange(SpecialCharactersString.Except(bannedCharacters));

            return new Characters()
            {
                Numbers = allowedNumbers,
                UpperCases = allowedUpperCases,
                LowerCases = allowedLowerCases,
                SpecialCharacters = allowedSpecialCharacters,
            };
        }

        /// <summary>
        /// 获取禁止出现的字符串集合
        /// </summary>
        /// <param name="passwordRule">生成密码的规则</param>
        private static BannedStrings GetBannedStrings(PasswordRule passwordRule)
        {
            HashSet<string> ignoreCases = new HashSet<string>();
            HashSet<string> distinguishCases = new HashSet<string>();

            // 如果禁止出现的字符串忽略大小写，添加大写的字符串
            if (passwordRule.BannedStringsIgnoreCase)
                ignoreCases.UnionWith(passwordRule.BannedStrings.Select(n => n.ToUpper()));
            else
                distinguishCases.UnionWith(passwordRule.BannedStrings);
            // 如果禁止出现的连续字符串忽略大小写，添加大写字符串
            List<string> bannedContinuousCharacters = passwordRule.BannedContinuousCharactersIgnoreCase ? passwordRule.BannedContinuousCharacters.Select(n => n.ToUpper()).ToList() : passwordRule.BannedContinuousCharacters;

            // 根据是否忽略大小写，创建对两个HashSet的引用
            HashSet<string> set = passwordRule.BannedContinuousCharactersIgnoreCase ? ignoreCases : distinguishCases;

            // 遍历每个禁止出现任意连续字符的字符串，把它的所有长度大于1的子字符串添加进集合
            foreach (string s in bannedContinuousCharacters)
            {
                int len = s.Length;
                for (int i = 0; i < len - 1; i++)
                    for (int j = 2; j <= len - i; j++)
                        set.Add(s.Substring(i, j));
            }

            return new BannedStrings()
            {
                IgnoreCase = ignoreCases,
                DistinguishCase = distinguishCases,
            };
        }

        /// <summary>
        /// 尝试生成密码
        /// </summary>
        /// <param name="passwordRule">生成密码的规则</param>
        /// <param name="allowedCharacters">允许出现的字符</param>
        /// <returns>尝试生成的密码（可能不合法，需要验证）</returns>
        private static string TryGenerate(PasswordRule passwordRule, Characters allowedCharacters)
        {
            // 复制一份最少出现的字符数量，便于修改
            int leastNumberCount = passwordRule.LeastNumberCount;
            int leastUpperCaseCount = passwordRule.LeastUpperCaseCount;
            int leastLowerCaseCount = passwordRule.LeastLowerCaseCount;
            int leastSpecialCharacterCount = passwordRule.LeastSpecialCharacterCount;
            // 获取首位允许出现的字符集合
            Characters localAllowedCharactersFirst = new Characters()
            {
                Numbers = passwordRule.AllowNumbersFirst ? allowedCharacters.Numbers : EmptyListOfChar,
                UpperCases = passwordRule.AllowUpperCasesFirst ? allowedCharacters.UpperCases : EmptyListOfChar,
                LowerCases = passwordRule.AllowLowerCasesFirst ? allowedCharacters.LowerCases : EmptyListOfChar,
                SpecialCharacters = passwordRule.AllowSpecialCharactersFirst ? allowedCharacters.SpecialCharacters : EmptyListOfChar,
            };
            // 由于RandomChar可能会修改Characters，此处复制一份Characters，便于修改，以保证纯函数性
            Characters localAllowedCharacters = new Characters()
            {
                Numbers = allowedCharacters.Numbers,
                UpperCases = allowedCharacters.UpperCases,
                LowerCases = allowedCharacters.LowerCases,
                SpecialCharacters = allowedCharacters.SpecialCharacters,
            };

            // 创建结果并添加第一个字符
            List<char> result = new List<char>
            {
                RandomChar(passwordRule, localAllowedCharactersFirst),
            };

            // 将第一个字符所属的类别的至少出现的数量减一
            if (leastNumberCount > 0 && NumbersString.Contains(result[0]))
                leastNumberCount--;
            if (leastUpperCaseCount > 0 && UpperCasesString.Contains(result[0]))
                leastUpperCaseCount--;
            if (leastLowerCaseCount > 0 && LowerCasesString.Contains(result[0]))
                leastLowerCaseCount--;
            if (leastSpecialCharacterCount > 0 && SpecialCharactersString.Contains(result[0]))
                leastSpecialCharacterCount--;

            // 生成字符保证每个字符类别的字符数量达到最少要求
            // 注意RandomChar函数非纯函数，如果禁止出现重复的字符，该函数会将随机得到的字符自动删去，保证下次不会随机到
            for (int i = 0; i < leastNumberCount; i++)
                result.Add(RandomChar(passwordRule, localAllowedCharacters.Numbers));
            for (int i = 0; i < leastUpperCaseCount; i++)
                result.Add(RandomChar(passwordRule, localAllowedCharacters.UpperCases));
            for (int i = 0; i < leastLowerCaseCount; i++)
                result.Add(RandomChar(passwordRule, localAllowedCharacters.LowerCases));
            for (int i = 0; i < leastSpecialCharacterCount; i++)
                result.Add(RandomChar(passwordRule, localAllowedCharacters.SpecialCharacters));

            // 满足最少出现字符数量要求后，从剩余所有字符中随机，以达到密码长度要求
            int leftCount = passwordRule.Length - result.Count;
            for (int i = 0; i < leftCount; i++)
                result.Add(RandomChar(passwordRule, localAllowedCharacters));

            // 保证首位不变，之后的所有字符打乱顺序
            // 遍历除首位外的每一位
            for (int i = 1; i < passwordRule.Length; i++)
            {
                // 随机出一个新的下标
                int index = Random.Next(passwordRule.Length - 1) + 1;
                // 如果新的下标和原来的下标不一样，交换这两个下标对应的字符
                if (index != i)
                {
                    char temp = result[i];
                    result[i] = result[index];
                    result[index] = temp;
                }
            }

            return new string(result.ToArray());
        }

        /// <summary>
        /// 从一个允许出现的字符列表中随机字符，如果不允许出现重复字符，则将其删去。
        /// 注意此函数非纯函数。
        /// </summary>
        /// <param name="passwordRule">生成密码的规则</param>
        /// <param name="allowedCharacters">允许出现的字符列表</param>
        /// <returns>随机得到的字符</returns>
        private static char RandomChar(PasswordRule passwordRule, List<char> allowedCharacters)
        {
            // 随机获得下标
            int index = Random.Next(allowedCharacters.Count);
            char result = allowedCharacters[index];
            // 如果不允许出现重复字符，删去随机得到的字符
            if (passwordRule.BanRepeatedCharacters)
                allowedCharacters.RemoveAt(index);
            return result;
        }

        /// <summary>
        /// 从允许出现的字符集合中随机字符，如果不允许出现重复字符，则将其删去。
        /// 注意此函数非纯函数。
        /// </summary>
        /// <param name="passwordRule">生成密码的规则</param>
        /// <param name="allowedCharacters">允许出现的字符集合</param>
        /// <returns>随机得到的字符</returns>
        private static char RandomChar(PasswordRule passwordRule, Characters allowedCharacters)
        {
            // 计算所有允许出现的字符数量
            int count = allowedCharacters.Numbers.Count + allowedCharacters.UpperCases.Count + allowedCharacters.LowerCases.Count + allowedCharacters.SpecialCharacters.Count;
            // 随机得到下标，此下标是allowedCharacters中所有List合并后得到的列表的下标
            int index = Random.Next(count);
            char result;

            // 如果随机得到的是数字的下标
            if (index < allowedCharacters.Numbers.Count)
            {
                result = allowedCharacters.Numbers[index];
                // 如果不允许出现重复字符，删去随机得到的字符
                if (passwordRule.BanRepeatedCharacters)
                    allowedCharacters.Numbers.RemoveAt(index);
                return result;
            }
            // 如果不是数字的下标，减去数字部分的数量
            index -= allowedCharacters.Numbers.Count;

            // 同上
            if (index < allowedCharacters.UpperCases.Count)
            {
                result = allowedCharacters.UpperCases[index];
                if (passwordRule.BanRepeatedCharacters)
                    allowedCharacters.UpperCases.RemoveAt(index);
                return result;
            }
            index -= allowedCharacters.UpperCases.Count;

            // 同上
            if (index < allowedCharacters.LowerCases.Count)
            {
                result = allowedCharacters.LowerCases[index];
                if (passwordRule.BanRepeatedCharacters)
                    allowedCharacters.LowerCases.RemoveAt(index);
                return result;
            }
            index -= allowedCharacters.LowerCases.Count;

            // 同上
            if (index < allowedCharacters.SpecialCharacters.Count)
            {
                result = allowedCharacters.SpecialCharacters[index];
                if (passwordRule.BanRepeatedCharacters)
                    allowedCharacters.SpecialCharacters.RemoveAt(index);
                return result;
            }

            // 下标越界
            throw new Exception("生成过程出错！");
        }

        /// <summary>
        /// 验证密码是否合法
        /// </summary>
        /// <param name="passwordRule">生成密码的规则</param>
        /// <param name="password">要检验的密码</param>
        /// <param name="bannedStrings">禁止出现的字符串集合</param>
        /// <returns>密码是否合法</returns>
        private static bool CheckValidity(PasswordRule passwordRule, string password, BannedStrings bannedStrings)
        {
            // 分别判断忽略大小写和区分大小写是否合法
            return CheckValidity(passwordRule, password, bannedStrings.DistinguishCase) &&
                CheckValidity(passwordRule, password.ToUpper(), bannedStrings.IgnoreCase);
        }

        /// <summary>
        /// 验证密码是否合法
        /// </summary>
        /// <param name="passwordRule">生成密码的规则</param>
        /// <param name="password">要检验的密码</param>
        /// <param name="bannedStrings">禁止出现的字符串集合</param>
        /// <returns>密码是否合法</returns>
        private static bool CheckValidity(PasswordRule passwordRule, string password, HashSet<string> bannedStrings)
        {
            // 遍历密码的每个子字符串，判断是否包含在禁止出现的字符串集合中
            int len = password.Length;
            for (int i = 0; i < len - 1; i++)
                for (int j = 2; j <= len - i; j++)
                    if (bannedStrings.Contains(password.Substring(i, j)))
                        return false;

            // 如果允许出现重复的字符，但又不允许出现连续重复的字符
            if (!passwordRule.BanRepeatedCharacters && passwordRule.BanContinuousRepeatedCharacters)
            {
                // 遍历一遍密码，如果当前字符与上一次字符一样，重复计数加一，不一样则赋为1
                int repeatedCount = 1;
                for (int i = 1; i < len; i++)
                {
                    if (password[i - 1] == password[i])
                    {
                        repeatedCount++;
                        // 如果连续出现的字符超出允许范围，返回密码不合法
                        if (repeatedCount >= passwordRule.BanContinuousRepeatedCharacterCount)
                            return false;
                    }
                    else
                        repeatedCount = 1;
                }
            }

            return true;
        }
    }
}
