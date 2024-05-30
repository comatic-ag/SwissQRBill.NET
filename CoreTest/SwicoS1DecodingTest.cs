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
using System.Globalization;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class SwicoS1DecodingTest
    {
        [Fact]
        public void Example1_FullyDecoded()
        {
            var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example1Text);
            Assert.Equal(SwicoExamples.CreateExample1(), billInformation);
        }

        [Fact]
        public void Example2_FullyDecoded()
        {
            var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example2Text);
            Assert.Equal(SwicoExamples.CreateExample2(), billInformation);
        }

        [Fact]
        public void Example3_FullyDecoded()
        {
            var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example3Text);
            Assert.Equal(SwicoExamples.CreateExample3(), billInformation);
        }

        [Fact]
        public void Example4_FullyDecoded()
        {
            var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example4Text);
            Assert.Equal(SwicoExamples.CreateExample4(), billInformation);
        }

        [Fact]
        public void Example5_FullyDecoded()
        {
            var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example5Text);
            Assert.Equal(SwicoExamples.CreateExample5(), billInformation);
        }

        [Fact]
        public void Example6_FullyDecoded()
        {
            var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example6Text);
            Assert.Equal(SwicoExamples.CreateExample6(), billInformation);
        }

        [Fact]
        public void NullValue_ReturnsNull()
        {
            Assert.Null(SwicoBillInformation.DecodeText(null));
        }

        [Theory]
        [InlineData("de-DE")]
        [InlineData("en-US")]
        [InlineData("de-CH")]
        [InlineData("fr-CH")]
        public void DifferentLocales_HaveNoEffect(string locale)
        {
            CultureInfo savedCurrentCulture = CultureInfo.CurrentCulture;
            CultureInfo savedCurrentUiCulture = CultureInfo.CurrentUICulture;

            CultureInfo culture = CultureInfo.CreateSpecificCulture(locale);

            try
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example3Text);
                Assert.Equal(SwicoExamples.CreateExample3(), billInformation);
            }
            finally
            {
                CultureInfo.CurrentCulture = savedCurrentCulture;
                CultureInfo.CurrentUICulture = savedCurrentUiCulture;
            }
        }

        [Theory]
        [InlineData("//S1/10//11//20//30/")]
        [InlineData("//S1/")]
        public void EmptyValues_Decoded(string rawBillInformation)
        {
            var billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation(), billInformation);
        }

        [Theory]
        [InlineData("/S1/10/X.66711")]
        [InlineData("///S1/10/X.66711")]
        [InlineData("S1/10/X.66711")]
        [InlineData("10/X.66711")]
        public void InvalidStart_ReturnsNull(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Null(billInformation);
        }

        [Theory]
        [InlineData("//S1/10/X.66711/XX/200")]
        [InlineData("//S1/10/X.66711/10.0/200")]
        public void InvalidTag_IsIgnored(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "X.66711"
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/11/190520/10/X.66711/30/123456789")]
        [InlineData("//S1/10/X.66711/30/123456789/11/190520")]
        [InlineData("//S1/11/201010/10/X.66711/11/190520/30/123456789")]
        public void InvalidTagOrder_IsIgnored(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "X.66711",
                InvoiceDate = new DateTime(2019, 5, 20, 0, 0, 0, DateTimeKind.Local),
                VatNumber = "123456789"
            },
                billInformation
            );
        }

        [Theory]
        [InlineData(@"//S1/10/X.66711/20/405\/1/40/0:30", @"405/1")]
        [InlineData(@"//S1/10/X.66711/20/405\\1/40/0:30", @"405\1")]
        [InlineData(@"//S1/10/X.66711/20/\/405-1/40/0:30", @"/405-1")]
        [InlineData(@"//S1/10/X.66711/20/\405-1/40/0:30", @"\405-1")]
        [InlineData(@"//S1/10/X.66711/20/405\\/40/0:30", @"405\")]
        public void EscapedCharacters_Unescaped(string rawBillInformation, string customerReference)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "X.66711",
                CustomerReference = customerReference,
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            },
                billInformation
            );
        }

        [Fact]
        public void BillInformationTruncated1_IsIgnored()
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText("//S1/10");
            Assert.Equal(new SwicoBillInformation() { },
                billInformation
            );
        }

        [Fact]
        public void BillInformationTruncated2_IsIgnored()
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText("//S1/10/X.66711/11/190520/20");
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "X.66711",
                InvoiceDate = new DateTime(2019, 5, 20, 0, 0, 0, DateTimeKind.Local)
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/10/X.66711/20/T.000-001/29/123")]
        [InlineData("//S1/10/X.66711/12/ABC/20/T.000-001")]
        public void UnknownTag_IsIgnored(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "X.66711",
                CustomerReference = "T.000-001"
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/10/10201409/11/190570/20/405.789.Q")]
        [InlineData("//S1/10/10201409/11/19.05.20/20/405.789.Q")]
        [InlineData("//S1/10/10201409/11/1905213/20/405.789.Q")]
        [InlineData("//S1/10/10201409/11/200301 /20/405.789.Q")]
        [InlineData("//S1/10/10201409/11/200301125977/20/405.789.Q")]
        [InlineData("//S1/10/10201409/11/2003011288/20/405.789.Q")]
        public void InvalidInvoiceDate_IsIgnored(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "10201409",
                CustomerReference = "405.789.Q"
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/10/10201409/31/190570")]
        [InlineData("//S1/10/10201409/31/1905213")]
        [InlineData("//S1/10/10201409/31/1905211905232")]
        [InlineData("//S1/10/10201409/31/19052119052 ")]
        [InlineData("//S1/10/10201409/31/190500190531")]
        [InlineData("//S1/10/10201409/31/190501190532")]
        public void InvalidVatDates_IsIgnored(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "10201409"
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/10/329348709/11/200629/32/AB/40/0:30")]
        [InlineData("//S1/10/329348709/11/200629/32/3.5./40/0:30")]
        public void InvalidVatRate_IsIgnored(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "329348709",
                InvoiceDate = new DateTime(2020, 6, 29, 0, 0, 0, DateTimeKind.Local),
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/10/10201409/11/200629/32/1;2/40/0:30")]
        [InlineData("//S1/10/10201409/11/200629/32/8:B/40/0:30")]
        [InlineData("//S1/10/10201409/11/200629/32/8:/40/0:30")]
        [InlineData("//S1/10/10201409/11/200629/32/:200;x:200/40/0:30")]
        public void InvalidVatRateDetails_AreIgnored_ResultEmpty(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "10201409",
                InvoiceDate = new DateTime(2020, 6, 29, 0, 0, 0, DateTimeKind.Local),
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/10/10201409/11/200629/32/8:500;5/40/0:30")]
        [InlineData("//S1/10/10201409/11/200629/32/;8:500/40/0:30")]
        [InlineData("//S1/10/10201409/11/200629/32/8:500;/40/0:30")]
        [InlineData("//S1/10/10201409/11/200629/32/8:500;x:200/40/0:30")]
        public void InvalidVatRateDetails_AreIgnored_PartialResult(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "10201409",
                InvoiceDate = new DateTime(2020, 6, 29, 0, 0, 0, DateTimeKind.Local),
                VatRateDetails = new List<(decimal, decimal)> { (8m, 500m) },
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/10/10202007/11/200629/33/1;2/40/0:10")]
        [InlineData("//S1/10/10202007/11/200629/33/8:B/40/0:10")]
        [InlineData("//S1/10/10202007/11/200629/33/8:/40/0:10")]
        public void InvalidVatImportTaxes_AreIgnored_EmptyResult(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "10202007",
                InvoiceDate = new DateTime(2020, 6, 29, 0, 0, 0, DateTimeKind.Local),
                PaymentConditions = new List<(decimal, int)> { (0m, 10) }
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/10/10201409/11/200629/33/8:24.5;5/40/0:30")]
        [InlineData("//S1/10/10201409/11/200629/33/;8:24.5/40/0:30")]
        [InlineData("//S1/10/10201409/11/200629/33/8:24.5;/40/0:30")]
        [InlineData("//S1/10/10201409/11/200629/33/8:24.5;x:200/40/0:30")]
        public void InvalidVatImportTaxes_AreIgnored_PartialResult(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "10201409",
                InvoiceDate = new DateTime(2020, 6, 29, 0, 0, 0, DateTimeKind.Local),
                VatImportTaxes = new List<(decimal, decimal)> { (8m, 24.50m) },
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/10/10201317/11/200321/40/1;60")]
        [InlineData("//S1/10/10201317/11/200321/40/1:5.0")]
        [InlineData("//S1/10/10201317/11/200321/40/3:B")]
        [InlineData("//S1/10/10201317/11/200321/40/ABC")]
        public void InvalidPaymentConditions_AreIgnored_EmptyResult(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "10201317",
                InvoiceDate = new DateTime(2020, 3, 21, 0, 0, 0, DateTimeKind.Local)
            },
                billInformation
            );
        }

        [Theory]
        [InlineData("//S1/10/10201409/11/220807/40/0:60;5")]
        [InlineData("//S1/10/10201409/11/220807/40/;0:60")]
        [InlineData("//S1/10/10201409/11/220807/40/0:60;")]
        [InlineData("//S1/10/10201409/11/220807/40/x:1;2:x;0:60;x:200")]
        public void InvalidPaymentConditions_AreIgnored_PartialResult(string rawBillInformation)
        {
            SwicoBillInformation billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation()
            {
                InvoiceNumber = "10201409",
                InvoiceDate = new DateTime(2022, 8, 7, 0, 0, 0, DateTimeKind.Local),
                PaymentConditions = new List<(decimal, int)> { (0m, 60) }
            },
                billInformation
            );
        }
    }
}
