﻿//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;
using static Codecrete.SwissQRBill.Generator.ValidationMessage;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class CreditorValidationTest : BillDataValidationBase
    {
        [Fact]
        public void ValidCreditor()
        {
            SourceBill = SampleData.CreateExample1();
            var address = CreateValidPerson();
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.NotNull(ValidatedBill.Creditor);
            Assert.Equal("Zuppinger AG", ValidatedBill.Creditor.Name);
            Assert.Equal("Industriestrasse", ValidatedBill.Creditor.Street);
            Assert.Equal("34a", ValidatedBill.Creditor.HouseNo);
            Assert.Equal("9548", ValidatedBill.Creditor.PostalCode);
            Assert.Equal("Matzingen", ValidatedBill.Creditor.Town);
            Assert.Equal("CH", ValidatedBill.Creditor.CountryCode);
        }

        [Fact]
        public void MissingCreditor()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Creditor = null;
            Validate();
            AssertMandatoryPersonMessages();
        }

        [Fact]
        public void EmptyCreditor()
        {
            SourceBill = SampleData.CreateExample1();
            var emptyAddress = new Address();
            SourceBill.Creditor = emptyAddress;
            Validate();
            AssertMandatoryPersonMessages();
        }

        [Fact]
        public void EmptyCreditorWithSpaces()
        {
            SourceBill = SampleData.CreateExample1();
            var emptyAddress = new Address
            {
                Name = "  "
            };
            SourceBill.Creditor = emptyAddress;
            Validate();
            AssertMandatoryPersonMessages();
        }

        [Fact]
        public void MissingCreditorName()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "  ";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldCreditorName, ValidationConstants.KeyFieldValueMissing);
        }

        [Fact]
        public void CreditorWithoutStreet()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = null;
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
        }

        [Fact]
        public void CreditorWithoutHouseNo()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.HouseNo = null;
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
        }

        [Fact]
        public void CreditorWithMissingPostalCode()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.PostalCode = "";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldCreditorPostalCode, ValidationConstants.KeyFieldValueMissing);
        }

        [Fact]
        public void CreditorWithMissingTown()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = null;
            SourceBill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldCreditorTown, ValidationConstants.KeyFieldValueMissing);
        }

        [Fact]
        public void CreditorWithMissingCountryCode()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.CountryCode = "  ";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldCreditorCountryCode, ValidationConstants.KeyFieldValueMissing);
        }

        [Fact]
        public void CreditorWithInvalidCountryCode()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.CountryCode = "Schweiz";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldCreditorCountryCode, ValidationConstants.KeyCountryCodeInvalid);
        }

        [Fact]
        public void CreditorWithInvalidCountryCode2()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.CountryCode = "R!";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldCreditorCountryCode, ValidationConstants.KeyCountryCodeInvalid);
        }

        [Fact]
        public void CreditorWithConflictingAddress()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Creditor.AddressLine1 = "Conflict";
            Validate();
            Assert.True(Result.HasErrors);
            Assert.False(Result.HasWarnings);
            Assert.True(Result.HasMessages);
            Assert.Equal(5, Result.ValidationMessages.Count);
            foreach (ValidationMessage msg in Result.ValidationMessages)
            {
                Assert.Equal(MessageType.Error, msg.Type);
                Assert.Equal(ValidationConstants.KeyAddressTypeConflict, msg.MessageKey);
                Assert.StartsWith(ValidationConstants.FieldRootCreditor, msg.Field);
            }
        }

        private void AssertMandatoryPersonMessages()
        {
            Assert.True(Result.HasErrors);
            Assert.False(Result.HasWarnings);
            Assert.True(Result.HasMessages);
            Assert.Equal(5, Result.ValidationMessages.Count);
            foreach (ValidationMessage msg in Result.ValidationMessages)
            {
                Assert.Equal(MessageType.Error, msg.Type);
                Assert.Equal(ValidationConstants.KeyFieldValueMissing, msg.MessageKey);
                Assert.StartsWith(ValidationConstants.FieldRootCreditor, msg.Field);
            }
        }
    }
}
