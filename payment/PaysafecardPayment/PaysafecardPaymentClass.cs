using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PaysafecardPayment
{
    class PaysafecardPaymentClass
    {
        public enum Environment { TEST, PRODUCTION };
        public enum Method {GET, POST};

        private String psc_key = "";
        private String api_url = "";
        private Response response;
        private int response_code = 0;
        private String request = "";
        private RestClient client = null;
        public IRestResponse<Response> rest_response = null;

        public PaysafecardPaymentClass(String psc_key, Environment environment)
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

        public Dictionary<String, Object> createPayment(double amount, String currency, JsonObject customer, String success_url, String failure_url, String notification_url, String correlation_id, String country_restriction, String kyc_restriction, int min_age, int shop_id, String submerchant_id)
        {
            Dictionary<String, String> headers = new Dictionary<String, String>();
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();

            if (!String.ReferenceEquals(correlation_id,""))
            {
                headers.Add("Correlation-ID", correlation_id);
            }

            JsonObject redirect = new JsonObject();
            redirect.Add("success_url", success_url);
            redirect.Add("failure_url", failure_url);

            JsonObject jsonObject = new JsonObject();

            jsonObject.Add("currency", currency);
            jsonObject.Add("amount", amount);
            jsonObject.Add("customer", customer);
            jsonObject.Add("type", "PAYSAFECARD");
            jsonObject.Add("redirect", redirect);
            jsonObject.Add("notification_url", notification_url);
            jsonObject.Add("shop_id", shop_id);

            if (!String.IsNullOrEmpty(country_restriction))
            {
                jsonObject.Add("country_restriction", country_restriction);
            }

            if (!String.IsNullOrEmpty(kyc_restriction))
            {
                jsonObject.Add("kyc_level", kyc_restriction);
            }

            if (min_age != 0)
            {
                jsonObject.Add("min_age", min_age);
            }

            if (!String.IsNullOrEmpty(submerchant_id))
            {
                jsonObject.Add("submerchant_id", submerchant_id);
            }

            String json = jsonObject.ToString();
            this.request = json;

            try{
                this.response = this.doRequest("", this.request, Method.POST, headers);

                Dictionary<String, Object> result = new Dictionary<String, Object>();

                // Convert Result in Dictionary
                result.Add("id", this.response.id);
                result.Add("created", this.response.created);
                result.Add("updated", this.response.updated);
                result.Add("amount", this.response.amount);
                result.Add("currency", this.response.created);
                result.Add("status", this.response.status);
                result.Add("type", this.response.type);
                Dictionary<String, Object> red = new Dictionary<String, Object>();
                red.Add("success_url", this.response.redirect.success_url);
                red.Add("failure_url", this.response.redirect.failure_url);
                red.Add("auth_url", this.response.redirect.auth_url);
                result.Add("redirect", red);
                Dictionary<String, Object> cust = new Dictionary<String, Object>();
                cust.Add("id", this.response.customer.id);
                result.Add("customer", cust);
                result.Add("notification_url", this.response.notification_url);

                return result;
            }
            catch
            {
                return null;
            }
           
        }

        public Dictionary<String, Object> capturePayment(String payment)
        {
            Dictionary<String, String> headers = new Dictionary<String, String>();

            JsonObject jo = new JsonObject();
            jo.Add("id", payment);

            this.response = this.doRequest(payment + "/capture", jo.ToString(), Method.POST, headers);
            Dictionary<String, Object> result = new Dictionary<String, Object>();

            // Convert Result in Dictionary
            result.Add("id", this.response.id);
            result.Add("created", this.response.created);
            result.Add("updated", this.response.updated);
            result.Add("amount", this.response.amount);
            result.Add("currency", this.response.created);
            result.Add("status", this.response.status);
            result.Add("type", this.response.type);
            Dictionary<String, Object> red = new Dictionary<String, Object>();
            red.Add("success_url", this.response.redirect.success_url);
            red.Add("failure_url", this.response.redirect.failure_url);
            red.Add("auth_url", this.response.redirect.auth_url);
            result.Add("redirect", red);
            Dictionary<String, Object> cust = new Dictionary<String, Object>();
            cust.Add("id", this.response.customer.id);
            cust.Add("ip", this.response.customer.ip);
            result.Add("customer", cust);
            result.Add("notification_url", this.response.notification_url);

            return result;
        }
        public Dictionary<String, Object> retrievePayment(String payment)
        {
            Dictionary<String, String> headers = new Dictionary<String, String>();
            this.response = this.doRequest(payment, "", Method.GET, headers);
            Dictionary<String, Object> result = new Dictionary<String, Object>();

            // Convert Result in Dictionary
            result.Add("id", this.response.id);
            result.Add("created", this.response.created);
            result.Add("updated", this.response.updated);
            result.Add("amount", this.response.amount);
            result.Add("currency", this.response.created);
            result.Add("status", this.response.status);
            result.Add("type", this.response.type);
            Dictionary<String, Object> red = new Dictionary<String, Object>();
            red.Add("success_url", this.response.redirect.success_url);
            red.Add("failure_url", this.response.redirect.failure_url);
            red.Add("auth_url", this.response.redirect.auth_url);
            result.Add("redirect", red);
            Dictionary<String, Object> cust = new Dictionary<String, Object>();
            cust.Add("id", this.response.customer.id);
            cust.Add("ip", this.response.customer.ip);
            result.Add("customer", cust);
            result.Add("notification_url", this.response.notification_url);

            return result;
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
                request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.psc_key)));
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
                request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.psc_key)));
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
                    case 4003:
                        err.Add("message", "The amount for this transaction exceeds the maximum amount. The maximum amount is 1000 EURO (equivalent in other currencies)");
                        break;
                    case 3001:
                        err.Add("message", "Transaction could not be initiated because the account is inactive.");
                        break;
                    case 2002:
                        err.Add("message", "payment id is unknown.");
                        break;
                    case 2010:
                        err.Add("message", "Currency is not supported.");
                        break;
                    case 2029:
                        err.Add("message", "Amount is not valid. Valid amount has to be above 0.");
                        break;
                    case 10028:
                        err.Add("message", "Argument missing.");
                        break;
                    default:
                        err.Add("message", "Transaction could not be initiated due to connection problems. If the problem persists, please contact our support. ");
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
        public virtual Redirect redirect { get; set; }
        public virtual string notification_url { get; set; }

    }

    public class Redirect
    {
        public virtual string success_url  { get; set; }
        public virtual string failure_url  { get; set; }
        public virtual string auth_url { get; set; }
    }

    public class Customer
    {
        public virtual string id { get; set; }
        public virtual string ip { get; set; }
    }

    public class Error
    {
        public virtual int number { get; set; }
        public virtual string message { get; set; }
        public virtual string param { get; set; }
        public virtual string code { get; set; }
    }
}
