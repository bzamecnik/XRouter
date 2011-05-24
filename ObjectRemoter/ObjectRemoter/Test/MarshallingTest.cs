namespace ObjectRemoter.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Xunit;

    public class MarshallingTest
    {
        #region Happy-day test methods

        [Fact]
        public void MarshalAndUnmarshallString()
        {
            MarshalAndUnmarshall<string>("foo bar");
        }

        [Fact]
        public void MarshalAndUnmarshallPrimitiveInt32()
        {
            MarshalAndUnmarshall<Int32>(42);
        }

        [Fact]
        public void MarshalAndUnmarshallPrimitiveDouble()
        {
            MarshalAndUnmarshall<double>(3.1415926535);
        }

        [Fact]
        public void MarshalAndUnmarshallPrimitiveDoubleNegativeInfinity()
        {
            MarshalAndUnmarshall<double>(double.NegativeInfinity);
        }

        [Fact]
        public void MarshalAndUnmarshallBool()
        {
            MarshalAndUnmarshall<bool>(true);
        }

        [Fact]
        public void MarshalAndUnmarshallArrayOfStrings()
        {
            MarshalAndUnmarshallArray(new[] { "foo", "bar", "baz" });
        }

        [Fact]
        public void MarshalAndUnmarshallArrayOfInt32()
        {
            MarshalAndUnmarshallArray(new[] { 654321354, 061506, 0, -546510 });
        }

        [Fact]
        public void MarshalAndUnmarshallArrayOfDouble()
        {
            MarshalAndUnmarshallArray(new[] {
                65432.1354, 0.123456, 1.1e5,
                double.NaN, double.NegativeInfinity });
        }

        [Fact]
        public void MarshalAndUnmarshallEmptyArrayOfString()
        {
            MarshalAndUnmarshallArray(new string[] { });
        }

        [Fact]
        public void MarshalAndUnmarshallAnonymousMethod()
        {
            System.Func<string> originalDelegate = () => string.Format("PI is {0}", 3.14);
            Type type = typeof(System.Func<string>);
            string marshalled = Marshalling.Marshal(originalDelegate, type);
            var unmarshalledDelegate = (System.Func<string>)Marshalling.Unmarshal(marshalled, type);
            string resultFromOriginal = originalDelegate();
            string resultFromUnmarshalled = unmarshalledDelegate();
            Assert.Equal(resultFromOriginal, resultFromUnmarshalled);
        }

        [Fact]
        public void MarshalAndUnmarshallSerializableWithColon()
        {
            System.Func<string> originalDelegate = () => string.Format("PI: {0}", 3.14);
            Type type = typeof(System.Func<string>);
            string marshalled = Marshalling.Marshal(originalDelegate, type);
            var unmarshalledDelegate = (System.Func<string>)Marshalling.Unmarshal(marshalled, type);
            string resultFromOriginal = originalDelegate();
            string resultFromUnmarshalled = unmarshalledDelegate();
            Assert.Equal(resultFromOriginal, resultFromUnmarshalled);
        }

        [Fact]
        public void MarshalAndUnmarshallAction()
        {
            System.Action<string> originalDelegate = (string value) => value = "foo";
            Type type = typeof(System.Action<string>);
            string marshalled = Marshalling.Marshal(originalDelegate, type);
            var unmarshalledDelegate = (System.Action<string>)Marshalling.Unmarshal(marshalled, type);
            Assert.DoesNotThrow(() => unmarshalledDelegate("bar"));
        }

        [Fact]
        public void MarshalAndUnmarshallObject()
        {
            Type type = typeof(object);
            object original = new object();
            string marshalled = null;
            Assert.DoesNotThrow(() => marshalled = Marshalling.Marshal(original, type));
            object result = null;
            Assert.DoesNotThrow(() => result = Marshalling.Unmarshal(marshalled, type));
            Assert.NotNull(result);

            // NOTE: The unmarshalled object might have different hash code,
            // even when both objects are equal in fact. An object with a
            // proper implementation of Equals() should be used.
        }

        [Fact]
        public void MarshalAndUnmarshallNullObject()
        {
            MarshalAndUnmarshall<object>(null);
        }

        [Fact]
        public void MarshalAndUnmarshallNullString()
        {
            MarshalAndUnmarshall<string>(null);
        }

        [Fact]
        public void MarshalNullIRemotelyReferable()
        {
            MarshalAndUnmarshall<IRemotelyReferable>(null);
        }

        [Fact]
        public void MarshalNullIRemotelyCloneable()
        {
            MarshalAndUnmarshall<IRemotelyCloneable>(null);
        }

        [Fact]
        public void MarshalNullDelegate()
        {
            MarshalAndUnmarshall<Delegate>(null);
        }

        [Fact]
        public void MarshalAndUnmarshallSerializable()
        {
            MarshalAndUnmarshall(new SampleSerializable("foo", 42,
                new SampleSerializable("bar", 13, null)));
        }

        [Fact]
        public void MarshalAndUnmarshallRemotelyReferable()
        {
            var original = new SampleRemotelyReferable("foo",
                new SampleRemotelyReferable("bar", null));
            var comparer = new SampleRemotelyReferableEquivalenceComparer();
            MarshalAndUnmarshall<ISampleRemotelyReferable>(original, comparer);
        }

        [Fact]
        public void MarshalAndUnmarshallRemotelyCloneable()
        {
            var original = new SampleRemotelyCloneable("foo", 1,
                new SampleRemotelyCloneable("bar", 2, null));
            MarshalAndUnmarshall<SampleRemotelyCloneable>(original);
        }

        [Fact]
        public void MarshalDetermineTypeRemotelyReferable()
        {
            var original = new SampleRemotelyReferable("foo",
                new SampleRemotelyReferable("bar", null));
            string marshalled = Marshalling.Marshal(original, typeof(IRemotelyReferable));
            string marshalledWithExplicitType = Marshalling.Marshal(original, typeof(object));
            Assert.Equal(marshalled, marshalledWithExplicitType);
        }

        [Fact]
        public void MarshalDetermineTypeRemotelyCloneable()
        {
            var original = new SampleRemotelyCloneable("foo", 1,
                new SampleRemotelyCloneable("bar", 2, null));
            string marshalled = Marshalling.Marshal(original, typeof(IRemotelyCloneable));
            // TODO: find out what is the corrent behavior:
            // - should Marshal determine IRemotelyCloneable or the real type?
            string marshalledWithExplicitType = Marshalling.Marshal(original, typeof(object));
            Assert.Equal(marshalled, marshalledWithExplicitType);
        }

        [Fact]
        public void MarshalRemotelyCloneableWithConcreteTypeAndUnmarshalWithAbstractType()
        {
            var original = new SampleRemotelyCloneable("foo", 1,
                new SampleRemotelyCloneable("bar", 2, null));
            // marshal with concrete type
            string marshalled = Marshalling.Marshal(original, typeof(SampleRemotelyCloneable));
            // unmarshal with abstract type
            object result = Marshalling.Unmarshal(marshalled, typeof(IRemotelyCloneable));
            Assert.Equal(original, result);
        }

        [Fact]
        public void MarshalDetermineTypeDelegate()
        {
            System.Func<string> originalDelegate = () => string.Format("PI is {0}", 3.14);
            string marshalled = Marshalling.Marshal(originalDelegate, typeof(object));
            var unmarshalledDelegate = (System.Func<string>)Marshalling.Unmarshal(marshalled, typeof(object));
            string resultFromOriginal = originalDelegate();
            string resultFromUnmarshalled = unmarshalledDelegate();
            Assert.Equal(resultFromOriginal, resultFromUnmarshalled);
        }

        // TODO:
        // - object types:
        //   - arrays of various types
        //   - more primitives

        #endregion

        #region Bad-day test methods

        [Fact]
        public void MarshalWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Marshalling.Marshal("foo bar", null)
            );
        }

        [Fact]
        public void UnmarshalWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Marshalling.Unmarshal("foo bar", null)
            );
        }

        [Fact]
        public void UnmarshalNullMarshalledString()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Marshalling.Unmarshal(null, typeof(string))
            );
        }

        [Fact]
        public void MarshallRemotelyReferableWithNonInterfaceType()
        {
            var original = new SampleRemotelyReferable("foo", null);
            Type type = typeof(SampleRemotelyReferable);
            Assert.Throws<ArgumentException>(() => Marshalling.Marshal(original, type));
        }

        [Fact]
        public void MarshalUnsupportedClass()
        {
            var original = new SampleClass("foo", 42);
            Type type = typeof(SampleClass);
            Assert.Throws<ArgumentException>(() => Marshalling.Marshal(original, type));
        }

        [Fact]
        public void MarshalUnsupportedStruct()
        {
            var original = new SampleStruct("foo", 42);
            Type type = typeof(SampleStruct);
            Assert.Throws<ArgumentException>(() => Marshalling.Marshal(original, type));
        }

        [Fact]
        public void UnmarshalCompletelyFakeString()
        {
            Type type = typeof(SampleStruct);
            string marshalled = "foobar+%%@$^@%^fagfg5468f84f5g4fg";
            Assert.Throws<ArgumentException>(() => Marshalling.Unmarshal(marshalled, type));
        }

        [Fact]
        public void UnmarshalFakeStringWithCorrectTypeAndEmptyRest()
        {
            Type type = typeof(SampleSerializable);
            string marshalled = "ObjectRemoter.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!ObjectRemoter.Test.MarshallingTest+SampleSerializable:";
            Assert.Throws<ArgumentException>(() => Marshalling.Unmarshal(marshalled, type));
        }

        [Fact]
        public void UnmarshalFakeStringWithCorrectTypeLengthAndEmptyRest()
        {
            Type type = typeof(SampleSerializable);
            string marshalled = "ObjectRemoter.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!ObjectRemoter.Test.MarshallingTest+SampleSerializable:0:";
            Assert.Throws<ArgumentException>(() => Marshalling.Unmarshal(marshalled, type));
        }

        [Fact]
        public void UnmarshalFakeStringWithBadContentsBase64()
        {
            Type type = typeof(SampleSerializable);
            string marshalled = "ObjectRemoter.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!ObjectRemoter.Test.MarshallingTest+SampleSerializable:6:foobar";
            Assert.Throws<ArgumentException>(() => Marshalling.Unmarshal(marshalled, type));
        }

        [Fact]
        public void UnmarshalFakeStringWithBadContentsLength()
        {
            Type type = typeof(SampleSerializable);
            string marshalled = "ObjectRemoter.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!ObjectRemoter.Test.MarshallingTest+SampleSerializable:8:Zm9vYmFy";
            Assert.Throws<ArgumentException>(() => Marshalling.Unmarshal(marshalled, type));
        }

        [Fact]
        public void UnmarshalFakeStringWithBadContents()
        {
            Type type = typeof(SampleSerializable);
            string marshalled = "ObjectRemoter.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!ObjectRemoter.Test.MarshallingTest+SampleSerializable:6:Zm9vYmFy";
            Assert.Throws<ArgumentException>(() => Marshalling.Unmarshal(marshalled, type));
        }

        [Fact]
        public void UnmarshalFakeStringUnsupportedClass()
        {
            Type type = typeof(SampleClass);
            string marshalled = "ObjectRemoter.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!ObjectRemoter.Test.MarshallingTest+SampleClass:6:Zm9vYmFy";
            Assert.Throws<ArgumentException>(() => Marshalling.Unmarshal(marshalled, type));
        }

        [Fact]
        public void UnmarshalFakeStringUnsupportedStruct()
        {
            Type type = typeof(SampleStruct);
            string marshalled = "ObjectRemoter.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!ObjectRemoter.Test.MarshallingTest+SampleStruct:6:Zm9vYmFy";
            Assert.Throws<ArgumentException>(() => Marshalling.Unmarshal(marshalled, type));
        }


        [Fact]
        public void MarshalAndUnmarshallRemotelyCloneableWithAbstractType()
        {
            var original = new SampleRemotelyCloneable("foo", 1,
                new SampleRemotelyCloneable("bar", 2, null));
            Assert.Throws<ArgumentException>(() => MarshalAndUnmarshall<IRemotelyCloneable>(original));
        }


        [Fact]
        public void MarshalRemotelyCloneableWithAbstractTypeAndUnmarshalWithConcreteType()
        {
            var original = new SampleRemotelyCloneable("foo", 1,
                new SampleRemotelyCloneable("bar", 2, null));
            // marshal with concrete type
            string marshalled = Marshalling.Marshal(original, typeof(IRemotelyCloneable));
            // unmarshal with abstract type
            // TODO: clear the specification - this might not throw exception
            Assert.Throws<ArgumentException>(() =>
                Marshalling.Unmarshal(marshalled, typeof(SampleRemotelyCloneable)));
        }

        #endregion

        #region Helper methods

        private static void MarshalAndUnmarshall<T>(T original)
        {
            Type type = typeof(T);
            string marshalled = Marshalling.Marshal(original, type);
            object result = Marshalling.Unmarshal(marshalled, type);
            Assert.Equal(original, result);
        }

        private static void MarshalAndUnmarshall<T>(T original, IEqualityComparer<T> comparer)
        {
            Type type = typeof(T);
            string marshalled = Marshalling.Marshal(original, type);
            object result = Marshalling.Unmarshal(marshalled, type);
            Assert.True(type.IsAssignableFrom(result.GetType()));
            Assert.Equal<T>((T)original, (T)result, comparer);
        }

        private static void MarshalAndUnmarshallArray<T>(T[] original)
        {
            Type type = typeof(T[]);
            string marshalled = Marshalling.Marshal(original, type);
            object result = Marshalling.Unmarshal(marshalled, type);
            Assert.True(result.GetType().IsArray);
            T[] convertedResult = Array.ConvertAll((object[])result, item => (T)item);
            Assert.Equal(original, convertedResult);
        }

        #endregion

        #region Testing classes and interfaces

        public interface ISampleRemotelyReferable : IRemotelyReferable
        {
            string Name { get; set; }
            ISampleRemotelyReferable Next { get; set; }
        }

        public class SampleRemotelyReferable : ISampleRemotelyReferable
        {
            public string Name { get; set; }
            public ISampleRemotelyReferable Next { get; set; }

            public SampleRemotelyReferable(string Name, ISampleRemotelyReferable Next)
            {
                this.Name = Name;
                this.Next = Next;
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                //       
                // See the full list of guidelines at
                //   http://go.microsoft.com/fwlink/?LinkID=85237  
                // and also the guidance for operator== at
                //   http://go.microsoft.com/fwlink/?LinkId=85238
                //

                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                ISampleRemotelyReferable other = (ISampleRemotelyReferable)obj;
                return object.Equals(this.Name, other.Name) &&
                    object.Equals(this.Next, other.Next);
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                int hashCode = 0;
                if (Name != null)
                {
                    hashCode ^= 7 * Name.GetHashCode();
                }
                if (Next != null)
                {
                    hashCode ^= 17 * Next.GetHashCode();
                }
                return hashCode;
            }
        }

        class SampleRemotelyReferableEquivalenceComparer : IEqualityComparer<ISampleRemotelyReferable>
        {
            public bool Equals(ISampleRemotelyReferable x, ISampleRemotelyReferable y)
            {
                if ((x == null) && (y == null))
                {
                    return true;
                }
                bool nameEquals = object.Equals(x.Name, y.Name);
                bool nextEquals = Equals(x.Next, y.Next);
                return nameEquals && nextEquals;
            }

            public int GetHashCode(ISampleRemotelyReferable obj)
            {
                throw new NotImplementedException();
            }
        }

        internal class SampleRemotelyCloneable : IRemotelyCloneable
        {
            public string text;
            public int number;
            public SampleRemotelyCloneable obj;

            public SampleRemotelyCloneable(string text, int number, SampleRemotelyCloneable obj)
            {
                this.text = text;
                this.number = number;
                this.obj = obj;
            }

            public string SerializeClone()
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(ms, text);
                    byte[] bytes = ms.GetBuffer();
                    int length = (int)ms.Length;
                    string result = length.ToString() + ":" + Convert.ToBase64String(bytes, 0, length);

                    ms.Seek(0, SeekOrigin.Begin);

                    binaryFormatter.Serialize(ms, number);
                    bytes = ms.GetBuffer();
                    length = (int)ms.Length;
                    result += "|" + length.ToString() + ":" + Convert.ToBase64String(bytes, 0, length);

                    ms.Seek(0, SeekOrigin.Begin);

                    if (obj != null)
                    {
                        string objSerialized = obj.SerializeClone();
                        result += "|" + objSerialized;
                    }
                    return result;
                }
            }

            public void DeserializeClone(string serialized)
            {
                int partCount = 3;
                string[] elements = serialized.Split(new[] { '|' }, partCount);
                if (elements.Length < 2)
                {
                    throw new ArgumentException("Bad number of parts.");
                }
                this.text = (string)DeserializeElement(elements[0]);
                this.number = (int)DeserializeElement(elements[1]);
                if (elements.Length > 2)
                {
                    this.obj = (SampleRemotelyCloneable)FormatterServices.GetUninitializedObject(typeof(SampleRemotelyCloneable));
                    this.obj.DeserializeClone(elements[2]);
                }
            }

            private static object DeserializeElement(string serialized)
            {
                int colonPos = serialized.IndexOf(':');
                if (colonPos < 0)
                {
                    throw new ArgumentException("Missing object contents length.", "serialized");
                }
                int length = int.Parse(serialized.Substring(0, colonPos));
                string base64 = serialized.Substring(colonPos + 1);
                if (string.IsNullOrEmpty(base64))
                {
                    throw new ArgumentException("Missing object contents.", "serialized");
                }
                byte[] bytes;
                try
                {
                    bytes = Convert.FromBase64String(base64);
                }
                catch (FormatException ex)
                {
                    throw new ArgumentException("Bad object contents.", "serialized", ex);
                }
                if (length != bytes.Length)
                {
                    throw new ArgumentException("Bad object contents length.", "serialized");
                }
                try
                {
                    using (MemoryStream ms = new MemoryStream(bytes, 0, length))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        object result = binaryFormatter.Deserialize(ms);
                        return result;
                    }
                }
                catch (SerializationException ex)
                {
                    throw new ArgumentException("Bad object contents.", "serialized", ex);
                }
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                //       
                // See the full list of guidelines at
                //   http://go.microsoft.com/fwlink/?LinkID=85237  
                // and also the guidance for operator== at
                //   http://go.microsoft.com/fwlink/?LinkId=85238
                //

                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                SampleRemotelyCloneable other = (SampleRemotelyCloneable)obj;
                return object.Equals(this.text, other.text) &&
                    (this.number == other.number) && object.Equals(this.obj, other.obj);
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                int hashCode = 13 * number;
                if (text != null)
                {
                    hashCode ^= 7 * text.GetHashCode();
                }
                if (obj != null)
                {
                    hashCode ^= 17 * obj.GetHashCode();
                }
                return hashCode;
            }
        }

        [Serializable]
        private class SampleSerializable
        {
            public string text;
            public int number;
            public SampleSerializable obj;

            public SampleSerializable(string text, int number, SampleSerializable obj)
            {
                this.text = text;
                this.number = number;
                this.obj = obj;
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                //       
                // See the full list of guidelines at
                //   http://go.microsoft.com/fwlink/?LinkID=85237  
                // and also the guidance for operator== at
                //   http://go.microsoft.com/fwlink/?LinkId=85238
                //

                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                SampleSerializable other = (SampleSerializable)obj;
                return object.Equals(this.text, other.text) &&
                    (this.number == other.number) && object.Equals(this.obj, other.obj);
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                int hashCode = 13 * number;
                if (text != null)
                {
                    hashCode ^= 7 * text.GetHashCode();
                }
                if (obj != null)
                {
                    hashCode ^= 17 * obj.GetHashCode();
                }
                return hashCode;
            }
        }

        private class SampleClass
        {
            public string text;
            public int number;

            public SampleClass(string text, int number)
            {
                this.text = text;
                this.number = number;
            }
        }

        private struct SampleStruct
        {
            public string text;
            public int number;

            public SampleStruct(string text, int number)
            {
                this.text = text;
                this.number = number;
            }
        }

        #endregion
    }
}
