﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentProcessorCOR
{
    public class EftPaymentTypeHandler : PaymentTypeHandlerBase
    {


        public EftPaymentTypeHandler(IEftProcessor eftProcessor, IPaymentsDao paymentsDao) 
        {
            this.eftProcessor = eftProcessor;
            this.paymentsDao = paymentsDao;
        }


        private IEftProcessor eftProcessor;
        private IPaymentsDao paymentsDao;



        protected override bool CanProcessPayment(BasePaymentData paymentData)
        {
            return paymentData.PaymentType == PaymentType.EFT;
        }

        protected override PaymentResult ExecutePaymentProcess(BasePaymentData paymentData)
        {
            EftPaymentData eftPaymentData = paymentData as EftPaymentData;

            EftAuthorization eftResult = this.eftProcessor.AuthorizeEftPayment(eftPaymentData.RoutingNumber,
                eftPaymentData.BankAccountNumber, eftPaymentData.AccountType, eftPaymentData.Amount);

            if (eftResult.Authorized)
            {
                int referenceNumber = paymentsDao.SaveSuccessfulEftPayment(eftPaymentData, eftResult);

                PaymentResult paymentResult = new PaymentResult()
                {
                    CustomerAccountNumber = eftPaymentData.CustomerAccountNumber,
                    PaymentDate = eftPaymentData.PaymentDate,
                    Success = eftResult.Authorized,
                    ReferenceNumber = referenceNumber
                };

                return paymentResult;
            }
            else
            {
                int referenceNumber = paymentsDao.SaveFailedEftPayment(eftPaymentData, eftResult);

                PaymentResult paymentResult = new PaymentResult()
                {
                    CustomerAccountNumber = eftPaymentData.CustomerAccountNumber,
                    PaymentDate = eftPaymentData.PaymentDate,
                    Success = eftResult.Authorized,
                    ReferenceNumber = referenceNumber
                };
                return paymentResult;
            }
        }
    }
}
