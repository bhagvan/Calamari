﻿using System;
using Calamari.Deployment;
using Calamari.Deployment.Conventions;
using Calamari.Deployment.Journal;
using Calamari.Tests.Helpers;
using NSubstitute;
using NUnit.Framework;
using Octostache;

namespace Calamari.Tests.Fixtures.Conventions
{
    [TestFixture]
    public class ContributePreviousInstallationConventionFixture
    {
        IDeploymentJournal journal;
        JournalEntry previous;
        VariableDictionary variables;

        [SetUp]
        public void SetUp()
        {
            journal = Substitute.For<IDeploymentJournal>();
            journal.GetLatestInstallation(Arg.Any<string>()).Returns(_ => previous);
            variables = new VariableDictionary();
        }

        [Test]
        public void ShouldAddVariablesIfPreviousInstallation()
        {
            previous = new JournalEntry("123", "env", "proj", "pkg", "0.0.9", "rp01", DateTime.Now, "C:\\PackageOld.nupkg", "C:\\App", "C:\\MyApp", true);
            RunConvention();
            Assert.That(variables.Get(SpecialVariables.Tentacle.PreviousInstallation.OriginalInstalledPath), Is.EqualTo("C:\\App"));
        }

        [Test]
        public void ShouldAddEmptyVariablesIfNoPreviousInstallation()
        {
            previous = null;
            RunConvention();
            Assert.That(variables.Get(SpecialVariables.Tentacle.PreviousInstallation.OriginalInstalledPath), Is.EqualTo(""));
        }

        void RunConvention()
        {
            var convention = new ContributePreviousInstallationConvention(journal);
            convention.Install(new RunningDeployment("C:\\Package.nupkg", variables));
        }
    }
}
