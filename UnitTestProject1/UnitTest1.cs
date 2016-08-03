using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace UnitTestProject1
{

    public class Mail
    {
        public string MailTo = "koshka@ru.ru";
        public string Body = "Привет! Я Клара!";
        public string Subject = "For you";
    }


    [TestFixture]
    public class UnitTest1
    {
        private const string Url = "http://google.com";
        private const string DriverPath = "D:/С#AutomationClasses/Module 4 Week 1/UnitTestProject1/packages/Selenium.WebDriver.ChromeDriver.2.22.0.0/driver";

        private const string SignInId = "gb_70";
        private const string AccountId = "Email";
        private const string PasswordId = "Passwd";
        private const string ButtonNextId = "next";
        private const string SignInId2 = "signIn";
        private const string Email = "irinatest9@gmail.com";
        private const string Password = "irinatest1234";
        private const string MailLintXpath = "//div/a[contains(@href, 'mail')][1]";

        private const string AccountTitleXpath = "//a[contains(@title,'irinatest9@gmail.com')]";
        private const string LogoutButtonXpath = "//a[contains(@href,'Logout')]";

        private const string CreateNewMailXpath = "//div[@class='z0']/div[@role='button' and @tabindex=0]";

        private const string NewMailBodyCss = ".Am.Al.editable";
        private const string SendToAdressCss = ".aoD.hl";
        private const string SendToAdressTextFieldCss = ".aoD.hl span[email]";
        private const string SubjectTextFieldCss = "[name = 'subject']";
        private const string SendToAdressTextAreaCss = ".wO.nr.l1 > textarea";
        private const string SubjectCss = "[name='subjectbox']";
        private const string SendButtonCss = ".n1tfz .gU.Up [role='button']";
        private const string CloseMailDialogCss = ".Hm>.Ha ";

        private const string DraftLink = "//a[contains(@href,'draft')]";
        private const string SentLink = "//a[contains(@href,'sent')]";

        private const string DraftRowTemplate = "//div[@class='y6']/span[text()='{0}']/../span[contains(text(),'{1}')]";
        private const string SentRowTemplate = "//tr[contains(@class,'zA')]//span[@email='{0}']/../../..//*[text()='{1}']";
        private ChromeDriver Driver;
        private WebDriverWait Wait;

        private Mail MailContent;


        [SetUp]
        public void Setup()
        {
            Driver = new ChromeDriver(DriverPath);
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            Wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 25));
            MailContent = new Mail();

            Driver.Navigate().GoToUrl(Url);
        }

        [Test]
        public void TestMethod1()
        {
            //Login to the mail box.
            Driver.FindElement(By.Id(SignInId)).Click();
            Driver.FindElement(By.Id(AccountId)).Clear();
            Driver.FindElement(By.Id(AccountId)).SendKeys(Email);
            Driver.FindElement(By.Id(ButtonNextId)).Click();
            Wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(PasswordId)));
            Driver.FindElement(By.Id(PasswordId)).Clear();
            Driver.FindElement(By.Id(PasswordId)).SendKeys(Password);
            Driver.FindElement(By.Id(SignInId2)).Click();

            //Assert, that the login is successful.
            Assert.AreEqual(true,Driver.FindElementByXPath(AccountTitleXpath).Displayed);

            Driver.FindElementByXPath(MailLintXpath).Click();

            //Create a new mail(fill addressee, subject and body fields)
            Driver.FindElementByXPath(CreateNewMailXpath).Click();
            Driver.FindElementByCssSelector(NewMailBodyCss).SendKeys(MailContent.Body);
            Wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(SendToAdressCss))).Click();
            Driver.FindElementByCssSelector(SendToAdressTextAreaCss).SendKeys(MailContent.MailTo);
            Driver.FindElementByCssSelector(SubjectCss).SendKeys(MailContent.Subject);

            //Save the mail as a draft. - MAIL IS SAVED IN DRAFT BY DEFAULT
            Driver.FindElementByCssSelector(CloseMailDialogCss).Click();

            //Verify, that the mail presents in ‘Drafts’ folder.
            Driver.FindElementByXPath(DraftLink).Click();
            Assert.AreEqual(true,Driver.FindElementByXPath(string.Format(DraftRowTemplate, MailContent.Subject, MailContent.Body)).Displayed);

            //Verify the draft content
            Driver.FindElementByXPath(string.Format(DraftRowTemplate, MailContent.Subject, MailContent.Body)).Click();
            Assert.AreEqual(MailContent.Body,Driver.FindElementByCssSelector(NewMailBodyCss).Text);
            Driver.FindElementByCssSelector(SendToAdressTextFieldCss).Click();
            Assert.AreEqual(MailContent.MailTo,Driver.FindElementByCssSelector(SendToAdressTextFieldCss).GetAttribute("email"));
            Assert.AreEqual(MailContent.Subject,Driver.FindElementByCssSelector(SubjectTextFieldCss).GetAttribute("value"));

            //Send the mail.
            Driver.FindElementByCssSelector(SendButtonCss).Click();

           //Verify, that the mail disappeared from ‘Drafts’ folder.
            Driver.FindElementByXPath(DraftLink).Click();
            Wait.Until(
                ExpectedConditions.InvisibilityOfElementLocated(
                By.XPath(string.Format(DraftRowTemplate, MailContent.Subject, MailContent.Body))));

            //Verify, that the mail is in ‘Sent’ folder.
            Driver.FindElementByXPath(SentLink).Click();
            Assert.AreEqual(true, Wait.Until(ExpectedConditions.ElementExists(By.XPath(string.Format(SentRowTemplate, MailContent.MailTo, MailContent.Subject)))).Displayed);

            //Logoff
            Driver.FindElementByXPath(AccountTitleXpath).Click();
            Driver.FindElementByXPath(LogoutButtonXpath).Click();

        }

        [TearDown]
        public void TearDown()
        {

            Driver.Quit();
        }
    }
}
