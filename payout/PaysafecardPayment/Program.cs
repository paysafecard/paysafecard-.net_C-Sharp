using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PaysafecardPayment
{
    class Program
    {
        static void Main(string[] args)
        {
            PaysafecardPaymentClass payment = new PaysafecardPaymentClass("psc_xl0EwfLX-96bEkjy-mXYD7SFviyvaqA", PaysafecardPaymentClass.Environment.TEST);

            JsonObject customer = new JsonObject();
            customer.Add("id", "asdfhgsdjfgsjdfghj");
            customer.Add("ip", "84.200.56.234");

            String currency = "EUR";
            double amount = 0.01;

            //Redirect URLs
            String success_url = "http://success.url/{payment_id}";
            String failture_url = "http://failture.url/{payment_id}";
            String notify_url = "http://notify.url/{payment_id}";

            String correlation_id = "";
            String country_restriction = "";
            String kyc_restriction = "";
            int min_age = 0;
            int shop_id = 1;
            String submerchant_id = "";
            Dictionary<String, Object> response;

            try
            {
                response = payment.createPayment(amount, currency, customer, success_url, failture_url, notify_url, correlation_id, country_restriction, kyc_restriction, min_age, shop_id, submerchant_id);

                //check if Request was OK
                if (payment.requestIsOk())
                {
                    Dictionary<String, Object> redirect = (Dictionary<String, Object>)response[ "redirect"];
                    String auth = (String)redirect["auth_url"];
                    String id = (String)response["id"];


                    // Open the Auth URL in the Browser

                    System.Diagnostics.Process.Start(auth);

                    // Asking the Client if the Payment is finished in the Browser.

                    Console.WriteLine("Payment finished?");
                    Console.ReadLine();

                    Dictionary<String, Object> responsePayment = payment.retrievePayment(id);
                    String status = (String)responsePayment["status"];

                    // Check status. 
                    if (status.Equals("AUTHORIZED"))
                    {
                        Dictionary<String, Object> responseCapture = payment.capturePayment(id);
                        status = (String)responseCapture["status"];
                        // If capturing is OK. Show success message.
                        if (status.Equals("SUCCESS"))
                        {
                            /*
                             * --------------------------------------
                             * 				Payment is Successfull
                             * --------------------------------------
                             */
                            Console.WriteLine("Payment SUCCESS");
                        }
                    }
                    else
                    {
                        // If capturing is not OK. Show error message.
                        if (status.Equals("CANCELED_CUSTOMER"))
                        {
                        
                        }
                        else
                        {
                            Console.WriteLine((String)payment.getError()["message"]);
                        }
                    }
                }
                else
                {
                    Console.WriteLine((String)payment.getError()["message"]);
                }
            }
            catch
            {
                Dictionary<String, String> error = payment.getError();
                Console.WriteLine(error["message"]);
            }
            Console.ReadLine();
        }  
    }
}
