using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stannieman.DI.UnitTests.TestTypes;
using System;
using System.Reflection;

namespace Stannieman.DI.UnitTests
{
    [TestClass]
    public class ContainerExtensionTests
    {
        private const string SampleKey = "Key";

        #region IsRegistered

        [TestMethod]
        public void IsRegistered_CorrectlyCallsContainerWithoutKey()
        {
            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.IsRegistered(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(true);

            mockContainer.Object.IsRegistered<IType1>();

            mockContainer.Verify(x => x.IsRegistered(It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.IsRegistered(typeof(IType1), null), Times.Once);
        }

        [TestMethod]
        public void IsRegistered_CorrectlyCallsContainerWithKey()
        {
            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.IsRegistered(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(true);

            mockContainer.Object.IsRegistered<IType1>(SampleKey);

            mockContainer.Verify(x => x.IsRegistered(It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.IsRegistered(typeof(IType1), SampleKey), Times.Once);
        }

        [TestMethod]
        public void IsRegistered_ReturnsResponseFromContainer()
        {
            var mockContainer = new Mock<IContainer>();
            mockContainer.SetupSequence(x => x.IsRegistered(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(true)
                .Returns(false);

            var actual1 = mockContainer.Object.IsRegistered<IType1>();
            var actual2 = mockContainer.Object.IsRegistered<IType1>();

            Assert.AreEqual(true, actual1);
            Assert.AreEqual(false, actual2);
        }

        #endregion

        #region IsSingleRegistered

        [TestMethod]
        public void IsSingleRegistered_CorrectlyCallsContainerWithoutKey()
        {
            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.IsSingleRegistered(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(true);

            mockContainer.Object.IsSingleRegistered<IType1>();

            mockContainer.Verify(x => x.IsSingleRegistered(It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.IsSingleRegistered(typeof(IType1), null), Times.Once);
        }

        [TestMethod]
        public void IsSingleRegistered_CorrectlyCallsContainerWithKey()
        {
            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.IsSingleRegistered(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(true);

            mockContainer.Object.IsSingleRegistered<IType1>(SampleKey);

            mockContainer.Verify(x => x.IsSingleRegistered(It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.IsSingleRegistered(typeof(IType1), SampleKey), Times.Once);
        }

        [TestMethod]
        public void IsSingleRegistered_ReturnsResponseFromContainer()
        {
            var mockContainer = new Mock<IContainer>();
            mockContainer.SetupSequence(x => x.IsSingleRegistered(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(true)
                .Returns(false);

            var actual1 = mockContainer.Object.IsSingleRegistered<IType1>();
            var actual2 = mockContainer.Object.IsSingleRegistered<IType1>();

            Assert.AreEqual(true, actual1);
            Assert.AreEqual(false, actual2);
        }

        #endregion

        #region RegisterPerRequest

        [TestMethod]
        public void RegisterPerRequest_CorrectlyCallsContainerWithoutKey()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterPerRequest<IType1, Type1>();

            mockContainer.Verify(x => x.RegisterPerRequest(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(IType1), typeof(Type1), null), Times.Once);
        }

        [TestMethod]
        public void RegisterPerRequest_CorrectlyCallsContainerWithKey()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterPerRequest<IType1, Type1>(SampleKey);

            mockContainer.Verify(x => x.RegisterPerRequest(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(IType1), typeof(Type1), SampleKey), Times.Once);
        }

        #endregion

        #region RegisterSingleton

        [TestMethod]
        public void RegisterSingleton_CorrectlyCallsContainerWithoutKey()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterSingleton<IType1, Type1>();

            mockContainer.Verify(x => x.RegisterSingleton(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(IType1), typeof(Type1), null), Times.Once);
        }

        [TestMethod]
        public void RegisterSingleton_CorrectlyCallsContainerWithKey()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterSingleton<IType1, Type1>(SampleKey);

            mockContainer.Verify(x => x.RegisterSingleton(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(IType1), typeof(Type1), SampleKey), Times.Once);
        }

        #endregion

        #region RegisterHandler

        [TestMethod]
        public void RegisterHandler_CorrectlyCallsContainerWithoutKey()
        {
            bool handlerCalled = false;
            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()))
                .Callback<Type, Func<HandlerParameter, object>, string>((x, y, z) => y.Invoke(null));

            mockContainer.Object.RegisterHandler<IType1>(x =>
            {
                handlerCalled = true;
                return new Type1();
            });

            Assert.IsTrue(handlerCalled);

            mockContainer.Verify(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterHandler(typeof(IType1), It.IsAny<Func<HandlerParameter, object>>(), null), Times.Once);
        }

        [TestMethod]
        public void RegisterHandler_CorrectlyCallsContainerWithKey()
        {
            bool handlerCalled = false;
            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()))
                .Callback<Type, Func<HandlerParameter, object>, string>((x, y, z) => y.Invoke(null));

            mockContainer.Object.RegisterHandler<IType1>(x =>
            {
                handlerCalled = true;
                return new Type1();
            }, SampleKey);

            Assert.IsTrue(handlerCalled);

            mockContainer.Verify(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterHandler(typeof(IType1), It.IsAny<Func<HandlerParameter, object>>(), SampleKey), Times.Once);
        }

        #endregion

        #region RegisterAllFromAssemblyByBaseType

        [TestMethod]
        public void RegisterAllFromAssemblyByBaseTypePerRequest_CorrectlyRegistersTypesWithInterface()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByBaseTypePerRequest(Assembly.GetExecutingAssembly(), typeof(IRegisterByBaseTypeInterface));

            mockContainer.Verify(x => x.RegisterPerRequest(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Exactly(3));
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(IRegisterByBaseTypeInterface), typeof(RegisterByBaseTypeType1), null), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(IRegisterByBaseTypeInterface), typeof(RegisterByBaseType2), null), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(IRegisterByBaseTypeInterface), typeof(RegisterByBaseType4), null), Times.Once);
        }

