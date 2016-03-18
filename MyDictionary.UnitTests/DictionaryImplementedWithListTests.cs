using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FluentAssertions;

namespace MyDictionary.UnitTests
{
    public class Animal
    {
        public string Name;
        public string Says;
    }

    // only compares on what the Animal says, NOT the name
    public class AnimalComparer : IEqualityComparer<Animal>
    {
        public bool Equals(Animal x, Animal y)
        {
            return x.Says.Equals(y.Says);
        }

        public int GetHashCode(Animal obj)
        {
            return obj.GetHashCode(); 
        }
    }

    [TestClass]
    public class DictionaryImplementedWithListTests
    {
        [TestMethod]
        public void Add_KeyNotExist_IncreasesCount()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Count.Should().Be(0);
            dctIntStr.Add(new KeyValuePair<int, string>(1, "A"));
            dctIntStr.Count.Should().Be(1);
        }

        [TestMethod]
        public void Add_KeyExists_ThrowsError()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();
            
            dctIntStr.Add(new KeyValuePair<int, string>(1, "A"));
            Action act = () => dctIntStr.Add(new KeyValuePair<int, string>(1, "A"));
            act.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void Clear_ReducesCountToZero()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "A");
            dctIntStr.Count.Should().Be(1);

            dctIntStr.Clear();
            dctIntStr.Count.Should().Be(0);
        }

        [TestMethod]
        public void DictionaryGet_KeyNotExists_ThrowsKeyNotFound()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();
            string val;
            Action act = () => val = dctIntStr[3];

            act.ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod]
        public void DictionaryGet_KeyExists_ReturnsValue()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "A");
            var val = dctIntStr[1];

            val.Should().Be("A");
        }

        [TestMethod]
        public void DictionarySet_KeyNotExists_ThrowsKeyNotFound()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "a");

            Action act = () => dctIntStr[2] = "A";
            act.ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod]
        public void DictionarySet_KeyExists_SetsValue()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "a");
            dctIntStr[1] = "b";

            dctIntStr[1].Should().Be("b");
        }

        [TestMethod]
        public void ContainsKey_StringKey_UsesCaseInsensitiveCompare_KeyExists_ReturnsTrue()
        {
            var dctStrStr = new DictionaryImplementedWithList<string, string>();

            dctStrStr.Add("mykey", "myvalue");
            dctStrStr.ContainsKey("MYKEY").Should().BeTrue();
        }

        [TestMethod]
        public void ContainsKey_ObjectKey_UseCustomComparer_KeyExists_ReturnsTrue()
        {
            var dctObjStr = new DictionaryImplementedWithList<Animal, string>(
                new AnimalComparer());
            var tony = new Animal { Name = "Tony", Says = "Roar" };
            var tigger = new Animal { Name = "Tigger", Says = "Roar" };

            dctObjStr.Add(tony, "they're great!");
            dctObjStr.ContainsKey(tigger).Should().BeTrue();
        }

        [TestMethod]
        public void ContainsKey_ObjectKey_UseCustomComparer_KeyNotExists_ReturnsFalse()
        {
            var dctObjStr = new DictionaryImplementedWithList<Animal, string>(
                new AnimalComparer());
            var tony = new Animal { Name = "Tony", Says = "Roar" };
            var simba = new Animal { Name = "Tigger", Says = "Meow" };

            dctObjStr.Add(tony, "they're great!");
            dctObjStr.ContainsKey(simba).Should().BeFalse();
        }

        [TestMethod]
        public void ContainsKey_NonSpecialKey_KeyExists_ReturnsTrue()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "a");
            dctIntStr.ContainsKey(1).Should().BeTrue();
        }

        [TestMethod]
        public void ContainsKey_NonSpecialKey_KeyNotExists_ReturnsFalse()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "a");
            dctIntStr.ContainsKey(2).Should().BeFalse();
        }

        [TestMethod]
        public void CopyTo_CopiesDictionaryToArray()
        {
            var myArray = new KeyValuePair<int, string>[5];
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "a");
            dctIntStr.Add(2, "b");
            dctIntStr.Add(3, "c");

            dctIntStr.CopyTo(myArray, 2);

            myArray[2].Key.Should().Be(1);
            myArray[2].Value.Should().Be("a");
            myArray[3].Key.Should().Be(2);
            myArray[3].Value.Should().Be("b");
            myArray[4].Key.Should().Be(3);
            myArray[4].Value.Should().Be("c");
        }

        [TestMethod]
        public void GetEnumerator_CanEnumerate()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "a");
            dctIntStr.Add(2, "b");
            dctIntStr.Add(3, "c");

            var enumerator = dctIntStr.GetEnumerator();
            var counter = 1;
            while(enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                pair.Key.Should().Be(counter);
                counter++;
            }
        }

        [TestMethod]
        public void RemoveItem_ItemExists_ReturnsTrue()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "a");
            dctIntStr.Remove(new KeyValuePair<int, string>(1, "a")).Should().BeTrue();
        }

        [TestMethod]
        public void RemoveItem_ItemNotExists_ReturnsFalse()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "a");
            dctIntStr.Remove(new KeyValuePair<int, string>(2, "a")).Should().BeFalse();
        }

        [TestMethod]
        public void RemoveByKey_KeyExists_ReturnsTrue()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "a");
            dctIntStr.Remove(1).Should().BeTrue();
        }

        [TestMethod]
        public void RemoveByKey_KeyNotExists_ReturnsFalse()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();

            dctIntStr.Add(1, "a");
            dctIntStr.Remove(2).Should().BeFalse();
        }

        [TestMethod]
        public void TryGetValue_KeyExists_ReturnTrue()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();
            string myValue;

            dctIntStr.Add(1, "a");

            dctIntStr.TryGetValue(1, out myValue).Should().BeTrue();
        }

        [TestMethod]
        public void TryGetValue_KeyNotExists_ReturnFalse()
        {
            var dctIntStr = new DictionaryImplementedWithList<int, string>();
            string myValue;

            dctIntStr.Add(1, "a");

            dctIntStr.TryGetValue(2, out myValue).Should().BeFalse();
        }
    }
}
