using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stashbox.Entity;
using Stashbox.Infrastructure.ContainerExtension;

namespace Stashbox.Tests
{
    [TestClass]
    public class BuildExtensionManagerTests
    {
        [TestMethod]
        public void BuildExtensionManagerTests_AddPostBuildExtension()
        {
            var post = new Mock<IPostBuildExtension>();
            using (var container = new StashboxContainer())
            {
                container.RegisterExtension(post.Object);
                var obj = new object();
                container.PrepareType<object>().WithFactory(() => obj).Register();

                post.Setup(p => p.PostBuild(obj, obj.GetType(), container.ContainerContext, It.IsAny<ResolutionInfo>(),
                    It.IsAny<TypeInformation>(), null)).Returns(obj).Verifiable();

                var inst = container.Resolve(typeof(object));
                post.Verify(p => p.Initialize(container.ContainerContext));
            }

            post.Verify(p => p.CleanUp());
        }

        [TestMethod]
        public void BuildExtensionManagerTests_AddRegistrationBuildExtension()
        {
            var post = new Mock<IRegistrationExtension>();
            using (var container = new StashboxContainer())
            {
                container.RegisterExtension(post.Object);
                container.RegisterInstance(new object());
                post.Verify(p => p.Initialize(container.ContainerContext));
                post.Verify(p => p.OnRegistration(container.ContainerContext,
                    It.IsAny<RegistrationInfo>(), null));
            }

            post.Verify(p => p.CleanUp());
        }

        [TestMethod]
        public void BuildExtensionManagerTests_CreateCopy()
        {
            var post = new Mock<IRegistrationExtension>();
            var post2 = new Mock<IRegistrationExtension>();
            using (var container = new StashboxContainer())
            {
                container.RegisterExtension(post.Object);

                post.Setup(p => p.CreateCopy()).Returns(post2.Object).Verifiable();

                using (var child = container.BeginScope())
                {
                    child.RegisterInstance(new object());

                    post2.Verify(p => p.Initialize(child.ContainerContext));
                    post2.Verify(p => p.OnRegistration(child.ContainerContext,
                        It.IsAny<RegistrationInfo>(), null));
                }

                post2.Verify(p => p.CleanUp());
            }

            post.Verify(p => p.CleanUp());
        }
    }
}
