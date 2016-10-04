using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PaysafecardRefund
{
    class PaysafecardRefundClass
    {
        public enum Environment { TEST, PRODUCTION };
        public enum Method { GET, POST };

        private String psc_key = "";
        private String api_url = "";
        private Response response;
        private int response_code = 0;
        private String request = "";
        private RestClient client = null;
        public IRestResponse<Response> rest_response = null;

        public PaysafecardRefundClass(String psc_key, Environment environment)
        {
            this.psc_key = psc_key;
            if (environment == Environment.TEST)
            {
                this.api_url = "https://apitest.paysafecard.com/v1/payments/"; ;
            }
            else if (environment == Environment.PRODUCTION)
            {
                this.api_url = "https://api.paysafecard.com/v1/payments/";
            }
            else
            {
                Console.WriteLine("Environment not supported");
            }
        }

        public Dictionary<String, Object> validateRefund(String payment_id, double amount, String currency, String merchantclientid, String customer_mail, String customer_ip, String correlation_id, String submerchant_id)
        {

            Dictionary<String, String> headers = new Dictionary<String, String>();

            if (!correlation_id.Equals(""))
            {
                headers.Add("Correlation-ID", correlation_id);
            }

            JsonObject customer = new JsonObject();
            customer.Add("id", merchantclientid);
            customer.Add("email", customer_mail);
            customer.Add("first_name", "TEST");
            customer.Add("last_name", "TEST");
            customer.Add("ip", customer_ip);


            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("amount", amount);
            jsonObject.Add("currency", currency);
            jsonObject.Add("type", "PAYSAFECARD");
            jsonObject.Add("customer", customer);
            jsonObject.Add("capture", "false");

            if (!submerchant_id.Equals(""))
            {
                jsonObject.Add("submerchant_id", submerchant_id);
            }

           this.response = this.doRequest(payment_id + "/refunds", jsonObject.ToString(), Method.POST, headers);

            Dictionary<String, Object> response = new Dictionary<String, Object>();

            // Convert Result in Dictionary
            response.Add("id", this.response.id);
            response.Add("created", this.response.created);
            response.Add("updated", this.response.updated);
            response.Add("amount", this.response.amount);
            response.Add("currency", this.response.created);
            response.Add("status", this.response.status);
            response.Add("type", this.response.type);
            Dictionary<String, Object> cust = new Dictionary<String, Object>();
            cust.Add("id", this.response.customer.id);
            cust.Add("email", this.response.customer.email);
            response.Add("customer", cust);

            return response;
        }

        public Dictionary<String, Object> executeRefund(String payment_id, String refund_id, double amount, String currency, String merchantclientid, String customer_mail, String customer_ip, String correlation_id, String submerchant_id)
        {

            Dictionary<String, String> headers = new Dictionary<String, String>();

            if (!correlation_id.Equals(""))
            {
                headers.Add("Correlation-ID", correlation_id);
            }

            JsonObject customer = new JsonObject();
            customer.Add("id", merchantclientid);
            customer.Add("email", customer_mail);
            customer.Add("first_name", "TEST");
            customer.Add("last_name", "TEST");
            customer.Add("ip", customer_ip);


            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("amount", amount);
            jsonObject.Add("currency", currency);
            jsonObject.Add("type", "PAYSAFECARD");
            jsonObject.Add("customer", customer);
            jsonObject.Add("capture", "false");

            if (!submerchant_id.Equals(""))
            {
                jsonObject.Add("submerchant_id", submerchant_id);
            }

            this.response = this.doRequest(payment_id + "/refunds/" + refund_id + "/capture", jsonObject.ToString(), Method.POST, headers);

            Dictionary<String, Object> response = new Dictionary<String, Object>();
            // Convert Result in Dictionary
            response.Add("id", this.response.id);
            response.Add("created", this.response.created);
            response.Add("updated", this.response.updated);
            response.Add("amount", this.response.amount);
            response.Add("currency", this.response.created);
            response.Add("status", this.response.status);
            response.Add("type", this.response.type);
            Dictionary<String, Object> cust = new Dictionary<String, Object>();
            cust.Add("id", this.response.customer.id);
            cust.Add("email", this.response.customer.email);
            response.Add("customer", cust);

            return response;
        }

        //Make direct Refund without validation
        public Dictionary<String, Object> directRefund(String payment_id, double amount, String currency, String merchantclientid, String customer_mail, String customer_ip, String correlation_id, String submerchant_id)
        {
            Dictionary<String, String> headers = new Dictionary<String, String>();

            if (!correlation_id.Equals(""))
            {
                headers.Add("Correlation-ID", correlation_id);
            }

            JsonObject customer = new JsonObject();
            customer.Add("id", merchantclientid);
            customer.Add("email", customer_mail);
            customer.Add("first_name", "TEST");
            customer.Add("last_name", "TEST");
            customer.Add("ip", customer_ip);


            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("amount", amount);
            jsonObject.Add("currency", currency);
            jsonObject.Add("type", "PAYSAFECARD");
            jsonObject.Add("customer", customer);
            jsonObject.Add("capture", "true");

            if (!submerchant_id.Equals(""))
            {
                jsonObject.Add("submerchant_id", submerchant_id);
            }

            this.response = this.doRequest(payment_id + "/refunds", jsonObject.ToString(), Method.POST, headers);
            Dictionary<String, Object> response = new Dictionary<String, Object>();

            // Convert Result in Dictionary
            response.Add("id", this.response.id);
            response.Add("created", this.response.created);
            response.Add("updated", this.response.updated);
            response.Add("amount", this.response.amount);
            response.Add("currency", this.response.created);
            response.Add("status", this.response.status);
            response.Add("type", this.response.type);
            Dictionary<String, Object> cust = new Dictionary<String, Object>();
            cust.Add("id", this.response.customer.id);
            cust.Add("email", this.response.customer.email);
            response.Add("customer", cust);

            return response;
        }
        /**
        * Requests
        */

        private Response doRequest(String url_param, String parameters, Method method, Dictionary<String, String> headers)
        {

            Response response = null;

            if (method == Method.GET)
            {

                response = sendGet(url_param, parameters, headers); // empty path, default URL request, 
            }
            else if (method == Method.POST)
            {
                response = sendPost(url_param, parameters, headers);
            }

            return response;
        }

        private Response sendGet(String urlparam, String parameters, Dictionary<String, String> headers)
        {
            try
            {

                String USER_AGENT = "Mozilla/5.0";
                var client = new RestClient(this.api_url);
                var request = new RestRequest(urlparam, RestSharp.Method.GET);
                //Prepare HTTP Header
                request.AddHeader("http.keepAlive", "false");
                request.AddHeader("Authorization", "Basic cHNjX3hsMEV3ZkxYLTk2YkVranktbVhZRDdTRnZpeXZhcUE=");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("User-Agent", USER_AGENT);
                request.AddHeader("Accept", "*/*");
                request.AddHeader("charset", "utf-8");
                foreach (KeyValuePair<string, string> kvp in headers)
                {
                    request.AddHeader(kvp.Key, kvp.Value);
                }

                this.rest_response = client.Execute<Response>(request);
                HttpStatusCode statusCode = this.rest_response.StatusCode;
                this.response_code = (int)statusCode;

                if (this.requestIsOk())
                {
                    return this.rest_response.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // HTTP POST request
        private Response sendPost(String url_param, String parameters, Dictionary<String, String> headers)
        {

            try
            {
                String USER_AGENT = "Mozilla/5.0";

                this.client = new RestClient(this.api_url);

                var request = new RestRequest(url_param, RestSharp.Method.POST);

                request.AddHeader("http.keepAlive", "false");
                request.AddHeader("Authorization", "Basic cHNjX3hsMEV3ZkxYLTk2YkVranktbVhZRDdTRnZpeXZhcUE=");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("User-Agent", USER_AGENT);
                request.AddHeader("Accept", "*/*");
                request.AddHeader("charset", "utf-8");
                request.AddParameter("application/json; charset=utf-8", parameters, ParameterType.RequestBody);

                foreach (KeyValuePair<string, string> kvp in headers)
                {
                    request.AddHeader(kvp.Key, kvp.Value);
                }

                rest_response = client.Execute<Response>(request);
                HttpStatusCode statusCode = rest_response.StatusCode;
                this.response_code = (int)statusCode;

                if (this.requestIsOk())
                {
                    return this.rest_response.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Dictionary<String, String> getError()
        {
            Dictionary<String, String> err = new Dictionary<String, String>();
            int error_number = JsonConvert.DeserializeObject<Error>(this.rest_response.Content).number;

            if (this.response_code > 200)
            {
                if (error_number < 1)
                {
                    switch (this.response_code)
                    {
                        case 400:
                            err.Add("number", "HTTP:400");
                            err.Add("message", "Logical error. Please check logs.");
                            break;
                        case 403:
                            err.Add("number", "HTTP:403");
                            err.Add("message", "IP not whitelisted!");
                            break;
                        case 500:
                            err.Add("number", "HTTP:500");
                            err.Add("message", "Server error. Please check logs.");
                            break;
                    }
                }

                switch (error_number)
                {
                    case 3162:
                        err.Add("message", "Unfortunately, no my paysafecard account exists under the e-mail address you have entered. Please check the address for a typing error. If you do not have a my paysafecard account, you can register for one online now for free.");
                        break;
                    case 3195:
                        err.Add("message", "The personal details associated with your my paysafecard account do not match the details of this account. Please check the first names, surnames and dates of birth entered in both accounts and request the payout again.");
                        break;
                    case 3167:
                    case 3170:
                    case 3194:
                    case 3168:
                    case 3230:
                    case 3231:
                    case 3232:
                    case 3233:
                    case 3234:
                        err.Add("message", "Unfortunately, the payout could not be completed due to a problem which has arisen with your my paysafecard account. paysafecard has already sent you an e-mail with further information on this. Please follow the instructions found in this e-mail before requesting the payout again.");
                        break;
                    case 3197:
                    case 3198:
                        err.Add("message", "Unfortunately, the payout could not be completed due to a problem which has arisen with your my paysafecard account. Please contact the paysafecard support team. info@paysafecard.com");
                        break;
                    case 10008:
                        err.Add("message", "Invalid API Key");
                        break;
                    default:
                        err.Add("message", "Unfortunately there has been a technical problem and your payout request could not be executed. If the problem persists, please contact our customer support: support@company.com");
                        break;
                }
            }
            return err;
        }

        public bool requestIsOk()
        {
            if (this.response_code < 300)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    public class Response
    {
        public virtual string id { get; set; }
        public virtual Int64 created { get; set; }
        public virtual Int64 updated { get; set; }
        public virtual double amount { get; set; }
        public virtual Customer customer { get; set; }
        public virtual string currency { get; set; }
        public virtual string status { get; set; }
        public virtual string type { get; set; }

    }

    public class Customer
    {
        public virtual string id { get; set; }
        public virtual string email { get; set; }
    }

    public class Error
    {
        public virtual int number { get; set; }
        public virtual string message { get; set; }
        public virtual string param { get; set; }
        public virtual string code { get; set; }
    }
}
