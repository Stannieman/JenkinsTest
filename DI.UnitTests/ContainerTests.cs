using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stannieman.DI.UnitTests.TestTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stannieman.DI.UnitTests
{
    [TestClass]
    public class ContainerTests
    {
        private const string SampleKey = "Key";

        [TestMethod]
        public void GetsingleInstance_ReturnsDifferentInstanceWhenRegisteredAsPerRequest()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType1), typeof(Type1));

            var actual1 = target.GetSingleInstance(typeof(IType1));
            var actual2 = target.GetSingleInstance(typeof(IType1));

            Assert.AreSame(actual1, actual2);
        }

        [TestMethod]
        public void GetSingleInstance_ReturnsSameInstanceWhenRegisteredAsSingleton()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(IType1), typeof(Type1));

            var actual1 = target.GetSingleInstance(typeof(IType1));
            var actual2 = target.GetSingleInstance(typeof(IType1));

            Assert.AreSame(actual1, actual2);
        }

        [TestMethod]
        public void GetSingleInstance_ReturnsInstanceFromHandlerAndCallsHandlerEveryTime()
        {
            var instances = new[] { new Type1(), new Type1() };
            var index = 0;

            var target = new Container(new ContainerConfiguration());

            target.RegisterHandler(typeof(IType1), x => instances[index++]);

            var actual1 = target.GetSingleInstance(typeof(IType1));
            var actual2 = target.GetSingleInstance(typeof(IType1));

            Assert.AreSame(instances[0], actual1);
            Assert.AreSame(instances[1], actual2);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void GetSingleInstance_ReturnsDifferentInstanceWhenRegisteredAsSingletonForDifferentKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(IType1), typeof(Type1));
            target.RegisterSingleton(typeof(IType1), typeof(Type1), SampleKey);

            var actual1 = target.GetSingleInstance(typeof(IType1));
            var actual2 = target.GetSingleInstance(typeof(IType1), SampleKey);

            Assert.AreNotSame(actual1, actual2);
        }

        [TestMethod]
        public void RegisterSingleton_ThrowsWhenImplementationTypeAlreadyRegisteredAsPerRequest()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType1), typeof(Type1));

            var thrown = false;
            try
            {
                target.RegisterSingleton(typeof(I2Type1), typeof(Type1));
            }
            catch (ContainerException e)
            {
                thrown = true;
                Assert.AreEqual(ContainerErrorCodes.TypeAlreadyRegisteredAsPerRequest, e.ErrorCode);
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void RegisterPerRequest_ThrowsWhenImplementationTypeAlreadyRegisteredAsSingleton()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(IType1), typeof(Type1));

            var thrown = false;
            try
            {
                target.RegisterPerRequest(typeof(I2Type1), typeof(Type1));
            }
            catch (ContainerException e)
            {
                thrown = true;
                Assert.AreEqual(ContainerErrorCodes.TypeAlreadyRegisteredAsSingleton, e.ErrorCode);
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void RegisterSingleton_DoesNotThrowWhenImplementationTypeAlreadyRegisteredAsPerRequestForOtherKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType1), typeof(Type1));
            target.RegisterSingleton(typeof(I2Type1), typeof(Type1), SampleKey);
        }

        [TestMethod]
        public void RegisterPerRequest_DoesNotThrowWhenImplementationTypeAlreadyRegisteredAsSingletonForOtherKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(IType1), typeof(Type1));
            target.RegisterPerRequest(typeof(I2Type1), typeof(Type1), SampleKey);
        }

        [TestMethod]
        public void RegisterSingleton_DoesNotThrowForSameRegistration()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(IType1), typeof(Type1));
            target.RegisterSingleton(typeof(IType1), typeof(Type1));
        }

        [TestMethod]
        public void RegisterPerRequest_DoesNotThrowForSameRegistration()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType1), typeof(Type1));
            target.RegisterPerRequest(typeof(IType1), typeof(Type1));
        }

        [TestMethod]
        public void GetSingleInstance_ReturnsCorrectType()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType1), typeof(Type1));
            target.RegisterPerRequest(typeof(IType2_1), typeof(Type2_1));

            var actual = target.GetSingleInstance(typeof(IType2_1));

            Assert.AreEqual(typeof(Type2_1), actual.GetType());
        }

        [TestMethod]
        public void GetSingleInstance_ReturnsNullIfRegistrationTypeNotFound()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType1), typeof(Type1));

            var actual = target.GetSingleInstance(typeof(IType2_1));

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetSingleInstance_ThrowsIfMultipleImplementationTypesRegisteredAsSingleton()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(ICollectionType1), typeof(CollectionType1_1));
            target.RegisterSingleton(typeof(ICollectionType1), typeof(CollectionType1_2));

            var thrown = false;

            try
            {
                target.GetSingleInstance(typeof(ICollectionType1));
            }
            catch (ContainerException e)
            {
                thrown = true;
                Assert.AreEqual(ContainerErrorCodes.MultipleImplementationTypesRegistered, e.ErrorCode);
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void GetSingleInstance_DoesNotThrowIfSameImplementationTypeRegisterdAsSingletonAgain()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(ICollectionType1), typeof(CollectionType1_1));
            target.RegisterSingleton(typeof(ICollectionType1), typeof(CollectionType1_1));

            target.GetSingleInstance(typeof(ICollectionType1));
        }

        [TestMethod]
        public void GetSingleInstance_ThrowsIfMultipleImplementationTypesRegisteredAsPerRequest()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_1));
            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_2));

            var thrown = false;

            try
            {
                target.GetSingleInstance(typeof(ICollectionType1));
            }
            catch (ContainerException e)
            {
                thrown = true;
                Assert.AreEqual(ContainerErrorCodes.MultipleImplementationTypesRegistered, e.ErrorCode);
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void GetSingleInstance_DoesNotThrowIfSameImplementationTypeRegisterdAsPerRequestAgain()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_1));
            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_1));

            target.GetSingleInstance(typeof(ICollectionType1));
        }

        [TestMethod]
        public void GetAllInstances_ReturnsEmptyListIfRegistrationTypeNotFound()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType1), typeof(Type1));
            target.RegisterHandler(typeof(IType1), x => new Type1());

            var actual = target.GetAllInstances(typeof(IType2_1));

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetAllInstances_ReturnsEnumerableWithAllInstancesOfCorrectTypes()
        {
            var target = new Container(new ContainerConfiguration());

            var expectedHandlerType1 = new CollectionType1_5();
            var expectedHandlerType2 = new CollectionType1_5();

            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_1));
            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_2));
            target.RegisterSingleton(typeof(ICollectionType1), typeof(CollectionType1_3));
            target.RegisterSingleton(typeof(ICollectionType1), typeof(CollectionType1_4));
            target.RegisterHandler(typeof(ICollectionType1), x => expectedHandlerType1);
            target.RegisterHandler(typeof(ICollectionType1), x => expectedHandlerType2);

            var actual = target.GetAllInstances(typeof(ICollectionType1));

            Assert.AreEqual(6, actual.Count());
            Assert.IsTrue(actual.Any(x => x.GetType() == typeof(CollectionType1_1)));
            Assert.IsTrue(actual.Any(x => x.GetType() == typeof(CollectionType1_2)));
            Assert.IsTrue(actual.Any(x => x.GetType() == typeof(CollectionType1_3)));
            Assert.IsTrue(actual.Any(x => x.GetType() == typeof(CollectionType1_4)));
            Assert.IsTrue(actual.Any(x => x == expectedHandlerType1));
            Assert.IsTrue(actual.Any(x => x == expectedHandlerType2));
        }

        [TestMethod]
        public void GetSingleInstance_ReturnsDifferentInstanceForSingletonsWithDifferentKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(IType1), typeof(Type1));
            target.RegisterSingleton(typeof(IType1), typeof(Type1), SampleKey);

            var actual1 = target.GetSingleInstance(typeof(IType1));
            var actual2 = target.GetSingleInstance(typeof(IType1), SampleKey);

            Assert.AreNotSame(actual1, actual2);
        }

        [TestMethod]
        public void GetSingleInstance_ReturnsNullIfRegisteredAsSingletonWithDifferentKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(IType1), typeof(Type1));

            var actual = target.GetSingleInstance(typeof(IType1), SampleKey);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetSingleInstance_ReturnsNullIfRegisteredAsPerRequestWithDifferentKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType1), typeof(Type1));

            var actual = target.GetSingleInstance(typeof(IType1), SampleKey);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetSingleInstance_ReturnsNullIfRegisteredAsHandlerWithDifferentKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterHandler(typeof(IType1), x => new Type1());

            var actual = target.GetSingleInstance(typeof(IType1), SampleKey);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetAllInstances_ReturnsEmptyListIfRegisteredAsSingletonWithDifferentKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(IType1), typeof(Type1));

            var actual = target.GetAllInstances(typeof(IType1), SampleKey);

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetAllInstances_ReturnsEmptyListIfRegisteredAsPerRequestWithDifferentKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType1), typeof(Type1));

            var actual = target.GetAllInstances(typeof(IType1), SampleKey);

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetAllInstances_ReturnsEmptyListIfRegisteredAsHandlerWithDifferentKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterHandler(typeof(IType1), x => new Type1());

            var actual = target.GetAllInstances(typeof(IType1), SampleKey);

            Assert.IsFalse(actual.Any());
        }

        [TestMethod]
        public void GetSingleInstance_ResolvesConstructorDependenciesForSingletonRegistration()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterSingleton(typeof(IType2), typeof(Type2));
            target.RegisterPerRequest(typeof(IType2_1), typeof(Type2_1));

            var actual = (Type2)target.GetSingleInstance(typeof(IType2));

            Assert.IsNotNull(actual.Type2_1);
        }

        [TestMethod]
        public void GetSingleInstance_ResolvesConstructorDependenciesForPerRequestRegistration()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType2), typeof(Type2));
            target.RegisterPerRequest(typeof(IType2_1), typeof(Type2_1));

            var actual = (Type2)target.GetSingleInstance(typeof(IType2));

            Assert.IsNotNull(actual.Type2_1);
        }

        [TestMethod]
        public void GetSingleInstance_DoesNotResolvePropertyDependenciesForSingletonRegistrationWithPropertyInjectionOff()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = false });

            target.RegisterSingleton(typeof(IType2), typeof(Type2));
            target.RegisterPerRequest(typeof(IType2_1), typeof(Type2_1));
            target.RegisterPerRequest(typeof(IType2_2), typeof(Type2_2));

            var actual = (Type2)target.GetSingleInstance(typeof(IType2));

            Assert.IsNull(actual.Type2_2);
        }

        [TestMethod]
        public void GetSingleInstance_DoesNotResolvePropertyDependenciesForPerRequestRegistrationWithPropertyInjectionOff()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = false });

            target.RegisterPerRequest(typeof(IType2), typeof(Type2));
            target.RegisterPerRequest(typeof(IType2_1), typeof(Type2_1));
            target.RegisterPerRequest(typeof(IType2_2), typeof(Type2_2));

            var actual = (Type2)target.GetSingleInstance(typeof(IType2));

            Assert.IsNull(actual.Type2_2);
        }

        [TestMethod]
        public void GetSingleInstance_ResolvesPropertyDependenciesForSingletonRegistrationWithPropertyInjectionOon()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterSingleton(typeof(IType2), typeof(Type2));
            target.RegisterPerRequest(typeof(IType2_1), typeof(Type2_1));
            target.RegisterPerRequest(typeof(IType2_2), typeof(Type2_2));

            var actual = (Type2)target.GetSingleInstance(typeof(IType2));

            Assert.IsNotNull(actual.Type2_2);
        }

        [TestMethod]
        public void GetSingleInstance_ResolvesPropertyDependenciesForPerRequestRegistrationWithPropertyInjectionOn()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType2), typeof(Type2));
            target.RegisterPerRequest(typeof(IType2_1), typeof(Type2_1));
            target.RegisterPerRequest(typeof(IType2_2), typeof(Type2_2));

            var actual = (Type2)target.GetSingleInstance(typeof(IType2));

            Assert.IsNotNull(actual.Type2_2);
        }

        [TestMethod]
        public void GetSingleInstance_ResolvesCollectionConstructorDependencies()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(ITypeWithCollections), typeof(TypeWithCollections));

            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_1));
            target.RegisterSingleton(typeof(ICollectionType1), typeof(CollectionType1_2));

            target.RegisterPerRequest(typeof(ICollectionType2), typeof(CollectionType2_1));
            target.RegisterSingleton(typeof(ICollectionType2), typeof(CollectionType2_2));

            var actual = (TypeWithCollections)target.GetSingleInstance(typeof(ITypeWithCollections));

            Assert.AreEqual(2, actual.CollectionType1s.Count());
            Assert.IsTrue(actual.CollectionType1s.Any(x => x.GetType() == typeof(CollectionType1_1)));
            Assert.IsTrue(actual.CollectionType1s.Any(x => x.GetType() == typeof(CollectionType1_2)));
        }

        [TestMethod]
        public void GetSingleInstance_ResolvesCollectionPropertyDependencies()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(ITypeWithCollections), typeof(TypeWithCollections));

            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_1));
            target.RegisterSingleton(typeof(ICollectionType1), typeof(CollectionType1_2));

            target.RegisterPerRequest(typeof(ICollectionType2), typeof(CollectionType2_1));
            target.RegisterSingleton(typeof(ICollectionType2), typeof(CollectionType2_2));

            var actual = (TypeWithCollections)target.GetSingleInstance(typeof(ITypeWithCollections));

            Assert.AreEqual(2, actual.CollectionType2s.Count());
            Assert.IsTrue(actual.CollectionType2s.Any(x => x.GetType() == typeof(CollectionType2_1)));
            Assert.IsTrue(actual.CollectionType2s.Any(x => x.GetType() == typeof(CollectionType2_2)));
        }

        [TestMethod]
        public void GetSingleInstance_ResolvesConstructorDependenciesForKey()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType3), typeof(Type3));

            target.RegisterSingleton(typeof(IType3_1), typeof(Type3_1), "Key1");
            target.RegisterSingleton(typeof(IType3_1), typeof(Type3_1), "Key2");

            var actual = (Type3)target.GetSingleInstance(typeof(IType3));

            var expected = target.GetSingleInstance(typeof(IType3_1), "Key1");

            Assert.AreSame(expected, actual.Type3_1);
        }

        [TestMethod]
        public void GetSingleInstance_ResolvesPropertyDependenciesForKey()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType3), typeof(Type3));

            target.RegisterSingleton(typeof(IType3_1), typeof(Type3_1), "Key1");

            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl1), "Key1");
            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl1), "Key2");

            var actual = (Type3)target.GetSingleInstance(typeof(IType3));

            Assert.AreEqual(typeof(Type3_2Impl1), actual.Type3_2.GetType());
        }

        [TestMethod]
        public void GetSingleInstance_ResolvesCollectionConstructorDependenciesForKey()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(ITypeWithCollectionsByKey), typeof(TypeWithCollectionsByKey));

            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_1), "Key1");
            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_2), "Key2");

            var actual = (TypeWithCollectionsByKey)target.GetSingleInstance(typeof(ITypeWithCollectionsByKey));

            Assert.AreEqual(1, actual.CollectionType1s.Count());
            Assert.AreEqual(typeof(CollectionType1_1), actual.CollectionType1s.First().GetType());
        }

        [TestMethod]
        public void GetSingleInstance_ResolvesCollectionPropertyDependenciesForKey()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(ITypeWithCollectionsByKey), typeof(TypeWithCollectionsByKey));

            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_1), "Key1");
            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_2), "Key2");

            var actual = (TypeWithCollectionsByKey)target.GetSingleInstance(typeof(ITypeWithCollectionsByKey));

            Assert.AreEqual(1, actual.CollectionType1s.Count());
            Assert.AreEqual(typeof(CollectionType1_2), actual.CollectionType2s.First().GetType());
        }

        [TestMethod]
        public void GetSingleInstance_ThrowsIfConstructorDepenencyCannotBeResolved()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType2), typeof(Type2));

            target.RegisterPerRequest(typeof(IType2_2), typeof(Type2_2));

            var thrown = false;
            try
            {
                target.GetSingleInstance(typeof(IType2));
            }
            catch (ContainerException e)
            {
                thrown = true;
                Assert.AreEqual(ContainerErrorCodes.NoEligibleConstructorFound, e.ErrorCode);
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void GetSingleInstance_LeavesPropertyDepenenciesNullIfCannotBeResolved()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType2), typeof(Type2));

            target.RegisterPerRequest(typeof(IType2_1), typeof(Type2_1));

            var actual = (Type2)target.GetSingleInstance(typeof(IType2));

            Assert.IsNull(actual.Type2_2);
        }

        [TestMethod]
        public void GetSingleInstance_InjectsEmptyCollectionInConstructorWhenNoTypesRegistered()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(ITypeWithCollections), typeof(TypeWithCollections));

            target.RegisterPerRequest(typeof(ICollectionType2), typeof(CollectionType2_1));

            var actual = (TypeWithCollections)target.GetSingleInstance(typeof(ITypeWithCollections));

            Assert.IsFalse(actual.CollectionType1s.Any());
        }

        [TestMethod]
        public void GetSingleInstance_InjectsEmptyCollectionInPropertyWhenNoTypesRegistered()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(ITypeWithCollections), typeof(TypeWithCollections));

            target.RegisterPerRequest(typeof(ICollectionType1), typeof(CollectionType1_1));

            var actual = (TypeWithCollections)target.GetSingleInstance(typeof(ITypeWithCollections));

            Assert.IsFalse(actual.CollectionType2s.Any());
        }

        [TestMethod]
        public void GetSingleInstance_DoesNotOverwriteNonNullProperty()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType4), typeof(Type4));
            target.RegisterSingleton(typeof(IType4_1), typeof(Type4_1));

            var notExpected = target.GetSingleInstance(typeof(IType4_1));

            var actual = (Type4)target.GetSingleInstance(typeof(IType4));

            Assert.AreNotSame(notExpected, actual.Type4_1_1);
        }

        [TestMethod]
        public void GetSingleInstance_InjectsOnlyPublicProperties()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType4), typeof(Type4));
            target.RegisterPerRequest(typeof(IType4_1), typeof(Type4_1));

            var actual = (Type4)target.GetSingleInstance(typeof(IType4));

            Assert.IsNull(actual.GetType4_1_2());
        }

        [TestMethod]
        public void GetSingleInstance_DoesNotInjectStaticProperty()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType4), typeof(Type4));
            target.RegisterPerRequest(typeof(IType4_1), typeof(Type4_1));

            var actual = (Type4)target.GetSingleInstance(typeof(IType4));

            Assert.IsNull(Type4.Type4_1_3);
        }

        [TestMethod]
        public void GetSingleInstance_DoesNotInjectFields()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType4), typeof(Type4));
            target.RegisterPerRequest(typeof(IType4_1), typeof(Type4_1));

            var actual = (Type4)target.GetSingleInstance(typeof(IType4));

            Assert.IsNull(actual.Type4_1_4);
        }

        [TestMethod]
        public void GetSingleInstance_DoesNotInjectPropertiesWithoutPublicGetter()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType4), typeof(Type4));
            target.RegisterPerRequest(typeof(IType4_1), typeof(Type4_1));

            var actual = (Type4)target.GetSingleInstance(typeof(IType4));

            Assert.IsNull(actual.GetType4_1_5());
        }

        [TestMethod]
        public void GetSingleInstance_DoesNotInjectPropertiesWithoutPublicSetter()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IType4), typeof(Type4));
            target.RegisterPerRequest(typeof(IType4_1), typeof(Type4_1));

            var actual = (Type4)target.GetSingleInstance(typeof(IType4));

            Assert.IsNull(actual.Type4_1_6);
        }

        [TestMethod]
        public void IsSingleRegistered_ReturnsTrueIfOnlyOneImplementationTypeRegisteredForRegistrationType()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl1));

            var actual = target.IsSingleRegistered(typeof(IType3_2));

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsSingleRegistered_ReturnsFalseIfMoreThanOneImplementationTypeRegisteredForRegistrationType()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl1));
            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl2));

            var actual = target.IsSingleRegistered(typeof(IType3_2));

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsSingleRegistered_ReturnsTrueIfImplementationTypeRegisteredForRegistrationTypeWithDifferentKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl1));
            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl2), SampleKey);

            var actual = target.IsSingleRegistered(typeof(IType3_2));

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsSingleRegistered_ReturnsTrueIfOnlyOneImplementationTypeRegisteredForRegistrationTypeWithKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl1));
            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl2), SampleKey);

            var actual = target.IsSingleRegistered(typeof(IType3_2), SampleKey);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsSingleRegistered_ReturnsFalseIfNoImplementationTypeRegisteredForRegistrationType()
        {
            var target = new Container(new ContainerConfiguration());

            var actual = target.IsSingleRegistered(typeof(IType3_2));

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsSingleRegistered_ReturnsFalseIfNoImplementationTypeRegisteredForRegistrationTypeWithKey()
        {
            var target = new Container(new ContainerConfiguration());

            var actual = target.IsSingleRegistered(typeof(IType3_2), SampleKey);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsRegistered_ReturnsTrueIfOnlyOneImplementationTypeRegisteredForRegistrationType()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl1));

            var actual = target.IsRegistered(typeof(IType3_2));

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsRegistered_ReturnsTrueIfMoreThanOneImplementationTypeRegisteredForRegistrationType()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl1));
            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl2));

            var actual = target.IsRegistered(typeof(IType3_2));

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsRegistered_ReturnsTrueIfImplementationTypeRegisteredForRegistrationTypeWithKey()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(IType3_2), typeof(Type3_2Impl1), SampleKey);

            var actual = target.IsRegistered(typeof(IType3_2), SampleKey);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsRegistered_ReturnsFalseIfNoImplementationTypeRegisteredForRegistrationType()
        {
            var target = new Container(new ContainerConfiguration());

            var actual = target.IsRegistered(typeof(IType3_2));

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsRegistered_ReturnsFalseIfNoImplementationTypeRegisteredForRegistrationTypeWithKey()
        {
            var target = new Container(new ContainerConfiguration());

            var actual = target.IsRegistered(typeof(IType3_2), SampleKey);

            Assert.IsFalse(actual);
        }

        #region ReleaseInstance

        [TestMethod]
        public void ReleaseInstance_DisposesAllComponentsWhenResolvedComponentDisposable()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IDisposableType), typeof(DisposableType));
            target.RegisterPerRequest(typeof(IDisposableType_1), typeof(DisposableType_1));
            target.RegisterPerRequest(typeof(INonDisposableType_2), typeof(NonDisposableType_2));
            target.RegisterPerRequest(typeof(IDisposableType_2_1), typeof(DisposableType_2_1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl2));

            var instance = target.GetSingleInstance(typeof(IDisposableType));

            target.ReleaseInstance(instance);

            Assert.IsTrue(((DisposableType)instance).Disposed);
            Assert.IsTrue(((DisposableType_1)((DisposableType)instance).DisposableType_1).Disposed);
            Assert.IsTrue(((DisposableType_2_1)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_1).Disposed);
            Assert.IsTrue(((DisposableType_2_2Impl1)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(0)).Disposed);
            Assert.IsTrue(((DisposableType_2_2Impl2)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(1)).Disposed);
        }

        [TestMethod]
        public void ReleaseInstance_DisposesAllComponentsWhenResolvedComponentNotDisposable()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(INonDisposableType), typeof(NonDisposableType));
            target.RegisterPerRequest(typeof(IDisposableType_1), typeof(DisposableType_1));
            target.RegisterPerRequest(typeof(INonDisposableType_2), typeof(NonDisposableType_2));
            target.RegisterPerRequest(typeof(IDisposableType_2_1), typeof(DisposableType_2_1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl2));

            var instance = target.GetSingleInstance(typeof(INonDisposableType));

            target.ReleaseInstance(instance);

            Assert.IsTrue(((DisposableType_1)((NonDisposableType)instance).DisposableType_1).Disposed);
            Assert.IsTrue(((DisposableType_2_1)((NonDisposableType_2)((NonDisposableType)instance).NonDisposableType_2).DisposableType_2_1).Disposed);
            Assert.IsTrue(((DisposableType_2_2Impl1)((NonDisposableType_2)((NonDisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(0)).Disposed);
            Assert.IsTrue(((DisposableType_2_2Impl2)((NonDisposableType_2)((NonDisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(1)).Disposed);
        }

        [TestMethod]
        public void ReleaseInstance_DoesNotDisposeSingletons()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IDisposableType), typeof(DisposableType));
            target.RegisterSingleton(typeof(IDisposableType_1), typeof(DisposableType_1));
            target.RegisterPerRequest(typeof(INonDisposableType_2), typeof(NonDisposableType_2));
            target.RegisterSingleton(typeof(IDisposableType_2_1), typeof(DisposableType_2_1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl1));
            target.RegisterSingleton(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl2));

            var instance = target.GetSingleInstance(typeof(IDisposableType));

            target.ReleaseInstance(instance);

            Assert.IsTrue(((DisposableType)instance).Disposed);
            Assert.IsFalse(((DisposableType_1)((DisposableType)instance).DisposableType_1).Disposed);
            Assert.IsFalse(((DisposableType_2_1)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_1).Disposed);
            Assert.IsTrue(((DisposableType_2_2Impl1)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(0)).Disposed);
            Assert.IsFalse(((DisposableType_2_2Impl2)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(1)).Disposed);
        }

        [TestMethod]
        public void ReleaseInstance_DoesNotDisposeDepencenciesOfSingletons()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IDisposableType), typeof(DisposableType));
            target.RegisterPerRequest(typeof(IDisposableType_1), typeof(DisposableType_1));
            target.RegisterSingleton(typeof(INonDisposableType_2), typeof(NonDisposableType_2));
            target.RegisterPerRequest(typeof(IDisposableType_2_1), typeof(DisposableType_2_1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl2));

            var instance = target.GetSingleInstance(typeof(IDisposableType));

            target.ReleaseInstance(instance);

            Assert.IsTrue(((DisposableType)instance).Disposed);
            Assert.IsTrue(((DisposableType_1)((DisposableType)instance).DisposableType_1).Disposed);
            Assert.IsFalse(((DisposableType_2_1)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_1).Disposed);
            Assert.IsFalse(((DisposableType_2_2Impl1)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(0)).Disposed);
            Assert.IsFalse(((DisposableType_2_2Impl2)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(1)).Disposed);
        }

        [TestMethod]
        public void ReleaseInstance_DoesNotDisposeInstancesFromHandlers()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IDisposableType), typeof(DisposableType));
            target.RegisterHandler(typeof(IDisposableType_1), x => new DisposableType_1());

            var instance = target.GetSingleInstance(typeof(IDisposableType));

            target.ReleaseInstance(instance);

            Assert.IsFalse(((DisposableType_1)((DisposableType)instance).DisposableType_1).Disposed);
        }

