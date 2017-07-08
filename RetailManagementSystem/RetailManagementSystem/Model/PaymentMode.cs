﻿namespace RetailManagementSystem.Model
{
    public class PaymentMode
    {
        PaymentMode[] _paymentModes;

        public char PaymentId { get; set; }
        public string PaymentName { get; set; }
        
        public PaymentMode[] PaymentModes
        {
            get
            {
                if (_paymentModes.Length == 0)
                {
                    _paymentModes[0] = new PaymentMode { PaymentId = '0', PaymentName = "Cash" };
                    _paymentModes[1] = new PaymentMode { PaymentId = '1', PaymentName = "Credit" };
                }
                return _paymentModes;
            }

        }

        public PaymentMode()
        {
            _paymentModes = new PaymentMode[2];
        }

        public string GetPaymentString(string paymentId)
        {
            if (paymentId == "0")
                return "Cash";
            return "Credit";
        }
    }
}
