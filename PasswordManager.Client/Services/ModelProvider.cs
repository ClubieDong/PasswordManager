using PasswordManager.Client.Models;
using System;
using System.Collections.Generic;

namespace PasswordManager.Client.Services
{
    /// <summary>
    /// 模型提供类
    /// </summary>
    /// <typeparam name="T">模型的类型</typeparam>
    public static class ModelProvider
    {
        #region 类别
        /// <summary>
        /// 类别名列表
        /// </summary>
        public static readonly List<string> CategoryNames = new List<string>()
        {
            "展开的类别", "收缩的类别"
        };
        /// <summary>
        /// 获取类别
        /// </summary>
        /// <param name="categoryName">类别名称</param>
        public static Category GetCategory(string categoryName, List<Category> fatherCategories)
        {
            Category result = new Category()
            {
                CategoryName = "新建类别",
                Categories = fatherCategories,
                Items = new List<Item>(),
            };
            switch (categoryName)
            {
                case "展开的类别":
                    result.IsExpanded = true;
                    break;
                case "收缩的类别":
                    result.IsExpanded = false;
                    break;
                default:
                    throw new Exception("无效的名称！");
            }
            return result;
        }
        #endregion

        #region 项目
        /// <summary>
        /// 项目名列表
        /// </summary>
        public static readonly List<string> ItemNames = new List<string>()
        {
            "空项目", "用户名密码", "用户名密码+手机邮箱", "用户名密码+手机邮箱+密保", "加密文件"
        };
        /// <summary>
        /// 获取项目
        /// </summary>
        /// <param name="itemName">项目名称</param>
        /// <param name="fatherCategory">项目的父类别</param>
        public static Item GetItem(string itemName, Category fatherCategory)
        {
            Item result = new Item()
            {
                Category = fatherCategory,
                ItemName = "新建项目",
                ItemData = new List<ItemData>(),
                LastOpenTime = DateTime.Now,
                CreateTime = DateTime.Now,
            };
            switch (itemName)
            {
                case "空项目":
                    break;
                case "用户名密码":
                    result.ItemData.Add(GetItemData("用户名", result));
                    result.ItemData.Add(GetItemData("密码", result));
                    result.ItemData.Add(GetItemData("分割线", result));
                    result.ItemData.Add(GetItemData("网站", result));
                    break;
                case "用户名密码+手机邮箱":
                    result.ItemData.Add(GetItemData("用户名", result));
                    result.ItemData.Add(GetItemData("密码", result));
                    result.ItemData.Add(GetItemData("分割线", result));
                    result.ItemData.Add(GetItemData("网站", result));
                    result.ItemData.Add(GetItemData("手机", result));
                    result.ItemData.Add(GetItemData("邮箱", result));
                    break;

                case "用户名密码+手机邮箱+密保":
                    result.ItemData.Add(GetItemData("用户名", result));
                    result.ItemData.Add(GetItemData("密码", result));
                    result.ItemData.Add(GetItemData("分割线", result));
                    result.ItemData.Add(GetItemData("网站", result));
                    result.ItemData.Add(GetItemData("手机", result));
                    result.ItemData.Add(GetItemData("邮箱", result));
                    result.ItemData.Add(GetItemData("分割线", result));
                    result.ItemData.Add(GetItemData("密保问题", result));
                    result.ItemData.Add(GetItemData("密保答案", result));
                    break;
                case "加密文件":
                    result.ItemData.Add(GetItemData("文件", result));
                    result.ItemData.Add(GetItemData("密码", result));
                    break;
                default:
                    throw new Exception("无效的名称！");
            }
            for (int i = 0; i < result.ItemData.Count; i++)
                result.ItemData[i].Order = i;
            return result;
        }
        #endregion

        #region 项目数据
        /// <summary>
        /// 项目数据名列表
        /// </summary>
        public static readonly List<string> ItemDataNames = new List<string>()
        {
            "空白项", "分割线", "用户名", "密码", "网站", "文件", "文件夹", "备注", "手机", "邮箱", "密保问题", "密保答案",
        };
        /// <summary>
        /// 获取项目数据
        /// </summary>
        /// <param name="itemDataName">项目数据名称</param>
        /// <param name="fatherItem">父项目</param>
        public static ItemData GetItemData(string itemDataName, Item fatherItem)
        {
            ItemData result = new ItemData()
            {
                Item = fatherItem,
            };
            switch (itemDataName)
            {
                case "空白项":
                    break;
                case "分割线":
                    result.Type = ItemDataType.Splitter;
                    break;
                case "用户名":
                    result.Key = "用户名";
                    break;
                case "密码":
                    result.Key = "密码";
                    result.Type = ItemDataType.Password;
                    break;
                case "网站":
                    result.Key = "网站";
                    result.Type = ItemDataType.Link;
                    break;
                case "文件":
                    result.Key = "文件";
                    result.Type = ItemDataType.Link;
                    break;
                case "文件夹":
                    result.Key = "文件夹";
                    result.Type = ItemDataType.Link;
                    break;
                case "备注":
                    result.Key = "备注";
                    break;
                case "手机":
                    result.Key = "手机";
                    break;
                case "邮箱":
                    result.Key = "邮箱";
                    break;
                case "密保问题":
                    result.Key = "密保问题";
                    break;
                case "密保答案":
                    result.Key = "密保答案";
                    result.Type = ItemDataType.Password;
                    break;
                default:
                    throw new Exception("无效的名称！");
            }
            return result;
        }
        #endregion
    }
}