﻿using System;

namespace CreditCardApplications
{
    public class CreditCardApplicationEvaluator
    {
        private readonly IFrequentFlyerNumberValidator _validator;

        private const int AutoReferralMaxAge = 20;
        private const int HighIncomeThreshhold = 100_000;
        private const int LowIncomeThreshhold = 20_000;

        public int ValidatorLookupCount { get; set; }

        public CreditCardApplicationEvaluator(IFrequentFlyerNumberValidator validator)
        {
            _validator = validator ?? 
                throw new ArgumentNullException(nameof(validator));

            _validator.ValidatorLookupPerformed += ValidatorLookupPerformed;
        }

        private void ValidatorLookupPerformed(object sender, EventArgs args)
        {
            ValidatorLookupCount++;
        }


        public CreditCardApplicationDecision Evaluate(CreditCardApplication application)
        {
            if (application.GrossAnnualIncome >= HighIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }

            if (_validator.ServiceInformation.License.LicenseKey == "EXPIRED")
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            _validator.ValidationMode = application.Age >= 30 ? ValidationMode.Detailed
                                                              : ValidationMode.Quick;

            bool isValidFrequentFlyerNumber;
            try
            {
                isValidFrequentFlyerNumber
                    = _validator.IsValid(application.FrequentFlyerNumber);
            }
            catch (Exception)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
                throw;
            }

            if (!isValidFrequentFlyerNumber)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

            return CreditCardApplicationDecision.ReferredToHuman;
        }

        public CreditCardApplicationDecision EvaluateUsingOut(CreditCardApplication application)
        {
            if (application.GrossAnnualIncome >= HighIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }

            if (_validator.ServiceInformation.License.LicenseKey == "EXPIRED")
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            _validator.IsValid(application.FrequentFlyerNumber, out var isValidFrequentFlyerNumber);

            if (!isValidFrequentFlyerNumber)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshhold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

            return CreditCardApplicationDecision.ReferredToHuman;
        }

    }
}
