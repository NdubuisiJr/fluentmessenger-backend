using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using FluentMessenger.API.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FluentMessenger.API.Controllers {
    [Route("api/payment")]
    public class PaymentController : Controller {
        private readonly IRepository<User> _userRepo;
        private readonly IConfiguration _configuration;
        private readonly IOptions<Secret> _appSettings;

        public PaymentController(IRepository<User> repositoryService,
            IConfiguration configuration, IOptions<Secret> appSettings) {
            _userRepo = repositoryService;
            _configuration = configuration;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Returns the payment page
        /// </summary>
        /// <param name="userId">The user's Id, that wants to make payment</param>
        /// <param name="amount">The amount the user wants to pay</param>
        /// <returns></returns>
        [HttpGet("{UserId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Index(int userId, [FromQuery] decimal amount) {
            var user = _userRepo.Get(userId);
            if (user == null) {
                return NotFound("No user was found");
            }

            ViewData["Title"] = "Payment";
            ViewData["UserId"] = userId;
            ViewData["UserEmail"] = user.Email;
            ViewData["UserAmount"] = amount;
            ViewData["UserFirstName"] = user.OtherNames;
            ViewData["UserLastName"] = user.Surname;
            return View();
        }

        /// <summary>
        /// This confirms the payment by the user and updates the user's smscredit
        /// </summary>
        /// <param name="paymentVerification">The input object for confirming payment</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Index([FromBody]
        PaymentVerificationDto paymentVerification) {

            using (var httpClient = new HttpClient()) {
                var url = "https://api.paystack.co/transaction?page=1&perPage=10";
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                    scheme: "Bearer",
                    parameter: _appSettings.Value.PayStackKey
                );
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode) {
                    var transactionDataContext = JsonConvert.
                        DeserializeObject<TransactionDataContext>(
                        await response.Content.ReadAsStringAsync());

                    var transactionData = transactionDataContext.Data
                                          .Find(x => x.Reference == paymentVerification.TransactionReference);
                    if (transactionData == null) {
                        return NotFound($"Invalid transaction ID {paymentVerification.TransactionReference}");
                    }

                    var user = _userRepo.Get(paymentVerification.UserId);
                    if (user == null) {
                        return NotFound("No user was found");
                    }

                    if (user.Email == paymentVerification.Email
                        && transactionData.Status == "success"
                        /*&& transactionData.Domain!= "test"*/) {
                        var numberOfUnits = transactionData.Amount / CostPerUnit;
                        user.SMSCredit += numberOfUnits.Value;
                        _userRepo.Update(user);
                        _userRepo.SaveChanges();
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// This confirms the payment by the user and updates the user's smscredit
        /// </summary>
        /// <param name="webhooksVerification">The input object for confirming payment</param>
        /// <returns></returns>
        [HttpPost("/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmPaymentFromWebhooks([FromBody]
        WebhooksVerificationDto webhooksVerification) {

            var customer = webhooksVerification.Data.Customer;

            var user = _userRepo.GetAll(true).Where(x => x.Email == customer.Email &&
                x.OtherNames == customer.First_Name && x.Surname == customer.Last_Name).FirstOrDefault();
            if (user == null) {
                return NotFound("No user was found");
            }

            // verify ip


            // offer service
            if (webhooksVerification.Event == "charge.success" 
                && webhooksVerification.Data.Status == "success" 
                && webhooksVerification.Data.Log.Success) {
                var numberOfUnits = webhooksVerification.Data.Amount / CostPerUnit;
                user.SMSCredit += numberOfUnits;
                _userRepo.Update(user);
                _userRepo.SaveChanges();
            }
            return View();
        }

        private const decimal CostPerUnit = 298;
    }
}
