using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Configuration;
using System.Collections.Generic;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class DictionariesGetResolvedToArraysOfKeyTypeByDefault
    {
        [TestMethod]
        public void Ensure_Dictionary_Resolves()
        {
            var container = new StashboxContainer(c => c.WithUnknownTypeResolution(c2 =>
                c2.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters)));
            var dict = container.Resolve<Dictionary<string, object>>();
        }
    }
}
