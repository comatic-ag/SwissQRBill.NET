﻿//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Threading.Tasks;

namespace Codecrete.SwissQRBill.WindowsTest
{
    public class A4BillTest : TestBase
    {
        [WindowsFact]
        public Task CreateA4PngBill1()
        {
            return GenerateAndCompareBill(SampleData.CreateExample1(), OutputSize.A4PortraitSheet, GraphicsFormat.PNG);
        }

        private static Task GenerateAndCompareBill(Bill bill, OutputSize outputSize, GraphicsFormat graphicsFormat)
        {
            bill.Format.OutputSize = outputSize;
            bill.Format.GraphicsFormat = graphicsFormat;
            byte[] imageData = QRBill.Generate(bill);
            return VerifyImages.VerifyPng(imageData);
        }
    }
}
