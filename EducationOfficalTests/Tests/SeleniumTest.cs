using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationOfficalTests.Tests
{
    [TestFixture]
    public class SeleniumTest
    {
        private IWebDriver driver;

        [SetUp]
        //run before each test- loading the browser
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.saucedemo.com");
            System.Threading.Thread.Sleep(2000);
        }
        
        [TearDown]
        //run after each test- closing the browser
        public void Teardown()
        {
            // סגירת הדפדפן
            driver.Quit();
        }


        [Test]
        //This test will run 6 times for all the users
        [TestCaseSource("ParameterSets")]
        public void LoginTestForAllUsers(string userName, string password, bool expectedResult)
        {
            //Loading browser, and send user name and password according to the test
            TestActions.Login(driver, userName,password);

            //verify the user loged in as expected
            if (expectedResult)
            {
                Assert.That(driver.Url, Is.EqualTo("https://www.saucedemo.com/inventory.html"));
            }

            //verify the user are not in because of worng password
            else
            {
                //still at home page
                Assert.That(driver.Url, Is.EqualTo("https://www.saucedemo.com/"));
                //error appear
                bool isErrorButtonExists = driver.FindElements(By.XPath("//button[@class='error-button']")).Any();
                Assert.That(isErrorButtonExists, Is.True);
            }

        }

        //Test case data of all the usernames and password, bool parameter as epected if the user shoud log in or not
        private static IEnumerable ParameterSets()
        {
            yield return new TestCaseData("standard_user", "secret_sauce", true);
            yield return new TestCaseData("locked_out_user", "secret_sauce", false);
            yield return new TestCaseData("problem_user", "secret_sauce", true);
            yield return new TestCaseData("performance_glitch_user", "secret_sauce", true);
            yield return new TestCaseData("error_user", "secret_sauce", true);
            yield return new TestCaseData("visual_user", "secret_sauce", true);

        }

        //-------------------------------------------------------------------------

        [Test]
        public void AddToCartAllTheItemsVerifyTheItemsAddedToCartAndAddBtnChanchedToRemove()
        {
            //Login with default user
            TestActions.Login(driver);

            //Add al the items to cart
            TestActions.AddAllItemsToCart(driver);

            //map the cart element, and verify the number of the items in cart
            IWebElement cartElement = driver.FindElement(By.XPath("//*[@id='shopping_cart_container']/a/span"));
            string cartText = cartElement.Text;

            //verify 6 items in cart
            Assert.That(cartText, Is.EqualTo("6"));

            // Verify all the Add to cart button changed to remove button
            bool areRemoveButtonsPresent = IsRemoveButtonPresent(TestHelper.ItemMappings);

            // Assert that
            Assert.That(areRemoveButtonsPresent, "Not all add buttons were replaced with remove buttons");


        }

        //Helper function that verify the items add to cart, and the button changed to remove
        public bool IsRemoveButtonPresent(Dictionary<string, string> itemMappings)
        {
            foreach (var itemMapping in itemMappings)
            {
                try
                {
                    // Test if there is remove button instaed add button
                    IWebElement removeButton = driver.FindElement(By.XPath(itemMapping.Value.Replace("add-to-cart", "remove")));
                }
                catch (NoSuchElementException)
                {
                    //If not, the test failed
                    return false;
                }
            }           
            return true;
        }


        //--------------------------------------------------------------------------
        
        [Test]
        public void TestRemoveFromCart()
        {
            //Login with default user
            TestActions.Login(driver);

            //Add al the items to cart          
            TestActions.AddAllItemsToCart(driver);

            //Verify 6 items in cart
            IWebElement cartElement = driver.FindElement(By.XPath("//*[@id='shopping_cart_container']/a/span"));
            string cartText = cartElement.Text;
            Assert.That(cartText, Is.EqualTo("6"));
            bool areRemoveButtonsPresent = IsRemoveButtonPresent(TestHelper.ItemMappings);
            Assert.That(areRemoveButtonsPresent, "Not all add buttons were replaced with remove buttons");

            //Find Remove buttons and click on it (This will remove all the items from cart)
            foreach (var itemMapping in TestHelper.ItemMappings)
            {
                IWebElement removeButton = driver.FindElement(By.XPath(itemMapping.Value.Replace("add-to-cart", "remove")));
                removeButton.Click();
            }

            //Verify there is no items in cart
            ReadOnlyCollection<IWebElement> cartElements = driver.FindElements(By.XPath("//*[@id='shopping_cart_container']/a/span"));
            Assert.That(cartElements, Is.Empty, "The cart element should not be present on the page.");

        }

        //--------------------------------------------------------------------------


        [Test]
        public void AddToCartLogOutLoginAndVerifyItemStillInCart()
        {
            //Login with default user
            TestActions.Login(driver);

            //Add 1 item to cart
            string elementXPath = TestHelper.ItemMappings["Sauce Labs Backpack"];
            IWebElement addToCartButton = driver.FindElement(By.XPath(elementXPath));
            addToCartButton.Click();

            //Click on buger-bar, and then logout
            IWebElement burger = driver.FindElement(By.XPath("//*[@id='react-burger-menu-btn']"));
            burger.Click();
            System.Threading.Thread.Sleep(2000);
            IWebElement logout = driver.FindElement(By.XPath("//*[@id='logout_sidebar_link']"));
            logout.Click();
            System.Threading.Thread.Sleep(2000);


            //Login again, and verify the item still saved in cart
            TestActions.Login(driver);

            IWebElement cartElement = driver.FindElement(By.XPath("//*[@id='shopping_cart_container']/a/span"));
            string cartText = cartElement.Text;
            Assert.That(cartText, Is.EqualTo("1"));

        }


        //---------------------------------------------------------------------------

        [Test]
        public void Checkout() 
        {
            //Login
            TestActions.Login(driver);

            //Add item to cart
            string elementXPath = TestHelper.ItemMappings["Sauce Labs Backpack"];
            IWebElement addToCartButton = driver.FindElement(By.XPath(elementXPath));
            addToCartButton.Click();

            //Click on cart
            IWebElement cart = driver.FindElement(By.XPath("//*[@id='shopping_cart_container']"));
            cart.Click();

            //Click on checkout
            IWebElement chekout = driver.FindElement(By.XPath("//*[@id='checkout']"));
            chekout.Click();

            //enter first name, last name, and code
            IWebElement firstname = driver.FindElement(By.XPath("//*[@id='first-name']"));
            firstname.SendKeys("Orit");

            IWebElement lastName = driver.FindElement(By.XPath("//*[@id='last-name']"));
            lastName.SendKeys("Didi");

            IWebElement zipCode = driver.FindElement(By.XPath("//*[@id='postal-code']"));
            zipCode.SendKeys("9909");

            //Click continue
            IWebElement continue1 = driver.FindElement(By.XPath("//*[@id='continue']"));
            continue1.Click();

            //Click finish
            IWebElement finish = driver.FindElement(By.XPath("//*[@id='finish']"));
            finish.Click();

            //Verify finish title appeaar at the screen
            IWebElement finishTitle = driver.FindElement(By.XPath("//*[@id='checkout_complete_container']/h2"));
            string finishText = finishTitle.Text;

            Assert.That(finishText, Is.EqualTo("Thank you for your order!"));


        }

        //-----------------------------------------------------------------------------

    }

    public class TestHelper
    {

        //Dictionary for all the items
        public static Dictionary<string, string> ItemMappings { get; } = new Dictionary<string, string>
        {
            { "Sauce Labs Backpack", "//*[@id='add-to-cart-sauce-labs-backpack']" },
            { "Sauce Labs Bike Light", "//*[@id='add-to-cart-sauce-labs-bike-light']" },
            { "Sauce labs bolt tshirt", "//*[@id='add-to-cart-sauce-labs-bolt-t-shirt']" },
            { "labs fleece jacket", "//*[@id='add-to-cart-sauce-labs-fleece-jacket']" },
            { "Sauce labs onesie", "//*[@id='add-to-cart-sauce-labs-onesie']" },
            { "Sauce  t-shirt (red)", "//*[@id='add-to-cart-test.allthethings()-t-shirt-(red)']" },
        };
    }
}
