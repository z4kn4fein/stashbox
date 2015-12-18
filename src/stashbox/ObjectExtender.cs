using Ronin.Common;
using Sendstorm;
using Sendstorm.Infrastructure;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using System.Linq;
using System.Threading;

namespace Stashbox
{
    public class ObjectExtender : IObjectExtender, IMessageReceiver<RegistrationAdded>
    {
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IMessagePublisher messagePublisher;
        private readonly InjectionParameter[] injectionParameters;
        private SpinLock spinLock;

        private ImmutableArray<ResolutionMethod> injectionMethods;
        private ImmutableArray<ResolutionMember> injectionMembers;

        private bool hasInjectionMethods;
        private bool hasInjectionProperties;

        public ObjectExtender(IMetaInfoProvider metaInfoProvider,
            IMessagePublisher messagePublisher, InjectionParameter[] injectionParameters = null)
        {
            if (injectionParameters != null)
                this.injectionParameters = injectionParameters;

            spinLock = new SpinLock();

            this.metaInfoProvider = metaInfoProvider;
            this.messagePublisher = messagePublisher;

            this.messagePublisher.Subscribe<RegistrationAdded>(this, addedEvent => this.metaInfoProvider.SensitivityList.Contains(addedEvent.RegistrationInfo.TypeFrom), ExecutionTarget.BackgroundThread);
            this.CollectInjectionMembers();
        }

        public object ExtendObject(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            if (this.hasInjectionProperties)
            {
                var lockTaken = false;
                try
                {
                    this.spinLock.Enter(ref lockTaken);
                    var members = this.injectionMembers.CreateCopy();
                    this.spinLock.Exit();

                    var count = members.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var value = containerContext.ResolutionStrategy.EvaluateResolutionTarget(containerContext,
                            members[i].ResolutionTarget, resolutionInfo);
                        members[i].MemberSetter(instance, value);
                    }
                }
                finally
                {
                    if (this.spinLock.IsHeldByCurrentThread)
                        this.spinLock.Exit();
                }
            }

            if (!this.hasInjectionMethods) return instance;
            {
                var lockTaken = false;
                try
                {
                    this.spinLock.Enter(ref lockTaken);
                    var methods = this.injectionMethods.CreateCopy();
                    this.spinLock.Exit();

                    var count = methods.Count;
                    for (var i = 0; i < count; i++)
                    {
                        methods[i].MethodDelegate(resolutionInfo, instance);
                    }
                }
                finally
                {
                    if (this.spinLock.IsHeldByCurrentThread)
                        this.spinLock.Exit();
                }
            }

            return instance;
        }

        public void Receive(RegistrationAdded message)
        {
            this.CollectInjectionMembers();
        }

        private void CollectInjectionMembers()
        {
            var methods = this.metaInfoProvider.GetResolutionMethods(this.injectionParameters).ToArray();
            var members = this.metaInfoProvider.GetResolutionMembers(this.injectionParameters).ToArray();

            var lockTaken = false;
            try
            {
                this.spinLock.Enter(ref lockTaken);
                this.injectionMembers = new ImmutableArray<ResolutionMember>(members);
                this.injectionMethods = new ImmutableArray<ResolutionMethod>(methods);
                this.hasInjectionMethods = methods.Length > 0;
                this.hasInjectionProperties = members.Length > 0;
            }
            finally
            {
                if (lockTaken)
                    this.spinLock.Exit();
            }
        }

        public void CleanUp()
        {
            this.messagePublisher.UnSubscribe<RegistrationAdded>(this);
        }
    }
}
