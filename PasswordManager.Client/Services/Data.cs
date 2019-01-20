using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.Client.Models;
using PasswordManager.Client.Models.ModelBases;

namespace PasswordManager.Client.Services
{
    /// <summary>
    /// 获取、保存数据类
    /// </summary>
    public class Data
    {
        /// <summary>
        /// 用于维护与数据库的连接
        /// </summary>
        private OleDbConnection Connection;
        /// <summary>
        /// 用于向数据库发送指令
        /// </summary>
        private OleDbCommand Command;

        /// <summary>
        /// 是否已连接数据库
        /// </summary>
        public bool Connected { get; private set; } = false;
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="password">数据库密码</param>
        /// <returns>是否成功连接</returns>
        public bool Connect(string password)
        {
            // 如果已连接，报错
            if (Connected)
                throw new Exception("已连接数据库！");

            try
            {
                // 创建连接和指令对象
                string ConnectionString = $"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = Passwords.mdb; Jet OleDb:Database Password = {password}";
                Connection = new OleDbConnection(ConnectionString);
                Command = new OleDbCommand()
                {
                    Connection = Connection,
                };
                Connection.Open();
                Connected = true;
            }
            catch (OleDbException e)
            {
                // 如果是密码错误
                if (e.Message == "密码无效。")
                    Connected = false;
                else
                    throw;
            }
            return Connected;
        }
        /// <summary>
        /// 断开数据库连接
        /// </summary>
        public void Disconnect()
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            Connection.Dispose();
            Command.Dispose();
            Connection = null;
            Command = null;
            Connected = false;
        }

        /// <summary>
        /// 获取设置
        /// </summary>
        public Settings GetSettings()
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            Settings result = new Settings();

            Command.CommandText = "SELECT * FROM [Settings]";
            using (OleDbDataReader reader = Command.ExecuteReader())
            {
                // 设置表格只有一行，如果读第一行失败，报错
                if (!reader.Read())
                    throw new Exception("数据库文件损坏！");
                result.SearchWhenTextChange = Convert.ToBoolean(reader["SearchWhenTextChange"]);
                result.SearchIgnoreCase = Convert.ToBoolean(reader["SearchIgnoreCase"]);
                result.SearchIncludeData = Convert.ToBoolean(reader["SearchIncludeData"]);
            }

