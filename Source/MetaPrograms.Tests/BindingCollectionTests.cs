using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.Tests
{
    [TestFixture]
    public class BindingCollectionTests
    {
        [Test]
        public void CollectionIsInitiallyEmpty()
        {
            // arrange

            var collection = new BindingCollection();

            // act & assert

            collection.Count.ShouldBe(0);
            collection.ShouldBeEmpty();
        }

        [Test]
        public void CanAdd()
        {
            // arrange

            var collection = new BindingCollection();
            var binding1 = new ExampleBindingOne();
            var binding2 = new ExampleBindingOne();

            // act 

            collection.Add(binding1);
            collection.Add(binding2);

            // assert

            collection.Count.ShouldBe(2);
            collection.ShouldBe(new object[] { binding1, binding2 });
        }

        [Test]
        public void SameExtensionCannotBeAddedTwice()
        {
            // arrange

            var collection = new BindingCollection();
            var binding1 = new ExampleBindingOne();
            var binding2 = new ExampleBindingOne();

            // act 

            collection.Add(binding1);
            collection.Add(binding2);
            collection.Add(binding1);

            // assert

            collection.Count.ShouldBe(2);
            collection.ShouldBe(new object[] { binding1, binding2 });
        }

        public class ExampleBindingOne
        {
        }

        public class ExampleBindingTwo
        {
        }
    }
}
