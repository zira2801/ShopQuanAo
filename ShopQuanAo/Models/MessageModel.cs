using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopQuanAo
{
    public class MessageModel
    {
        public String msg { get; set; }
        public String msg_css { get; set; }
    }
}

//var listproduct = db.Products.Where(m => m.status == 1).ToList();
//            foreach (var item in listproduct)
//            {
//                link link = new link();
//link.parentId = item.ID;
//                link.slug = item.slug;
//                link.type = "ProductDetail";
//                link.tableId = 1;
//                db.Link.Add(link);
//                db.SaveChanges();

//            }
//            var listproduct1 = db.Categorys.Where(m => m.status == 1).ToList();
//            foreach (var item in listproduct1)
//            {
//                link link = new link();
//link.parentId = item.ID;
//                link.slug = item.slug;
//                link.type = "category";
//                link.tableId = 2;
//                db.Link.Add(link);
//                db.SaveChanges();
//            }
//            var listproduct2 = db.topics.Where(m => m.status == 1).ToList();
//            foreach (var item in listproduct1)
//            {
//                link link = new link();
//link.parentId = item.ID;
//                link.slug = item.slug;
//                link.type = "topic";
//                link.tableId = 3;
//                db.Link.Add(link);
//                db.SaveChanges();
//            }
//            var listproduct3 = db.posts.Where(m => m.status == 1).ToList();
//            foreach (var item in listproduct2)
//            {
//                link link = new link();
//link.parentId = item.ID;
//                link.slug = item.slug;
//                link.type = "PostDetail";
//                link.tableId = 4;
//                db.Link.Add(link);
//                db.SaveChanges();
//            }

