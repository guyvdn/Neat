using System;
using System.Collections.Generic;
using System.Linq;
using GuyVdN.Neat.Extensions;
using NUnit.Framework;
using Shouldly;

namespace GuyVdN.Neat.Test
{
    public class GetRandomValueTest
    {
        [Test]
        public void Double_should_return_a_correct_value()
        {
            var sut = new GetRandom();
            const double minValue = 5.78;
            const double maxValue = 100.65;

            var generatedValues = new List<double>();
            1000.Times(_ => generatedValues.Add(sut.Double(minValue, maxValue)));

            generatedValues.Min().ShouldBeGreaterThanOrEqualTo(minValue);
            generatedValues.Max().ShouldBeLessThanOrEqualTo(maxValue);
            generatedValues.GroupBy(x => x).Count().ShouldBe(1000);
        }

        [Test]
        public void Bool_should_return_a_correct_value()
        {
            var sut = new GetRandom();
            const double truePercentage = 0.8;

            var generatedValues = new List<bool>();
            1000.Times(_ => generatedValues.Add(sut.Bool(truePercentage)));

            generatedValues.Count(x => x == true).ShouldBeInRange(780, 820);
        }

        [Test, Explicit("Not sure how to test yet")]
        public void Gaussian_should_generate_numbers_that_do_not_deviate_too_much()
        {
            var sut = new GetRandom();

            var generatedValues = new List<double>();
            1000.Times(_ => generatedValues.Add(sut.Gaussian()));

           // (generatedValues.Max() - generatedValues.Min()).ShouldBe(0);

            var groupBy = generatedValues.GroupBy(x => (int)(x * 100)).OrderBy(x => x.Key);

            groupBy.Count().ShouldBe(1000);

        }

    }
}