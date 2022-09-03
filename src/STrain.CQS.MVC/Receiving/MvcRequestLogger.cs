using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.CQS.Receivers;

namespace STrain.CQS.MVC.Receiving
{
    public class MvcRequestLogger : RequestLogger<IActionResult>, IMvcRequestReceiver
    {
        public MvcRequestLogger(IMvcRequestReceiver requestReceiver, ILogger<MvcRequestLogger> logger)
            : base(requestReceiver, logger)
        {
        }
    }
}
