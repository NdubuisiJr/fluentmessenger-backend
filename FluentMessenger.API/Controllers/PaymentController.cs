using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using FluentMessenger.API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

        [HttpGet("{UserId}")]
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

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] 
        PaymentVerificationDto paymentVerification) {

            using(var httpClient=new HttpClient()) {
                var url = "https://api.paystack.co/transaction?page=1&perPage=10";
                httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue(
                    scheme:"Bearer",
                    parameter:_appSettings.Value.PayStackKey
                );
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode) {
                    var transactionDataContext = JsonConvert.
                        DeserializeObject<TransactionDataContext>(
                        await response.Content.ReadAsStringAsync());

                    var transactionData = transactionDataContext.Data
                                          .Find(x => x.Reference == paymentVerification.TransactionReference);
                    if (transactionData == null ) {
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

        private const decimal CostPerUnit = 298;
    }
}
