using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationOfficalTests.Tests
{
    public static class TestActions
    {

        public static void Login(IWebDriver driver, string userName = "standard_user", string password = "secret_sauce")
        {
            driver.FindElement(By.Id("user-name")).SendKeys(userName);
            driver.FindElement(By.Id("password")).SendKeys(password);
            driver.FindElement(By.Id("login-button")).Click();
        }
        public static void AddToCart(IWebDriver driver, string productName)
        {
            string addToCartButtonXPath = TestHelper.ItemMappings[productName];
            IWebElement addToCartButton = driver.FindElement(By.XPath(addToCartButtonXPath));
            addToCartButton.Click();
        }

        public static void AddAllItemsToCart(IWebDriver driver)
        {
            foreach (var itemMapping in TestHelper.ItemMappings)
            {
                IWebElement addToCartButton = driver.FindElement(By.XPath(itemMapping.Value));
                addToCartButton.Click();
            }
        }

    }
}