#if DEBUG
        [Ignore]
#endif
        [TestMethod]
        public void ReleaseInstance_DoesNotKeepReferenceToReleasedComponents()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IDisposableType), typeof(DisposableType));
            target.RegisterPerRequest(typeof(IDisposableType_1), typeof(DisposableType_1));
            target.RegisterPerRequest(typeof(INonDisposableType_2), typeof(NonDisposableType_2));
            target.RegisterPerRequest(typeof(IDisposableType_2_1), typeof(DisposableType_2_1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl2));

            var instance = (IDisposableType)target.GetSingleInstance(typeof(IDisposableType));

            var instanceReference = new WeakReference<IDisposableType>(instance);
            var disposableType_1 = new WeakReference<IDisposableType_1>(((DisposableType)instance).DisposableType_1);
            var nonDisposableType_2 = new WeakReference<INonDisposableType_2>(((DisposableType)instance).NonDisposableType_2);
            var disposableType_2_1 = new WeakReference<IDisposableType_2_1>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_1);
            var disposableType_2_2s = new WeakReference<IEnumerable<IDisposableType_2_2>>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s);
            var disposableType_2_2Impl1 = new WeakReference<IDisposableType_2_2>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(0));
            var disposableType_2_2Impl2 = new WeakReference<IDisposableType_2_2>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(1));

            target.ReleaseInstance(instance);

            instance = null;

            GC.Collect();

            Assert.IsFalse(instanceReference.TryGetTarget(out var instance1));
            Assert.IsFalse(disposableType_1.TryGetTarget(out var instance2));
            Assert.IsFalse(nonDisposableType_2.TryGetTarget(out var instance3));
            Assert.IsFalse(disposableType_2_1.TryGetTarget(out var intsance4));
            Assert.IsFalse(disposableType_2_2s.TryGetTarget(out var instance5));
            Assert.IsFalse(disposableType_2_2Impl1.TryGetTarget(out var instance6));
            Assert.IsFalse(disposableType_2_2Impl2.TryGetTarget(out var instance7));

            // Do at least 1 action on target to make sure entire target is not collected.
            target.Dispose();
        }

