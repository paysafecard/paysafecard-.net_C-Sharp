using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaysafecardRefund
{
    class Program
    {
        static void Main(string[] args)
        {
            PaysafecardRefundClass pscrefund = new PaysafecardRefundClass("psc_xl0EwfLX-96bEkjy-mXYD7SFviyvaqA", PaysafecardRefundClass.Environment.TEST);

            String payment_id = "pay_1000005843_testCorrID_5780325650790_EUR";
            double amount = 0.01;
            String currency = "EUR";
            String merchant_client_id = "cc03e747a6afbbcbf8be7668acfebee5";
            String client_mail = "psc.mypins+matwal_blFxgFUJfbNS@gmail.com";
            String client_ip = "127.0.0.1";

            String correlation_id = "";
            String submerchant_id = "";

            Dictionary<String, Object> responseRefund = pscrefund.validateRefund(payment_id, amount, currency, merchant_client_id, client_mail, client_ip, correlation_id, submerchant_id);
            String refund_id = (String)responseRefund["id"];
            String status = (String)responseRefund["status"];

            if (pscrefund.requestIsOk())
            {
                if (status.Equals("VALIDATION_SUCCESSFUL"))
                {
                    Dictionary<String, Object> responseRefundExecute = pscrefund.executeRefund(payment_id, refund_id, amount, currency, merchant_client_id, client_mail, client_ip, correlation_id, submerchant_id);
                    status = (String)responseRefundExecute["status"];
                    if (status.Equals("SUCCESS"))
                    {
                        /*
                         * --------------------------------------
                         * 				Refund is successful
                         * --------------------------------------
                         */
                        Console.WriteLine("Refund SUCCESS");
                    }
                    else
                    {
                        Dictionary<String, String> error = pscrefund.getError();
                        Console.WriteLine(error["message"]);
                    }
                }
                else
                {
                    Dictionary<String, String> error = pscrefund.getError();
                    Console.WriteLine(error["message"]);
                }
            }
            else
            {
                Dictionary<String, String> error = pscrefund.getError();
                Console.WriteLine(error["message"]);
            }
            Console.ReadLine();
        }
    }
}
