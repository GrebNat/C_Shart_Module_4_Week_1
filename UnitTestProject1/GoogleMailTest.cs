using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace GoogleTest
{

    public class Mail
    {
        public string mailTo = "koshka@ru.ru";
        public string body = "Привет! Я Клара!";
        public string subject = "For you";
    }


    [TestFixture]
    public class GoogleMailTest
    {
        private const string url = "http://google.com";

        private const string signInId = "gb_70";
        private const string accountId = "Email";
        private const string passwordId = "Passwd";
        private const string buttonNextId = "next";
        private const string signInId2 = "signIn";
        private const string mailLintXpath = "//div/a[contains(@href, 'mail')][1]";

        private const string accountTitleXpath = "//a[contains(@title,'irinatest9@gmail.com')]";
        private const string logoutButtonXpath = "//a[contains(@href,'Logout')]";

        private const string createNewMailXpath = "//div[@class='z0']/div[@role='button' and @tabindex=0]";

        private const string newMailBodyCss = ".Am.Al.editable";
        private const string sendToAdressCss = ".aoD.hl";
        private const string sendToAdressTextFieldCss = ".aoD.hl span[email]";
        private const string subjectTextFieldCss = "[name = 'subject']";
        private const string sendToAdressTextAreaCss = ".wO.nr.l1 > textarea";
        private const string subjectCss = "[name='subjectbox']";
        private const string sendButtonCss = ".n1tfz .gU.Up [role='button']";
        private const string closeMailDialogCss = ".Hm>.Ha ";

        private const string draftLink = "//a[contains(@href,'draft')]";
        private const string sentLink = "//a[contains(@href,'sent')]";

        private const string draftRowTemplate = "//div[@class='y6']/span[text()='{0}']/../span[contains(text(),'{1}')]";
        private const string sentRowTemplate = "//tr[contains(@class,'zA')]//span[@email='{0}']/../../..//*[text()='{1}']";

        private const string email = "irinatest9@gmail.com";
        private const string password = "irinatest1234";

        private ChromeDriver driver;
        private WebDriverWait wait;

        private Mail mail;


        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            wait = new WebDriverWait(driver, new TimeSpan(0, 0, 25));
            mail = new Mail();

            driver.Navigate().GoToUrl(url);
        }

        [Test]
        public void VerifyMailSentCorrectly()
        {
            //Login to the mail box.
            driver.FindElement(By.Id(signInId)).Click();
            driver.FindElement(By.Id(accountId)).Clear();
            driver.FindElement(By.Id(accountId)).SendKeys(email);
            driver.FindElement(By.Id(buttonNextId)).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(passwordId)));
            driver.FindElement(By.Id(passwordId)).Clear();
            driver.FindElement(By.Id(passwordId)).SendKeys(password);
            driver.FindElement(By.Id(signInId2)).Click();

            //Assert, that the login is successful.
            Assert.AreEqual(true,driver.FindElementByXPath(accountTitleXpath).Displayed);

            driver.FindElementByXPath(mailLintXpath).Click();

            //Create a new mail(fill addressee, subject and body fields)
            driver.FindElementByXPath(createNewMailXpath).Click();
            driver.FindElementByCssSelector(newMailBodyCss).SendKeys(mail.body);
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(sendToAdressCss))).Click();
            driver.FindElementByCssSelector(sendToAdressTextAreaCss).SendKeys(mail.mailTo);
            driver.FindElementByCssSelector(subjectCss).SendKeys(mail.subject);

            //Save the mail as a draft. - MAIL IS SAVED IN DRAFT BY DEFAULT
            driver.FindElementByCssSelector(closeMailDialogCss).Click();

            //Verify, that the mail presents in ‘Drafts’ folder.
            driver.FindElementByXPath(draftLink).Click();
            Assert.AreEqual(true,driver.FindElementByXPath(string.Format(draftRowTemplate, mail.subject, mail.body)).Displayed);

            //Verify the draft content
            driver.FindElementByXPath(string.Format(draftRowTemplate, mail.subject, mail.body)).Click();
            Assert.AreEqual(mail.body,driver.FindElementByCssSelector(newMailBodyCss).Text);
            driver.FindElementByCssSelector(sendToAdressTextFieldCss).Click();
            Assert.AreEqual(mail.mailTo,driver.FindElementByCssSelector(sendToAdressTextFieldCss).GetAttribute("email"));
            Assert.AreEqual(mail.subject,driver.FindElementByCssSelector(subjectTextFieldCss).GetAttribute("value"));

            //Send the mail.
            driver.FindElementByCssSelector(sendButtonCss).Click();

           //Verify, that the mail disappeared from ‘Drafts’ folder.
            driver.FindElementByXPath(draftLink).Click();
            wait.Until(
                ExpectedConditions.InvisibilityOfElementLocated(
                By.XPath(string.Format(draftRowTemplate, mail.subject, mail.body))));

            //Verify, that the mail is in ‘Sent’ folder.
            driver.FindElementByXPath(sentLink).Click();
            Assert.AreEqual(true, wait.Until(ExpectedConditions.ElementExists(By.XPath(string.Format(sentRowTemplate, mail.mailTo, mail.subject)))).Displayed);

            //Logoff
            driver.FindElementByXPath(accountTitleXpath).Click();
            driver.FindElementByXPath(logoutButtonXpath).Click();

        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
