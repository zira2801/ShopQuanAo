using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopQuanAo.MomoAPI
{
    public class momoInfo
    {
        public static readonly string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
        public static readonly string partnerCode = "MOMOP6JQ20201004";
        public static readonly string accessKey = "IkOjzm9u3ibIdAHn";
        public static readonly string serectkey = "aLlILxnIGc1KlKDjDPqt9d4KwGt0KraY";
        public static readonly string orderInfo = " Test Thanh toán momo";
        public static readonly string returnUrl = "http://localhost:22222/confirm-orderPaymentOnline-momo";
        public static readonly string notifyurl = "http://localhost:22222/cancel-order-momo";
    }
}