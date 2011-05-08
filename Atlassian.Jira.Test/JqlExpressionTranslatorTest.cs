﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Atlassian.Jira.Linq;
using Moq;

namespace Atlassian.Jira.Test
{
    public class JqlExpressionTranslatorTest
    {
        private JqlExpressionTranslator _translator;

        private Jira CreateJiraInstance()
        {
            _translator = new JqlExpressionTranslator();
            var soapClient = new Mock<IJiraSoapServiceClient>();

            soapClient.Setup(r => r.GetIssuesFromJqlSearch(
                                        It.IsAny<string>(),
                                        It.IsAny<string>(),
                                        It.IsAny<int>())).Returns(new RemoteIssue[0]);

            return new Jira(_translator, soapClient.Object, "username", "password");
        }

        [Fact]
        public void TranslateEqualsOperatorForNonString()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                         where i.Votes == 5
                         select i).ToArray();


            Assert.Equal("Votes = 5", _translator.Jql);
        }

        [Fact]
        public void TranslateEqualsOperatorForStringWithFuzzyEquality()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Summary == "Foo"
                          select i).ToArray();


            Assert.Equal("Summary ~ \"Foo\"", _translator.Jql);
        }

        [Fact]
        public void TranslateEqualsOperatorForString()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Assignee == "Foo"
                          select i).ToArray();


            Assert.Equal("Assignee = \"Foo\"", _translator.Jql);
        }



        [Fact]
        public void TranslateNotEqualsOperatorForNonString()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Votes != 5
                          select i).ToArray();


            Assert.Equal("Votes != 5", _translator.Jql);
        }

        [Fact]
        public void TranslateNotEqualsOperatorForStringWithFuzzyEquality()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Summary != "Foo"
                          select i).ToArray();

            Assert.Equal("Summary !~ \"Foo\"", _translator.Jql);
        }

        [Fact]
        public void TranslateNotEqualsOperatorForString()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Assignee != "Foo"
                          select i).ToArray();

            Assert.Equal("Assignee != \"Foo\"", _translator.Jql);
        }

        [Fact]
        public void TranslateGreaterThanOperator()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Votes > 5
                          select i).ToArray();


            Assert.Equal("Votes > 5", _translator.Jql);
        }

        [Fact]
        public void TranslateGreaterThanEqualsOperator()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Votes >= 5
                          select i).ToArray();

            Assert.Equal("Votes >= 5", _translator.Jql);
        }

        [Fact]
        public void TranslateLessThanOperator()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Votes < 5
                          select i).ToArray();

            Assert.Equal("Votes < 5", _translator.Jql);
        }

        [Fact]
        public void TranslateLessThanOrEqualsOperator()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Votes <= 5
                          select i).ToArray();

            Assert.Equal("Votes <= 5", _translator.Jql);
        }

        [Fact]
        public void TranslateAndKeyWord()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Votes > 5 && i.Votes < 10 
                          select i).ToArray();

            Assert.Equal("(Votes > 5 and Votes < 10)", _translator.Jql);
        }

        [Fact]
        public void TranslateOrKeyWord()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Votes > 5 || i.Votes < 10
                          select i).ToArray();

            Assert.Equal("(Votes > 5 or Votes < 10)", _translator.Jql);
        }

        [Fact]
        public void TranslateAssociativeGrouping()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Votes > 5 && (i.Votes < 10 || i.Votes == 20)
                          select i).ToArray();

            Assert.Equal("(Votes > 5 and (Votes < 10 or Votes = 20))", _translator.Jql);
        }

        [Fact]
        public void TranslateIsOperatorForEmptyString()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Summary == ""
                          select i).ToArray();

            Assert.Equal("Summary is empty", _translator.Jql);
        }

        [Fact]
        public void TranslateIsNotOperatorForEmptyString()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Summary != ""
                          select i).ToArray();

            Assert.Equal("Summary is not empty", _translator.Jql);
        }

        [Fact]
        public void TranslateIsOperatorForNull()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Summary == null
                          select i).ToArray();

            Assert.Equal("Summary is null", _translator.Jql);
        }

        [Fact]
        public void TranslateGreaterThanOperatorWhenUsingComparableFieldWithString()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Priority > "foo"
                          select i).ToArray();

            Assert.Equal("Priority > \"foo\"", _translator.Jql);
        }

        [Fact]
        public void TranslateEqualsOperatorWhenUsingComparableFieldWithString()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Priority == "foo"
                          select i).ToArray();

            Assert.Equal("Priority = \"foo\"", _translator.Jql);
        }

        [Fact]
        public void TranslateGreaterThanOperatorWhenUsingComparableFieldWithInt()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Priority > 1
                          select i).ToArray();

            Assert.Equal("Priority > 1", _translator.Jql);
        }

        [Fact]
        public void TranslateEqualsOperatorWhenUsingComparableFieldWithInt()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Priority == 1
                          select i).ToArray();

            Assert.Equal("Priority = 1", _translator.Jql);
        }

        [Fact]
        public void TranslateOrderBy()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Priority == 1
                          orderby i.Created
                          select i).ToArray();

            Assert.Equal("Priority = 1 order by Created", _translator.Jql);
        }

        [Fact]
        public void TranslateOrderByDescending()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Priority == 1
                          orderby i.Created descending
                          select i).ToArray();

            Assert.Equal("Priority = 1 order by Created desc", _translator.Jql);
        }

        [Fact]
        public void TranslateMultipleOrderBys()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Priority == 1
                          orderby i.Created, i.DueDate
                          select i).ToArray();

            Assert.Equal("Priority = 1 order by Created, DueDate", _translator.Jql);
        }

        [Fact]
        public void TranslateMultipleOrderByDescending()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Priority == 1
                          orderby i.Created, i.DueDate descending
                          select i).ToArray();

            Assert.Equal("Priority = 1 order by Created, DueDate desc", _translator.Jql);
        }

        [Fact]
        public void TranslateNewDateTime()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Created > new DateTime(2011,1,1)
                          select i).ToArray();

            Assert.Equal("Created > \"2011/01/01\"", _translator.Jql);
        }

        [Fact]
        public void TranslateMultipleDateTimes()
        {
            var jira = CreateJiraInstance();

            var issues = (from i in jira.Issues
                          where i.Created > new DateTime(2011, 1, 1) && i.Created < new DateTime(2012,1,1) 
                          select i).ToArray();

            Assert.Equal("(Created > \"2011/01/01\" and Created < \"2012/01/01\")", _translator.Jql);
        }

        [Fact]
        public void TranslateLocalStringVariables()
        {
            var jira = CreateJiraInstance();
            var user = "farmas";

            var issues = (from i in jira.Issues
                          where i.Assignee == user
                          select i).ToArray();

            Assert.Equal("Assignee = \"farmas\"", _translator.Jql);
        }

        [Fact]
        public void TranslateLocalDateVariables()
        {
            var jira = CreateJiraInstance();
            var date = new DateTime(2011, 1, 1);

            var issues = (from i in jira.Issues
                          where i.Created >  date
                          select i).ToArray();

            Assert.Equal("Created > \"2011/01/01\"", _translator.Jql);
        }

        [Fact]
        public void TranslateDateTimeNow()
        {
            var jira = CreateJiraInstance();
            var date = new DateTime(2011, 1, 1);

            var issues = (from i in jira.Issues
                          where i.Created > DateTime.Now
                          select i).ToArray();

            Assert.Equal("Created > \"" + DateTime.Now.ToString("yyyy/MM/dd") + "\"", _translator.Jql);
        }
    }
}