#if DEBUGG
        [Ignore]
#endif
        [TestMethod]
        public void ReleaseInstance_DoesKeepReferenceToSingletons()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IDisposableType), typeof(DisposableType));
            target.RegisterSingleton(typeof(IDisposableType_1), typeof(DisposableType_1));
            target.RegisterPerRequest(typeof(INonDisposableType_2), typeof(NonDisposableType_2));
            target.RegisterSingleton(typeof(IDisposableType_2_1), typeof(DisposableType_2_1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl2));

            var instance = (IDisposableType)target.GetSingleInstance(typeof(IDisposableType));

            var instanceReference = new WeakReference<IDisposableType>(instance);
            var disposableType_1 = new WeakReference<IDisposableType_1>(((DisposableType)instance).DisposableType_1);
            var nonDisposableType_2 = new WeakReference<INonDisposableType_2>(((DisposableType)instance).NonDisposableType_2);
            var disposableType_2_1 = new WeakReference<IDisposableType_2_1>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_1);
            var disposableType_2_2s = new WeakReference<IEnumerable<IDisposableType_2_2>>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s);
            var disposableType_2_2Impl1 = new WeakReference<IDisposableType_2_2>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(0));
            var disposableType_2_2Impl2 = new WeakReference<IDisposableType_2_2>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(1));

            target.ReleaseInstance(instance);

            instance = null;

            GC.Collect();

            Assert.IsFalse(instanceReference.TryGetTarget(out var instance1));
            Assert.IsTrue(disposableType_1.TryGetTarget(out var instance2));
            Assert.IsFalse(nonDisposableType_2.TryGetTarget(out var instance3));
            Assert.IsTrue(disposableType_2_1.TryGetTarget(out var intsance4));
            Assert.IsFalse(disposableType_2_2s.TryGetTarget(out var instance5));
            Assert.IsFalse(disposableType_2_2Impl1.TryGetTarget(out var instance6));
            Assert.IsFalse(disposableType_2_2Impl2.TryGetTarget(out var instance7));

            // Do at least 1 action on target to make sure entire target is not collected.
            target.Dispose();
        }

