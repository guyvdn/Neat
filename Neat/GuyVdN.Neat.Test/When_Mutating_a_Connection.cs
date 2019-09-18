//using FakeItEasy;
//using NUnit.Framework;
//using Shouldly;

//namespace GuyVdN.Neat.Test
//{
//    [TestFixture]
//    public class When_Mutating_a_Connection
//    {
//        [Test]
//        public void It_should_get_a_completely_new_value_in_10_pct_of_the_cases()
//        {
//            // Given
//            var randomMock = A.Fake<IGetRandom>();
//            A.CallTo(() => randomMock.Bool(0.1)).Returns(true);
//            A.CallTo(() => randomMock.Double(-1, 1)).Returns(0.123);
//            GetRandom.Instance = () => randomMock;

//            // When
//            var sut = new Connection(new Node(0), new Node(1), 0.456);
//            sut.MutateWeight();

//            // Then
//            sut.Weight.ShouldBe(0.123);
//        }

//        [Test]
//        public void It_should_add_a_small_number_otherwise()
//        {
//            // Given
//            var randomMock = A.Fake<IGetRandom>();
//            A.CallTo(() => randomMock.Bool(0.1)).Returns(false);
//            A.CallTo(() => randomMock.Double(-0.01, 0.01)).Returns(0.00078);
//            GetRandom.Instance = () => randomMock;

//            // When
//            var sut = new Connection(new Node(0), new Node(1), 0.456);
//            sut.MutateWeight();

//            // Then
//            sut.Weight.ShouldBe(0.45678);
//        }

//        [Test]
//        public void It_should_not_return_a_value_lower_than_minus_one()
//        {
//            // Given
//            var randomMock = A.Fake<IGetRandom>();
//            A.CallTo(() => randomMock.Bool(0.1)).Returns(false);
//            A.CallTo(() => randomMock.Double(-0.01, 0.01)).Returns(-10);
//            GetRandom.Instance = () => randomMock;

//            // When
//            var sut = new Connection(new Node(0), new Node(1), 0.456);
//            sut.MutateWeight();

//            // Then
//            sut.Weight.ShouldBe(-1);
//        }

//        [Test]
//        public void It_should_not_return_a_value_higher_than_plus_one()
//        {
//            // Given
//            var randomMock = A.Fake<IGetRandom>();
//            A.CallTo(() => randomMock.Bool(0.1)).Returns(false);
//            A.CallTo(() => randomMock.Double(-0.01, 0.01)).Returns(10);
//            GetRandom.Instance = () => randomMock;

//            // When
//            var sut = new Connection(new Node(0), new Node(1), 0.456);
//            sut.MutateWeight();

//            // Then
//            sut.Weight.ShouldBe(1);
//        }
//    }
//}