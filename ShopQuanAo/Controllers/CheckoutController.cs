using API_NganLuong;
using MoMo;
using Newtonsoft.Json.Linq;
using ShopQuanAo.Models;
using ShopQuanAo.MomoAPI;
using ShopQuanAo.nganluonAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopQuanAo.Controllers
{
    public class CheckoutController : BaseController
    {
        private const string SessionCart = "SessionCart";
        ShopQuanAoDbContext db = new ShopQuanAoDbContext();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult Index()
        {
            var cart = Session[SessionCart];
            var list = new List<Cart_item>();
            if (cart != null)
            {
                list = (List<Cart_item>)cart;
            }
            return View(list);

        }
        [HttpPost]
        public ActionResult Index(Morder order)
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            int numIterations = 0;
            numIterations = rand.Next(1, 100000);
            DateTime time = DateTime.Now;

            string orderCode = numIterations + "" + time;
            string sumOrder = Request["sumOrder"];
            string payment_method = Request["option_payment"];
            // Neu Ship COde
            if (payment_method.Equals("COD")) {
                // cap nhat thong tin sau khi dat hang thanh cong

                saveOrder(order,"COD",2, orderCode);
                var cart = Session[SessionCart];
                var list = new List<Cart_item>();
                ViewBag.cart = (List<Cart_item>)cart;
                Session["SessionCart"] = null;
                var listProductOrder = db.Orderdetails.Where(m => m.orderid == order.ID);
                return View("payment");
            }
            //Neu Thanh toan MOMO
            else if (payment_method.Equals("MOMO")) {
                //request params need to request to MoMo system
                string endpoint = momoInfo.endpoint;
                string partnerCode = momoInfo.partnerCode;
                string accessKey = momoInfo.accessKey;
                string serectkey = momoInfo.serectkey;
                string orderInfo = momoInfo.orderInfo;
                string returnUrl = momoInfo.returnUrl;
                string notifyurl = momoInfo.notifyurl;

                string amount = sumOrder;
                string orderid = Guid.NewGuid().ToString();
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";

                //Before sign HMAC SHA256 signature
                string rawHash = "partnerCode=" +
                    partnerCode + "&accessKey=" +
                    accessKey + "&requestId=" +
                    requestId + "&amount=" +
                    amount + "&orderId=" +
                    orderid + "&orderInfo=" +
                    orderInfo + "&returnUrl=" +
                    returnUrl + "&notifyUrl=" +
                    notifyurl + "&extraData=" +
                    extraData;

                log.Debug("rawHash = " + rawHash);

                MoMoSecurity crypto = new MoMoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);
                log.Debug("Signature = " + signature);

                //build body json request
                JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };
                log.Debug("Json request to MoMo: " + message.ToString());
                string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
                JObject jmessage = JObject.Parse(responseFromMomo);

                saveOrder(order, "Cổng thanh toán MOMO", 2, orderid);
                return Redirect(jmessage.GetValue("payUrl").ToString());        
            }
            //Neu Thanh toan Ngan Luong
            else if (payment_method.Equals("NL"))
            {
                string str_bankcode = Request["bankcode"];
                RequestInfo info = new RequestInfo();
                info.Merchant_id = nganluongInfo.Merchant_id;
                info.Merchant_password = nganluongInfo.Merchant_password;
                info.Receiver_email = nganluongInfo.Receiver_email;
                info.cur_code = "vnd";
                info.bank_code = str_bankcode;
                info.Order_code = orderCode;
                info.Total_amount = sumOrder;
                info.fee_shipping = "0";
                info.Discount_amount = "0";
                info.order_description = "Thanh toán ngân lượng cho đơn hàng";
                info.return_url = nganluongInfo.return_url;
                info.cancel_url = nganluongInfo.cancel_url;
                info.Buyer_fullname = order.deliveryname;
                info.Buyer_email = order.deliveryemail;
                info.Buyer_mobile = order.deliveryphone;
                APICheckoutV3 objNLChecout = new APICheckoutV3();
                ResponseInfo result = objNLChecout.GetUrlCheckout(info, payment_method);
                // neu khong gap loi gi
                if (result.Error_code == "00")
                {
                    saveOrder(order, "Cổng thanh toán Ngân Lượng", 2, orderCode);
                    // chuyen sang trang ngan luong
                    return Redirect(result.Checkout_url);
                }
                else
                {
                    ViewBag.errorPaymentOnline = result.Description;
                    return View("payment");
                }
               
            }
            //Neu Thanh Toán ATM online
            else if (payment_method.Equals("ATM_ONLINE"))
            {
                string str_bankcode = Request["bankcode"];
                RequestInfo info = new RequestInfo();
                info.Merchant_id = nganluongInfo.Merchant_id;
                info.Merchant_password = nganluongInfo.Merchant_password;
                info.Receiver_email = nganluongInfo.Receiver_email;
                info.cur_code = "vnd";
                info.bank_code = str_bankcode;
                info.Order_code = orderCode;
                info.Total_amount = sumOrder;
                info.fee_shipping = "0";
                info.Discount_amount = "0";
                info.order_description = "Thanh toán ngân lượng cho đơn hàng";
                info.return_url = nganluongInfo.return_url;
                info.cancel_url = nganluongInfo.cancel_url;
                info.Buyer_fullname = order.deliveryname;
                info.Buyer_email = order.deliveryemail;
                info.Buyer_mobile = order.deliveryphone;
                APICheckoutV3 objNLChecout = new APICheckoutV3();
                ResponseInfo result = objNLChecout.GetUrlCheckout(info, payment_method);
                // neu khong gap loi gi
                if (result.Error_code == "00")
                {
                    saveOrder(order, "ATM Online qua ngân lượng", 2, orderCode);
                    return Redirect(result.Checkout_url);
                }
                else
                {
                    ViewBag.errorPaymentOnline = result.Description;
                    return View("payment");
                }
            }
            return View("payment");
        }
        //Khi huy thanh toán Ngan Luong
        public ActionResult cancel_order(){

            return View("cancel_order");
        }
        //Khi thanh toán Ngan Luong XOng
        public ActionResult confirm_orderPaymentOnline() {

            String Token = Request["token"];
            RequestCheckOrder info = new RequestCheckOrder();
            info.Merchant_id = nganluongInfo.Merchant_id;
            info.Merchant_password = nganluongInfo.Merchant_password;
            info.Token = Token;
            APICheckoutV3 objNLChecout = new APICheckoutV3();
            ResponseCheckOrder result = objNLChecout.GetTransactionDetail(info);
            if (result.errorCode=="00")
            {

                var cart = Session[SessionCart];
                var list = new List<Cart_item>();
                ViewBag.cart = (List<Cart_item>)cart;
                Session["SessionCart"] = null;
                var OrderInfo = db.Orders.OrderByDescending(m=>m.ID).FirstOrDefault();
                ViewBag.name = OrderInfo.deliveryname;
                ViewBag.email = OrderInfo.deliveryemail;
                ViewBag.address = OrderInfo.deliveryaddress;
                ViewBag.code = OrderInfo.code;
                ViewBag.phone = OrderInfo.deliveryphone;
                OrderInfo.StatusPayment = 1;
                db.Entry(OrderInfo).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.paymentStatus = OrderInfo.StatusPayment;
                ViewBag.Methodpayment = OrderInfo.deliveryPaymentMethod;
                return View("payment");
            }
            else
            {
                 ViewBag.status = false;
            }

            return View("confirm_orderPaymentOnline");
        }

        //Khi huy thanh toán MOMO
        public ActionResult cancel_order_momo()
        {

            return View("cancel_order");
        }
        //Khi Thanh toám momo xong
        public ActionResult confirm_orderPaymentOnline_momo()
        {

            String errorCode = Request["errorCode"];
            String orderId = Request["orderId"];
            if (errorCode == "0")
            {
                var cart = Session[SessionCart];
                var list = new List<Cart_item>();
                ViewBag.cart = (List<Cart_item>)cart;
                Session["SessionCart"] = null;
                var OrderInfo = db.Orders.Where(m => m.code == orderId).FirstOrDefault();

                ViewBag.name = OrderInfo.deliveryname;
                ViewBag.email = OrderInfo.deliveryemail;
                ViewBag.address = OrderInfo.deliveryaddress;
                ViewBag.code = OrderInfo.code;
                ViewBag.phone = OrderInfo.deliveryphone;
                OrderInfo.StatusPayment = 1;
                db.Entry(OrderInfo).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.paymentStatus = OrderInfo.StatusPayment;
                ViewBag.Methodpayment = OrderInfo.deliveryPaymentMethod;
                return View("payment");
            }
            else
            {
                ViewBag.status = false;
            }

            return View("confirm_orderPaymentOnline");
        }
        //function ssave order when order success!
        public void saveOrder(Morder order,string paymentMethod,int StatusPayment,string ordercode)
        {
            var cart = Session[SessionCart];
            var list = new List<Cart_item>();
            if (cart != null)
            {
                list = (List<Cart_item>)cart;
            }
           
            if (ModelState.IsValid)
            {

                order.code = ordercode;
                order.userid = 1;
                order.deliveryPaymentMethod = paymentMethod;
                order.StatusPayment = StatusPayment;
                order.created_ate = DateTime.Now;
                order.updated_by = 1;
                order.updated_at = DateTime.Now;
                order.updated_by = 1;
                order.status = 2;
                order.exportdate = DateTime.Now;
                db.Orders.Add(order);
                db.SaveChanges();
                ViewBag.name = order.deliveryname;
                ViewBag.email = order.deliveryemail;
                ViewBag.address = order.deliveryaddress;
                ViewBag.code = order.code;
                ViewBag.phone = order.deliveryphone;
                Mordersdetail orderdetail = new Mordersdetail();

                foreach (var item in list)
                {
                    float price = 0;
                    int sale = (int)item.product.pricesale;
                    if (sale > 0)
                    {
                        price = (float)item.product.price - (int)item.product.price / 100 * (int)sale * item.quantity;
                    }
                    else
                    {
                        price = (float)item.product.price * (int)item.quantity;
                    }
                    orderdetail.orderid = order.ID;
                    orderdetail.productid = item.product.ID;
                    orderdetail.priceSale = (int)item.product.pricesale;
                    orderdetail.price = item.product.price;
                    orderdetail.quantity = item.quantity;
                    orderdetail.amount = price;

                    db.Orderdetails.Add(orderdetail);
                    db.SaveChanges();
                    //ViewBag.sump = list.Sum((Func<Cart_item, int>)(m => (int)m.product.price * (int) m.quantity));
                    // change number product         
                    var updatedProduct = db.Products.Find(item.product.ID);
                    //updatedProduct.catid = item.product.catid;
                    //updatedProduct.Submenu = item.product.Submenu;
                    //updatedProduct.name = item.product.name;
                    //updatedProduct.slug = item.product.slug;
                    //updatedProduct.img = item.product.img;
                    //updatedProduct.detail = item.product.detail;
                    updatedProduct.number = (int)updatedProduct.number - (int)item.quantity;
                    //updatedProduct.pricesale = item.product.pricesale;
                    //updatedProduct.price = item.product.price;
                    //updatedProduct.metakey = item.product.metakey;
                    //updatedProduct.metadesc = item.product.metadesc;
                    //updatedProduct.created_by = item.product.created_by;
                    //updatedProduct.created_at = item.product.created_at;
                    //updatedProduct.updated_by = item.product.updated_by;
                    //updatedProduct.updated_at = item.product.updated_at;
                    //updatedProduct.status = item.product.status;
                    db.Products.Attach(updatedProduct);
                    db.Entry(updatedProduct).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
            }
        }
        //
    }
}