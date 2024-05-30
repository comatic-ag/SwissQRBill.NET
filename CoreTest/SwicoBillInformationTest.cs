﻿//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Christian Bernasconi
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System;
using System.Collections.Generic;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class SwicoBillInformationTest
    {
        [Fact]
        public void SetInvoiceNumber()
        {
            var billInformation = new SwicoBillInformation
            {
                InvoiceNumber = "ABC"
            };
            Assert.Equal("ABC", billInformation.InvoiceNumber);
        }

        [Fact]
        public void SetInvoiceDate()
        {
            var billInformation = new SwicoBillInformation
            {
                InvoiceDate = new DateTime(2020, 6, 30, 0, 0, 0, DateTimeKind.Local)
            };
            Assert.Equal(new DateTime(2020, 6, 30, 0, 0, 0, DateTimeKind.Local), billInformation.InvoiceDate);
        }

        [Fact]
        public void SetCustomerReference()
        {
            var billInformation = new SwicoBillInformation
            {
                CustomerReference = "1234-ABC"
            };
            Assert.Equal("1234-ABC", billInformation.CustomerReference);
        }

        [Fact]
        public void SetVatNumber()
        {
            var billInformation = new SwicoBillInformation
            {
                VatNumber = "109030864"
            };
            Assert.Equal("109030864", billInformation.VatNumber);
        }

        [Fact]
        public void SetVatDate()
        {
            var billInformation = new SwicoBillInformation
            {
                VatDate = new DateTime(2020, 3, 1, 0, 0, 0, DateTimeKind.Local)
            };
            Assert.Equal(new DateTime(2020, 3, 1, 0, 0, 0, DateTimeKind.Local), billInformation.VatDate);
        }

        [Fact]
        public void SetVatStartDate()
        {
            var billInformation = new SwicoBillInformation
            {
                VatStartDate = new DateTime(2019, 3, 1, 0, 0, 0, DateTimeKind.Local)
            };
            Assert.Equal(new DateTime(2019, 3, 1, 0, 0, 0, DateTimeKind.Local), billInformation.VatStartDate);
        }

        [Fact]
        public void SetVatEndDate()
        {
            var billInformation = new SwicoBillInformation
            {
                VatEndDate = new DateTime(2020, 2, 29, 0, 0, 0, DateTimeKind.Local)
            };
            Assert.Equal(new DateTime(2020, 2, 29, 0, 0, 0, DateTimeKind.Local), billInformation.VatEndDate);
        }

        [Fact]
        public void SetVatRate()
        {
            var billInformation = new SwicoBillInformation
            {
                VatRate = 7.7m
            };
            Assert.Equal(7.7m, billInformation.VatRate);
        }

        [Fact]
        public void SetVatDetails()
        {
            var billInformation = new SwicoBillInformation
            {
                VatRateDetails = new List<(decimal, decimal)> { (8m, 1000m), (2.5m, 400m) }
            };
            Assert.Equal(2, billInformation.VatRateDetails.Count);
            Assert.Equal(8m, billInformation.VatRateDetails[0].Item1);
            Assert.Equal(400m, billInformation.VatRateDetails[1].Item2);
        }

        [Fact]
        public void SetVatImportTaxes()
        {
            var billInformation = new SwicoBillInformation
            {
                VatImportTaxes = new List<(decimal, decimal)> { (7.7m, 48.12m), (2.5m, 17.23m) }
            };
            Assert.Equal(2, billInformation.VatImportTaxes.Count);
            Assert.Equal(7.7m, billInformation.VatImportTaxes[0].Item1);
            Assert.Equal(17.23m, billInformation.VatImportTaxes[1].Item2);
        }

        [Fact]
        public void SetPaymentConditions()
        {
            var billInformation = new SwicoBillInformation
            {
                PaymentConditions = new List<(decimal, int)> { (2m, 10), (0m, 30) }
            };
            Assert.Equal(2, billInformation.PaymentConditions.Count);
            Assert.Equal(2m, billInformation.PaymentConditions[0].Item1);
            Assert.Equal(30, billInformation.PaymentConditions[1].Item2);
        }

        [Fact]
        public void DueDate_IsValid()
        {
            var billInformation = new SwicoBillInformation
            {
                InvoiceDate = new DateTime(2020, 6, 30, 0, 0, 0, DateTimeKind.Local),
                PaymentConditions = new List<(decimal, int)> { (2m, 10), (0m, 30) }
            };
            Assert.Equal(new DateTime(2020, 7, 30, 0, 0, 0, DateTimeKind.Local), billInformation.DueDate);
        }

        [Fact]
        public void DueDate_IsNull()
        {
            var billInformation = new SwicoBillInformation();
            Assert.Null(billInformation.DueDate);

            billInformation.InvoiceDate = new DateTime(2020, 6, 30, 0, 0, 0, DateTimeKind.Local);
            Assert.Null(billInformation.DueDate);

            billInformation.PaymentConditions = new List<(decimal, int)>();
            Assert.Null(billInformation.DueDate);

            billInformation.InvoiceDate = null;
            Assert.Null(billInformation.DueDate);

            billInformation.InvoiceDate = new DateTime(2020, 6, 30, 0, 0, 0, DateTimeKind.Local);
            Assert.Null(billInformation.DueDate);

            billInformation.PaymentConditions = new List<(decimal, int)> { (2m, 10) };
            Assert.Null(billInformation.DueDate);
        }

        [Fact]
        public void TestEqualsTrivial()
        {
            var info = new SwicoBillInformation();
            Assert.Equal(info, info);

            SwicoBillInformation nullBill = null;
            Assert.NotEqual(nullBill, info);
            Assert.NotEqual((object)"xxx", info);
        }

        [Fact]
        public void TestEquals()
        {
            SwicoBillInformation info1 = CreateBillInformation();
            SwicoBillInformation info2 = CreateBillInformation();
            Assert.Equal(info1, info2);
            Assert.Equal(info2, info1);

            info2.CustomerReference = "ABC";
            Assert.NotEqual(info1, info2);
        }

        [Fact]
        public void TestHashCode()
        {
            SwicoBillInformation info1 = CreateBillInformation();
            SwicoBillInformation info2 = CreateBillInformation();
            Assert.Equal(info1.GetHashCode(), info2.GetHashCode());
        }

        private static SwicoBillInformation CreateBillInformation()
        {
            return new SwicoBillInformation
            {
                InvoiceNumber = "R0000700312",
                InvoiceDate = new DateTime(2020, 7, 10, 0, 0, 0, DateTimeKind.Local),
                CustomerReference = "Q.30007.100002",
                VatNumber = "105815317",
                VatStartDate = new DateTime(2019, 11, 1, 0, 0, 0, DateTimeKind.Local),
                VatEndDate = new DateTime(2020, 4, 30, 0, 0, 0, DateTimeKind.Local),
                VatRate = 8m,
                VatImportTaxes = new List<(decimal, decimal)> { (8m, 48.12m), (2.5m, 17.23m) },
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            };
        }
    }
}