#if DEBUGG
        [Ignore]
#endif
        [TestMethod]
        public void Release_DoesNotKeepReferencesOfHandlerInstances()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            var handlerInstance = new DisposableType_1();
            var disposable_type1 = new WeakReference<IDisposableType_1>(handlerInstance);

            target.RegisterPerRequest(typeof(IDisposableType), typeof(DisposableType));
            target.RegisterHandler(typeof(IDisposableType_1), x => handlerInstance);

            handlerInstance = null;

            var instance = (IDisposableType)target.GetSingleInstance(typeof(IDisposableType));

            target.ReleaseInstance(instance);

            instance = null;

            GC.Collect();

            Assert.IsFalse(disposable_type1.TryGetTarget(out var instance1));

            // Do at least 1 action on target to make sure entire target is not collected.
            target.Dispose();
        }

        #endregion

        #region Dispose

        [TestMethod]
        public void Dispose_DisposesAllComponents()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IDisposableType), typeof(DisposableType));
            target.RegisterPerRequest(typeof(IDisposableType_1), typeof(DisposableType_1));
            target.RegisterSingleton(typeof(INonDisposableType_2), typeof(NonDisposableType_2));
            target.RegisterPerRequest(typeof(IDisposableType_2_1), typeof(DisposableType_2_1));
            target.RegisterSingleton(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl2));

            var instance = target.GetSingleInstance(typeof(IDisposableType));

            target.ReleaseInstance(instance);

            target.Dispose();

            Assert.IsTrue(((DisposableType)instance).Disposed);
            Assert.IsTrue(((DisposableType_1)((DisposableType)instance).DisposableType_1).Disposed);
            Assert.IsTrue(((DisposableType_2_1)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_1).Disposed);
            Assert.IsTrue(((DisposableType_2_2Impl1)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(0)).Disposed);
            Assert.IsTrue(((DisposableType_2_2Impl2)((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(1)).Disposed);
        }

#if DEBUGG
        [Ignore]
#endif
        [TestMethod]
        public void Dispose_DoesNotKeepReferencesOfSingletons()
        {
            var target = new Container(new ContainerConfiguration { EnablePropertyInjection = true });

            target.RegisterPerRequest(typeof(IDisposableType), typeof(DisposableType));
            target.RegisterSingleton(typeof(IDisposableType_1), typeof(DisposableType_1));
            target.RegisterPerRequest(typeof(INonDisposableType_2), typeof(NonDisposableType_2));
            target.RegisterSingleton(typeof(IDisposableType_2_1), typeof(DisposableType_2_1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl1));
            target.RegisterPerRequest(typeof(IDisposableType_2_2), typeof(DisposableType_2_2Impl2));

            var instance = (IDisposableType)target.GetSingleInstance(typeof(IDisposableType));

            var instanceReference = new WeakReference<IDisposableType>(instance);
            var disposableType_1 = new WeakReference<IDisposableType_1>(((DisposableType)instance).DisposableType_1);
            var nonDisposableType_2 = new WeakReference<INonDisposableType_2>(((DisposableType)instance).NonDisposableType_2);
            var disposableType_2_1 = new WeakReference<IDisposableType_2_1>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_1);
            var disposableType_2_2s = new WeakReference<IEnumerable<IDisposableType_2_2>>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s);
            var disposableType_2_2Impl1 = new WeakReference<IDisposableType_2_2>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(0));
            var disposableType_2_2Impl2 = new WeakReference<IDisposableType_2_2>(((NonDisposableType_2)((DisposableType)instance).NonDisposableType_2).DisposableType_2_2s.ElementAt(1));

            target.ReleaseInstance(instance);

            instance = null;

            target.Dispose();

            GC.Collect();

            Assert.IsFalse(instanceReference.TryGetTarget(out var instance1));
            Assert.IsFalse(disposableType_1.TryGetTarget(out var instance2));
            Assert.IsFalse(nonDisposableType_2.TryGetTarget(out var instance3));
            Assert.IsFalse(disposableType_2_1.TryGetTarget(out var intsance4));
            Assert.IsFalse(disposableType_2_2s.TryGetTarget(out var instance5));
            Assert.IsFalse(disposableType_2_2Impl1.TryGetTarget(out var instance6));
            Assert.IsFalse(disposableType_2_2Impl2.TryGetTarget(out var instance7));

            // Do at least 1 action on target to make sure entire target is not collected.
            target.Dispose();
        }

        #endregion

        [TestMethod]
        public void GetSingleInstance_UsesCorrectConstructor()
        {
            var target = new Container(new ContainerConfiguration());

            target.RegisterPerRequest(typeof(ITypeWithConstructors), typeof(TypeWithConstructors));
            target.RegisterPerRequest(typeof(ITypeWithConstructors_1), typeof(TypeWithConstructors_1));
            target.RegisterSingleton(typeof(ITypeWithConstructors_2), typeof(TypeWithConstructors_2));
            target.RegisterPerRequest(typeof(ITypeWithConstructors_4), typeof(TypeWithConstructors_4));

            var actual = (TypeWithConstructors)target.GetSingleInstance(typeof(ITypeWithConstructors));

            Assert.IsTrue(actual.CorrectConstructorCalled);
        }
    }
}
