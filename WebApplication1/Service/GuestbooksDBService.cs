using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class GuestbooksDBService
    {
        MyGuestbookEntities db = new MyGuestbookEntities();

        public List<Guestbooks> GetDataList()
        {
            return db.Guestbooks.ToList();
        }

        #region 新增資料
        //新增資料方法
        public void InsertGuestbooks(Guestbooks newData)
        {
            //設定新增時間為現在
            newData.CreateTime = DateTime.Now;
            //將資料加入資料庫實體
            db.Guestbooks.Add(newData);
            //儲存資料庫變更
            db.SaveChanges();

        }
        #endregion

        #region 查詢一筆資料
        //藉由標號取得單筆資料的方法
        public Guestbooks GetDataById(int Id)
        {
            //回傳根據標號所取得的資料
            return db.Guestbooks.Find(Id);
        }
        #endregion

        #region 修改留言
        //修改留言方法
        public void UpdateGuestbooks(Guestbooks UpdateData)
        {
            //取得要修改的資料
            Guestbooks OldData = db.Guestbooks.Find(UpdateData.Id);
            //修改資料庫裡的值
            OldData.Name = UpdateData.Name;
            OldData.Content = UpdateData.Content;
            //儲存資料庫變更
            db.SaveChanges();
        }
        #endregion

        #region 回覆留言
        //回覆留言方法
        public void ReplyGuestbooks(Guestbooks ReplyData)
        {
            //取得要修改的資料
            Guestbooks OldData = db.Guestbooks.Find(ReplyData.Id);
            //設定回覆內容
            OldData.Reply = ReplyData.Reply;
            //設定回覆時間為現在
            OldData.ReplyTime = DateTime.Now;
            //儲存資料庫變更
            db.SaveChanges();
        }
        #endregion

        #region 檢查相關
        //修改資料判斷的方法
        public bool CheckUpdate(int Id)
        {
            //根據Id取得要修改的資料
            Guestbooks Data = db.Guestbooks.Find(Id);
            //判斷並回傳(判斷是否有資料以及是否有回覆)
            return (Data != null && Data.ReplyTime == null);
        }
        #endregion

        #region 刪除資料
        //刪除資料方法
        public void DeleteGuestbooks(int Id)
        {
            //根據Id取得要刪除的資料
            Guestbooks DeleteData = db.Guestbooks.Find(Id);
            //從資料庫實體中刪除資料
            db.Guestbooks.Remove(DeleteData);
            //儲存資料庫變更
            db.SaveChanges();
        }
        #endregion

        #region 查詢陣列資料
        //根據搜尋來取得資料陣列的方法
        public List<Guestbooks> GetDataList(string Search)
        {
            //宣告要接受全部搜尋資料的物件
            IQueryable<Guestbooks> SearchData;
            //判斷搜尋是否為空或Null，用於決定要呼叫取得搜尋資料
            if (String.IsNullOrEmpty(Search))
            {
                SearchData = db.Guestbooks;
            }
            else
            {
                SearchData = db.Guestbooks.Where(p => p.Name.Contains(Search)
|| p.Content.Contains(Search) || p.Reply.Contains(Search));
            }
            //先排序再根據分頁來回傳所需部分的資料陣列
            return SearchData.ToList();
        }
        #endregion
        //根據分頁以及搜尋來取得資料陣列的方法
        public List<Guestbooks> GetDataList(ForPaging Paging, string Search)
        {
            //宣告要接受全部搜尋資料的物件
            IQueryable<Guestbooks> SearchData;
            //判斷搜尋是否為空或Null，用於決定要呼叫取得搜尋資料
            if (String.IsNullOrEmpty(Search))
            {
                SearchData = GetAllDataList(Paging);
            }
            else
            {
                SearchData = GetAllDataList(Paging, Search);
            }
            //先排序再根據分頁來回傳所需部分的資料陣列
            return SearchData.OrderByDescending(p => p.Id)
                .Skip((Paging.NowPage - 1) *
                    Paging.ItemNum).Take(Paging.ItemNum).ToList();
        }
        //無搜尋值的搜尋資料方法
        public IQueryable<Guestbooks> GetAllDataList(ForPaging Paging)
        {
            //宣告要回傳的搜尋資料為資料庫中的Guestbooks資料表
            IQueryable<Guestbooks> Data = db.Guestbooks;
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Data.Count()) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
            //回傳搜尋資料
            return Data;
        }

        //包含搜尋值的搜尋資料方法
        public IQueryable<Guestbooks> GetAllDataList(ForPaging Paging, string Search)
        {
            //根據搜尋值來搜尋資料
            IQueryable<Guestbooks> Data = db.Guestbooks
                .Where(p => p.Name.Contains(Search) || p.Content.Contains(Search) || p.Reply.Contains(Search));
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Data.Count()) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
            //回傳搜尋資料
            return Data;
        }
    }

}