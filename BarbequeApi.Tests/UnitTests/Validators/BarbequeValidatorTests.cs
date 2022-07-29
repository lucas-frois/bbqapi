﻿using BarbequeApi.Models.Dtos;
using BarbequeApi.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BarbequeApi.Tests.UnitTests
{
    public class BarbequeValidatorTests
    {
        [Fact]
        public async Task ValidateBarbequeDtoReturnsFalseBeucaseOfBeingNull()
        {
            var barbequeValidator = new BarbequeValidator();
            var (succesful, errorMessages) = barbequeValidator.Validate(null);
            Assert.False(succesful);
            Assert.Single(errorMessages);
            Assert.Equal("BarbequeDto should not be null.", errorMessages.First());
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task ValidateBarbequeDtoReturnsFalseBeucaseOfTitle(string title)
        {
            var barbequeValidator = new BarbequeValidator();
            var (succesful, errorMessages) = barbequeValidator.Validate(new BarbequeDto
            {
                Title = title, 
                Persons = null, 
                Date = new DateTime(2000, 1, 1)
            });
            Assert.False(succesful);
            Assert.Single(errorMessages);
            Assert.Equal(
                "BarbequeDto.Title should not be null, empty string or white spaces.", 
                errorMessages.First());
        }

        [Fact]
        public async Task ValidateBarbequeDtoReturnsFalseBeucaseInvalidPersons()
        {
            var barbequeValidator = new BarbequeValidator();

            var (succesful, errorMessages) = barbequeValidator.Validate(new BarbequeDto
            {
                Title = "Barbeque",
                Persons = new List<PersonDto>()
                {
                    new PersonDto()
                    {
                        Name = "Invalid test user", 
                        FoodMoneyShare = -10
                    }, 
                    new PersonDto()
                    {
                        Name = "Valid test user"
                    }
                },
                Date = new DateTime(2000, 1, 1)
            });

            Assert.False(succesful);
            Assert.Single(errorMessages);
            Assert.Equal(
                "BarbequeDto.Person[0] is not valid: PersonDto.FoodMoneyShare should not be negative.",
                errorMessages.First());
        }

        [Theory]
        [MemberData(nameof(DateTimeRangeForSqlServer))]
        public async Task ValidateBarbequeDtoReturnsFalseBecauseOfSqlServerDateTimeLimitations(
            DateTime date, 
            bool successful)
        {
            var barbequeValidator = new BarbequeValidator();

            var (succesful, errorMessages) = barbequeValidator.Validate(new BarbequeDto
            {
                Title = "Barbeque",
                Persons = new List<PersonDto>()
                {
                    new PersonDto()
                    {
                        Name = "Valid test user"
                    }
                },
                Date = date
            });

            if(successful)
            {
                Assert.Empty(errorMessages);
            }
            
            if(!succesful)
            {
                Assert.Single(errorMessages);
                Assert.Equal(
                    "BarbequeDto.Date should be equals or after January 1, 1753.",
                    errorMessages.First());
            }
        }

        public static IEnumerable<object[]> DateTimeRangeForSqlServer =>
        new List<object[]>
        {
            new object[] { new DateTime(1751, 11, 30), false },
            new object[] { new DateTime(1753, 1, 1), true },
            new object[] { new DateTime(2000, 1, 1), true }
        };

    }
}