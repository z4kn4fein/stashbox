using FastExpressionCompiler;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class FastExpressionCompilerIssue
    {
        static Test T { get; } = new Test();

        [Fact]
        public void Ensure_FastExpressionCompiler_Works()
        {
            var prop = typeof(FastExpressionCompilerIssue).GetProperty("T", BindingFlags.NonPublic | BindingFlags.Static);
            var memberLambda = Expression.MakeMemberAccess(null, prop).AsLambda<Func<object>>().CompileFast();

            Assert.NotNull(new StashboxContainer()
                .Register<Test>(c => c.WithFactory(memberLambda, true))
                .Resolve<Test>());
        }

        class Test { };
    }
}
