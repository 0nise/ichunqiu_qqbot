﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Newbe.Mahua.Plugins.iChunqiuQQBoot.Beta.Tools
{
    /// <summary>  
    /// 数据访问抽象基础类  
    /// </summary>  
    public class DBHelperMySQL
    {
        //private readonly String connectionStr = "server=localhost;user id=root;password=root;database=ichunqiu;charset=utf8";

        /// <summary>  
        /// 根据SQL获取DataTable数据表  
        /// </summary>  
        /// <param name="SQL">查询语句</param>  
        /// <param name="Table_name">返回表的表名</param>  
        /// <returns></returns>  
        public static string GetContent(string sql, List<MySqlParameter> parameter, string content)
        {
            string resultStr = "";
            if (content == "" || "".Equals(content) || content.Length == 0 || content == null)
            {
                content = "\n【文章】";
            }
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {
                    // 添加参数
                    if (parameter.Count > 0)
                    {
                        mySqlCommand.Parameters.AddRange(parameter.ToArray());
                    }
                    // 提取数据
                    using (MySqlDataReader myDataReader = mySqlCommand.ExecuteReader())
                    {
                        while (myDataReader.Read() == true)
                        {
                            string tmpStr = content + myDataReader["title"] + "\n" + myDataReader["url"] + "\n";
                            resultStr += tmpStr;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                // 关闭数据库链接
                ConnectionPool.getPool().closeConnection(connection);
            }
            return resultStr;
        }

        /// <summary>
        /// 提取课程信息
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="parameter">参数信息</param>
        /// <returns></returns>
        private static string geClass(string sql, List<MySqlParameter> parameter)
        {

            string resultStr = "";
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {
                    // 添加参数
                    if (parameter.Count > 0)
                    {
                        mySqlCommand.Parameters.AddRange(parameter.ToArray());
                    }
                    // 提取数据
                    using (MySqlDataReader myDataReader = mySqlCommand.ExecuteReader())
                    {
                        while (myDataReader.Read() == true)
                        {
                            string tmpStr = "\n【课程】" + myDataReader["class_name"] + "\n" + myDataReader["class_url"] + "\n";
                            resultStr += tmpStr;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                // 关闭数据库链接
                ConnectionPool.getPool().closeConnection(connection);
            }
            return resultStr;
        }

        /// <summary>
        /// 判断是否为管理员
        /// </summary>
        /// <param name="qq">qq号</param>
        /// <returns></returns>
        public static bool isAdmin(string qq)
        {
            bool flag = false;
            string sql = "SELECT qq FROM ichunqiu_admin WHERE qq = @qq";
            var parameter = new List<MySqlParameter>
            {
                new MySqlParameter("@qq", qq)
            };
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                while (connection == null)
                {

                }
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {
                    // 添加参数
                    if (parameter.Count > 0)
                    {
                        mySqlCommand.Parameters.AddRange(parameter.ToArray());
                    }
                    // 提取数据
                    using (MySqlDataReader myDataReader = mySqlCommand.ExecuteReader())
                    {
                        int count = 0;
                        while (myDataReader.Read() == true)
                        {
                            count++;
                        }
                        if (count > 0)
                        {
                            flag = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                // 关闭数据库链接
                ConnectionPool.getPool().closeConnection(connection);
            }
            return flag;
        }

        /// <summary>
        /// 根据标题随机获取课程信息
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string getClassRandomByTitle(string title)
        {
            title = "%" + title + "%";
            var param = new List<MySqlParameter>
            {
                new MySqlParameter("@title", title)
            };
            string sql = "SELECT class_name,class_url FROM ichunqiu_class WHERE class_name LIKE @title ORDER BY rand() LIMIT 0,1";
            return geClass(sql, param);
        }

        /// <summary>
        /// 根据标题获取最新的课程信息
        /// </summary>
        /// <param name="title">标题信息</param>
        /// <returns></returns>
        public static string getClassDateByTitle(string title)
        {
            title = "%" + title + "%";
            var param = new List<MySqlParameter>
            {
                new MySqlParameter("@title", title)
            };
            string sql = "SELECT class_name,class_url FROM ichunqiu_class WHERE class_name LIKE @title ORDER BY create_time DESC LIMIT 0,1";
            return geClass(sql, param);
        }

        /// <summary>
        /// 根据作者随机提取课程信息
        /// </summary>
        /// <param name="author">作者名称</param>
        /// <returns></returns>
        public static string getClassRandomByAuthor(string author)
        {
            author = "%" + author + "%";
            var param = new List<MySqlParameter>
            {
                new MySqlParameter("@author_name", author)
            };
            string sql = "SELECT class_name,class_url FROM ichunqiu_class WHERE author_name LIKE @author_name ORDER BY rand() LIMIT 0,1";
            return geClass(sql, param);
        }

        /// <summary>
        /// 根据作者提取最新的课程信息
        /// </summary>
        /// <param name="author">作者名称</param>
        /// <returns></returns>
        public static string getClassDateByAuthor(string author)
        {
            author = "%" + author + "%";
            // author_name
            var param = new List<MySqlParameter>
            {
                new MySqlParameter("@author_name", author)
            };
            string sql = "SELECT class_name,class_url FROM ichunqiu_class WHERE author_name LIKE @author_name ORDER BY create_time DESC LIMIT 0,1";
            return geClass(sql, param);
        }

        /// <summary>
        /// 通过标题查询文章(随机返回4条)
        /// </summary>
        /// <returns></returns>
        public static string GetContentRandomByTitle(string title)
        {
            title = "%" + title + "%";
            string sql = "SELECT title,url,author FROM ichunqiu_content WHERE content_id IN (SELECT content_id FROM ichunqiu_content WHERE fid = 59 OR (fid = 60) OR (fid = 61) OR (fid = 81 AND type_id = 158) OR (fid = 42 AND type_id = 29)  ) AND title LIKE  @title order by rand() LIMIT 0,4";
            var param = new List<MySqlParameter>
            {
                new MySqlParameter("@title", title)
            };
            return GetContent(sql, param, "\n【文章】");
        }

        /// <summary>
        /// 通过标题查询文章(最新4条)
        /// </summary>
        /// <returns></returns>
        public static string getContentDateByTitle(string title)
        {
            title = "%" + title + "%";
            //string sql = "SELECT title,url,author FROM ichunqiu_content WHERE title LIKE @title  AND fid = 59 OR fid = 60 OR fid = 61 order by content_date DESC LIMIT 0,4";
            string sql = "SELECT title,url,author FROM ichunqiu_content WHERE content_id IN (SELECT content_id FROM ichunqiu_content WHERE fid = 59 OR (fid = 60) OR (fid = 61) OR (fid = 81 AND type_id = 158) OR (fid = 42 AND type_id = 29)  ) AND title LIKE  @title order by content_date DESC LIMIT 0,4";
            var param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@title", title));
            return GetContent(sql, param, "\n【文章】");
        }

        /// <summary>
        /// 通过作者查询文章(最新4条)
        /// </summary>
        /// <returns></returns>
        public static string getContentDateByAuthor(string author)
        {
            author = "%" + author + "%";
            string sql = "SELECT title,url,author FROM ichunqiu_content WHERE author LIKE @author  order by content_date DESC LIMIT 0,4";
            var param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@author", author));
            return GetContent(sql, param, "\n【文章】");
        }

        /// <summary>
        /// 通过作者查询文章(随机4条)
        /// </summary>
        /// <returns></returns>
        public static string getContentRandomByAuthor(string author)
        {
            author = "%" + author + "%";
            string sql = "SELECT title,url,author FROM ichunqiu_content WHERE author LIKE @author  order by  rand() DESC LIMIT 0,4";
            var param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@author", author));
            return GetContent(sql, param, "\n【文章】");
        }

        /// <summary>
        /// 今日最新||最新帖子
        /// </summary>
        /// <returns></returns>
        public static string getContentDateByToday()
        {
            string sql = "SELECT title,url,author FROM ichunqiu_content WHERE fid = 59 OR  fid = 60 OR fid = 61 OR fid = 81 OR fid = 65 OR fid = 42 OR fid = 76 order by content_date DESC LIMIT 0,4";
            var param = new List<MySqlParameter>();
            return GetContent(sql, param, "\n【文章】");
        }

        /// <summary>
        /// 通过标题获取随机教程信息
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns></returns>
        public static string getCourseRandomByTitle(string title)
        {
            title = "%" + title + "%";
            //string sql = "SELECT title,url,author FROM ichunqiu_content WHERE title LIKE @title  AND fid = 81 OR fid = 42 AND type_id = 158 OR type_id = 29 order by rand() LIMIT 0,4";
            string sql = "SELECT title,url,author FROM ichunqiu_content WHERE content_id IN (SELECT content_id FROM ichunqiu_content WHERE fid = 42 OR fid = 65 ) AND title LIKE @title order by rand() LIMIT 0,4";
            var param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@title", title));
            return GetContent(sql, param, "\n【教程】");
        }

        /// <summary>
        /// 通过标题获取最新教程信息
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns></returns>
        public static string getCourseDateByTitle(string title)
        {
            title = "%" + title + "%";
            //string sql = "SELECT title,url,author FROM ichunqiu_content WHERE title LIKE @title  AND fid = 81 OR fid = 42 AND type_id = 158 OR type_id = 29 order by content_date DESC LIMIT 0,4";
            string sql = "SELECT title,url,author FROM ichunqiu_content WHERE content_id IN (SELECT content_id FROM ichunqiu_content WHERE fid = 42 OR fid = 65 ) AND title LIKE @title order by  content_date DESC LIMIT 0,4";
            var param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@title", title));
            return GetContent(sql, param, "\n【教程】");
        }

        /// <summary>
        /// 通过QQ获取作家的余额信息
        /// </summary>
        /// <param name="qq">qq号</param>
        /// <returns>余额</returns>
        public static string getBlankMoney(string qq)
        {
            string sql = "SELECT user_money FROM ichunqiu_blank WHERE user_qq = @qq";
            var parameter = new List<MySqlParameter>();
            string userMoney = "";
            parameter.Add(new MySqlParameter("@qq", qq));
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                while (connection == null)
                {

                }
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {
                    // 添加参数
                    if (parameter.Count > 0)
                    {
                        mySqlCommand.Parameters.AddRange(parameter.ToArray());
                    }
                    // 提取数据
                    using (MySqlDataReader myDataReader = mySqlCommand.ExecuteReader())
                    {
                        while (myDataReader.Read() == true)
                        {
                            userMoney = myDataReader["user_money"].ToString();
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                // 关闭数据库链接
                ConnectionPool.getPool().closeConnection(connection);
            }
            return userMoney;
        }

        /// <summary>
        /// 提现记录
        /// </summary>
        /// <param name="qq">qq号</param>
        /// <returns>提现记录</returns>
        public static string getBlankHistory(string qq)
        {
            string sql = "SELECT money,create_date FROM ichunqiu_blank_history WHERE user_qq = @qq AND operation = '0' ;";
            var parameter = new List<MySqlParameter>();
            string resultStr = "";
            parameter.Add(new MySqlParameter("@qq", qq));
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                while (connection == null)
                {

                }
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {
                    // 添加参数
                    if (parameter.Count > 0)
                    {
                        mySqlCommand.Parameters.AddRange(parameter.ToArray());
                    }
                    // 提取数据
                    using (MySqlDataReader myDataReader = mySqlCommand.ExecuteReader())
                    {
                        while (myDataReader.Read() == true)
                        {
                            resultStr += myDataReader["money"].ToString() + "\t" + myDataReader["create_date"] + "\n";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                // 关闭数据库链接
                ConnectionPool.getPool().closeConnection(connection);
            }
            return resultStr;
        }

        /// <summary>
        /// 金额提现
        /// </summary>
        /// <param name="qq">QQ号</param>
        /// <param name="money">金额</param>
        public static string updateBlank(string qq, decimal money,string operation)
        {
            string resultStr = "";
            string sql = "UPDATE ichunqiu_blank SET user_money = user_money - @user_money WHERE user_qq =@qq ";
            // 加钱
            if ("1".Equals(operation))
            {
                sql = "UPDATE ichunqiu_blank SET user_money = user_money + @user_money WHERE user_qq =@qq ";
            }
            else
            {
                string tmpMoney = getBlankMoney(qq);
                if (tmpMoney != null && tmpMoney.Length > 0)
                {
                    if (Convert.ToDecimal(tmpMoney) < money)
                    {
                        return "提现金额必须小于存款金额。";
                    }
                }
                else
                {
                    return "没有该作家！";
                }
            }
            var parameter = new List<MySqlParameter>();
            parameter.Add(new MySqlParameter("@user_money", money));
            parameter.Add(new MySqlParameter("@qq", qq));
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                while (connection == null)
                {

                }
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {
                    // 添加参数
                    if (parameter.Count > 0)
                    {
                        mySqlCommand.Parameters.AddRange(parameter.ToArray());
                    }
                    // 提取数据
                    int count = mySqlCommand.ExecuteNonQuery();
                    // 执行数据插入
                    int flagCount = inserMoneyHistory(qq, money, operation);
                    if (count > 0 && flagCount > 0)
                    {
                        resultStr = "更新成功!";
                    }
                    else
                    {
                        resultStr = "更新失败！";
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                // 关闭数据库链接
                ConnectionPool.getPool().closeConnection(connection);
            }
            return resultStr;
        }

        /// <summary>
        /// 插入提现记录
        /// </summary>
        /// <returns></returns>
        public static int inserMoneyHistory(string qq, decimal money,string operation)
        {
            string sql = "INSERT INTO ichunqiu_blank_history(id,user_qq,money,operation,create_date,update_date) VALUES(DEFAULT,@qq,@user_money,@operation,NOW(),NOW())";
            var parameter = new List<MySqlParameter>();
            int count = 0;
            parameter.Add(new MySqlParameter("@user_money", money));
            parameter.Add(new MySqlParameter("@qq", qq));
            parameter.Add(new MySqlParameter("@operation", operation));
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                // 链接为null就执行等待
                while (connection == null)
                {

                }
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {
                    // 添加参数
                    if (parameter.Count > 0)
                    {
                        mySqlCommand.Parameters.AddRange(parameter.ToArray());
                    }
                    // 提取数据
                    count = mySqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                count = 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                ConnectionPool.getPool().closeConnection(connection);
            }
            return count;
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="qq"></param>
        /// <returns></returns>
        public static void addUserData(string qq, string content, string msg)
        {
            string sql = "INSERT INTO ichunqiu_user(id,user_qq,content,send_msg,create_date,update_date) VALUES(DEFAULT,@qq,@content,@msg,NOW(),NOW())";
            var parameter = new List<MySqlParameter>();
            parameter.Add(new MySqlParameter("@qq", qq));
            parameter.Add(new MySqlParameter("@content", content));
            parameter.Add(new MySqlParameter("@msg", msg));
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                // 链接为null就执行等待
                while (connection == null)
                {

                }
                using (MySqlCommand cmd = new MySqlCommand(sql,connection))
                {
                    // 添加参数
                    if (parameter.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameter.ToArray());
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                ConnectionPool.getPool().closeConnection(connection);
            }
        }

        /// <summary>
        /// 获取今日使用的数据
        /// </summary>
        /// <returns></returns>
        public static int getToday()
        {
            int count = 0;
            string sql = "select user_qq from ichunqiu_user where to_days(create_date) = to_days(now()) GROUP BY user_qq;";
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                // 链接为null就执行等待
                while (connection == null)
                {

                }
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {

                    using (MySqlDataReader myDataReader = mySqlCommand.ExecuteReader())
                    {
                        while (myDataReader.Read() == true)
                        {
                            count++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                ConnectionPool.getPool().closeConnection(connection);
            }
            return count;

        }

        /// <summary>
        /// 获取使用总数人
        /// </summary>
        /// <returns></returns>
        public static int getUserTotal()
        {
            int count = 0;
            string sql = "SELECT user_qq FROM ichunqiu_user GROUP BY user_qq;";
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                // 链接为null就执行等待
                while (connection == null)
                {

                }
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader myDataReader = mySqlCommand.ExecuteReader())
                    {
                        while (myDataReader.Read() == true)
                        {
                            count++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                ConnectionPool.getPool().closeConnection(connection);
            }
            return count;
        }


        /// <summary>
        /// 获得某一个时间使用的人数
        /// </summary>
        /// <returns></returns>
        public static Hashtable getThisWeek()
        {
            string sql = "select user_qq,DATE_FORMAT(create_date,'%Y-%m-%d') as date from ichunqiu_user where DATE_SUB(CURDATE(), INTERVAL 7 DAY) <= date(create_date) GROUP BY user_qq,DATE_FORMAT(create_date,'%Y-%m-%d');";
            Hashtable hashtable = new Hashtable();
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                // 链接为null就执行等待
                while (connection == null)
                {

                }
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {
                    // 提取数据
                    using (MySqlDataReader myDataReader = mySqlCommand.ExecuteReader())
                    {
                        while (myDataReader.Read() == true)
                        {
                            string date = myDataReader["date"].ToString();
                            if (hashtable.Contains(date))
                            {
                                hashtable[date] = (Convert.ToInt16(hashtable[date]) + 1).ToString();
                            }
                            else
                            {
                                hashtable.Add(date, "1");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                ConnectionPool.getPool().closeConnection(connection);
            }
            return hashtable;
        }

        /// <summary>
        /// 获取财富榜前三十
        /// </summary>
        /// <returns></returns>
        public static string Top30Money()
        {
            // 未提取存款
            Hashtable userMoney = new Hashtable();
            Hashtable userInfo = new Hashtable();
            // 已经提取存款
            Hashtable userBlank = new Hashtable();
            // 财富榜前20
            Hashtable userData = new Hashtable();
            string resultMsg = "";
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                // 链接为null就执行等待
                while (connection == null)
                {

                }
                string sql = "SELECT bbs_nick_name,user_money,user_qq FROM ichunqiu_blank";
                using (MySqlCommand cmd = new MySqlCommand(sql,connection))
                {
                    using (MySqlDataReader myDataReader = cmd.ExecuteReader())
                    {
                        while (myDataReader.Read() == true)
                        {
                            userMoney.Add(myDataReader["bbs_nick_name"], myDataReader["user_money"]);
                            userInfo.Add(myDataReader["user_qq"], myDataReader["bbs_nick_name"]);
                        }
                    }
                }
                // 提取历史提取总金额
                userBlank = getAllHistorySumMoney(userInfo);
                //  ArrayList allMoney = new ArrayList();
                List<Decimal> allMoney = new List<decimal>();
                foreach (DictionaryEntry item in userMoney)
                {
                    decimal blance = Convert.ToDecimal(item.Value);
                    decimal historyBlance = Convert.ToDecimal(userBlank[item.Key]);
                    userData.Add(item.Key, historyBlance + blance);
                    allMoney.Add(historyBlance + blance);
                }
                /*
                List<decimal> allMoney = new List<decimal>();
                foreach (string item in userData.Values)
                {
                    allMoney.Add(Convert.ToDecimal(item));
                }*/
                // 降序
                allMoney.Sort((x, y) => -x.CompareTo(y));
                int i = 0;
                foreach (decimal money in allMoney)
                {
                    foreach (DictionaryEntry item in userData)
                    {
                        if (Convert.ToDecimal(item.Value) == money)
                        {
                            resultMsg += string.Format("NO【{0}】：{1}身价{2},已提取存款{3},未提取存款{4}\n", (i + 1).ToString(), Convert.ToString(item.Key), Convert.ToString(money), Convert.ToString(userBlank[item.Key]), Convert.ToString(userMoney[item.Key]));
                            // 删除用户避免出现重复的情况
                            userData.Remove(item.Key);
                            break;
                        }
                    }
                    i++;
                    if (i > 29)
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally {
                if (connection != null) {
                    connection.Close();
                }
                // 关闭数据库链接
                ConnectionPool.getPool().closeConnection(connection);
            }
            return resultMsg;
        }

        /// <summary>
        /// 获取所有的用户的历史提现总金额
        /// </summary>
        private static Hashtable getAllHistorySumMoney(Hashtable userInfo)
        {
            Hashtable userBlank = new Hashtable();
            MySqlConnection connection = null;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                // 链接为null就执行等待
                while (connection == null)
                {

                }
                foreach (DictionaryEntry item in userInfo)
                {
                    string sql = string.Format("SELECT SUM(ABS(money)) as money FROM ichunqiu_blank_history WHERE user_qq = '{0}' AND operation = '0'", item.Key);
                    using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader myDataReader = mySqlCommand.ExecuteReader())
                        {
                            while (myDataReader.Read() == true)
                            {
                                if (myDataReader["money"] is DBNull)
                                {
                                    userBlank.Add(userInfo[item.Key], 0);
                                }
                                else
                                {
                                    userBlank.Add(userInfo[item.Key], myDataReader["money"]);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally {
                if (connection != null)
                {
                    connection.Close();
                }
                // 关闭数据库链接
                ConnectionPool.getPool().closeConnection(connection);
            }
            return userBlank;
        }

        /// <summary>
        /// 检索用户是否为作家团成员
        /// </summary>
        /// <param name="qq">qq号</param>
        /// <returns></returns>
        public static bool exitsUser(string qq)
        {
            MySqlConnection connection = null;
            bool resultFlag = false;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                // 链接为null就执行等待
                while (connection == null)
                {

                }
                string sql = "SELECT user_money FROM ichunqiu_blank WHERE user_qq = @qq";
                var parameter = new List<MySqlParameter>();
                parameter.Add(new MySqlParameter("@qq", qq));
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    if (parameter.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameter.ToArray());
                    }
                    using (MySqlDataReader myDataReader = cmd.ExecuteReader())
                    {
                        while (myDataReader.Read() == true)
                        {
                            resultFlag = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                // 关闭数据库链接
                ConnectionPool.getPool().closeConnection(connection);
            }
            return resultFlag;
        }
        

        /// <summary>
        /// 是否为超级管理员
        /// </summary>
        /// <param name="qq">检验qq</param>
        /// <returns></returns>
        public static bool isSuperAdmin(string qq) {
            string sql = "SELECT is_admin FROM ichunqiu_admin WHERE qq = @qq ";
            var parameter = new List<MySqlParameter>();
            parameter.Add(new MySqlParameter("@qq", qq));
            MySqlConnection connection = null;
            bool flag = false;
            try
            {
                connection = ConnectionPool.getPool().getConnection();
                while (connection == null)
                {

                }
                using (MySqlCommand cmd = new MySqlCommand(sql,connection))
                {
                    using (MySqlDataReader myReader = cmd.ExecuteReader())
                    {
                        while (myReader.Read() == true)
                        {
                            if ("1".Equals(myReader["is_admin"].ToString()))
                            {
                                flag = true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                // 关闭数据库链接
                ConnectionPool.getPool().closeConnection(connection);
            }
            return flag;
        }
    }
}