            return result;
        }
        /// <summary>
        /// 保存设置
        /// </summary>
        public void SaveSettings(Settings settings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取类别列表，包括每个类别下的项目，不包括项目下的数据
        /// </summary>
        public List<Category> GetCategoriesWithItems()
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            // 获取不含项目的类别列表
            List<Category> result = new List<Category>();
            Command.CommandText = "SELECT * FROM [Categories] ORDER BY [Order]";
            using (OleDbDataReader reader = Command.ExecuteReader())
                while (reader.Read())
                {
                    Category category = new Category()
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Order = Convert.ToInt32(reader["Order"]),
                        Categories = result,
                        CategoryName = Convert.ToString(reader["CategoryName"]),
                        IsExpanded = Convert.ToBoolean(reader["IsExpanded"]),
                    };
                    result.Add(category);
                }
            // 为每个类别添加项目列表
            foreach (Category i in result)
            {
                i.Items = new List<Item>();
                Command.CommandText = "SELECT * FROM [Items] WHERE [CategoryID] = @CategoryID ORDER BY [Order]";
                Command.Parameters.Clear();
                Command.Parameters.AddWithValue("CategoryID", i.ID);
                using (OleDbDataReader reader = Command.ExecuteReader())
                    while (reader.Read())
                    {
                        Item item = new Item()
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Order = Convert.ToInt32(reader["Order"]),
                            Category = i,
                            ItemName = Convert.ToString(reader["ItemName"]),
                            LastOpenTime = Convert.ToDateTime(reader["LastOpenTime"]),
                            CreateTime = Convert.ToDateTime(reader["CreateTime"]),
                        };
                        i.Items.Add(item);
                    }
            }

            return result;
        }
        /// <summary>
        /// 重命名类别
        /// </summary>
        /// <param name="category">类别</param>
        public void RenameCategory(Category category, string newName)
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            // 更改类别信息
            category.CategoryName = newName;
            // 更新数据库信息
            Command.CommandText = "UPDATE [Categories] SET [CategoryName] = @CategoryName WHERE [ID] = @ID";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("CategoryName", newName);
            Command.Parameters.AddWithValue("ID", category.ID);
            Command.ExecuteNonQuery();
        }
        /// <summary>
        /// 添加类别（不包括类别下的项目）
        /// </summary>
        /// <param name="category">要添加的类别</param>
        /// <param name="categoryList">要添加类别的类别列表</param>
        public void AddCategory(Category category, List<Category> categoryList)
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            // 更改列表数据
            category.Order = categoryList.Count;
            categoryList.Add(category);
            // 更改数据库数据
            Command.CommandText = "INSERT INTO [Categories] ([Order], [CategoryName], [IsExpanded]) VALUES (@Order, @CategoryName, @IsExpanded)";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("Order", category.Order);
            Command.Parameters.AddWithValue("CategoryName", category.CategoryName);
            Command.Parameters.AddWithValue("IsExpanded", category.IsExpanded);
            Command.ExecuteNonQuery();
            // 获取新添加的类别的ID
            Command.CommandText = "SELECT MAX([ID]) FROM [Categories]";
            category.ID = (int)Command.ExecuteScalar();
        }
        /// <summary>
        /// 移除类别，必须保证该类别下没有项目
        /// </summary>
        /// <param name="categoryID">要移除的类别ID</param>
        public void RemoveCategory(Category category)
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");
            // 获取该类别下项目的数量
            Command.CommandText = "SELECT COUNT(*) FROM [Items] WHERE [CategoryID] = @CategoryID";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("CategoryID", category.ID);
            // 如果数量不为0，报错
            if ((int)Command.ExecuteScalar() > 0)
                throw new Exception("请先移除该类别下的所有项目！");

            // 移除列表项
            category.Categories.Remove(category);
            // 移除数据库项
            Command.CommandText = "DELETE FROM [Categories] WHERE [ID] = @ID";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("ID", category.ID);
            Command.ExecuteNonQuery();
            // 更新Order
            UpdateOrder(category.Categories);
        }

        /// <summary>
        /// 获取项目数据和密码生成规则，保存到项目中
        /// </summary>
        /// <param name="item">要获取项目数据的项目</param>
        public void EnrichItem(Item item)
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            // 获取项目名字
            Command.CommandText = "SELECT [ItemName] FROM [Items] WHERE [ID] = @ID";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("ID", item.ID);
            using (OleDbDataReader reader = Command.ExecuteReader())
            {
                // 如果没有数据，则报错
                if (!reader.Read())
                    throw new Exception("找不到指定项目！");
                item.ItemName = Convert.ToString(reader["ItemName"]);
            }
            // 获取项目数据
            item.ItemData = new List<ItemData>();
            Command.CommandText = "SELECT * FROM [ItemData] WHERE [ItemID] = @ItemID ORDER BY [Order]";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("ItemID", item.ID);
            using (OleDbDataReader reader = Command.ExecuteReader())
                while (reader.Read())
                {
                    ItemData itemData = new ItemData()
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Order = Convert.ToInt32(reader["Order"]),
                        Item = item,
                        Key = Convert.ToString(reader["Key"]),
                        Data = Convert.ToString(reader["Data"]),
                        Type = (ItemDataType)Convert.ToInt32(reader["Type"]),
                    };
                    item.ItemData.Add(itemData);
                }
            // 由于不能同时打开两个DataReader，所以PasswordRule到最后单独获取
            foreach (ItemData i in item.ItemData)
            {
                Command.CommandText = "SELECT * FROM [PasswordRules] WHERE [ItemDataID] = @ItemDataID";
                Command.Parameters.Clear();
                Command.Parameters.AddWithValue("ItemDataID", i.ID);
                using (OleDbDataReader reader = Command.ExecuteReader())
                {
                    // 如果没有数据，则返回空值，表示该数据不是密码或密码不是自动生成的
                    if (!reader.Read())
                        continue;
                    i.PasswordRule = new PasswordRule()
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        ItemData = i,
                        Length = Convert.ToInt32(reader["Length"]),
                        AllowNumbers = Convert.ToBoolean(reader["AllowNumbers"]),
                        AllowUpperCases = Convert.ToBoolean(reader["AllowUpperCases"]),
                        AllowLowerCases = Convert.ToBoolean(reader["AllowLowerCases"]),
                        AllowSpecialCharacters = Convert.ToBoolean(reader["AllowSpecialCharacters"]),
                        AllowNumbersFirst = Convert.ToBoolean(reader["AllowNumbersFirst"]),
                        AllowUpperCasesFirst = Convert.ToBoolean(reader["AllowUpperCasesFirst"]),
                        AllowLowerCasesFirst = Convert.ToBoolean(reader["AllowLowerCasesFirst"]),
                        AllowSpecialCharactersFirst = Convert.ToBoolean(reader["AllowSpecialCharactersFirst"]),
                        LeastNumberCount = Convert.ToInt32(reader["LeastNumberCount"]),
                        LeastUpperCaseCount = Convert.ToInt32(reader["LeastUpperCaseCount"]),
                        LeastLowerCaseCount = Convert.ToInt32(reader["LeastLowerCaseCount"]),
                        LeastSpecialCharacterCount = Convert.ToInt32(reader["LeastSpecialCharacterCount"]),
                        BannedCharacters = Convert.ToString(reader["BannedCharacters"]),
                        BannedStrings = Convert.ToString(reader["BannedStrings"]).Split(Environment.NewLine.ToCharArray()).ToList(),
                        BannedContinuousCharacters = Convert.ToString(reader["BannedContinuousCharacters"]).Split(Environment.NewLine.ToCharArray()).ToList(),
                        BanRepeatedCharacters = Convert.ToBoolean(reader["BanRepeatedCharacters"]),
                        BanContinuousRepeatedCharacters = Convert.ToBoolean(reader["BanContinuousRepeatedCharacters"]),
                        BanContinuousRepeatedCharacterCount = Convert.ToInt32(reader["BanContinuousRepeatedCharacterCount"]),
                        BannedCharactersIgnoreCase = Convert.ToBoolean(reader["BannedCharactersIgnoreCases"]),
                        BannedStringsIgnoreCase = Convert.ToBoolean(reader["BannedStringsIgnoreCases"]),
                        BannedContinuousCharactersIgnoreCase = Convert.ToBoolean(reader["BannedContinuousCharactersIgnoreCases"]),
                    };
                }
            }
        }
        /// <summary>
        /// 更新项目的最近打开时间
        /// </summary>
        /// <param name="item">要更新时间的项目</param>
        public void UpdateLastOpenTime(Item item)
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            // 更改项目信息
            item.LastOpenTime = DateTime.Now;
            // 如果ID不为0，意味着这个项目不是新建的，则更新数据库信息
            if (item.ID != 0)
            {
                Command.CommandText = "UPDATE [Items] SET [LastOpenTime] = @LastOpenTime WHERE [ID] = @ID";
                Command.Parameters.Clear();
                Command.Parameters.AddWithValue("LastOpenTime", item.LastOpenTime.ToString());
                Command.Parameters.AddWithValue("ID", item.ID);
                Command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 把项目从一个类别移动到另一个类别
        /// </summary>
        /// <param name="item">要移动的项目</param>
        /// <param name="category">移动到的新类别</param>
        public void MoveItem(Item item, Category category)
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            // 更新本地数据
            item.Category.Items.Remove(item);
            UpdateOrder(item.Category.Items);
            category.Items.Add(item);
            item.Category = category;
            // 更新数据库数据
            Command.CommandText = "UPDATE [Items] SET [CategoryID] = @CategoryID, [Order] = @Order WHERE [ID] = @ID";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("CategoryID", category.ID);
            Command.Parameters.AddWithValue("Order", category.Items.Count - 1);
            Command.Parameters.AddWithValue("ID", item.ID);
            Command.ExecuteNonQuery();
        }
        /// <summary>
        /// 如果ID存在，则更新项目，如果ID不存在，则保存新建项目
        /// </summary>
        /// <param name="item">要保存或更新的项目</param>
        public void SaveItem(Item item)
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            if (item.ID == 0)
            {
                // 更新本地数据
                item.Order = item.Category.Items.Count;
                item.Category.Items.Add(item);
                // 添加数据库数据
                Command.CommandText = "INSERT INTO [Items] ([Order], [CategoryID], [ItemName], [LastOpenTime], [CreateTime]) VALUES (@Order, @CategoryID, @ItemName, @LastOpenTime, @CreateTime)";
                Command.Parameters.Clear();
                Command.Parameters.AddWithValue("Order", item.Order);
                Command.Parameters.AddWithValue("CategoryID", item.Category.ID);
                Command.Parameters.AddWithValue("ItemName", item.ItemName);
                Command.Parameters.AddWithValue("LastOpenTime", item.LastOpenTime.ToString());
                Command.Parameters.AddWithValue("CreateTime", item.CreateTime.ToString());
                Command.ExecuteNonQuery();
                // 获取新添加的项目的ID
                Command.CommandText = "SELECT MAX([ID]) FROM [Items]";
                item.ID = (int)Command.ExecuteScalar();
            }
            else
            {
                // 更新数据库数据
                Command.CommandText = "UPDATE [Items] SET [ItemName] = @ItemName WHERE [ID] = @ID";
                Command.Parameters.Clear();
                Command.Parameters.AddWithValue("ItemName", item.ItemName);
                Command.Parameters.AddWithValue("ID", item.ID);
                Command.ExecuteNonQuery();
            }
            // 移除项目数据
            Command.CommandText = "DELETE FROM [ItemData] WHERE [ItemID] = @ItemID";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("ItemID", item.ID);
            Command.ExecuteNonQuery();
            // 重新添加项目数据
            foreach (ItemData i in item.ItemData)
            {
                Command.CommandText = "INSERT INTO [ItemData] ([Order], [ItemID], [Key], [Data], [IsPassword], [IsLink], [IsSplitter]) VALUES (@Order, @ItemID, @Key, @Data, @IsPassword, @IsLink, @IsSplitter)";
                Command.Parameters.Clear();
                Command.Parameters.AddWithValue("Order", i.Order);
                Command.Parameters.AddWithValue("ItemID", item.ID);
                Command.Parameters.AddWithValue("Key", i.Key ?? string.Empty);
                Command.Parameters.AddWithValue("Data", i.Data ?? string.Empty);
                Command.Parameters.AddWithValue("IsPassword", i.Type);
                Command.ExecuteNonQuery();
                // 获取新添加的项目数据s的ID
                Command.CommandText = "SELECT MAX([ID]) FROM [ItemData]";
                i.ID = (int)Command.ExecuteScalar();
                // 如果有密码生成规则，添加密码生成规则
                if (i.PasswordRule != null)
                {
                    Command.CommandText = "INSERT INTO [PasswordRules] ([Length], [AllowNumbers], [AllowUpperCases], [AllowLowerCases], [AllowSpecialCharacters], [AllowNumbersFirst], [AllowUpperCasesFirst], [AllowLowerCasesFirst], [AllowSpecialCharactersFirst], [LeastNumberCount], [LeastUpperCaseCount], [LeastLowerCaseCount], [LeastSpecialCharacterCount], [BannedCharacters], [BannedStrings], [BannedContinuousCharacters], [BanRepeatedCharacters], [BanContinuousRepeatedCharacters], [BanContinuousRepeatedCharacterCount], [BannedCharactersIgnoreCases], [BannedStringsIgnoreCases], [BannedContinuousCharactersIgnoreCases]) VALUES (@ItemDataID, @Length, @AllowNumbers, @AllowUpperCases, @AllowLowerCases, @AllowSpecialCharacters, @AllowNumbersFirst, @AllowUpperCasesFirst, @AllowLowerCasesFirst, @AllowSpecialCharactersFirst, @LeastNumberCount, @LeastUpperCaseCount, @LeastLowerCaseCount, @LeastSpecialCharacterCount, @BannedCharacters, @BannedStrings, @BannedContinuousCharacters, @BanRepeatedCharacters, @BanContinuousRepeatedCharacters, @BanContinuousRepeatedCharacterCount, @BannedCharactersIgnoreCases, @BannedStringsIgnoreCases, @BannedContinuousCharactersIgnoreCases)";
                    Command.Parameters.Clear();
                    Command.Parameters.AddWithValue("Length", i.PasswordRule.Length);
                    Command.Parameters.AddWithValue("AllowNumbers", i.PasswordRule.AllowNumbers);
                    Command.Parameters.AddWithValue("AllowUpperCases", i.PasswordRule.AllowUpperCases);
                    Command.Parameters.AddWithValue("AllowLowerCases", i.PasswordRule.AllowLowerCases);
                    Command.Parameters.AddWithValue("AllowSpecialCharacters", i.PasswordRule.AllowSpecialCharacters);
                    Command.Parameters.AddWithValue("AllowNumbersFirst", i.PasswordRule.AllowNumbersFirst);
                    Command.Parameters.AddWithValue("AllowUpperCasesFirst", i.PasswordRule.AllowUpperCasesFirst);
                    Command.Parameters.AddWithValue("AllowLowerCasesFirst", i.PasswordRule.AllowLowerCasesFirst);
                    Command.Parameters.AddWithValue("AllowSpecialCharactersFirst", i.PasswordRule.AllowSpecialCharactersFirst);
                    Command.Parameters.AddWithValue("LeastNumberCount", i.PasswordRule.LeastNumberCount);
                    Command.Parameters.AddWithValue("LeastUpperCaseCount", i.PasswordRule.LeastUpperCaseCount);
                    Command.Parameters.AddWithValue("LeastLowerCaseCount", i.PasswordRule.LeastLowerCaseCount);
                    Command.Parameters.AddWithValue("LeastSpecialCharacterCount", i.PasswordRule.LeastSpecialCharacterCount);
                    Command.Parameters.AddWithValue("BannedCharacters", i.PasswordRule.BannedCharacters ?? string.Empty);
                    Command.Parameters.AddWithValue("BannedStrings", string.Join(Environment.NewLine, i.PasswordRule.BannedStrings ?? new List<string>()));
                    Command.Parameters.AddWithValue("BannedContinuousCharacters", string.Join(Environment.NewLine, i.PasswordRule.BannedContinuousCharacters ?? new List<string>()));
                    Command.Parameters.AddWithValue("BanRepeatedCharacters", i.PasswordRule.BanRepeatedCharacters);
                    Command.Parameters.AddWithValue("BanContinuousRepeatedCharacters", i.PasswordRule.BanContinuousRepeatedCharacters);
                    Command.Parameters.AddWithValue("BanContinuousRepeatedCharacterCount", i.PasswordRule.BanContinuousRepeatedCharacterCount);
                    Command.Parameters.AddWithValue("BannedCharactersIgnoreCases", i.PasswordRule.BannedCharactersIgnoreCase);
                    Command.Parameters.AddWithValue("BannedStringsIgnoreCases", i.PasswordRule.BannedStringsIgnoreCase);
                    Command.Parameters.AddWithValue("BannedContinuousCharactersIgnoreCases", i.PasswordRule.BannedContinuousCharactersIgnoreCase);
                    Command.ExecuteNonQuery();
                    // 获取并返回新添加的密码规则的ID
                    Command.CommandText = "SELECT MAX([ID]) FROM [PasswordRules]";
                    i.PasswordRule.ID = (int)Command.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// 移除项目及其下的数据和密码规则
        /// </summary>
        /// <param name="itemID">要移除的项目ID</param>
        public void RemoveItem(Item item)
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            // 移除密码规则
            Command.CommandText = "DELETE FROM [PasswordRules] WHERE [ItemDataID] IN (SELECT [ID] FROM [ItemData] WHERE ItemID = @ItemID)";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("ItemID", item.ID);
            Command.ExecuteNonQuery();
            // 移除项目数据
            Command.CommandText = "DELETE FROM [ItemData] WHERE [ItemID] = @ItemID";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("ItemID", item.ID);
            Command.ExecuteNonQuery();
            // 移除项目
            Command.CommandText = "DELETE FROM [Items] WHERE [ID] = @ID";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("ID", item.ID);
            Command.ExecuteNonQuery();
            // 更改本地数据
            item.Category.Items.Remove(item);
            UpdateOrder(item.Category.Items);
        }

        /// <summary>
        /// 根据泛型类型获得数据库表名
        /// </summary>
        /// <typeparam name="T">模型的类型</typeparam>
        /// <returns>数据库表名</returns>
        private string GetTableName<T>() where T : BaseOrder
        {
            Type type = typeof(T);
            // 由于不能使用switch语句，只能连续使用if语句
            if (type == typeof(Category))
                return "Categories";
            else if (type == typeof(Item))
                return "Items";
            else if (type == typeof(ItemData))
                return "ItemData";
            else
                throw new Exception("类型无效！");
        }
        /// <summary>
        /// 更新列表项的Order，并更新数据库
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="list">按Order顺序排序的列表</param>
        private void UpdateOrder<T>(List<T> list) where T : BaseOrder
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Order != i)
                {
                    list[i].Order = i;
                    Command.CommandText = $"UPDATE {GetTableName<T>()} SET [Order] = @Order WHERE [ID] = @ID";
                    Command.Parameters.Clear();
                    Command.Parameters.AddWithValue("Order", list[i].Order);
                    Command.Parameters.AddWithValue("ID", list[i].ID);
                    Command.ExecuteNonQuery();
                }
        }
        /// <summary>
        /// 移动类别或项目或项目数据
        /// </summary>
        /// <param name="list">列表</param>
        /// <param name="index1">要移动的类别或项目或项目数据在列表中的下标</param>
        /// <param name="index2">移动到这个下标对应的类别或项目或项目数据之前</param>
        public void MoveToBefore<T>(List<T> list, int index1, int index2) where T : BaseOrder
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");
            // 检查参数合法性
            if (index1 > list.Count || index2 > list.Count || index1 < 0 || index2 < 0)
                throw new Exception("参数无效！");
            // 如果两个下标相同，或者后者比前者多1，则不需要移动，直接退出
            if (index1 == index2 || index1 == index2 - 1)
                return;

            // 更改列表数据
            // 为了保证删除前后下标不变，先置空，再插入，最后删除空项
            T t = list[index1];
            list[index1] = null;
            list.Insert(index2, t);
            list.Remove(null);
            // 更新列表项的Order
            for (int i = 0; i < list.Count; i++)
                list[i].Order = i;
        }
        /// <summary>
        /// 把类别或项目或项目数据向上移
        /// </summary>
        /// <param name="list">列表</param>
        /// <param name="index">要移动的类别或项目或项目数据在列表中的下标</param>
        public void MoveUp<T>(List<T> list, int index) where T : BaseOrder
        {
            // 我知道这样子效率很低，但是顶多差几毫秒吧，感觉不出来
            MoveToBefore(list, index, index - 1);
        }
        /// <summary>
        /// 把类别或项目或项目数据向下移
        /// </summary>
        /// <param name="list">列表</param>
        /// <param name="index">要移动的类别或项目或项目数据在列表中的下标</param>
        public void MoveDown<T>(List<T> list, int index) where T : BaseOrder
        {
            // 我知道这样子效率很低，但是顶多差几毫秒吧，感觉不出来
            MoveToBefore(list, index, index + 2);
        }

        /// <summary>
        /// 判断一个项目的数据Key和Data中是否包含指定文本
        /// </summary>
        /// <param name="text">要搜索的文本</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>是否包含指定文本</returns>
        public bool ContainText(Item item, string text, bool ignoreCase)
        {
            // 如果未连接数据库，报错
            if (!Connected)
                throw new Exception("未连接数据库！");

            // 获取该项目的所有键值对
            Command.CommandText = "SELECT [Key], [Data] FROM [ItemData] WHERE [ItemID] = @ItemID";
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("ItemID", item.ID);
            using (OleDbDataReader reader = Command.ExecuteReader())
                while (reader.Read())
                {
                    // 分别判断key和data是否包含text
                    string key = Convert.ToString(reader["Key"]);
                    if (IsTextIncluded(key, text, ignoreCase))
                        return true;
                    string data = Convert.ToString(reader["Data"]);
                    if (IsTextIncluded(data, text, ignoreCase))
                        return true;
                }
            return false;
        }
        /// <summary>
        /// 判断subText是否包含在text中
        /// </summary>
        /// <param name="ignoreCase">是否忽略空格</param>
        public static bool IsTextIncluded(string text, string subText, bool ignoreCase)
        {
            return ignoreCase ? text.ToUpper().Contains(subText.ToUpper()) : text.Contains(subText);
        }
    }
}