        [TestMethod]
        public void RegisterAllFromAssemblyByBaseTypePerRequest_CorrectlyRegistersTypesWithAbstractClass()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByBaseTypePerRequest(Assembly.GetExecutingAssembly(), typeof(RegisterByBaseType3));

            mockContainer.Verify(x => x.RegisterPerRequest(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(RegisterByBaseType3), typeof(RegisterByBaseType4), null), Times.Once);
        }

        [TestMethod]
        public void RegisterAllFromAssemblyByBaseTypePerRequest_CorrectlyRegistersTypesWithNormalClass()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByBaseTypePerRequest(Assembly.GetExecutingAssembly(), typeof(RegisterByBaseTypeType1));

            mockContainer.Verify(x => x.RegisterPerRequest(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Exactly(3));
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(RegisterByBaseTypeType1), typeof(RegisterByBaseTypeType1), null), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(RegisterByBaseTypeType1), typeof(RegisterByBaseType2), null), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(RegisterByBaseTypeType1), typeof(RegisterByBaseType4), null), Times.Once);
        }

        [TestMethod]
        public void RegisterAllFromAssemblyByBaseTypePerRequest_CorrectlyRegistersTypesWithKey()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByBaseTypePerRequest(Assembly.GetExecutingAssembly(), typeof(IRegisterByBaseTypeInterface), SampleKey);

            mockContainer.Verify(x => x.RegisterPerRequest(It.IsAny<Type>(), It.IsAny<Type>(), SampleKey), Times.Exactly(3));
        }

        [TestMethod]
        public void RegisterAllFromAssemblyByBaseTypeSingleton_CorrectlyRegistersTypesWithInterface()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByBaseTypeSingleton(Assembly.GetExecutingAssembly(), typeof(IRegisterByBaseTypeInterface));

            mockContainer.Verify(x => x.RegisterSingleton(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Exactly(3));
            mockContainer.Verify(x => x.RegisterSingleton(typeof(IRegisterByBaseTypeInterface), typeof(RegisterByBaseTypeType1), null), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(IRegisterByBaseTypeInterface), typeof(RegisterByBaseType2), null), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(IRegisterByBaseTypeInterface), typeof(RegisterByBaseType4), null), Times.Once);
        }

        [TestMethod]
        public void RegisterAllFromAssemblyByBaseTypeSingleton_CorrectlyRegistersTypesWithAbstractClass()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByBaseTypeSingleton(Assembly.GetExecutingAssembly(), typeof(RegisterByBaseType3));

            mockContainer.Verify(x => x.RegisterSingleton(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(RegisterByBaseType3), typeof(RegisterByBaseType4), null), Times.Once);
        }

        [TestMethod]
        public void RegisterAllFromAssemblyByBaseTypeSingleton_CorrectlyRegistersTypesWithNormalClass()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByBaseTypeSingleton(Assembly.GetExecutingAssembly(), typeof(RegisterByBaseTypeType1));

            mockContainer.Verify(x => x.RegisterSingleton(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Exactly(3));
            mockContainer.Verify(x => x.RegisterSingleton(typeof(RegisterByBaseTypeType1), typeof(RegisterByBaseTypeType1), null), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(RegisterByBaseTypeType1), typeof(RegisterByBaseType2), null), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(RegisterByBaseTypeType1), typeof(RegisterByBaseType4), null), Times.Once);
        }

        [TestMethod]
        public void RegisterAllFromAssemblyByBaseTypeSingleton_CorrectlyRegistersTypesWithKey()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByBaseTypeSingleton(Assembly.GetExecutingAssembly(), typeof(IRegisterByBaseTypeInterface), SampleKey);

            mockContainer.Verify(x => x.RegisterSingleton(It.IsAny<Type>(), It.IsAny<Type>(), SampleKey), Times.Exactly(3));
        }

        #endregion

        #region RegisterAllFromAssemblyByConvention

        [TestMethod]
        public void RegisterAllFromAssemblyByConventionPerRequest_CorrectlyRegistersTypesWithAllInterfacesAndSelf()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByConventionPerRequest(Assembly.GetExecutingAssembly(), "ByConvention");

            mockContainer.Verify(x => x.RegisterPerRequest(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Exactly(7));
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(RegisterByConventionType1ByConvention), typeof(RegisterByConventionType1ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(RegisterByConventionType3ByConvention), typeof(RegisterByConventionType3ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(IRegisterByConventionInterface1), typeof(RegisterByConventionType3ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(RegisterByConventionType4ByConvention), typeof(RegisterByConventionType4ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(IRegisterByConventionInterface1), typeof(RegisterByConventionType4ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(IRegisterByConventionInterface2), typeof(RegisterByConventionType4ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterPerRequest(typeof(RegisterByConventionType5ByConvention), typeof(RegisterByConventionType5ByConvention), null), Times.Once);
        }

        [TestMethod]
        public void RegisterAllFromAssemblyByConventionPerRequest_CorrectlyRegistersTypesWithKey()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByConventionPerRequest(Assembly.GetExecutingAssembly(), "ByConvention", SampleKey);

            mockContainer.Verify(x => x.RegisterPerRequest(It.IsAny<Type>(), It.IsAny<Type>(), SampleKey), Times.Exactly(7));
        }

        [TestMethod]
        public void RegisterAllFromAssemblyByConventionSingleton_CorrectlyRegistersTypesWithAllInterfacesAndSelf()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByConventionSingleton(Assembly.GetExecutingAssembly(), "ByConvention");

            mockContainer.Verify(x => x.RegisterSingleton(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Exactly(7));
            mockContainer.Verify(x => x.RegisterSingleton(typeof(RegisterByConventionType1ByConvention), typeof(RegisterByConventionType1ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(RegisterByConventionType3ByConvention), typeof(RegisterByConventionType3ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(IRegisterByConventionInterface1), typeof(RegisterByConventionType3ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(RegisterByConventionType4ByConvention), typeof(RegisterByConventionType4ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(IRegisterByConventionInterface1), typeof(RegisterByConventionType4ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(IRegisterByConventionInterface2), typeof(RegisterByConventionType4ByConvention), null), Times.Once);
            mockContainer.Verify(x => x.RegisterSingleton(typeof(RegisterByConventionType5ByConvention), typeof(RegisterByConventionType5ByConvention), null), Times.Once);
        }

        [TestMethod]
        public void RegisterAllFromAssemblyByConventionSingleton_CorrectlyRegistersTypesWithKey()
        {
            var mockContainer = new Mock<IContainer>();

            mockContainer.Object.RegisterAllFromAssemblyByConventionSingleton(Assembly.GetExecutingAssembly(), "ByConvention", SampleKey);

            mockContainer.Verify(x => x.RegisterSingleton(It.IsAny<Type>(), It.IsAny<Type>(), SampleKey), Times.Exactly(7));
        }

        #endregion

        #region RegisterInstance

        [TestMethod]
        public void RegisterInstance_CorrectlyCallsContainerWithoutKey()
        {
            object registerdInstance = null;

            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()))
                .Callback<Type, Func<HandlerParameter, object>, string>((x, y, z) => registerdInstance = y.Invoke(null));

            var instance = new Type1();
            mockContainer.Object.RegisterInstance(typeof(IType1), instance);

            Assert.AreSame(registerdInstance, instance);

            mockContainer.Verify(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterHandler(typeof(IType1), It.IsAny<Func<HandlerParameter, object>>(), null), Times.Once);
        }

        [TestMethod]
        public void RegisterInstance_CorrectlyCallsContainerWithKey()
        {
            object registerdInstance = null;

            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()))
                .Callback<Type, Func<HandlerParameter, object>, string>((x, y, z) => registerdInstance = y.Invoke(null));

            var instance = new Type1();
            mockContainer.Object.RegisterInstance(typeof(IType1), instance, SampleKey);

            Assert.AreSame(registerdInstance, instance);

            mockContainer.Verify(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterHandler(typeof(IType1), It.IsAny<Func<HandlerParameter, object>>(), SampleKey), Times.Once);
        }

        [TestMethod]
        public void RegisterInstanceT_CorrectlyCallsContainerWithoutKey()
        {
            object registerdInstance = null;

            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()))
                .Callback<Type, Func<HandlerParameter, object>, string>((x, y, z) => registerdInstance = y.Invoke(null));

            var instance = new Type1();
            mockContainer.Object.RegisterInstance<IType1>(instance);

            Assert.AreSame(registerdInstance, instance);

            mockContainer.Verify(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterHandler(typeof(IType1), It.IsAny<Func<HandlerParameter, object>>(), null), Times.Once);
        }

        [TestMethod]
        public void RegisterInstanceT_CorrectlyCallsContainerWithKey()
        {
            object registerdInstance = null;

            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()))
                .Callback<Type, Func<HandlerParameter, object>, string>((x, y, z) => registerdInstance = y.Invoke(null));

            var instance = new Type1();
            mockContainer.Object.RegisterInstance<IType1>(instance, SampleKey);

            Assert.AreSame(registerdInstance, instance);

            mockContainer.Verify(x => x.RegisterHandler(It.IsAny<Type>(), It.IsAny<Func<HandlerParameter, object>>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.RegisterHandler(typeof(IType1), It.IsAny<Func<HandlerParameter, object>>(), SampleKey), Times.Once);
        }

        #endregion

        #region GetSingleInstance

        [TestMethod]
        public void GetSingleInstance_CorrectlyCallsContainerWithoutKey()
        {
            var instance = new Type1();

            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.GetSingleInstance(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(instance);

            var actual = mockContainer.Object.GetSingleInstance<IType1>();

            Assert.AreSame(instance, actual);

            mockContainer.Verify(x => x.GetSingleInstance(It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.GetSingleInstance(typeof(IType1), null), Times.Once);
        }

        [TestMethod]
        public void GetSingleInstance_CorrectlyCallsContainerWithKey()
        {
            var instance = new Type1();

            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.GetSingleInstance(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(instance);

            var actual = mockContainer.Object.GetSingleInstance<IType1>(SampleKey);

            Assert.AreSame(instance, actual);

            mockContainer.Verify(x => x.GetSingleInstance(It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.GetSingleInstance(typeof(IType1), SampleKey), Times.Once);
        }

        #endregion

        #region GetAllInstances

        [TestMethod]
        public void GetAllInstances_CorrectlyCallsContainerWithoutKey()
        {
            var instances = new IType1[0];

            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.GetAllInstances(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(instances);

            var actual = mockContainer.Object.GetAllInstances<IType1>();

            Assert.AreSame(instances, actual);

            mockContainer.Verify(x => x.GetAllInstances(It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.GetAllInstances(typeof(IType1), null), Times.Once);
        }

        [TestMethod]
        public void GetAllInstances_CorrectlyCallsContainerWithKey()
        {
            var instances = new IType1[0];

            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(x => x.GetAllInstances(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(instances);

            var actual = mockContainer.Object.GetAllInstances<IType1>(SampleKey);

            Assert.AreSame(instances, actual);

            mockContainer.Verify(x => x.GetAllInstances(It.IsAny<Type>(), It.IsAny<string>()), Times.Once);
            mockContainer.Verify(x => x.GetAllInstances(typeof(IType1), SampleKey), Times.Once);
        }

        #endregion
    }
}
